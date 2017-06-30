using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.GameMath;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public class OverworldNode_Tutorial : OverworldNode
	{
		public const float ICONSIZE = 1.5f * GDConstants.TILE_WIDTH;
		public const float ICONSIZESWIGGLE = 0.125f * GDConstants.TILE_WIDTH;

		public override bool IsNodeEnabled => true;

		public OverworldNode_Tutorial(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, L10NImpl.STR_WORLD_TUTORIAL, Levels.WORLD_ID_TUTORIAL)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			if (!MainGame.Inst.Profile.GetLevelData(Levels.LEVEL_TUTORIAL.UniqueID).HasAnyCompleted())
			{
				var iconBounds = FRectangle.CreateByCenter(Position, new FSize(ICONSIZE + ICONSIZESWIGGLE * FloatMath.Sin(Lifetime), ICONSIZE + ICONSIZESWIGGLE * FloatMath.Sin(Lifetime)));

				FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
				SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

				sbatch.FillRectangle(innerBounds, FlatColors.Background);

				sbatch.DrawStretched(Textures.TexIconTutorial, iconBounds, Color.White * AlphaOverride);

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
			}
			else
			{
				var iconBounds = FRectangle.CreateByCenter(Position, new FSize(ICONSIZE, ICONSIZE));

				FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
				SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

				sbatch.FillRectangle(innerBounds, FlatColors.Background);

				sbatch.DrawStretched(Textures.TexIconTutorial, iconBounds, FlatColors.Emerald * AlphaOverride);

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));

			}
		}

		protected override void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
			var ownr = ((GDOverworldScreen)Owner);
			if (ownr.IsTransitioning) return;
			ownr.IsTransitioning = true;

			ownr.AddAgent(new TransitionZoomInToTutorialAgent(ownr, this));

			MainGame.Inst.GDSound.PlayEffectZoomIn();
		}
	}
}
