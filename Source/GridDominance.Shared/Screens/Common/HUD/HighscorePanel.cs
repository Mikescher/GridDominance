using System;
using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HighscorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 13 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float TEXT_HEIGHT = 70f;
		public const float TEXT_HEIGHT_REAL = (HEIGHT - TAB_HEIGHT + TEXT_HEIGHT) / 2f;

		public const float TAB_WIDTH = 12 * GDConstants.TILE_WIDTH;
		public const float TAB_HEIGHT = 8 * GDConstants.TILE_WIDTH - TEXT_HEIGHT;

		public override int Depth => 0;

		private HUDRotatingImage _loader;
		private HUDScrollTable _table;

		public HighscorePanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, TEXT_HEIGHT_REAL),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				Text = "Global Ranking",
				TextColor = FlatColors.Clouds,
			});

			_loader = new HUDRotatingImage(0)
			{
				RelativePosition = new FPoint(0, TEXT_HEIGHT / 2f),
				Alignment = HUDAlignment.CENTER,
				Size = new FSize(2 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH),
				Color = FlatColors.Clouds,
				Image = Textures.CannonCog,

				RotationSpeed = 0.1f,

				IsVisible = true,
			};
			AddElement(_loader);

			_table = new HUDScrollTable(0)
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
			_table.AddColumn("Name", null);
			_table.AddColumn("Points", 128);
			_table.AddColumn("Total Time", 175);

			LoadHighscore().EnsureNoError();
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;


		private async Task LoadHighscore()
		{
			try
			{
				var data = await MainGame.Inst.Backend.GetRanking();
				MainGame.Inst.DispatchBeginInvoke(() =>
				{
					if (data != null)
					{
						_table.IsVisible = true;
						_loader.IsVisible = false;

						for (int i = 0; i < data.Count; i++)
						{
							if (data[i].userid == MainGame.Inst.Profile.OnlineUserID)
								_table.AddRowWithColor(FlatColors.SunFlower, (i + 1).ToString(), data[i].username, data[i].totalscore.ToString(), FormatSeconds(data[i].totaltime));
							else if (data[i].username == "anonymous")
								_table.AddRowWithColor(FlatColors.Concrete, (i + 1).ToString(), data[i].username, data[i].totalscore.ToString(), FormatSeconds(data[i].totaltime));
							else
								_table.AddRow((i + 1).ToString(), data[i].username, data[i].totalscore.ToString(), FormatSeconds(data[i].totaltime));
						}

					}
					else
					{
						_loader.Color = FlatColors.Pomegranate;
						HUD.ShowToast("Could not connect to highscore server", 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
					}
				});
			}
			catch (Exception e)
			{
				SAMLog.Error("HighscorePanel", e);
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