using System.Collections.Generic;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
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
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);

		private readonly int musicIdx;
		private readonly GDMultiplayerClient _server;
		private bool _doNotStop = false;

		private readonly Fraction _localPlayerFraction;
		public override Fraction LocalPlayerFraction => _localPlayerFraction;

		public GDGameScreen_MPClient(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, GameSpeedModes speed, int music, GDMultiplayerClient server) 
			: base(game, gdm, bp, FractionDifficulty.KI_NORMAL, false, true)
		{
			musicIdx = music;
			GameSpeedMode = speed;
			_server = server;
			CanPause = false;

			_server.Screen = this;

			if (server.SessionUserID == 1) _localPlayerFraction = GetFractionByID(2);
			else if (server.SessionUserID == 2) _localPlayerFraction = GetFractionByID(3);
			else if (server.SessionUserID == 3) _localPlayerFraction = GetFractionByID(4);
			else if (server.SessionUserID == 4) _localPlayerFraction = GetFractionByID(5);
			else if (server.SessionUserID == 5) _localPlayerFraction = GetFractionByID(6);
			else SAMLog.Error("GDGSMPC", "Client with SSID: " + server.SessionUserID);

			foreach (var c in GetEntities<Cannon>()) c.ForceUpdateController();

			GameHUD = new GDMultiplayerGameHUD(this, server);

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
				HUD.ShowToast("SCRNSC::ERR", L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				MainGame.Inst.SetOverworldScreen(); //TODO Perhaps not kill so suddenly ??
			}
		}

		protected override void OnRemove()
		{
			base.OnRemove();

			if (!_doNotStop) _server.Stop();
		}

		protected override void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			if (_server.Mode == SAMNetworkConnection.ServerMode.IdleAfterGame)
			{
				if (_server.WinnerID == _server.SessionUserID)
				{
					HasFinished = true;
					PlayerWon = true;
					MainGame.Inst.GDSound.PlayEffectGameWon();

					int inc = MainGame.Inst.Profile.IncMultiplayerScore(_server.SessionCapacity - 1, true);

					ShowScorePanel(Blueprint, GDOwner.Profile, new HashSet<FractionDifficulty>(), true, inc);
				}
				else
				{
					HasFinished = true;
					PlayerWon = false;
					MainGame.Inst.GDSound.PlayEffectGameOver();

					int inc = MainGame.Inst.Profile.DecMultiplayerScore(1, true);

					ShowScorePanel(Blueprint, GDOwner.Profile, new HashSet<FractionDifficulty>(), false, inc);
				}

				foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
				{
					cannon.ForceUpdateController();
				}
			}
		}

		public override void RestartLevel()
		{
			SAMLog.Error("GDGSC::Restart", "Try restart level");
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			SAMLog.Error("GDGSC::Replay", "Try replay level");
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints)
		{
			GameSpeedMode = GameSpeedModes.NORMAL;
			HUD.AddModal(new HUDMultiplayerScorePanel(lvl, profile, playerHasWon, addPoints, _server, () => { _doNotStop = true; }), false);
		}

		public override void ExitToMap()
		{
			if (!_doNotStop)
			{
				_server.KillSession();
				_server.Stop();
			}
			MainGame.Inst.SetOverworldScreen();
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished) return new EndGameAutoPlayerController(this, cannon, f);

			if (f == LocalPlayerFraction) return new PlayerController(this, cannon, f, false, true);

			return new RemoteController(this, cannon, f, false, true);
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(musicIdx);
		}
	}
}
