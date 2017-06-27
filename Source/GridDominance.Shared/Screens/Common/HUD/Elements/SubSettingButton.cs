using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public abstract class SubSettingButton : HUDEllipseButton
	{
		private const float DIAMETER  =  96 * 0.8f;
		private const float SIZE_ICON =  56 * 0.8f;
		private const float MARGIN_Y  =  6  * 0.8f;
		private const float DIST_Y    = -4  * 0.8f;

		public override int Depth => 1;

		private readonly SettingsButton master;
		public readonly SubSettingClickZone Slave;
		private readonly int position;

		public float OffsetProgress = 0;
		public float ScaleProgress = 1;
		public float IconScale = 1;
		public float FontProgress = 0;

		protected SubSettingButton(SettingsButton master, int position, float icscale)
		{
			this.master = master;
			this.position = position;
			Slave = new SubSettingClickZone(this);
			IconScale = icscale;

			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		protected abstract TextureRegion2D GetIcon();
		protected abstract string ButtonText { get; }

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawCentered(Textures.TexHUDButtonBase, Center, DIAMETER * ScaleProgress, DIAMETER * ScaleProgress, ColorMath.Blend(FlatColors.Asbestos, FlatColors.Alizarin, OffsetProgress*ScaleProgress));

			sbatch.DrawCentered(GetIcon(), Center, SIZE_ICON * ScaleProgress * IconScale, SIZE_ICON * ScaleProgress * IconScale, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds);

			FontRenderHelper.DrawTextVerticallyCenteredWithBackground(sbatch, Textures.HUDFontRegular, SIZE_ICON, ButtonText, FlatColors.Clouds * FontProgress, new FPoint(CenterX + SIZE_ICON, CenterY), Color.Black * 0.5f * FontProgress);
		}

		public override void OnInitialize()
		{
			var px = master.RelativeCenter.X - DIAMETER / 2;
			var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

			py += MARGIN_Y;

			RelativePosition = new FPoint(px, py);

			var bounds = FontRenderHelper.MeasureStringCached(Textures.HUDFontRegular, ButtonText);
			var scale = FontRenderHelper.GetFontScale(Textures.HUDFontRegular, SIZE_ICON);
			Slave.Size = bounds * scale;

			Owner.AddElement(Slave);
		}

		public override void OnRemove()
		{
			Slave.Remove();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			var px = master.RelativeCenter.X - DIAMETER / 2;
			var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

			py += MARGIN_Y;
			py += (position + 1) * (DIAMETER + DIST_Y) * OffsetProgress;

			RelativePosition = new FPoint(px, py);

			Slave.RelativePosition = new FPoint(CenterX + SIZE_ICON, CenterY);
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

		public void SlavePress(InputState istate)
		{
			OnPress(istate);
		}
	}

	class HUDButtonAbout : SubSettingButton
	{
		public HUDButtonAbout(SettingsButton master) : base(master, 0, 1f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAbout;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_ABOUT);

		protected override void OnPress(InputState istate)
		{
			var hud = (ISettingsOwnerHUD)HUD;

			hud.ShowAboutPanel();
		}
	}

	class ButtonAccount : SubSettingButton
	{
		public ButtonAccount(SettingsButton master) : base(master, 1, 1f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAccount;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_ACCOUNT);

		protected override void OnPress(InputState istate)
		{
			var hud = (ISettingsOwnerHUD) HUD;

			hud.ShowAccountPanel();
		}
	}

	class ButtonHighscore : SubSettingButton
	{
		public ButtonHighscore(SettingsButton master) : base(master, 2, 1f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconHighscore;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_HIGHSCORE);

		protected override void OnPress(InputState istate)
		{
			var hud = (ISettingsOwnerHUD)HUD;

			hud.ShowHighscorePanel();
		}
	}

	class ButtonVolume : SubSettingButton
	{
		public ButtonVolume(SettingsButton master) : base(master, 3, 1f) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.SoundsEnabled ? Textures.TexHUDButtonIconVolumeOn : Textures.TexHUDButtonIconVolumeOff;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_MUTE);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.SoundsEnabled = !MainGame.Inst.Profile.SoundsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}

	class ButtonMusic : SubSettingButton
	{
		public ButtonMusic(SettingsButton master) : base(master, 4, 1f) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.MusicEnabled ? Textures.TexHUDButtonIconMusicOn : Textures.TexHUDButtonIconMusicOff;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_MUSIC);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.MusicEnabled = !MainGame.Inst.Profile.MusicEnabled;
			MainGame.Inst.SaveProfile();
		}
	}

	class ButtonEffects : SubSettingButton
	{
		public ButtonEffects(SettingsButton master) : base(master, 5, 1f) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.EffectsEnabled ? Textures.TexHUDButtonIconEffectsOn : Textures.TexHUDButtonIconEffectsOff;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_EFFECTS);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.EffectsEnabled = !MainGame.Inst.Profile.EffectsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}

	class ButtonLanguage : SubSettingButton
	{
		public ButtonLanguage(SettingsButton master) : base(master, 6, 0.85f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDFlags[FloatMath.IClamp(MainGame.Inst.Profile.Language, 0, L10N.LANG_COUNT)];
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_LANGUAGE);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.Language = (MainGame.Inst.Profile.Language + 1) % L10N.LANG_COUNT;
			MainGame.Inst.SaveProfile();
			L10N.ChangeLanguage(MainGame.Inst.Profile.Language);
		}
	}
}
