using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Tabs
{
	class SCCMTabSearch : HUDContainer
	{
		public override int Depth => 0;

		private HUDSimpleTextBox _textbox;
		private SCCMListPresenter _presenter;
		private SCCMListScrollbar _scrollbar;
		private HUDImage _waitingCog;

		public SCCMTabSearch()
		{

		}

		public override void OnInitialize()
		{
			AddElement(_textbox = new HUDSimpleTextBox
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(16, 16),
				Size = new FSize(Width - 16 - 16 - 64, 64),

				ColorText = Color.Black,
				Font = Textures.HUDFontRegular,
				FontSize = 48,

				MaxLength = SCCMLevelData.MaxNameLength,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, HUD.PixelWidth),
				BackgroundFocused = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, HUD.PixelWidth),

				EnterKey = (s, e) => StartSearch(),

				CloseKeyboard = (s, e) => StartSearch(),
			});
			
			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPRIGHT,
				RelativePosition = new FPoint(16 + HUD.PixelWidth, 16),
				Size = new FSize(64, 64),

				Image = Textures.TexHUDButtonIconMagnifier,
				ImagePadding = 8,
				ImageColor = FlatColors.Asbestos,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Concrete, Color.Black, HUD.PixelWidth),

				Click = (s, e) => StartSearch(),
			});

			AddElement(_presenter = new SCCMListPresenter
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(16, 16),
				Size = new FSize(Width - 16 - 16 - 48 - 16, Height - 16 - 16 - 16 - 64),

				IsVisible = true,
			});

			AddElement(_scrollbar = new SCCMListScrollbar
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(16, 16),
				Size = new FSize(48, Height - 16 - 16 - 16 - 64),

				IsVisible = true,
			});

			AddElement(_waitingCog = new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 16+32),
				Image = Textures.CannonCogBig,
				RotationSpeed = 0.35f,
				Color = FlatColors.Clouds,
				Size = new FSize(192, 192),

				IsVisible = false,
			});

			_presenter.Load(QueryData, _scrollbar, _waitingCog);
		}

		private void StartSearch()
		{
			_presenter.Load(QueryData, _scrollbar, _waitingCog);
		}
		
		private async Task<SCCMListPresenter.LoadFuncResult> QueryData(SCCMListPresenter list, int page, int reqid)
		{
			var txt =_textbox.Text;

			if (string.IsNullOrWhiteSpace(txt)) return SCCMListPresenter.LoadFuncResult.LastPage;

			try
			{
				var r = await MainGame.Inst.Backend.QueryUserLevel(MainGame.Inst.Profile, QueryUserLevelCategory.Search, txt, page);

				if (r == null)
				{
					return SCCMListPresenter.LoadFuncResult.Error;
				}
				else
				{
					if (r.Count == 0)
					{
						return SCCMListPresenter.LoadFuncResult.LastPage;
					}
					else
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							if (list.IsCurrentRequest(reqid))
							{
								foreach (var levelmeta in r)
								{
									_presenter.AddEntry(new SCCMListElementOnlinePlayable(levelmeta));
								}
							}
							else
							{
								SAMLog.Warning("SCCMTS::DirtyUpdate2", "Dirty presenter update ignored (response=ok)");
							}

						});
						
						return SCCMListPresenter.LoadFuncResult.Success;
					}
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMTS::QD", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_CPP_COMERR,
						TextColor = FlatColors.Clouds,
						Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

						CloseOnClick = true,

					}, true);
				});
						
				return SCCMListPresenter.LoadFuncResult.Error;
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
