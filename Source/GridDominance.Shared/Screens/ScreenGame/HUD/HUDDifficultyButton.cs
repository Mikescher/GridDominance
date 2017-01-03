using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.ScreenGame.HUDOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	public class HUDDifficultyButton : HUDEllipseButton
	{
		public enum HUDDifficultyButtonMode { DEACTIVATED, ACTIVATED, UNLOCKANIMATION}

		public override int Depth { get; }

		private readonly FractionDifficulty difficulty;
		private readonly TextureRegion2D icon;

		public Color BackgroundColor;
		public Color ForegroundColor;
		public float IconScale = 1f;

		public HUDDifficultyButton(int depth, FractionDifficulty diff, HUDDifficultyButtonMode mode)
		{
			Depth = depth;
			difficulty = diff;

			icon = FractionDifficultyHelper.GetIcon(diff);

			switch (mode)
			{
				case HUDDifficultyButtonMode.DEACTIVATED:
					BackgroundColor = FlatColors.ButtonHUD;
					ForegroundColor = FlatColors.BackgroundHUD;
					break;
				case HUDDifficultyButtonMode.UNLOCKANIMATION:
					BackgroundColor = FlatColors.ButtonHUD;
					ForegroundColor = GDColors.GetColorForDifficulty(diff);
					AddHUDOperation(new HUDDifficultyButtonGainOperation());
					AddHUDOperation(new HUDBlinkingDifficultyButtonIconOperation());
					break;
				case HUDDifficultyButtonMode.ACTIVATED:
					BackgroundColor = FlatColors.BackgroundHUD2;
					ForegroundColor = GDColors.GetColorForDifficulty(diff);
					AddHUDOperation(new HUDBlinkingDifficultyButtonIconOperation());
					break;
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (IsPointerDownOnElement)
			{
				sbatch.Draw(Textures.TexCircle, bounds, BackgroundColor.Darken());
			}
			else
			{
				sbatch.Draw(Textures.TexCircle, bounds, BackgroundColor);
			}

			sbatch.Draw(icon, bounds.AsDeflated(24, 24).AsScaled(IconScale), ForegroundColor);
		}

		public override void OnInitialize()
		{
		}

		public override void OnRemove()
		{
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
		}

		protected override void OnPress(InputState istate)
		{
			((GDGameScreen)HUD.Screen).ReplayLevel(FractionDifficulty.KI_EASY);
		}

		protected override void OnDoublePress(InputState istate)
		{
		}

		protected override void OnTriplePress(InputState istate)
		{
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
		}
	}
}
