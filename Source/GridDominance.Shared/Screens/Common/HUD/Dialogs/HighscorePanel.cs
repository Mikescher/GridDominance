using System;
using System.Threading.Tasks;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Table;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HighscorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 13 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float TEXT_HEIGHT = 70f;
		public const float TEXT_HEIGHT_REAL = (HEIGHT - TAB_HEIGHT + TEXT_HEIGHT) / 2f;

		public const float TAB_WIDTH     = 12 * GDConstants.TILE_WIDTH;
		public const float TAB_HEIGHT    = 8 * GDConstants.TILE_WIDTH - TEXT_HEIGHT;
		public const float BOTTOM_HEIGHT = 1 * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private readonly GraphBlueprint _focus;
		private readonly bool _showMultiplayer;

		private HUDImage _loader;
		private HUDScrollTable _table;
		private HUDTextButton _btn;

		public HighscorePanel(GraphBlueprint focus, bool mp)
		{
			_focus = focus;
			_showMultiplayer = mp;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			string txt = "?";
			if (_showMultiplayer)
				txt = L10N.T(L10NImpl.STR_HSP_MULTIPLAYERRANKING);
			else if (_focus == null)
				txt = L10N.T(L10NImpl.STR_HSP_GLOBALRANKING);
			else
				txt = L10N.TF(L10NImpl.STR_HSP_RANKINGFOR, L10N.T(Levels.WORLD_NAMES[_focus.ID]));

			AddElement(new HUDLabel(1)
			{
				TextAlignment = (_showMultiplayer || _focus==null) ? HUDAlignment.CENTER : HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(TAB_WIDTH, TEXT_HEIGHT_REAL),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				Text = txt,
				TextColor = FlatColors.Clouds,
			});

			if (!_showMultiplayer && MainGame.Inst.Profile.HasMultiplayerGames)
			{
				AddElement(_btn = new HUDTextButton(1)
				{
					Alignment = HUDAlignment.TOPRIGHT,
					RelativePosition = FPoint.Zero,
					Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 48),

					L10NText = L10NImpl.STR_WORLD_MULTIPLAYER,
					TextColor = FlatColors.Foreground,
					Font = Textures.HUDFontBold,
					FontSize = 42,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Amethyst, 16, false, true, false, false),
					BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Wisteria, 16, false, true, false, false),

					Click = ShowMultiplayer,

					IsVisible = false,
				});
			}

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
			AddElement(_table);

			_table.AddColumn("", 64);
			_table.AddColumn(L10N.T(L10NImpl.STR_TAB_NAME), null);
			_table.AddColumn(L10N.T(L10NImpl.STR_TAB_POINTS), 128);
			if (!_showMultiplayer) _table.AddColumn(L10N.T(L10NImpl.STR_TAB_TIME), 175);

			LoadHighscore().EnsureNoError();
		}

		private void ShowMultiplayer(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new HighscorePanel(null, true), true);
		}

		private async Task LoadHighscore()
		{
			try
			{
				var data = await MainGame.Inst.Backend.GetRanking(MainGame.Inst.Profile, _focus, _showMultiplayer);
				MainGame.Inst.DispatchBeginInvoke(() =>
				{
					if (data != null)
					{
						ShowData(data);
					}
					else
					{
						_loader.Color = FlatColors.Pomegranate;
						HUD.ShowToast(null, L10N.T(L10NImpl.STR_HSP_CONERROR), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
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
			if (_btn != null) _btn.IsVisible = true;

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