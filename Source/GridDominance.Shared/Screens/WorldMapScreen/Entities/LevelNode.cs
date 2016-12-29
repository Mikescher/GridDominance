using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;

		private readonly FRectangle rectExpanderNorth;
		private readonly FRectangle rectExpanderEast;
		private readonly FRectangle rectExpanderSouth;
		private readonly FRectangle rectExpanderWest;

		private float expansionProgress = 0;

		public LevelNode(GameScreen scrn, Vector2 pos) : base(scrn)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER), DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER));

			rectExpanderNorth = FRectangle.CreateByCenter(pos, 0, -EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderEast  = FRectangle.CreateByCenter(pos, EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
			rectExpanderSouth = FRectangle.CreateByCenter(pos, 0, EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER);
			rectExpanderWest  = FRectangle.CreateByCenter(pos, -EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			this.AddClickMouseArea(new FCircle(0, 0, DIAMETER / 2f), OnClickCenter);

			this.AddClickMouseArea(rectExpanderNorth.AsTranslated(-Position).AsDeflated(0, 0, INSET_EXTENDER, 0), OnClickDiff1);
			this.AddClickMouseArea(rectExpanderEast.AsTranslated(-Position).AsDeflated(0, 0, 0, INSET_EXTENDER),  OnClickDiff2);
			this.AddClickMouseArea(rectExpanderSouth.AsTranslated(-Position).AsDeflated(INSET_EXTENDER, 0, 0, 0), OnClickDiff3);
			this.AddClickMouseArea(rectExpanderWest.AsTranslated(-Position).AsDeflated(0, INSET_EXTENDER, 0, 0),  OnClickDiff4);
		}

		private void OnClickCenter(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (FloatMath.IsZero(expansionProgress))
				AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(EXPANSION_TIME, (n, p) => n.expansionProgress = p));
			else if (FloatMath.IsOne(expansionProgress))
				AddEntityOperation(new SimpleGameEntityOperation<LevelNode>(EXPANSION_TIME, (n, p) => n.expansionProgress = 1-p));
		}

		private void OnClickDiff1(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (FloatMath.IsOne(expansionProgress))
			{
				Owner.PushNotification("CLICK DIFF 0");
			}
		}

		private void OnClickDiff2(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (FloatMath.IsOne(expansionProgress))
			{
				Owner.PushNotification("CLICK DIFF 1");
			}
		}

		private void OnClickDiff3(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (FloatMath.IsOne(expansionProgress))
			{
				Owner.PushNotification("CLICK DIFF 2");
			}
		}

		private void OnClickDiff4(GameEntityMouseArea owner, GameTime dateTime, InputState istate)
		{
			if (FloatMath.IsOne(expansionProgress))
			{
				Owner.PushNotification("CLICK DIFF 3");
			}
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			float iep = 1 - FloatMath.FunctionEaseOutCubic(expansionProgress);

			#region Expander

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderNorth.AsTranslated(0, HEIGHT_EXTENDER * iep).LimitSingleCoordSouth(Position.Y),
				2,
				GDColors.COLOR_DIFFICULTY_0,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderEast.AsTranslated(-HEIGHT_EXTENDER * iep, 0).LimitSingleCoordWest(Position.X),
				2,
				GDColors.COLOR_DIFFICULTY_1,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderSouth.AsTranslated(0, -HEIGHT_EXTENDER * iep).LimitSingleCoordNorth(Position.Y), 
				2, 
				GDColors.COLOR_DIFFICULTY_2, 
				COLOR_BORDER, 
				8, 
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				rectExpanderWest.AsTranslated(HEIGHT_EXTENDER * iep, 0).LimitSingleCoordEast(Position.X),
				2,
				COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

			#endregion

			#region Icons

			sbatch.Draw(
				Textures.TexDifficulty0,
				rectExpanderNorth
					.ToSquare(ICON_SIZE, FlatAlign9.NORTH)
					.AsTranslated(0, +ICON_OFFSET)
					.AsTranslated(0, HEIGHT_EXTENDER * iep),
				Color.White);

			sbatch.Draw(
				Textures.TexDifficulty1,
				rectExpanderEast
					.ToSquare(ICON_SIZE, FlatAlign9.EAST)
					.AsTranslated(-ICON_OFFSET, 0)
					.AsTranslated(-HEIGHT_EXTENDER * iep, 0),
				Color.White);

			sbatch.Draw(
				Textures.TexDifficulty2,
				rectExpanderSouth
					.ToSquare(ICON_SIZE, FlatAlign9.SOUTH)
					.AsTranslated(0, -ICON_OFFSET)
					.AsTranslated(0, -HEIGHT_EXTENDER * iep),
				Color.White);

			sbatch.Draw(
				Textures.TexDifficulty3,
				rectExpanderWest
					.ToSquare(ICON_SIZE, FlatAlign9.WEST)
					.AsTranslated(+ICON_OFFSET, 0)
					.AsTranslated(HEIGHT_EXTENDER * iep, 0),
				Color.White);

			#endregion

			#region Ground

			sbatch.Draw(
				Textures.TexCircle.Texture,
				Position,
				Textures.TexCircle.Bounds,
				FlatColors.Asbestos,
				0f,
				Textures.TexCircle.Center(),
				DIAMETER / Textures.TexCircle.Width,
				SpriteEffects.None,
				0);

			#endregion

			#region Segments

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				GDColors.COLOR_DIFFICULTY_0,
				FloatMath.RAD_000 + FloatMath.TAU * expansionProgress,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				GDColors.COLOR_DIFFICULTY_1,
				FloatMath.RAD_POS_090 + FloatMath.TAU * expansionProgress,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				GDColors.COLOR_DIFFICULTY_2,
				FloatMath.RAD_POS_180 + FloatMath.TAU * expansionProgress,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				COLOR_DEACTIVATED, //GDColors.COLOR_DIFFICULTY_3,
				FloatMath.RAD_POS_270 + FloatMath.TAU * expansionProgress,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			#endregion
			
			#region Structure

			sbatch.Draw(
				Textures.TexLevelNodeStructure.Texture,
				Position,
				Textures.TexLevelNodeStructure.Bounds,
				FlatColors.MidnightBlue,
				0 + FloatMath.TAU * expansionProgress,
				Textures.TexLevelNodeStructure.Center(),
				DIAMETER / Textures.TexLevelNodeStructure.Width,
				SpriteEffects.None,
				0);

			#endregion

			#region Text

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, FONTSIZE, "1-2", FlatColors.Clouds, Position);

			#endregion
		}
	}
}
