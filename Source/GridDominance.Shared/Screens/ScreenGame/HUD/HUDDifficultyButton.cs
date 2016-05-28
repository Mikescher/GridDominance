using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
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

		public HUDDifficultyButton(int depth, FractionDifficulty diff, HUDDifficultyButtonMode mode)
		{
			Depth = depth;
			difficulty = diff;

			icon = FractionDifficultyHelper.GetIcon(diff);

			switch (mode)
			{
				case HUDDifficultyButtonMode.DEACTIVATED:
				case HUDDifficultyButtonMode.UNLOCKANIMATION:
					BackgroundColor = FlatColors.ButtonHUD;
					ForegroundColor = FlatColors.BackgroundHUD;
					break;
				case HUDDifficultyButtonMode.ACTIVATED:
					BackgroundColor = FlatColors.BackgroundHUD2;
					ForegroundColor = FlatColors.SunFlower;
					break;
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.Draw(Textures.TexCircle, bounds, BackgroundColor);
			sbatch.Draw(icon, bounds.AsInflated(24, 24), ForegroundColor);
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
			//TODO Start with diff
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
