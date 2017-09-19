using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD;
using GridDominance.Shared.Screens.Common.HUD.Operations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
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
	public enum SSBOrientation { H, V }

	public abstract class SubSettingButton : HUDEllipseButton
	{
		public  const float DIAMETER     =  96 * 0.8f;
		private const float SIZE_ICON    =  56 * 0.8f;
		private const float MARGIN_FIRST =  6  * 0.8f;
		private const float DIST_Y       = -4  * 0.8f;

		public override int Depth => 1;

		private readonly SettingsButton master;
		public readonly SubSettingClickZone Slave;
		private readonly int position;
		private readonly SSBOrientation orientation;

		public float OffsetProgress = 0;
		public float ScaleProgress = 1;
		public float FontProgress = 0;
		public float IconRotation = 0f;
		
		public readonly float IconScale;

		protected SubSettingButton(SettingsButton master, SSBOrientation orientation, int position, float icscale)
		{
			this.master = master;
			this.position = position;
			this.orientation = orientation;
			Slave = new SubSettingClickZone(this);
			IconScale = icscale;

			RelativePosition = new FPoint(8, 8);
			Size = new FSize(DIAMETER, DIAMETER);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		protected abstract TextureRegion2D GetIcon();
		protected abstract string ButtonText { get; }

		public FPoint TargetPosition
		{
			get
			{
				if (orientation == SSBOrientation.V)
					return new FPoint(master.RelativeCenter.X - DIAMETER / 2, master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER + MARGIN_FIRST + (position + 1) * (DIAMETER + DIST_Y));

				if (orientation == SSBOrientation.H)
					return new FPoint(master.RelativeCenter.X + SettingsButton.DIAMETER / 2 - DIAMETER + MARGIN_FIRST + (position + 1) * (DIAMETER + DIST_Y), master.RelativeCenter.Y - DIAMETER / 2);
				return new FPoint();
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			sbatch.DrawCentered(Textures.TexHUDButtonBase, Center, DIAMETER * ScaleProgress, DIAMETER * ScaleProgress, ColorMath.Blend(FlatColors.Asbestos, FlatColors.Alizarin, OffsetProgress*ScaleProgress));

			sbatch.DrawCentered(GetIcon(), Center, SIZE_ICON * ScaleProgress * IconScale, SIZE_ICON * ScaleProgress * IconScale, IsPressed ? FlatColors.WetAsphalt : FlatColors.Clouds, IconRotation);

			if (!string.IsNullOrWhiteSpace(ButtonText))
				FontRenderHelper.DrawTextVerticallyCenteredWithBackground(sbatch, Textures.HUDFontRegular, SIZE_ICON, ButtonText, FlatColors.Clouds * FontProgress, new FPoint(CenterX + SIZE_ICON, CenterY), Color.Black * 0.5f * FontProgress);
		}

		public override void OnInitialize()
		{
			if (orientation == SSBOrientation.V)
			{
				var px = master.RelativeCenter.X - DIAMETER / 2;
				var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

				py += MARGIN_FIRST;

				RelativePosition = new FPoint(px, py);

				var bounds = FontRenderHelper.MeasureStringCached(Textures.HUDFontRegular, ButtonText);
				var scale = FontRenderHelper.GetFontScale(Textures.HUDFontRegular, SIZE_ICON);
				Slave.Size = bounds * scale;
				Owner.AddElement(Slave);

				Slave.IsEnabled = !string.IsNullOrWhiteSpace(ButtonText);
			}
			else if (orientation == SSBOrientation.H)
			{
				var px = master.RelativeCenter.X - DIAMETER / 2;
				var py = master.RelativeCenter.Y - DIAMETER / 2;

				py += MARGIN_FIRST;

				RelativePosition = new FPoint(px, py);

				var bounds = FontRenderHelper.MeasureStringCached(Textures.HUDFontRegular, ButtonText);
				bounds = bounds.Rotate90();

				var scale = FontRenderHelper.GetFontScale(Textures.HUDFontRegular, SIZE_ICON);
				Slave.Size = bounds * scale;
				Owner.AddElement(Slave);

				Slave.IsEnabled = !string.IsNullOrWhiteSpace(ButtonText);
			}
		}

		public override void OnRemove()
		{
			Slave.Remove();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (orientation == SSBOrientation.V)
			{
				var px = master.RelativeCenter.X - DIAMETER / 2;
				var py = master.RelativeCenter.Y + SettingsButton.DIAMETER / 2 - DIAMETER;

				py += MARGIN_FIRST;
				py += (position + 1) * (DIAMETER + DIST_Y) * OffsetProgress;

				RelativePosition = new FPoint(px, py);

				Slave.RelativePosition = new FPoint(CenterX + SIZE_ICON, CenterY);
			}
			else if (orientation == SSBOrientation.H)
			{
				Slave.RelativePosition = new FPoint(CenterX, CenterY + SIZE_ICON);
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

		public void SlavePress(InputState istate)
		{
			OnPress(istate);
		}
	}

	class ButtonAbout : SubSettingButton
	{
		public ButtonAbout(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAbout;
		protected override string ButtonText => string.Empty;

		protected override void OnPress(InputState istate)
		{
			var hud = (ISettingsOwnerHUD)HUD;

			hud.ShowAboutPanel();
		}
	}

	class ButtonAccount : SubSettingButton
	{
		public ButtonAccount(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f)
		{
			if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous) AddHUDOperation(new SubSettingsButtonShakeOperation());
		}

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
		public ButtonHighscore(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f) { }

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
		public ButtonVolume(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f) { }

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
		public ButtonMusic(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f) { }

		protected override TextureRegion2D GetIcon() => (MainGame.Inst.Profile.MusicEnabled && MainGame.Inst.Profile.SoundsEnabled) ? Textures.TexHUDButtonIconMusicOn : Textures.TexHUDButtonIconMusicOff;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_MUSIC);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.MusicEnabled = !MainGame.Inst.Profile.MusicEnabled;
			if (MainGame.Inst.Profile.MusicEnabled && !MainGame.Inst.Profile.SoundsEnabled) MainGame.Inst.Profile.SoundsEnabled = true;
			MainGame.Inst.SaveProfile();
		}
	}

	class ButtonEffects : SubSettingButton
	{
		public ButtonEffects(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1f) { }

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
		public ButtonLanguage(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 0.85f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDFlags[FloatMath.IClamp(MainGame.Inst.Profile.Language, 0, L10N.LANG_COUNT)];
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_LANGUAGE);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.Language = (MainGame.Inst.Profile.Language + 1) % L10N.LANG_COUNT;
			MainGame.Inst.SaveProfile();
			L10N.ChangeLanguage(MainGame.Inst.Profile.Language);
		}
	}

	class ButtonShare : SubSettingButton
	{
		public ButtonShare(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1.20f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconShare;
		protected override string ButtonText => string.Empty;

		protected override void OnPress(InputState istate)
		{
			MonoSAMGame.CurrentInst.Bridge.ShareAppLink();
		}
	}

	class ButtonReddit : SubSettingButton
	{
		public ButtonReddit(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1.10f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconReddit;
		protected override string ButtonText => string.Empty;

		protected override void OnPress(InputState istate)
		{
			MonoSAMGame.CurrentInst.Bridge.OpenURL(GDConstants.URL_REDDIT);
		}
	}

	class ButtonBFB : SubSettingButton
	{
		public ButtonBFB(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 1.20f) { }

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconBFB;
		protected override string ButtonText => string.Empty;

		protected override void OnPress(InputState istate)
		{
			MonoSAMGame.CurrentInst.Bridge.OpenURL(GDConstants.URL_BLACKFORESTBYTES);
		}
	}

	class ButtonColorblind : SubSettingButton
	{
		public ButtonColorblind(SettingsButton master, SSBOrientation o, int idx) : base(master, o, idx, 0.95f) { }

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.ColorblindMode ? Textures.TexHUDButtonIconColorblind : Textures.TexHUDButtonIconEye;
		protected override string ButtonText => L10N.T(L10NImpl.STR_SSB_COLOR);

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.ColorblindMode = !MainGame.Inst.Profile.ColorblindMode;
			MainGame.Inst.SaveProfile();
		}
	}
}
