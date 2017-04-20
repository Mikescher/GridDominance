using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelNode : GameEntity, IWorldNode
	{
		public  const float DIAMETER        = 2.75f          * GDConstants.TILE_WIDTH;
		private const float WIDTH_EXTENDER  = (1496f / 768f) * GDConstants.TILE_WIDTH;
		private const float HEIGHT_EXTENDER = (1756f / 768f) * GDConstants.TILE_WIDTH;
		private const float INSET_EXTENDER  = ( 450f / 768f) * GDConstants.TILE_WIDTH;
		private const float ICON_SIZE       = 1.375f         * GDConstants.TILE_WIDTH;
		private const float ICON_OFFSET     = 0.2f           * GDConstants.TILE_WIDTH;
		private const float EXTENDER_OFFSET = DIAMETER/ 2 - INSET_EXTENDER + HEIGHT_EXTENDER / 2;
		private const float FONTSIZE = 70;

		private static readonly Color COLOR_DEACTIVATED = FlatColors.Silver;
		private static readonly Color COLOR_BORDER = FlatColors.MidnightBlue;

		private const float EXPANSION_TIME = 0.7f;
		private const float CLOSING_TIME   = 0.25f;
		private const float CENTERING_TIME = 0.55f;

		public const float ORB_SPAWN_TIME = 0.35f;

		private GDWorldMapScreen GDScreen => (GDWorldMapScreen) Owner;

		public readonly LevelBlueprint Level;
		public readonly LevelData LevelData;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;
		IEnumerable<IWorldNode> IWorldNode.NextLinkedNodes => NextLinkedNodes;

		private Vector2 centeringStartOffset;

		private readonly FRectangle rectExpanderNorth;
		private readonly FRectangle rectExpanderEast;
		private readonly FRectangle rectExpanderSouth;
		private readonly FRectangle rectExpanderWest;

		private GameEntityMouseArea clickAreaThis;
		private GameEntityMouseArea clickAreaD0;
		private GameEntityMouseArea clickAreaD1;
		private GameEntityMouseArea clickAreaD2;
		private GameEntityMouseArea clickAreaD3;

		private float expansionProgress = 0;

		public readonly List<LevelNode> NextLinkedNodes = new List<LevelNode>();
		public readonly List<LevelNodePipe> OutgoingPipes = new List<LevelNodePipe>();


		public BistateProgress State = BistateProgress.Initial;

		public bool NodeEnabled = false;

		public LevelNode(GDWorldMapScreen scrn, Vector2 pos, LevelBlueprint lvlf, LevelData lvldat) : base(scrn, GDConstants.ORDER_MAP_NODE)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER), DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER));

			Level = lvlf;
			LevelData = lvldat;

			rectExpanderNorth = FRectangle.CreateByCenter(pos, 0, -EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderEast  = FRectangle.CreateByCenter(pos, EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
			rectExpanderSouth = FRectangle.CreateByCenter(pos, 0, EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderWest  = FRectangle.CreateByCenter(pos, -EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);

			AddEntityOperation(new CyclicGameEntityOperation<LevelNode>("LevelNode::OrbSpawn", ORB_SPAWN_TIME, false, SpawnOrb));
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(new FCircle(0, 0, DIAMETER / 2f), OnClickCenter);

			clickAreaD0   = AddClickMouseArea(rectExpanderNorth.AsTranslated(-Position).AsDeflated(0, 0, INSET_EXTENDER, 0), OnClickDiff1);
			clickAreaD1   = AddClickMouseArea(rectExpanderEast.AsTranslated(-Position).AsDeflated(0, 0, 0, INSET_EXTENDER),  OnClickDiff2);
			clickAreaD2   = AddClickMouseArea(rectExpanderSouth.AsTranslated(-Position).AsDeflated(INSET_EXTENDER, 0, 0, 0), OnClickDiff3);
			clickAreaD3   = AddClickMouseArea(rectExpanderWest.AsTranslated(-Position).AsDeflated(0, INSET_EXTENDER, 0, 0),  OnClickDiff4);
		}
		
		private void SpawnOrb(LevelNode me, int cycle)
		{
			if (!NextLinkedNodes.Any()) return;
			if (!MainGame.Inst.Profile.EffectsEnabled) return;

			FractionDifficulty d = FractionDifficulty.NEUTRAL;
			switch (cycle % 4)
			{
				case 0: d = FractionDifficulty.DIFF_0; break;
				case 1: d = FractionDifficulty.DIFF_1; break;
				case 2: d = FractionDifficulty.DIFF_2; break;
				case 3: d = FractionDifficulty.DIFF_3; break;
			}

			if (!LevelData.HasCompleted(d)) d = FractionDifficulty.NEUTRAL;

			foreach (var t in OutgoingPipes)
			{
				if (!((GameEntity)t.NodeSource).IsInViewport && !((GameEntity)t.NodeSink).IsInViewport) return;

				var orb = new ConnectionOrb(Owner, t, d);

				Manager.AddEntity(orb);
			}
		}

		public void CreatePipe(LevelNode target, PipeBlueprint.Orientation orientation)
		{
			NextLinkedNodes.Add(target);

			var p = new LevelNodePipe(Owner, this, target, orientation);
			OutgoingPipes.Add(p);
			Manager.AddEntity(p);
		}

		private void OnClickCenter(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
			if (State == BistateProgress.Closed)
			{
				OpenNode();
			}
			else if (State == BistateProgress.Open)
			{
				CloseNode();
			}
		}

		public void CloseNode()
		{
			if (State == BistateProgress.Open)
			{
				if (((GDWorldHUD)Owner.HUD).SelectedNode == this) ((GDWorldHUD)Owner.HUD).SelectNode(null);

				AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(
					"LevelNode::Close::0", 
					CLOSING_TIME, 
					(n, p) => n.expansionProgress = 1 - p,
					n => n.State = BistateProgress.Closing,
					n => n.State = BistateProgress.Closed));

				MainGame.Inst.GDSound.PlayEffectClose();
			}
			else if (State == BistateProgress.Forward)
			{
				if (((GDWorldHUD)Owner.HUD).SelectedNode == this) ((GDWorldHUD)Owner.HUD).SelectNode(null);

				float initProgress = 0f;

				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Open::0");
				AbortAllOperations(p => p.Name == "LevelNode::Open::0");
				AbortAllOperations(p => p.Name == "LevelNode::Open::1");
				if (progress != null) initProgress = 1 - progress.Value;

				var o = AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(
					"LevelNode::Close::0", 
					CLOSING_TIME, 
					(n, p) => n.expansionProgress = 1 - p,
					n => n.State = BistateProgress.Closing,
					n => n.State = BistateProgress.Closed));

				if (initProgress > 0f) o.ForceSetProgress(initProgress);

				MainGame.Inst.GDSound.PlayEffectClose();
			}
		}

		private void OpenNode()
		{
			if (!NodeEnabled)
			{
				//TODO Sound
				//TODO Shake effect
				return;
			}

			if (State == BistateProgress.Closed)
			{
				((GDWorldHUD)Owner.HUD).SelectNode(this);

				float initProgress = 0f;

				centeringStartOffset = new Vector2(Owner.MapViewportCenterX, Owner.MapViewportCenterY);

				var o = AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(
					"LevelNode::Open::0", 
					EXPANSION_TIME, 
					(n, p) => n.expansionProgress = p, n => 
					n.State = BistateProgress.Opening,
					n => n.State = BistateProgress.Open));

				AddEntityOperation(new SimpleGameEntityOperation<LevelNode>("LevelNode::Open::1", CENTERING_TIME, UpdateScreenCentering));

				if (initProgress > 0f) o.ForceSetProgress(initProgress);

				MainGame.Inst.GDSound.PlayEffectOpen();
			}
			else if (State == BistateProgress.Closing)
			{
				((GDWorldHUD)Owner.HUD).SelectNode(this);

				float initProgress = 0f;

				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Close::0");
				AbortAllOperations(p => p.Name == "LevelNode::Close::0");
				if (progress != null) initProgress = 1 - progress.Value;

				centeringStartOffset = new Vector2(Owner.MapViewportCenterX, Owner.MapViewportCenterY);

				var o = AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(
					"LevelNode::Open::0", 
					EXPANSION_TIME, 
					(n, p) => n.expansionProgress = p, 
					n => n.State = BistateProgress.Opening, 
					n => n.State = BistateProgress.Open));

				AddEntityOperation(new SimpleGameEntityOperation<LevelNode>("LevelNode::Open::1", CENTERING_TIME, UpdateScreenCentering));

				if (initProgress > 0f) o.ForceSetProgress(initProgress);

				MainGame.Inst.GDSound.PlayEffectOpen();
			}
		}

		private void UpdateScreenCentering(LevelNode n, float progress)
		{
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			Owner.MapViewportCenterX = centeringStartOffset.X + p * (Position.X - centeringStartOffset.X);
			Owner.MapViewportCenterY = centeringStartOffset.Y + p * (Position.Y - centeringStartOffset.Y);
		}

		private void OnClickDiff1(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Level.UniqueID, FractionDifficulty.DIFF_0, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Level.UniqueID, FractionDifficulty.DIFF_0); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Level, FractionDifficulty.DIFF_0, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff2(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Level.UniqueID, FractionDifficulty.DIFF_1, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Level.UniqueID, FractionDifficulty.DIFF_1); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Level, FractionDifficulty.DIFF_1, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff3(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Level.UniqueID, FractionDifficulty.DIFF_2, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Level.UniqueID, FractionDifficulty.DIFF_2); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Level, FractionDifficulty.DIFF_2, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff4(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Level.UniqueID, FractionDifficulty.DIFF_3, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Level.UniqueID, FractionDifficulty.DIFF_3); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Level, FractionDifficulty.DIFF_3, GDScreen.GraphBlueprint);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			clickAreaD0.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD1.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD2.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD3.IsEnabled = (expansionProgress > 0.5f);
			
			if (State == BistateProgress.Open || State == BistateProgress.Opening)
			{
				if (((GDWorldMapScreen)Owner).IsBackgroundPressed) CloseNode();
			}

		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			float iep = 1 - FloatMath.FunctionEaseOutCubic(expansionProgress);
			float lep = 1 - expansionProgress;
			
			#region Expander

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderNorth.AsTranslated(0, HEIGHT_EXTENDER * iep).LimitSingleCoordSouth(Position.Y),
				2,
				LevelData.HasCompleted(FractionDifficulty.DIFF_0) ? GDColors.COLOR_DIFFICULTY_0 : COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderEast.AsTranslated(-HEIGHT_EXTENDER * iep, 0).LimitSingleCoordWest(Position.X),
				2,
				LevelData.HasCompleted(FractionDifficulty.DIFF_1) ? GDColors.COLOR_DIFFICULTY_1 : COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderSouth.AsTranslated(0, -HEIGHT_EXTENDER * iep).LimitSingleCoordNorth(Position.Y), 
				2,
				LevelData.HasCompleted(FractionDifficulty.DIFF_2) ? GDColors.COLOR_DIFFICULTY_2 : COLOR_DEACTIVATED, 
				COLOR_BORDER, 
				8, 
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderWest.AsTranslated(HEIGHT_EXTENDER * iep, 0).LimitSingleCoordEast(Position.X),
				2,
				LevelData.HasCompleted(FractionDifficulty.DIFF_3) ? GDColors.COLOR_DIFFICULTY_3 : COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			#endregion

			#region Icons

			sbatch.DrawStretched(
				Textures.TexDifficulty0,
				rectExpanderNorth
					.ToSquare(ICON_SIZE, FlatAlign9.NORTH)
					.AsTranslated(0, +ICON_OFFSET)
					.AsTranslated(0, HEIGHT_EXTENDER * iep),
				clickAreaD0.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			sbatch.DrawStretched(
				Textures.TexDifficulty1,
				rectExpanderEast
					.ToSquare(ICON_SIZE, FlatAlign9.EAST)
					.AsTranslated(-ICON_OFFSET, 0)
					.AsTranslated(-HEIGHT_EXTENDER * iep, 0),
				clickAreaD1.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			sbatch.DrawStretched(
				Textures.TexDifficulty2,
				rectExpanderSouth
					.ToSquare(ICON_SIZE, FlatAlign9.SOUTH)
					.AsTranslated(0, -ICON_OFFSET)
					.AsTranslated(0, -HEIGHT_EXTENDER * iep),
				clickAreaD2.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			sbatch.DrawStretched(
				Textures.TexDifficulty3,
				rectExpanderWest
					.ToSquare(ICON_SIZE, FlatAlign9.WEST)
					.AsTranslated(+ICON_OFFSET, 0)
					.AsTranslated(HEIGHT_EXTENDER * iep, 0),
				clickAreaD3.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			#endregion

			#region Ground

			if (NodeEnabled)
				sbatch.DrawCentered(Textures.TexCircle, Position, DIAMETER, DIAMETER, FlatColors.Asbestos);
			else
				sbatch.DrawCentered(Textures.TexCircle, Position, DIAMETER, DIAMETER, FlatColors.Silver);

			#endregion

			#region Segments

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment, 
				Position, 
				DIAMETER, 
				DIAMETER, 
				LevelData.HasCompleted(FractionDifficulty.DIFF_0) ? GDColors.COLOR_DIFFICULTY_0.BlendTo(COLOR_DEACTIVATED, 0.3f * lep) : COLOR_DEACTIVATED, FloatMath.RAD_POS_000 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompleted(FractionDifficulty.DIFF_1) ? GDColors.COLOR_DIFFICULTY_1.BlendTo(COLOR_DEACTIVATED, 0.3f * lep) : COLOR_DEACTIVATED, FloatMath.RAD_POS_090 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompleted(FractionDifficulty.DIFF_2) ? GDColors.COLOR_DIFFICULTY_2.BlendTo(COLOR_DEACTIVATED, 0.3f * lep) : COLOR_DEACTIVATED, FloatMath.RAD_POS_180 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompleted(FractionDifficulty.DIFF_3) ? GDColors.COLOR_DIFFICULTY_3.BlendTo(COLOR_DEACTIVATED, 0.3f * lep) : COLOR_DEACTIVATED, FloatMath.RAD_POS_270 + FloatMath.TAU * expansionProgress);
			
			#endregion

			#region Structure

			sbatch.DrawCentered(Textures.TexLevelNodeStructure, Position, DIAMETER, DIAMETER, FlatColors.MidnightBlue, FloatMath.TAU * expansionProgress);

			#endregion

			#region Text

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, FONTSIZE, Level.Name, ColorMath.Blend(FlatColors.Clouds, FlatColors.MidnightBlue, expansionProgress), Position);

			#endregion
		}
	}
}
