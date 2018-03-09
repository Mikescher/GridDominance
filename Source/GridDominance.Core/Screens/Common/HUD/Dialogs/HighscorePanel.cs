using System;
using System.Threading.Tasks;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Network.Backend.QueryResult;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Table;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD.Dialogs
{
	class HighscorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 13 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float TEXT_HEIGHT = 70f;
		public const float TEXT_HEIGHT_REAL = (HEIGHT - TAB_HEIGHT + TEXT_HEIGHT) / 2f;

		public const float TAB_WIDTH     = 12 * GDConstants.TILE_WIDTH;
		public const float TAB_HEIGHT    = 8 * GDConstants.TILE_WIDTH - TEXT_HEIGHT + 25f;
		public const float BOTTOM_HEIGHT = 1 * GDConstants.TILE_WIDTH - 25f;
		
		public override int Depth => 0;

		private readonly GraphBlueprint _focus;
		private readonly HighscoreCategory _mode;

		private HUDImage _loader;
		private HUDScrollTable _table;
		private HUDButton _btnPrev;
		private HUDButton _btnNext;

		public HighscorePanel(GraphBlueprint focus, HighscoreCategory mod)
		{
			_focus = focus;
			_mode = mod;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			string txt = "?";
			switch (_mode)
			{
				case HighscoreCategory.GlobalPoints:
					txt = L10N.T(L10NImpl.STR_HSP_GLOBALRANKING);
					break;
				case HighscoreCategory.WorldPoints:
					txt = L10N.TF(L10NImpl.STR_HSP_RANKINGFOR, L10N.T(Levels.WORLD_NAMES[_focus.ID]));
					break;
				case HighscoreCategory.MultiplayerPoints:
					txt = L10N.T(L10NImpl.STR_HSP_MULTIPLAYERRANKING);
					break;
				case HighscoreCategory.CustomLevelStars:
					txt = L10N.T(L10NImpl.STR_HSP_STARRANKING);
					break;
				case HighscoreCategory.CustomLevelPoints:
					txt = L10N.T(L10NImpl.STR_HSP_SCCMRANKING);
					break;
				default:
					SAMLog.Error("HP::EnumSwitch_OI", "_mode: " + _mode);
					break;
			}

			AddElement(new HUDLabel(1)
			{
				TextAlignment = (txt.Length > 24) ? HUDAlignment.CENTERLEFT : HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(TAB_WIDTH, TEXT_HEIGHT_REAL),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				Text = txt,
				TextColor = FlatColors.Clouds,
			});
			
			AddElement(_btnPrev = new HUDImageButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = FPoint.Zero,
				Size = new FSize(72, 72),

				Image = GetModeIcon(NextCategory(_mode, false)),
				ImageColor = GetModeColor(NextCategory(_mode, false)),
				ImagePadding = 8,
				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.UNDERSCALE,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16, true, false, false, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16, true, false, false, false),

				Click = (s,e) => SwitchMode(false),

				IsVisible = false,
			});
			
			AddElement(_btnNext = new HUDImageButton(1)
			{
				Alignment = HUDAlignment.TOPRIGHT,
				RelativePosition = FPoint.Zero,
				Size = new FSize(72, 72),

				Image = GetModeIcon(NextCategory(_mode, true)),
				ImageColor = GetModeColor(NextCategory(_mode, true)),
				ImagePadding = 8,
				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.UNDERSCALE,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16, false, true, false, false),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16, false, true, false, false),

				Click = (s,e) => SwitchMode(true),

				IsVisible = false,
			});

			_loader = new HUDImage
			{
				RelativePosition = new FPoint(0, TEXT_HEIGHT / 2f),
				Alignment = HUDAlignment.CENTER,
				Size = new FSize(2 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH),
				Color = FlatColors.Clouds,
				Image = Textures.CannonCogBig,

				RotationSpeed = 0.1f,

				IsVisible = true,
			};
			AddElement(_loader);

			_table = new HUDScrollTable
			{
				RelativePosition = new FPoint(0, TEXT_HEIGHT / 2f),
				Alignment = HUDAlignment.CENTER,
				Size = new FSize(TAB_WIDTH, TAB_HEIGHT),

				Background = FlatColors.BackgroundHUD2,
				Foreground = FlatColors.TextHUD,
				LineColor = Color.Black,
				HeaderBackground = FlatColors.Asbestos,
				HeaderForeground = Color.Black,
				ScrollThumbColor = FlatColors.Silver,
				ScrollWidth = 16,
				ScrollHeight = 64,
				LineWidth = 2 * HUD.PixelWidth,
				FontSize = 32,

				IsVisible = false,
			};
			_table.FixHeightToMultipleOfRowHeight();
			AddElement(_table);

			switch (_mode)
			{
				case HighscoreCategory.GlobalPoints:
					_table.AddColumn("", 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_POINTS), 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_TIME), 175);
					break;

				case HighscoreCategory.WorldPoints:
					_table.AddColumn("", 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_POINTS), 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_TIME), 175);
					break;

				case HighscoreCategory.MultiplayerPoints:
					_table.AddColumn("", 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_POINTS), 100);
					break;

				case HighscoreCategory.CustomLevelStars:
					_table.AddColumn("", 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_STARS), 100);
					break;

				case HighscoreCategory.CustomLevelPoints:
					_table.AddColumn("", 100);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
					_table.AddColumn(L10N.T(L10NImpl.STR_TAB_POINTS), 100);
					break;

				default:
					SAMLog.Error("HP::EnumSwitch_OI2", "_mode: " + _mode);
					break;
			}

			LoadHighscore().EnsureNoError();
		}

		private HighscoreCategory NextCategory(HighscoreCategory cat, bool delta)
		{
			if (delta)
			{
				// FWD
				
				switch (cat)
				{
					case HighscoreCategory.GlobalPoints:
						if (_focus != null) return HighscoreCategory.WorldPoints;
						return HighscoreCategory.MultiplayerPoints;

					case HighscoreCategory.WorldPoints:
						return HighscoreCategory.MultiplayerPoints;

					case HighscoreCategory.MultiplayerPoints:
						return HighscoreCategory.CustomLevelStars;

					case HighscoreCategory.CustomLevelStars:
						return HighscoreCategory.CustomLevelPoints;

					case HighscoreCategory.CustomLevelPoints:
						return HighscoreCategory.GlobalPoints;

					default:
						SAMLog.Error("HP::EnumSwitch_SM1", "cat: " + _mode);
						return HighscoreCategory.GlobalPoints;
				}
			}
			else
			{
				// RWD
				
				switch (cat)
				{
					case HighscoreCategory.GlobalPoints:
						return HighscoreCategory.CustomLevelPoints;

					case HighscoreCategory.WorldPoints:
						return HighscoreCategory.GlobalPoints;

					case HighscoreCategory.MultiplayerPoints:
						if (_focus != null) return HighscoreCategory.WorldPoints;
						return HighscoreCategory.GlobalPoints;

					case HighscoreCategory.CustomLevelStars:
						return HighscoreCategory.MultiplayerPoints;

					case HighscoreCategory.CustomLevelPoints:
						return HighscoreCategory.CustomLevelStars;

					default:
						SAMLog.Error("HP::EnumSwitch_SM2", "cat: " + _mode);
						return HighscoreCategory.GlobalPoints;
				}
			}
		}

		private void SwitchMode(bool delta)
		{
			Remove();
			HUD.AddModal(new HighscorePanel(_focus, NextCategory(_mode, delta)), true);
		}

		private Color GetModeColor(HighscoreCategory cat)
		{
			switch (cat)
			{
				case HighscoreCategory.GlobalPoints:
					return Color.White;
				case HighscoreCategory.WorldPoints:
					return Color.White;
				case HighscoreCategory.MultiplayerPoints:
					return FlatColors.Amethyst;
				case HighscoreCategory.CustomLevelStars:
					return FlatColors.SunFlower;
				case HighscoreCategory.CustomLevelPoints:
					return FlatColors.Alizarin;
				default:
					SAMLog.Error("HP::EnumSwitch_GMC", "cat: " + _mode);
					return Color.Magenta;
			}
		}

		private TextureRegion2D GetModeIcon(HighscoreCategory cat)
		{
			switch (cat)
			{
				case HighscoreCategory.GlobalPoints:
					return Textures.TexIconScore;
				case HighscoreCategory.WorldPoints:
					return Textures.TexIconScore;
				case HighscoreCategory.MultiplayerPoints:
					return Textures.TexIconMPScore;
				case HighscoreCategory.CustomLevelStars:
					return Textures.TexIconStar;
				case HighscoreCategory.CustomLevelPoints:
					return Textures.TexIconTetromino;
				default:
					SAMLog.Error("HP::EnumSwitch_GMI", "cat: " + _mode);
					return Textures.TexPixel;
			}
		}

		private async Task LoadHighscore()
		{
			try
			{
				var data = await MainGame.Inst.Backend.GetRanking(MainGame.Inst.Profile, _focus, _mode);
				MainGame.Inst.DispatchBeginInvoke(() =>
				{
					if (data != null)
					{
						ShowData(data);
					}
					else
					{
						_loader.Color = FlatColors.Pomegranate;
					}
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("HighscorePanel", e);
			}
		}

		private void ShowData(QueryResultRanking data)
		{
			_table.IsVisible = true;
			_loader.IsVisible = false;
			if (_btnPrev != null) _btnPrev.IsVisible = true;
			if (_btnNext != null) _btnNext.IsVisible = true;

			bool foundyourself = false;

			for (int i = 0; i < data.ranking.Count; i++)
			{
				var r = data.ranking[i];

				var r1 = (i + 1).ToString();
				var r2 = FontRenderHelper.MakeTextSafe(_table.Font, r.username, '?');
				var r3 = r.totalscore.ToString();
				var r4 = FormatSeconds(r.totaltime);

				if (r.userid == MainGame.Inst.Profile.OnlineUserID)
				{
					_table.AddRowWithColor(FlatColors.SunFlower, r1, r2, r3, r4);
					foundyourself = true;
				}
				else if (r.username == "anonymous")
				{
					_table.AddRowWithColor(FlatColors.Concrete, r1, r2, r3, r4);
				}
				else
				{
					_table.AddRow(r1, r2, r3, r4);
				}
			}

			if (!foundyourself && data.personal.Count > 0 && MainGame.Inst.Profile.AccountType != AccountType.Local)
			{
				_table.Size = new FSize(TAB_WIDTH, TAB_HEIGHT - BOTTOM_HEIGHT);
				_table.FixHeightToMultipleOfRowHeight();

				_table.RelativePosition = new FPoint(_table.RelativePosition.X, _table.RelativePosition.Y - BOTTOM_HEIGHT/2f);

				var r1 = data.personal[0].rank.ToString();
				var r2 = FontRenderHelper.MakeTextSafe(_table.Font, data.personal[0].username, '?');
				var r3 = data.personal[0].totalscore.ToString();
				var r4 = FormatSeconds(data.personal[0].totaltime);

				var rowp = _table.CreateSingleRowPresenter(r1, r2, r3, r4);
				rowp.Foreground = FlatColors.SunFlower;

				rowp.Size = new FSize(TAB_WIDTH, _table.GetRowHeight());
				rowp.RelativePosition = new FPoint(0, _table.RelativeBottom + (HEIGHT - _table.RelativeBottom) / 2f);
				rowp.Alignment = HUDAlignment.TOPCENTER;

				AddElement(rowp);
			}
		}

		private string FormatSeconds(int totaltime)
		{
			int d = totaltime / (24 * 60 * 60 * 1000);
			totaltime -= d * (24 * 60 * 60 * 1000);

			int h = totaltime / (60 * 60 * 1000);
			totaltime -= h * (60 * 60 * 1000);

			int m = totaltime / (60 * 1000);
			totaltime -= m * (60 * 1000);

			int s = totaltime / (1000);

			return (d > 0 ? $"{d:00}d " : "") + (h > 0 ? $"{h:00}h " : "") + $"{m:00}m" + (d == 0 ? $" {s:00}s" : "");
		}
	}
}