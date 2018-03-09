using System;
using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.SCCM;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMListElementNewUserLevel : SCCMListElement
	{
		public SCCMListElementNewUserLevel()
		{

		}

		public override void OnInitialize()
		{
			AddElement(new HUDTextButton
			{
				RelativePosition = new FPoint(8, 0),
				Size = new FSize(384, 48),
				Alignment = HUDAlignment.CENTERRIGHT,

				L10NText = L10NImpl.STR_LVLED_BTN_NEWLVL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 32,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = CreateNewUserLevel,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDImage
			{
				RelativePosition = new FPoint(8, 0),
				Size = new FSize(48, 48),
				Alignment = HUDAlignment.CENTERLEFT,

				Image = Textures.CannonCog,
				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.STRETCH,
				RotationSpeed = 0.10f,
				Color = FlatColors.Asbestos,
			});
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Clouds);
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);
		}

		private void CreateNewUserLevel(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (MainGame.Inst.Profile.AccountType == AccountType.Local)
			{
				CreateUserAndShowAnonPanel().EnsureNoError();
				return;
			}
			else if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
			{
				ShowAnonPanel();
				HUD.ShowToast(null, L10N.T(L10NImpl.STR_SCCM_NEEDS_ACC), 40, FlatColors.Orange, FlatColors.Foreground, 2f);
				return;
			}

			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_GENERIC_SERVER_QUERY,
				TextColor = FlatColors.TextHUD,
				Background = HUDBackgroundDefinition.CreateRounded(FlatColors.BelizeHole, 16),

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCogBig,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			HUD.AddModal(waitDialog, false, 0.7f);

			CreateNewUserLevelInternal(waitDialog).RunAsync();
		}

		private async Task CreateNewUserLevelInternal(HUDElement spinner)
		{
			try
			{
				var r = await MainGame.Inst.Backend.GetNewCustomLevelID(MainGame.Inst.Profile);

				if (!r.Item1)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						spinner.Remove();
						HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
						{
							L10NText = L10NImpl.STR_CPP_COMERR,
							TextColor = FlatColors.Clouds,
							Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

							CloseOnClick = true,

						}, true);
					});
					return;
				}

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					MainGame.Inst.SetLevelEditorScreen(new SCCMLevelData(r.Item2, MainGame.Inst.Profile.OnlineUserID));
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMLENUL::DL", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					spinner.Remove();
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_CPP_COMERR,
						TextColor = FlatColors.Clouds,
						Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

						CloseOnClick = true,

					}, true);
				});
			}
		}
		
		private void ShowAnonPanel()
		{
			if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
			{
				HUD.AddModal(new AnonymousAccountPanel(), true);
			}
		}

		private async Task CreateUserAndShowAnonPanel()
		{
			var waitDialog = new HUDIconMessageBox
			{
				L10NText = L10NImpl.STR_GLOB_WAITFORSERVER,
				TextColor = FlatColors.TextHUD,
				Background = HUDBackgroundDefinition.CreateRounded(FlatColors.BelizeHole, 16),

				IconColor = FlatColors.Clouds,
				Icon = Textures.CannonCogBig,
				RotationSpeed = 1f,

				CloseOnClick = false,
			};

			MainGame.Inst.DispatchBeginInvoke(() => { HUD.AddModal(waitDialog, false, 0.7f); });

			await MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile);

			waitDialog.Remove();

			MainGame.Inst.DispatchBeginInvoke(() =>
			{
				if (MainGame.Inst.Profile.AccountType == AccountType.Anonymous)
				{
					HUD.AddModal(new AnonymousAccountPanel(), true);
				}
			});
		}
	}
}
