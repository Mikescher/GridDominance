using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUDOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class HUDTutorialDifficultyButton : HUDEllipseButton
	{
		public override int Depth { get; }

		private readonly TextureRegion2D icon;

		public Color BackgroundColor;
		public Color ForegroundColor;
		public float IconScale = 1f;

		public HUDTutorialDifficultyButton(int depth)
		{
			Depth = depth;

			icon = Textures.TexIconTutorial;

			BackgroundColor = FlatColors.BackgroundHUD2;
			ForegroundColor = FlatColors.SunFlower;
			AddOperation(new HUDTutorialDifficultyButtonBlinkingIconOperation());
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
			// nothing in tutorial
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
