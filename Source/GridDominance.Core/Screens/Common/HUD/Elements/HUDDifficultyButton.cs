using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	public class HUDDifficultyButton : HUDEllipseButton
	{
		public enum HUDDifficultyButtonMode { DEACTIVATED, ACTIVATED, UNLOCKANIMATION}

		public override int Depth { get; }

		private readonly FractionDifficulty difficulty;
		private readonly TextureRegion2D icon;
		private readonly Action action;

		public Color BackgroundColor;
		public Color ForegroundColor;
		public float IconScale = 1f;


		public HUDDifficultyButton(int depth, FractionDifficulty diff, HUDDifficultyButtonMode mode, Action a)
		{
			Depth = depth;
			difficulty = diff;

			icon = FractionDifficultyHelper.GetIcon(diff);
			action = a;

			switch (mode)
			{
				case HUDDifficultyButtonMode.DEACTIVATED:
					BackgroundColor = FlatColors.ButtonHUD;
					ForegroundColor = FlatColors.BackgroundHUD;
					break;
				case HUDDifficultyButtonMode.UNLOCKANIMATION:
					BackgroundColor = FlatColors.ButtonHUD;
					ForegroundColor = FlatColors.SunFlower;
					AddOperation(new HUDDifficultyButtonGainOperation());
					AddOperation(new HUDDifficultyButtonBlinkingIconOperation());
					break;
				case HUDDifficultyButtonMode.ACTIVATED:
					BackgroundColor = FlatColors.BackgroundHUD2;
					ForegroundColor = FlatColors.SunFlower;
					AddOperation(new HUDDifficultyButtonBlinkingIconOperation());
					break;
				default:
					SAMLog.Error("HDB::EnumSwitch_CTR", "value: " + mode);
					break;
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (IsPointerDownOnElement)
			{
				sbatch.DrawStretched(Textures.TexCircle, bounds, BackgroundColor.Darken());
			}
			else
			{
				sbatch.DrawStretched(Textures.TexCircle, bounds, BackgroundColor);
			}

			sbatch.DrawStretched(icon, bounds.AsDeflated(24, 24).AsScaled(IconScale), ForegroundColor);
		}

		public override void OnInitialize()
		{
		}

		public override void OnRemove()
		{
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
		}

		protected override void OnPress(InputState istate)
		{
			action();
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
