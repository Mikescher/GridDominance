using GridDominance.Levelformat.Parser;
using GridDominance.Shared.PlayerProfile;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
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
	class LevelNode : GameEntity
	{
		private const float DIAMETER = 2.75f * GDConstants.TILE_WIDTH;
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
		private const float CENTERING_TIME = 0.55f;

		private readonly LevelFile level;
		private readonly PlayerProfileLevelData levelData;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;

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

		public bool IsOpening = false;
		public bool IsClosing = false;
		public bool IsOpened => FloatMath.IsOne(expansionProgress);
		public bool IsClosed => FloatMath.IsZero(expansionProgress);

		public LevelNode(GameScreen scrn, Vector2 pos, LevelFile lvlf, PlayerProfileLevelData lvldat) : base(scrn)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER), DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER));

			level = lvlf;
			levelData = lvldat;

			rectExpanderNorth = FRectangle.CreateByCenter(pos, 0, -EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderEast  = FRectangle.CreateByCenter(pos, EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
			rectExpanderSouth = FRectangle.CreateByCenter(pos, 0, EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderWest  = FRectangle.CreateByCenter(pos, -EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(new FCircle(0, 0, DIAMETER / 2f), OnClickCenter);

			clickAreaD0   = AddClickMouseArea(rectExpanderNorth.AsTranslated(-Position).AsDeflated(0, 0, INSET_EXTENDER, 0), OnClickDiff1);
			clickAreaD1   = AddClickMouseArea(rectExpanderEast.AsTranslated(-Position).AsDeflated(0, 0, 0, INSET_EXTENDER),  OnClickDiff2);
			clickAreaD2   = AddClickMouseArea(rectExpanderSouth.AsTranslated(-Position).AsDeflated(INSET_EXTENDER, 0, 0, 0), OnClickDiff3);
			clickAreaD3   = AddClickMouseArea(rectExpanderWest.AsTranslated(-Position).AsDeflated(0, INSET_EXTENDER, 0, 0),  OnClickDiff4);
		}

		private void OnClickCenter(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (IsClosed)
			{
				OpenNode();
			}
			else if (IsOpened)
			{
				CloseNode();
			}
		}

		private void CloseNode()
		{
			if (IsClosing) return;

			float initProgress = 0f;
			if (IsOpening)
			{
				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Open::0");
				AbortAllOperations(p => p.Name == "LevelNode::Open::0");
				AbortAllOperations(p => p.Name == "LevelNode::Open::1");
				if (progress != null) initProgress = 1 - progress.Value;

				IsOpening = false;
			}

			var o = AddEntityOperation(new SimpleGameEntityOperation<LevelNode>("LevelNode::Close::0", EXPANSION_TIME, (n, p) => n.expansionProgress = 1 - p, n => n.IsClosing = true, n => n.IsClosing = false));

			if (initProgress > 0f)
			{
				o.ForceSetProgress(initProgress);
			}
		}

		private void OpenNode()
		{
			if (IsOpening) return;

			float initProgress = 0f;
			if (IsClosing)
			{
				var progress = FindFirstOperationProgress(p => p.Name == "LevelNode::Close::0");
				AbortAllOperations(p => p.Name == "LevelNode::Close::0");
				if (progress != null) initProgress = 1 - progress.Value;

				IsClosing = false;
			}

			centeringStartOffset = new Vector2(Owner.MapViewportCenterX, Owner.MapViewportCenterY);

			var o = AddEntityOperation(new SimpleGameEntityOperation<LevelNode>("LevelNode::Open::0", EXPANSION_TIME, (n, p) => n.expansionProgress = p, n => n.IsOpening = true, n => n.IsOpening = false));
			AddEntityOperation(new SimpleGameEntityOperation<LevelNode>("LevelNode::Open::1", CENTERING_TIME, UpdateScreenCentering));

			if (initProgress > 0f)
			{
				o.ForceSetProgress(initProgress);
			}
		}

		private void UpdateScreenCentering(LevelNode n, float progress)
		{
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			Owner.MapViewportCenterX = centeringStartOffset.X + p * (Position.X - centeringStartOffset.X);
			Owner.MapViewportCenterY = centeringStartOffset.Y + p * (Position.Y - centeringStartOffset.Y);
		}

		private void OnClickDiff1(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
#if DEBUG
			Owner.PushNotification("CLICK DIFF 0");

			if (istate.IsKeyDown(SKeys.ShiftAny))
			{
				levelData.SetCompleted(FractionDifficulty.DIFF_0, !levelData.HasCompleted(FractionDifficulty.DIFF_0));
				MainGame.Inst.SaveProfile();

				return;
			}
#endif

			MainGame.Inst.SetLevelScreen(level, FractionDifficulty.DIFF_0);
		}

		private void OnClickDiff2(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
#if DEBUG
			Owner.PushNotification("CLICK DIFF 1");

			if (istate.IsKeyDown(SKeys.ShiftAny))
			{
				levelData.SetCompleted(FractionDifficulty.DIFF_1, !levelData.HasCompleted(FractionDifficulty.DIFF_1));
				MainGame.Inst.SaveProfile();

				return;
			}
#endif

			MainGame.Inst.SetLevelScreen(level, FractionDifficulty.DIFF_1);
		}

		private void OnClickDiff3(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
#if DEBUG
			Owner.PushNotification("CLICK DIFF 2");

			if (istate.IsKeyDown(SKeys.ShiftAny))
			{
				levelData.SetCompleted(FractionDifficulty.DIFF_2, !levelData.HasCompleted(FractionDifficulty.DIFF_2));
				MainGame.Inst.SaveProfile();

				return;
			}
#endif

			MainGame.Inst.SetLevelScreen(level, FractionDifficulty.DIFF_2);
		}

		private void OnClickDiff4(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
#if DEBUG
			Owner.PushNotification("CLICK DIFF 3");

			if (istate.IsKeyDown(SKeys.ShiftAny))
			{
				levelData.SetCompleted(FractionDifficulty.DIFF_3, !levelData.HasCompleted(FractionDifficulty.DIFF_3));
				MainGame.Inst.SaveProfile();

				return;
			}
#endif

			MainGame.Inst.SetLevelScreen(level, FractionDifficulty.DIFF_3);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			clickAreaD0.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD1.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD2.IsEnabled = (expansionProgress > 0.5f);
			clickAreaD3.IsEnabled = (expansionProgress > 0.5f);

			if (IsOpened || IsOpening)
			{
				var clickOutside =
					istate.IsRealDown &&
					!clickAreaD0.IsMouseDown() &&
					!clickAreaD1.IsMouseDown() &&
					!clickAreaD2.IsMouseDown() &&
					!clickAreaD3.IsMouseDown() &&
					!clickAreaThis.IsMouseDown();

				if (clickOutside) CloseNode();
			}

		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			float iep = 1 - FloatMath.FunctionEaseOutCubic(expansionProgress);

			#region Expander

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderNorth.AsTranslated(0, HEIGHT_EXTENDER * iep).LimitSingleCoordSouth(Position.Y),
				2,
				levelData.HasCompleted(FractionDifficulty.DIFF_0) ? GDColors.COLOR_DIFFICULTY_0 : COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderEast.AsTranslated(-HEIGHT_EXTENDER * iep, 0).LimitSingleCoordWest(Position.X),
				2,
				levelData.HasCompleted(FractionDifficulty.DIFF_1) ? GDColors.COLOR_DIFFICULTY_1 : COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderSouth.AsTranslated(0, -HEIGHT_EXTENDER * iep).LimitSingleCoordNorth(Position.Y), 
				2,
				levelData.HasCompleted(FractionDifficulty.DIFF_2) ? GDColors.COLOR_DIFFICULTY_2 : COLOR_DEACTIVATED, 
				COLOR_BORDER, 
				8, 
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderWest.AsTranslated(HEIGHT_EXTENDER * iep, 0).LimitSingleCoordEast(Position.X),
				2,
				levelData.HasCompleted(FractionDifficulty.DIFF_3) ? GDColors.COLOR_DIFFICULTY_3 : COLOR_DEACTIVATED,
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

			sbatch.DrawCentered(Textures.TexCircle, Position, DIAMETER, DIAMETER, FlatColors.Asbestos);

			#endregion

			#region Segments

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment, 
				Position, 
				DIAMETER, 
				DIAMETER, 
				levelData.HasCompleted(FractionDifficulty.DIFF_0) ? GDColors.COLOR_DIFFICULTY_0 : COLOR_DEACTIVATED, 
				FloatMath.RAD_POS_000 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				levelData.HasCompleted(FractionDifficulty.DIFF_1) ? GDColors.COLOR_DIFFICULTY_1 : COLOR_DEACTIVATED,
				FloatMath.RAD_POS_090 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				levelData.HasCompleted(FractionDifficulty.DIFF_2) ? GDColors.COLOR_DIFFICULTY_2 : COLOR_DEACTIVATED,
				FloatMath.RAD_POS_180 + FloatMath.TAU * expansionProgress);

			sbatch.DrawCentered(
				Textures.TexLevelNodeSegment,
				Position,
				DIAMETER,
				DIAMETER,
				levelData.HasCompleted(FractionDifficulty.DIFF_3) ? GDColors.COLOR_DIFFICULTY_3 : COLOR_DEACTIVATED,
				FloatMath.RAD_POS_270 + FloatMath.TAU * expansionProgress);
			
			#endregion

			#region Structure

			sbatch.DrawCentered(Textures.TexLevelNodeStructure, Position, DIAMETER, DIAMETER, FlatColors.MidnightBlue, FloatMath.TAU * expansionProgress);

			#endregion

			#region Text

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, FONTSIZE, level.Name, ColorMath.Blend(FlatColors.Clouds, FlatColors.MidnightBlue, expansionProgress), Position);

			#endregion
		}
	}
}
