using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.Elements
{
	public class HUDSCCMUploadDifficultyButton : HUDEllipseButton
	{
		public override int Depth { get; }

		private readonly TextureRegion2D icon;

		public Color BackgroundColor;
		public Color ForegroundColor;
		public float IconScale = 1f;

		public HUDSCCMUploadDifficultyButton(int depth, bool active, int idx)
		{
			Depth = depth;

			icon = Textures.TexIconInternet;

			if (active)
			{
				BackgroundColor = FlatColors.ButtonHUD;
				ForegroundColor = FlatColors.SunFlower;
				IconScale = 0f;

				AddOperationDelayed(new HUDSCCMUploadButtonGainOperation(), 0.75f + idx * 0.6f);
				AddOperationDelayed(new HUDSCCMUploadButtonBlinkingIconOperation(), 0.75f + idx * 0.6f);
			}
			else
			{
				BackgroundColor = FlatColors.ButtonHUD;
				ForegroundColor = FlatColors.BackgroundHUD;
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
			// nothing in sccm
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
