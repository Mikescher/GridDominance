using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
{
	public class EnhancedHUDDifficultyButton : HUDEllipseButton
	{
		public override int Depth { get; }

		private readonly TextureRegion2D icon;
		private readonly Action action;

		public float IconScale = 1f;

		public bool Active   = false;
		public bool Selected = false;

		public EnhancedHUDDifficultyButton(int depth, FractionDifficulty diff, Action a)
		{
			Depth = depth;

			icon = FractionDifficultyHelper.GetIcon(diff);
			action = a;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var back = Selected ? FlatColors.BackgroundHUD2 : FlatColors.ButtonHUD;
			var fore = Active ? FlatColors.SunFlower : FlatColors.BackgroundHUD;

			if (IsPointerDownOnElement)
			{
				sbatch.DrawStretched(Textures.TexCircle, bounds, back.Darken());
			}
			else
			{
				sbatch.DrawStretched(Textures.TexCircle, bounds, back);
			}

			sbatch.DrawStretched(icon, bounds.AsDeflated(24, 24).AsScaled(IconScale), fore);
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
