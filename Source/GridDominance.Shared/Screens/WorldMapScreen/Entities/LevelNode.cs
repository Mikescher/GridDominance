using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelNode : BaseWorldNode, IWorldNode
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

		private GDWorldMapScreen GDOwner => (GDWorldMapScreen)Owner;

		public const float EXPANSION_TIME       = 0.5f;
		public const float CLOSING_TIME         = 0.25f;
		public const float CENTERING_TIME       = 0.55f;
		public const float SHAKE_TIME           = 0.55f;
		public const float EXTENDER_DELAY       = 0.1f;
		public const float TOTAL_EXPANSION_TIME = 0.7f; // ~ EXPANSION_TIME+3*EXTENDER_DELAY   ; but a bit less 'cause it looks better

		public const float ORB_SPAWN_TIME = 0.35f;

		private GDWorldMapScreen GDScreen => (GDWorldMapScreen) Owner;

		public readonly LevelBlueprint Blueprint;
		public readonly LevelData LevelData;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;
		IEnumerable<IWorldNode> IWorldNode.NextLinkedNodes => NextLinkedNodes;
		public Guid ConnectionID => Blueprint.UniqueID;

		private readonly FRectangle rectExpanderNorth;
		private readonly FRectangle rectExpanderEast;
		private readonly FRectangle rectExpanderSouth;
		private readonly FRectangle rectExpanderWest;

		private GameEntityMouseArea clickAreaThis;
		private GameEntityMouseArea clickAreaD0;
		private GameEntityMouseArea clickAreaD1;
		private GameEntityMouseArea clickAreaD2;
		private GameEntityMouseArea clickAreaD3;

		public readonly List<IWorldNode> NextLinkedNodes = new List<IWorldNode>(); // ordered by pipe priority
		public readonly List<LevelNodePipe> OutgoingPipes = new List<LevelNodePipe>();

		public float RootExpansionProgress = 0f;
		public readonly float[] ExpansionProgress = {0f, 0f, 0f, 0f};
		public readonly BistateProgress[] State = { BistateProgress.Closed, BistateProgress.Closed, BistateProgress.Closed, BistateProgress.Closed };

		public BistateProgress StateSum
		{
			get
			{
				if (State.All(s => s == BistateProgress.Closed)) return BistateProgress.Closed;
				if (State.All(s => s == BistateProgress.Open)) return BistateProgress.Open;
				if (State.All(s => s == BistateProgress.Open || s == BistateProgress.Opening)) return BistateProgress.Opening;
				if (State.All(s => s == BistateProgress.Closed || s == BistateProgress.Closing)) return BistateProgress.Closing;

				return BistateProgress.Undefined;
			}
		}

		public bool NodeEnabled { get; set; } = false;

		public LevelNode(GDWorldMapScreen scrn, FPoint pos, LevelBlueprint lvlf, LevelData lvldat) : base(scrn, GDConstants.ORDER_MAP_NODE)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER), DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER));

			Blueprint = lvlf;
			LevelData = lvldat;

			rectExpanderNorth = FRectangle.CreateByCenter(pos, 0, -EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderEast  = FRectangle.CreateByCenter(pos, EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
			rectExpanderSouth = FRectangle.CreateByCenter(pos, 0, EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderWest  = FRectangle.CreateByCenter(pos, -EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);

			AddOperation(new CyclicLambdaOperation<LevelNode>("LevelNode::OrbSpawn", ORB_SPAWN_TIME, false, SpawnOrb));
		}

		public bool CenterContains(FPoint p)
		{
			return clickAreaThis.AbsoluteShape.Contains(p);
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(new FCircle(0, 0, DIAMETER / 2f), OnClickCenter);

			clickAreaD0   = AddClickMouseArea(rectExpanderNorth.RelativeTo(Position).AsDeflated(0, 0, INSET_EXTENDER, 0), OnClickDiff1);
			clickAreaD1   = AddClickMouseArea(rectExpanderEast.RelativeTo(Position).AsDeflated(0, 0, 0, INSET_EXTENDER),  OnClickDiff2);
			clickAreaD2   = AddClickMouseArea(rectExpanderSouth.RelativeTo(Position).AsDeflated(INSET_EXTENDER, 0, 0, 0), OnClickDiff3);
			clickAreaD3   = AddClickMouseArea(rectExpanderWest.RelativeTo(Position).AsDeflated(0, INSET_EXTENDER, 0, 0),  OnClickDiff4);
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

				default:
					SAMLog.Error("LN::EnumSwitch_SO", "value: " + (cycle%4));
					break;
			}

			if (!LevelData.HasCompletedOrBetter(d)) d = FractionDifficulty.NEUTRAL;

			foreach (var t in OutgoingPipes)
			{
				if (!((GameEntity)t.NodeSource).IsInViewport && !((GameEntity)t.NodeSink).IsInViewport) return;

				var orb = new ConnectionOrb(Owner, t, d);

				Manager.AddEntity(orb);
			}
		}

		public void CreatePipe(IWorldNode target, PipeBlueprint.Orientation orientation)
		{
			NextLinkedNodes.Add(target);

			var p = new LevelNodePipe(Owner, this, target, orientation);
			OutgoingPipes.Add(p);
			Manager.AddEntity(p);
		}

		private void OnClickCenter(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
			if (GDOwner.ZoomState != BistateProgress.Normal) return;

			var stat = StateSum;

			if (stat == BistateProgress.Closed || stat == BistateProgress.Closing)
			{
				OpenNode();
			}
			else if (stat == BistateProgress.Open || stat == BistateProgress.Opening || stat == BistateProgress.Undefined)
			{
				CloseNode();
			}
		}

		public void CloseNode()
		{
			if (((GDWorldHUD)Owner.HUD).SelectedNode == this) ((GDWorldHUD)Owner.HUD).SelectNode(null);

			if (StateSum == BistateProgress.Closing) return;

			var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Open::root");
			AbortAllOperations(p => p.Name == "LevelNode::Open::root");
			var o = new LambdaOperation<LevelNode>("LevelNode::Close::root", TOTAL_EXPANSION_TIME, (n, p) => RootExpansionProgress = 1 - p);
			AddOperation(o);
			if (progress != null) o.ForceSetProgress(1-progress.Value);

			CloseExtender(FractionDifficulty.DIFF_0);
			CloseExtender(FractionDifficulty.DIFF_1);
			CloseExtender(FractionDifficulty.DIFF_2);
			CloseExtender(FractionDifficulty.DIFF_3);

			MainGame.Inst.GDSound.PlayEffectClose();
		}

		private void CloseExtender(FractionDifficulty d)
		{
			if (State[(int)d] == BistateProgress.Open)
			{
				AddOperation(new CloseNodeOperation(d));
			}
			else if (State[(int) d] == BistateProgress.Opening)
			{
				float initProgress = 0f;

				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Open::" + (int) d);
				AbortAllOperations(p => p.Name == "LevelNode::Open::" + (int) d);
				AbortAllOperations(p => p.Name == "LevelNode::Open::" + (int) d + "#delay");
				if (progress != null)
					initProgress = 1 - progress.Value;
				else
					initProgress = 0.9999f;

				var o = new CloseNodeOperation(d);
				AddOperation(o);
				if (initProgress > 0f) o.ForceSetProgress(initProgress);
			}
		}

		public void OpenNode()
		{
			AbortAllOperations(p => p.Name == "LevelNode::Center");
			AbortAllOperations(p => p.Name == "LevelNode::CenterShake");

#if DEBUG
			if (!NodeEnabled && DebugSettings.Get("UnlockNode"))
			{
				NodeEnabled = true;
			}
#endif
			if (!NodeEnabled)
			{
				MainGame.Inst.GDSound.PlayEffectError();

				AddOperation(new ScreenShakeAndCenterOperation(this, GDOwner));

				Owner.HUD.ShowToast("LN::LOCKED", L10N.T(L10NImpl.STR_GLOB_LEVELLOCK), 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);

				return;
			}
			
			((GDWorldHUD)Owner.HUD).SelectNode(this);

			var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Close::root");
			AbortAllOperations(p => p.Name == "LevelNode::Close::root");
			var o = new LambdaOperation<LevelNode>("LevelNode::Open::root", TOTAL_EXPANSION_TIME, (n, p) => RootExpansionProgress = p);
			AddOperation(o);
			if (progress != null) o.ForceSetProgress(1 - progress.Value);

			if (Blueprint.UniqueID == Levels.LEVELID_1_1 && !LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_0))
			{
				OpenExtender(FractionDifficulty.DIFF_0);
			}
			else
			{
				OpenExtender(FractionDifficulty.DIFF_0);
				OpenExtender(FractionDifficulty.DIFF_1);
				OpenExtender(FractionDifficulty.DIFF_2);
				OpenExtender(FractionDifficulty.DIFF_3);
			}

			AddOperation(new CenterNodeOperation(GDOwner));

			MainGame.Inst.GDSound.PlayEffectOpen();
		}

		private void OpenExtender(FractionDifficulty d)
		{
			if (State[(int)d] == BistateProgress.Closed)
			{
				float initProgress = 0f;

				State[(int)d] = BistateProgress.Opening;
				var o = new OpenNodeOperation(d);
				AddOperationDelayed(o, EXTENDER_DELAY * (int)d);

				if (initProgress > 0f) o.ForceSetProgress(initProgress);

			}
			else if (State[(int) d] == BistateProgress.Closing)
			{
				float initProgress = 0f;

				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Close::" + (int) d);
				AbortAllOperations(p => p.Name == "LevelNode::Close::" + (int) d);
				AbortAllOperations(p => p.Name == "LevelNode::Close::" + (int) d + "#delay");
				if (progress != null) initProgress = 1 - progress.Value;

				var o = new OpenNodeOperation(d);
				AddOperation(o);

				if (initProgress > 0f) o.ForceSetProgress(initProgress);
			}
		}

		private void OnClickDiff1(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_0, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_0); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Blueprint, FractionDifficulty.DIFF_0, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff2(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_1, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_1); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Blueprint, FractionDifficulty.DIFF_1, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff3(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_2, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_2); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Blueprint, FractionDifficulty.DIFF_2, GDScreen.GraphBlueprint);
		}

		private void OnClickDiff4(GameEntityMouseArea owner, SAMTime dateTime, InputState istate)
		{
#if DEBUG
			if (istate.IsKeyDown(SKeys.A)) { MainGame.Inst.Profile.SetCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_3, 60000, true); MainGame.Inst.SaveProfile(); return; }
			if (istate.IsKeyDown(SKeys.X)) { MainGame.Inst.Profile.SetNotCompleted(Blueprint.UniqueID, FractionDifficulty.DIFF_3); MainGame.Inst.SaveProfile(); return; }
#endif

			MainGame.Inst.SetLevelScreen(Blueprint, FractionDifficulty.DIFF_3, GDScreen.GraphBlueprint);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			clickAreaD0.IsEnabled = (ExpansionProgress[0] > 0.5f);
			clickAreaD1.IsEnabled = (ExpansionProgress[1] > 0.5f);
			clickAreaD2.IsEnabled = (ExpansionProgress[2] > 0.5f);
			clickAreaD3.IsEnabled = (ExpansionProgress[3] > 0.5f);
			
			if (StateSum == BistateProgress.Open || StateSum == BistateProgress.Opening || StateSum == BistateProgress.Undefined)
			{
				if (((GDWorldMapScreen)Owner).IsBackgroundPressed) CloseNode();
			}
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			float nepX = RootExpansionProgress;
			float lepX = 1 - RootExpansionProgress;

			float iep0 = 1 - FloatMath.FunctionEaseOutCubic(ExpansionProgress[0]);
			float lep0 = 1 - ExpansionProgress[0];
			float nep0 = ExpansionProgress[0];

			float iep1 = 1 - FloatMath.FunctionEaseOutCubic(ExpansionProgress[1]);
			float lep1 = 1 - ExpansionProgress[1];
			float nep1 = ExpansionProgress[1];

			float iep2 = 1 - FloatMath.FunctionEaseOutCubic(ExpansionProgress[2]);
			float lep2 = 1 - ExpansionProgress[2];
			float nep2 = ExpansionProgress[2];

			float iep3 = 1 - FloatMath.FunctionEaseOutCubic(ExpansionProgress[3]);
			float lep3 = 1 - ExpansionProgress[3];
			float nep3 = ExpansionProgress[3];

			#region Expander

			if (State[(int)FractionDifficulty.DIFF_0] != BistateProgress.Closed)
				FlatRenderHelper.DrawOutlinesBlurRectangle(
					sbatch,
					rectExpanderNorth.AsTranslated(0, HEIGHT_EXTENDER * iep0).LimitSingleCoordSouth(Position.Y),
					2,
					LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_0) ? FractionDifficultyHelper.COLOR_DIFFICULTY_0 : COLOR_DEACTIVATED,
					COLOR_BORDER,
					8,
					10);

			if (State[(int)FractionDifficulty.DIFF_1] != BistateProgress.Closed)
				FlatRenderHelper.DrawOutlinesBlurRectangle(
					sbatch,
					rectExpanderEast.AsTranslated(-HEIGHT_EXTENDER * iep1, 0).LimitSingleCoordWest(Position.X),
					2,
					LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_1) ? FractionDifficultyHelper.COLOR_DIFFICULTY_1 : COLOR_DEACTIVATED,
					COLOR_BORDER,
					8,
					10);

			if (State[(int)FractionDifficulty.DIFF_2] != BistateProgress.Closed)
				FlatRenderHelper.DrawOutlinesBlurRectangle(
					sbatch,
					rectExpanderSouth.AsTranslated(0, -HEIGHT_EXTENDER * iep2).LimitSingleCoordNorth(Position.Y), 
					2,
					LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_2) ? FractionDifficultyHelper.COLOR_DIFFICULTY_2 : COLOR_DEACTIVATED, 
					COLOR_BORDER, 
					8, 
					10);

			if (State[(int)FractionDifficulty.DIFF_3] != BistateProgress.Closed)
				FlatRenderHelper.DrawOutlinesBlurRectangle(
					sbatch,
					rectExpanderWest.AsTranslated(HEIGHT_EXTENDER * iep3, 0).LimitSingleCoordEast(Position.X),
					2,
					LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_3) ? FractionDifficultyHelper.COLOR_DIFFICULTY_3 : COLOR_DEACTIVATED,
					COLOR_BORDER,
					8,
					10);

			#endregion

			#region Icons

			if (State[(int)FractionDifficulty.DIFF_0] != BistateProgress.Closed)
				sbatch.DrawStretched(
					Textures.TexDifficultyLine0,
					rectExpanderNorth
						.ToSquare(ICON_SIZE, FlatAlign9.NORTH)
						.AsTranslated(0, +ICON_OFFSET)
						.AsTranslated(0, HEIGHT_EXTENDER * iep0),
					clickAreaD0.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			if (State[(int)FractionDifficulty.DIFF_1] != BistateProgress.Closed)
				sbatch.DrawStretched(
					Textures.TexDifficultyLine1,
					rectExpanderEast
						.ToSquare(ICON_SIZE, FlatAlign9.EAST)
						.AsTranslated(-ICON_OFFSET, 0)
						.AsTranslated(-HEIGHT_EXTENDER * iep1, 0),
					clickAreaD1.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			if (State[(int)FractionDifficulty.DIFF_2] != BistateProgress.Closed)
				sbatch.DrawStretched(
					Textures.TexDifficultyLine2,
					rectExpanderSouth
						.ToSquare(ICON_SIZE, FlatAlign9.SOUTH)
						.AsTranslated(0, -ICON_OFFSET)
						.AsTranslated(0, -HEIGHT_EXTENDER * iep2),
					clickAreaD2.IsMouseDown() ? FlatColors.WetAsphalt : Color.White);

			if (State[(int)FractionDifficulty.DIFF_3] != BistateProgress.Closed)
				sbatch.DrawStretched(
					Textures.TexDifficultyLine3,
					rectExpanderWest
						.ToSquare(ICON_SIZE, FlatAlign9.WEST)
						.AsTranslated(+ICON_OFFSET, 0)
						.AsTranslated(HEIGHT_EXTENDER * iep3, 0),
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
				LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_0) ? FractionDifficultyHelper.COLOR_DIFFICULTY_0.BlendTo(COLOR_DEACTIVATED, 0.3f * lepX) : COLOR_DEACTIVATED, FloatMath.RAD_POS_000 + FloatMath.TAU * nepX);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_1) ? FractionDifficultyHelper.COLOR_DIFFICULTY_1.BlendTo(COLOR_DEACTIVATED, 0.3f * lepX) : COLOR_DEACTIVATED, FloatMath.RAD_POS_090 + FloatMath.TAU * nepX);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_2) ? FractionDifficultyHelper.COLOR_DIFFICULTY_2.BlendTo(COLOR_DEACTIVATED, 0.3f * lepX) : COLOR_DEACTIVATED, FloatMath.RAD_POS_180 + FloatMath.TAU * nepX);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				LevelData.HasCompletedOrBetter(FractionDifficulty.DIFF_3) ? FractionDifficultyHelper.COLOR_DIFFICULTY_3.BlendTo(COLOR_DEACTIVATED, 0.3f * lepX) : COLOR_DEACTIVATED, FloatMath.RAD_POS_270 + FloatMath.TAU * nepX);
			
			#endregion

			#region Structure

			sbatch.DrawCentered(Textures.TexLevelNodeStructure, Position, DIAMETER, DIAMETER, FlatColors.MidnightBlue, FloatMath.TAU * nepX);

			#endregion

			#region Text

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, FONTSIZE, Blueprint.Name, ColorMath.Blend(FlatColors.Clouds, FlatColors.MidnightBlue, nepX), Position);

			#endregion
		}

		public bool HasAnyCompleted()
		{
			return LevelData.HasAnyCompleted();
		}
	}
}
