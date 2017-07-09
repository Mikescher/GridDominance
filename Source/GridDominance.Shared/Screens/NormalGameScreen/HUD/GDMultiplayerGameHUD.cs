using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class GDMultiplayerGameHUD : GameHUD, IGDGameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;
		
		public GDMultiplayerGameHUD(GDGameScreen scrn, GDMultiplayerCommon mp) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(new HUDPauseButton(false, false, true));

			AddElement(new MultiplayerConnectionStateControl(mp)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(4, 4),
				TextColor = FlatColors.Clouds,
			});

			if (mp.SessionUserID == 0)
			{
				if (mp.SessionCapacity == 2)
				{
					AddElement(new HUDLambdaLabel
					{
						Alignment = HUDAlignment.TOPLEFT,
						RelativePosition = new FPoint(4, 44),
						Font = Textures.HUDFontBold,
						FontSize = 32,
						Size = new FSize(200, 32),
						TextColor = FlatColors.Clouds,

						Lambda = () => $"Ping: {(int)(mp.UserConn[1].InGamePing.Value * 1000)}ms",
					});
				}
				else
				{
					int idx = 0;
					for (int i = 1; i < mp.SessionCapacity; i++)
					{
						int uid = i;

						AddElement(new HUDLambdaLabel
						{
							Alignment = HUDAlignment.TOPLEFT,
							RelativePosition = new FPoint(4, 44 + 40*idx),
							Font = Textures.HUDFontBold,
							FontSize = 32,
							Size = new FSize(200, 32),
							TextColor = Color.White,

							Lambda = () => $"Ping[{uid}]: {(int)(mp.UserConn[uid].InGamePing.Value * 1000)}ms",
						});

						idx++;
					}
				}

			}
			else
			{
				AddElement(new HUDLambdaLabel
				{
					Alignment = HUDAlignment.TOPLEFT,
					RelativePosition = new FPoint(12, 56),
					Font = Textures.HUDFontBold,
					FontSize = 32,
					Size = new FSize(200, 32),
					TextColor = Color.White,

					Lambda = () => $"Ping: {(int)(mp.UserConn[0].InGamePing.Value * 1000)}ms",
				});
			}

		}
		
#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
