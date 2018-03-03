using System;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.LevelEditorScreen.HUD.Elements
{
	class LevelEditorConfigPanel : HUDContainer
	{
		public override int Depth => 10;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		private readonly SCCMLevelData _data;

		private Int64 _id;
		private int _music;
		private string _name;
		private DSize _size;
		private FlatAlign9 _view;
		private GameWrapMode _geometry;

		private HUDLabel _ctrlID;
		private HUDTextBox _ctrlName;
		private HUDTextButton _ctrlSize;
		private HUDTextButton _ctrlView;
		private HUDTextButton _ctrlGeometry;
		private HUDTextButton _ctrlMusic;

		public LevelEditorConfigPanel(SCCMLevelData data) : base()
		{
			_data = data;

			_id = _data.OnlineID;
			_name = _data.Name;
			_size = _data.Size;
			_view = _data.View;
			_geometry = _data.Geometry;
			_music = data.Music;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawSimpleBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, 16);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void RecalculatePosition()
		{
			base.RecalculatePosition();

			RelativePosition = new FPoint(GDConstants.TILE_WIDTH, 0);
			Size = new FSize(HUD.Width - 6 * GDConstants.TILE_WIDTH, HUD.Height - 2 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTERLEFT;
		}

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(GDConstants.TILE_WIDTH, 0);
			Size = new FSize(HUD.Width - 6 * GDConstants.TILE_WIDTH, HUD.Height - 2 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTERLEFT;

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 0)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_ID,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlID = new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 0)),
				Size = new FSize(512, 64),

				Text = "000000000000",
				TextColor = Color.Black,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,

				Background = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 1)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_NAME,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlName = new HUDSimpleTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 1)),
				Size = new FSize(512, 64),

				ColorText = Color.Black,
				Font = Textures.HUDFontRegular,
				FontSize = 48,

				MaxLength = SCCMLevelData.MaxNameLength,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
				BackgroundFocused = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),

				Changed = UpdateText,
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 2)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_SIZE,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlSize = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 2)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = ToggleSize,
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 3)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_VIEW,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlView = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 3)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = ToggleView,
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 4)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_GEOMETRY,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlGeometry = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 4)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = ToggleGeometry,
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 5)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_MUSIC,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlMusic = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 5)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = ToggleMusic,
			});

			//------

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24 + 384 + 24, 16 + 75 * 0),
				Size = new FSize(384, 64),

				L10NText = L10NImpl.STR_MENU_CANCEL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Pomegranate, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Alizarin, Color.Black, HUD.PixelWidth),

				Click = Abort,
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 16 + 75 * 0),
				Size = new FSize(384, 64),

				L10NText = L10NImpl.STR_BTN_OK,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Emerald, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Nephritis, Color.Black, HUD.PixelWidth),

				Click = Close,
			});

			RefreshControls();
		}

		private void UpdateText(HUDTextBox sender, EventArgs e)
		{
			_name = _ctrlName.Text;
		}

		private void ToggleMusic(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_music = (((_music+1)+1) % MainGame.Inst.GDSound.LevelMusic.Length) - 1;
			
			if (_music != -1) 
				MainGame.Inst.GDSound.PlayMusicLevel(_music);
			else
				MainGame.Inst.GDSound.StopSong();

			RefreshControls();
		}

		private void ToggleSize(HUDTextButton sender, HUDButtonEventArgs e)
		{
			_size = SCCMLevelData.SIZES[(SCCMLevelData.SIZES.IndexOf(_size) + 1) % SCCMLevelData.SIZES.Length];
			if (_size == SCCMLevelData.SIZES[0]) _view = FlatAlign9.CC;

			RefreshControls();
		}

		private void ToggleGeometry(HUDTextButton sender, HUDButtonEventArgs e)
		{
			switch (_geometry)
			{
				case GameWrapMode.Death:   _geometry = GameWrapMode.Donut;   break;
				case GameWrapMode.Donut:   _geometry = GameWrapMode.Reflect; break;
				case GameWrapMode.Reflect: _geometry = GameWrapMode.Death;   break;
				default:                   _geometry = GameWrapMode.Donut;   break;
			}

			RefreshControls();
		}

		private void ToggleView(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (_size == SCCMLevelData.SIZES[0])
			{
				_view = FlatAlign9.CC;
			}
			else
			{
				switch (_view)
				{
					case FlatAlign9.TOP:         _view = FlatAlign9.TOPRIGHT;    break;
					case FlatAlign9.TOPRIGHT:    _view = FlatAlign9.RIGHT;       break;
					case FlatAlign9.RIGHT:       _view = FlatAlign9.BOTTOMRIGHT; break;
					case FlatAlign9.BOTTOMRIGHT: _view = FlatAlign9.BOTTOM;      break;
					case FlatAlign9.BOTTOM:      _view = FlatAlign9.BOTTOMLEFT;  break;
					case FlatAlign9.BOTTOMLEFT:  _view = FlatAlign9.LEFT;        break;
					case FlatAlign9.LEFT:        _view = FlatAlign9.TOPLEFT;     break;
					case FlatAlign9.TOPLEFT:     _view = FlatAlign9.CENTER;      break;
					case FlatAlign9.CENTER:      _view = FlatAlign9.TOP;         break;
					default:                     _view = FlatAlign9.CENTER;      break;
				}
			}

			RefreshControls();
		}

		private void RefreshControls()
		{
			_ctrlID.Text    = $"{_id:000000000000}";
			_ctrlName.Text  = _name;
			_ctrlSize.Text  = _size.Width + "x"+_size.Height;
			_ctrlView.Text  = FlatAlign9Helper.LETTERS[_view];
			_ctrlMusic.Text = _music < 0 ? L10N.T(L10NImpl.STR_MUSIC_NONE) : L10N.TF(L10NImpl.STR_MUSIC_INT, _music + 1);
			switch (_geometry)
			{
				case GameWrapMode.Death:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_INFINITY;
					break;
				case GameWrapMode.Donut:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_DONUT;
					break;
				case GameWrapMode.Reflect:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_REFLECT;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Abort(HUDTextButton sender, HUDButtonEventArgs e)
		{
			this.Remove();
		}

		private void Close(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_name))
			{
				HUD.ShowToast("LECP::NONAME", L10N.T(L10NImpl.STR_LVLED_ERR_NONAME), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				return;
			}

			_data.Name = _name;
			_data.Size = _size;
			_data.View = _view;
			_data.Geometry = _geometry;
			_data.Music = _music;

			this.Remove();
		}

		public override void OnRemove()
		{
			MainGame.Inst.GDSound.PlayMusicBackground();
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
