using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	public class GDGameScreen_MPClient : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular); //TODO Countdown 3 2 1 before game (but in leveltime, for sync and ui reasons)

		private readonly int musicIdx;
		private readonly GDMultiplayerClient _server;

		private readonly Fraction _localPlayerFraction;
		public override Fraction LocalPlayerFraction => _localPlayerFraction;

		public GDGameScreen_MPClient(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, GameSpeedModes speed, int music, GDMultiplayerClient server) 
			: base(game, gdm, bp, FractionDifficulty.KI_NORMAL, false, true)
		{
			musicIdx = music;
			GameSpeedMode = speed;
			_server = server;

			_server.Screen = this;

			if (server.SessionUserID == 1) _localPlayerFraction = GetFractionByID(2);
			else if (server.SessionUserID == 2) _localPlayerFraction = GetFractionByID(3);
			else if (server.SessionUserID == 3) _localPlayerFraction = GetFractionByID(4);
			else SAMLog.Error("GDGSMPC", "Client with SSID: " + server.SessionUserID);

			foreach (var c in GetEntities<Cannon>()) c.ForceUpdateController();

#if DEBUG
			_server.AddDebugLine(this);
#endif
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			base.OnUpdate(gameTime, istate);
			
			_server.Update(gameTime, istate);

			if (_server.Mode == SAMNetworkConnection.ServerMode.Error)
			{
				HUD.ShowToast(L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				MainGame.Inst.SetOverworldScreen(); //TODO Perhaps not kill so suddenly ??
			}
		}

		protected override void OnRemove()
		{
			base.OnRemove();

			_server.Stop();
		}

		protected override void TestForGameEndingCondition()
		{
			// NOP
		}

		public override void RestartLevel()
		{
			//TODO
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			//TODO
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			//TODO
		}

		public override void ExitToMap()
		{
			//TODO
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished) return new EndGameAutoPlayerController(this, cannon, f);

			if (f == LocalPlayerFraction) return new PlayerController(this, cannon, f, false);

			return new RemoteController(this, cannon, f, false);
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(musicIdx);
		}
	}
}
