using System.Linq;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public class SettingsButton : HUDEllipseButton
	{
		public const float DIAMETER   = 124 * 0.8f;
		private const float SIZE_ICON = 72  * 0.8f;

		public override int Depth => 1;

		private float rotation = 0f;

		public float RotationSpeed = 1f;
		public float OpeningProgress = 0f;
		public SubSettingButton[] SubButtons;

		public BistateProgress OpeningState = BistateProgress.Closed;

		public SettingsButton()
		{
			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
			ClickMode = HUDButtonClickMode.Single;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawCentered(Textures.TexHUDButtonBase, Center, DIAMETER, DIAMETER, ColorMath.Blend(FlatColors.Alizarin, FlatColors.Asbestos, OpeningProgress));

			sbatch.DrawCentered(Textures.TexHUDButtonIconSettings, Center, SIZE_ICON, SIZE_ICON, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds, rotation);
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			rotation += gameTime.ElapsedSeconds * RotationSpeed;

			if (istate.IsRealJustDown && OpeningState == BistateProgress.Open && !IsPressed && SubButtons != null && !SubButtons.Any(p => p.IsPressed) && !SubButtons.Any(p => p.Slave.IsPointerDownOnElement))
			{
				// Close when clicked somewhere else
				AddHUDOperation(new HUDSettingsCloseOperation());
			}
		}

		public void Close()
		{
			if (OpeningState == BistateProgress.Closing) return;
			if (OpeningState == BistateProgress.Closed) return;

			if (OpeningState == BistateProgress.Opening) RemoveAllOperations();

			AddHUDOperation(new HUDSettingsCloseOperation());
		}

		protected override void OnPress(InputState istate)
		{
			if (OpeningState == BistateProgress.Closed)
			{
				AddCagedHUDOperationSequence<SettingsButton>(
					e => e.OpeningState = BistateProgress.Opening,
					e => e.OpeningState = BistateProgress.Open,
					new HUDSettingsOpenOperation(),
					new HUDSettingsFontAppearOperation(0),
					new HUDSettingsFontAppearOperation(1),
					new HUDSettingsFontAppearOperation(2),
					new HUDSettingsFontAppearOperation(3),
					new HUDSettingsFontAppearOperation(4),
					new HUDSettingsFontAppearOperation(5),
					new HUDSettingsFontAppearOperation(6)
					);
			}
			else if (OpeningState == BistateProgress.Open)
			{
				AddHUDOperation(new HUDSettingsCloseOperation());
			}
		}

		protected override void OnDoublePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnTriplePress(InputState istate)
		{
			// Not Available
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			// Not Available
		}
	}
}
