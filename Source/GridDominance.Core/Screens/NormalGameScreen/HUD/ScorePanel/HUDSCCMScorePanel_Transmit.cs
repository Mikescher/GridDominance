using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDSCCMScorePanel_Transmit : HUDRoundedPanel
	{
		public const float TW = GDConstants.TILE_WIDTH;

		public const float WIDTH = 11 * TW;
		public const float HEIGHT = 8.5f * TW;

		private GDGameScreen_SP GDScreen => (GDGameScreen_SP)HUD.Screen;

		public override int Depth => 0;

		private readonly LevelBlueprint _level;
		private readonly FractionDifficulty _difficulty;
		private readonly int _time;

		private HUDImage _cannonCog;
		private HUDLabel _centerLabel;
		private HUDIconTextButton _btnReupload;

		public HUDSCCMScorePanel_Transmit(LevelBlueprint lvl, FractionDifficulty d, int time)
		{
			_level = lvl;
			_difficulty = d;
			_time = time;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			
			AddElement(new HUDLabel
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 48),

				TextAlignment = HUDAlignment.CENTER,
				Text = _level.FullName,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 48,
			});

			AddElement(_cannonCog = new HUDImage(1)
			{
				Alignment = HUDAlignment.CENTER,
				Size = new FSize(4.5f*TW, 4.5f*TW),
				RelativePosition = FPoint.Zero,

				Rotation = 0f,
				RotationSpeed = 0.5f,

				Image = Textures.CannonCogBig,
				Color = FlatColors.BackgroundHUD.Lighten(0.9f),
			});
			
			AddElement(_centerLabel = new HUDLabel(3)
			{
				Alignment = HUDAlignment.CENTER,
				Size = new FSize(WIDTH, 2*TW),
				RelativePosition = FPoint.Zero,

				TextAlignment = HUDAlignment.CENTER,
				FontSize = 128,

				L10NText = L10NImpl.STR_LVLED_UPLOADING,

				TextColor = FlatColors.Clouds,

				Font = Textures.HUDFontBold,
			});
			
			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, 24),
				Size = new FSize(3.5f * TW, 60),

				L10NText = L10NImpl.STR_LVLED_BTN_ABORT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconError,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Pomegranate, 16),

				Click = (s, a) => MainGame.Inst.SetOverworldScreen(),
			});
			
			AddElement(_btnReupload = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 24),
				Size = new FSize(3.5f * TW, 60),

				L10NText = L10NImpl.STR_LVLED_BTN_RETRY,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconError,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Orange, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.SunFlower, 16),

				IsVisible = false,

				Click = (s, a) => StartUpload(),
			});

			StartUpload();
		}

		private void StartUpload()
		{
			_btnReupload.IsVisible = false;
			_cannonCog.Color = FlatColors.BackgroundHUD.Lighten(0.9f);
			_cannonCog.RotationSpeed = 0.5f;
			_centerLabel.IsVisible = true;

			DoUpload().EnsureNoError();
		}

		private async Task DoUpload()
		{
			var result = await MainGame.Inst.Backend.SetCustomLevelCompleted(MainGame.Inst.Profile, _level.CustomMeta_LevelID, _difficulty, _time);

			if (result.IsError)
			{
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					_btnReupload.IsVisible = true;
					_cannonCog.Color = FlatColors.Alizarin;
					_cannonCog.RotationSpeed = 0f;
					_centerLabel.IsVisible = false;
				});
			}
			else
			{
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					Remove();
					
					HUD.AddModal(new HUDSCCMScorePanel_Won(_level, result.Metadata, _difficulty, result.ScoreGain, _time), false);

					if (result.IsFirstClear)
					{
						AchievementPopup.Show(L10N.T(L10NImpl.STR_ACH_FIRSTCLEAR));
					}
					else if (result.IsWorldRecord)
					{
						AchievementPopup.Show(L10N.T(L10NImpl.STR_ACH_WORLDRECORD));
					}

				});
			}
		}
	}
}
