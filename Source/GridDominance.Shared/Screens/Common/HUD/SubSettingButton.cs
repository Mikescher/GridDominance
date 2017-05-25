using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public abstract class SubSettingButton : HUDEllipseButton
	{
		private const float DIAMETER  = 96 * 0.8f;
		private const float SIZE_ICON = 56 * 0.8f;
		private const float MARGIN_X  = 0  * 0.8f;
		private const float MARGIN_Y  = 6  * 0.8f;

		public override int Depth => 1;

		private readonly SettingsButton master;
		private readonly int position;

		public float OffsetProgress = 0;
		public float ScaleProgress = 1;
		public float FontProgress = 0;

		protected SubSettingButton(SettingsButton master, int position)
		{
			this.master = master;
			this.position = position;

			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		protected abstract TextureRegion2D GetIcon();
		protected abstract string GetText();

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawCentered(Textures.TexHUDButtonBase, Center, DIAMETER * ScaleProgress, DIAMETER * ScaleProgress, ColorMath.Blend(FlatColors.Asbestos, FlatColors.Alizarin, OffsetProgress*ScaleProgress));

			sbatch.DrawCentered(GetIcon(), Center, SIZE_ICON * ScaleProgress, SIZE_ICON * ScaleProgress, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds);

			FontRenderHelper.DrawTextVerticallyCenteredWithBackground(sbatch, Textures.HUDFontRegular, SIZE_ICON, GetText(), FlatColors.Clouds * FontProgress, new Vector2(CenterX + SIZE_ICON, CenterY), Color.Black * 0.5f);
		}

		public override void OnInitialize()
		{
			var px = master.RelativeCenter.X - DIAMETER / 2;
			var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

			py += MARGIN_Y;

			RelativePosition = new FPoint(px, py);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			var px = master.RelativeCenter.X - DIAMETER / 2;
			var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

			py += MARGIN_Y;
			py += (position + 1) * (DIAMETER + MARGIN_X / 2) * OffsetProgress;

			RelativePosition = new FPoint(px, py);
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

	class HUDButtonAbout : SubSettingButton
	{
		public HUDButtonAbout(SettingsButton master) : base(master, 0) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAbout;
		protected override string GetText() => "About";

		protected override void OnPress(InputState istate)
		{
			//TODO About
		}
	}

	class ButtonAccount : SubSettingButton
	{
		public ButtonAccount(SettingsButton master) : base(master, 1) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAccount;
		protected override string GetText() => "Account";

		protected override void OnPress(InputState istate)
		{
			var hud = (ISettingsOwnerHUD) HUD;

			hud.ShowAccountPanel();
		}
	}

	class ButtonHighscore : SubSettingButton
	{
		public ButtonHighscore(SettingsButton master) : base(master, 2) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconHighscore;
		protected override string GetText() => "Highscore";

		protected override void OnPress(InputState istate)
		{
			//TODO Show Highscores
		}
	}

	class ButtonVolume : SubSettingButton
	{
		public ButtonVolume(SettingsButton master) : base(master, 3) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.SoundsEnabled ? Textures.TexHUDButtonIconVolumeOn : Textures.TexHUDButtonIconVolumeOff;
		protected override string GetText() => "Mute";

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.SoundsEnabled = !MainGame.Inst.Profile.SoundsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}
	
	class ButtonEffects : SubSettingButton
	{
		public ButtonEffects(SettingsButton master) : base(master, 4) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.EffectsEnabled ? Textures.TexHUDButtonIconEffectsOn : Textures.TexHUDButtonIconEffectsOff;
		protected override string GetText() => "Effects";

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.EffectsEnabled = !MainGame.Inst.Profile.EffectsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}
}
