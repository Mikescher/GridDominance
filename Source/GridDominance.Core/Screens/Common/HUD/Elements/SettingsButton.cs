using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Operations;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Elements
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

			if (istate.IsRealJustDown && (OpeningState == BistateProgress.Open || OpeningState == BistateProgress.Opening) && !IsPressed && SubButtons != null && !SubButtons.Any(p => p.IsPressed) && !SubButtons.Any(p => p.Slave.IsPointerDownOnElement))
			{
				// Close when clicked somewhere else
				Close();
			}
		}

		public void Close()
		{
			if (OpeningState == BistateProgress.Closing) return;
			if (OpeningState == BistateProgress.Closed) return;

			if (OpeningState == BistateProgress.Opening) RemoveAllOperations();

			AddOperation(new HUDSettingsBaseCloseOperation());

			for (int i = 0; i < 11; i++)
			{
				AddOperation(new HUDSettingsCloseOperation(i));
				AddOperation(new HUDSettingsFontCloseOperation(i));
			}
		}

		public void Open()
		{
			if (OpeningState != BistateProgress.Closed) return;

			AddCagedOperationSequence<SettingsButton>(
				e => e.OpeningState = BistateProgress.Opening,
				e => e.OpeningState = BistateProgress.Open,
				new HUDSettingsOpenOperation(),
				new HUDSettingsFontAppearOperation(0),
				new HUDSettingsFontAppearOperation(1),
				new HUDSettingsFontAppearOperation(2),
				new HUDSettingsFontAppearOperation(3),
				new HUDSettingsFontAppearOperation(4),
				new HUDSettingsFontAppearOperation(5),
				new HUDSettingsFontAppearOperation(6),
				new SleepOperation<SettingsButton>(0.25f),
				new HUDSettingsHorizontalOpenOperation(7, 0),
				new SleepOperation<SettingsButton>(0.10f),
				new HUDSettingsSlantedOpenOperation(8, 1),
				new SleepOperation<SettingsButton>(0.10f),
				new HUDSettingsSlantedOpenOperation(9, 2),
				new SleepOperation<SettingsButton>(0.10f),
				new HUDSettingsSlantedOpenOperation(10, 3)
			);
		}

		protected override void OnPress(InputState istate)
		{
			if (OpeningState == BistateProgress.Closed)
			{
				Open();
			}
			else if (OpeningState == BistateProgress.Open || OpeningState == BistateProgress.Opening)
			{
				Close();
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

		public void CreateSubButtons()
		{
			SubButtons = new SubSettingButton[11];

			SubButtons[0]  = new ButtonVolume(    this, SSBOrientation.V, 0);
			SubButtons[1]  = new ButtonMusic(     this, SSBOrientation.V, 1);
			SubButtons[2]  = new ButtonAccount(   this, SSBOrientation.V, 2);
			SubButtons[3]  = new ButtonHighscore( this, SSBOrientation.V, 3);
			SubButtons[4]  = new ButtonEffects(   this, SSBOrientation.V, 4);
			SubButtons[5]  = new ButtonColorblind(this, SSBOrientation.V, 5);
			SubButtons[6]  = new ButtonLanguage(  this, SSBOrientation.V, 6);

			SubButtons[7]  = new ButtonShare(     this, SSBOrientation.H, 0);
			SubButtons[8]  = new ButtonReddit(    this, SSBOrientation.H, 1);
			SubButtons[9]  = new ButtonBFB(       this, SSBOrientation.H, 2);
			SubButtons[10] = new ButtonAbout(     this, SSBOrientation.H, 3);

			HUD.AddElements(SubButtons);
		}
	}
}
