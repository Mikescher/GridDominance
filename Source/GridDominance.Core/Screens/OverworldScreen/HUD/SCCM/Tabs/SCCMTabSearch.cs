using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
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

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
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
				Size = new FSize(Width-16-16-64-16, 64),

				ColorText = Color.Black,
				Font = Textures.HUDFontRegular,
				FontSize = 48,

				MaxLength = SCCMLevelData.MaxNameLength,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
				BackgroundFocused = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),

				EnterKey = (s, e) => StartSearch(),
			});
			
			AddElement(new HUDImageButton
			{
				Alignment = HUDAlignment.TOPRIGHT,
				RelativePosition = new FPoint(16, 16),
				Size = new FSize(64, 64),

				Image = Textures.TexHUDButtonIconBFB, //TODO TexMagnifier
				ImagePadding = 2,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = (s, e) => StartSearch(),
			});

			AddElement(_presenter = new SCCMListPresenter
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(16, 16),
				Size = Size - new FSize(16 + 16 + 48 + 16, Height - 16 - 16 - 16 - 64),

				IsVisible = true,
			});

			AddElement(_scrollbar = new SCCMListScrollbar
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(16, 16),
				Size = new FSize(48, Height - 16 - 16 - 16 - 64),

				IsVisible = true,
			});

			_presenter.Scrollbar = _scrollbar;
			_scrollbar.Presenter = _presenter;

			_presenter.IsVisible = false;
			_scrollbar.IsVisible = false;

			AddElement(_waitingCog = new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 16+64),
				Image = Textures.CannonCogBig,
				RotationSpeed = 0.35f,
				Color = FlatColors.Clouds,
				Size = new FSize(192, 192),

				IsVisible = false,
			});
		}

		private void StartSearch()
		{
			if (string.IsNullOrWhiteSpace(_textbox.Text))
			{
				QueryData(_textbox.Text, 0).EnsureNoError();
			}
			else
			{
				_waitingCog.IsVisible = false;
				_presenter.Clear();
				_presenter.IsVisible = true;
				_scrollbar.IsVisible = true;
			}
		}

		private async Task QueryData(string txt, int page)
		{
			try
			{
				var r = await MainGame.Inst.Backend.QueryUserLevel(MainGame.Inst.Profile, QueryUserLevelCategory.Search, txt, page);

				if (r == null)
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						_presenter.IsVisible = true;
						_scrollbar.IsVisible = true;
						_waitingCog.IsVisible = false;

						if (_textbox.Text == txt)
						{
							_presenter.Clear();
						}
						else
						{
							SAMLog.Warning("SCCMTS::DirtyUpdate1", "Dirty presenter update ignored (response=null)");
						}
					});
					return;
				}
				else
				{
					MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
					{
						_presenter.IsVisible = true;
						_scrollbar.IsVisible = true;
						_waitingCog.IsVisible = false;

						if (_textbox.Text == txt)
						{
							_presenter.Clear();
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
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMTS::QD", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					_presenter.IsVisible = true;
					_scrollbar.IsVisible = true;
					_waitingCog.IsVisible = false;
					
					if (_textbox.Text == txt)
					{
						_presenter.Clear();
					}
					else
					{
						SAMLog.Warning("SCCMTS::DirtyUpdate3", "Dirty presenter update ignored (response=err)");
					}

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
