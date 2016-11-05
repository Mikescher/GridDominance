using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	class LevelNode : GameEntity
	{
		private const float DIAMETER = 2.75f * GDConstants.TILE_WIDTH;
		private const float WIDTH_EXTENDER  = (1496f / 768f) * GDConstants.TILE_WIDTH;
		private const float HEIGHT_EXTENDER = (1756f / 768f) * GDConstants.TILE_WIDTH;
		private const float INSET_EXTENDER  = ( 450f / 768f) * GDConstants.TILE_WIDTH;
		private const float EXTENDER_OFFSET = DIAMETER/ 2 - INSET_EXTENDER + HEIGHT_EXTENDER / 2;
		private const float FONTSIZE = 70;

		private static readonly Color COLOR_DEACTIVATED = FlatColors.Silver;
		private static readonly Color COLOR_BORDER = FlatColors.MidnightBlue;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;

		public LevelNode(GameScreen scrn, Vector2 pos) : base(scrn)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER), DIAMETER + 2 * (HEIGHT_EXTENDER - INSET_EXTENDER));
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
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
			#region Expander

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				FRectangle.CreateByCenter(Position, 0, -EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER),
				2,
				GDColors.COLOR_DIFFICULTY_0,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				FRectangle.CreateByCenter(Position, EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER),
				2,
				GDColors.COLOR_DIFFICULTY_1,
				COLOR_BORDER,
				8,
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch, 
				FRectangle.CreateByCenter(Position, 0, EXTENDER_OFFSET, WIDTH_EXTENDER, HEIGHT_EXTENDER), 
				2, 
				GDColors.COLOR_DIFFICULTY_2, 
				COLOR_BORDER, 
				8, 
				10);

			FlatRenderHelper.DrawOutlinesBlurRectangle(
				sbatch,
				FRectangle.CreateByCenter(Position, -EXTENDER_OFFSET, 0, HEIGHT_EXTENDER, WIDTH_EXTENDER),
				2,
				COLOR_DEACTIVATED,
				COLOR_BORDER,
				8,
				10);

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
				FloatMath.RAD_000,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				GDColors.COLOR_DIFFICULTY_1,
				FloatMath.RAD_POS_090,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				GDColors.COLOR_DIFFICULTY_2,
				FloatMath.RAD_POS_180,
				Textures.TexLevelNodeSegment.Center(),
				DIAMETER / Textures.TexLevelNodeSegment.Width,
				SpriteEffects.None,
				0);

			sbatch.Draw(
				Textures.TexLevelNodeSegment.Texture,
				Position,
				Textures.TexLevelNodeSegment.Bounds,
				COLOR_DEACTIVATED, //GDColors.COLOR_DIFFICULTY_3,
				FloatMath.RAD_POS_270,
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
				0f,
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
