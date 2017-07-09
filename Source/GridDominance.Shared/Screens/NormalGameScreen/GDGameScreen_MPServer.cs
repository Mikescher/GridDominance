using System;
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
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	public class GDGameScreen_MPServer : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);

		private readonly int musicIdx;
		private readonly GDMultiplayerServer _server;

		private readonly Fraction _localPlayerFraction;
		public override Fraction LocalPlayerFraction => _localPlayerFraction;

		public GDGameScreen_MPServer(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, GameSpeedModes speed, int music, GDMultiplayerServer server) 
			: base(game, gdm, bp, FractionDifficulty.KI_IMPOSSIBLE, false, true)
		{
			musicIdx = music;
			GameSpeedMode = speed;
			_server = server;
			CanPause = false;

			_server.Screen = this;

			_localPlayerFraction = GetFractionByID(1);

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
				HUD.ShowToast(L10NImpl.FormatNetworkErrorMessage(_server.Error, _server.ErrorData), 32, FlatColors.Flamingo, FlatColors.Foreground, 7f);

				MainGame.Inst.SetOverworldScreen(); //TODO Perhaps not kill so suddenly ??
			}
		}

		protected override void OnRemove()
		{
			base.OnRemove();

			_server.Stop();
		}

		public override void RestartLevel()
		{
			SAMLog.Error("GDGSS::Restart", "Try restart level");
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			SAMLog.Error("GDGSS::Replay", "Try replay level");
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			GameSpeedMode = GameSpeedModes.NORMAL;
			HUD.AddModal(new HUDMultiplayerScorePanel(lvl, profile, playerHasWon), false);
		}

		protected override void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			Fraction winner = null;

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon.Fraction.IsNeutral) continue;

				if (winner != null && winner != cannon.Fraction) return;
				winner = cannon.Fraction;
			}

			if (winner == LocalPlayerFraction)
			{
				HasFinished = true;
				PlayerWon = true;
				MainGame.Inst.GDSound.PlayEffectGameWon();
				ShowScorePanel(Blueprint, GDOwner.Profile, null, true, 0);
			}
			else
			{
				HasFinished = true;
				PlayerWon = false;
				MainGame.Inst.GDSound.PlayEffectGameOver();
				ShowScorePanel(Blueprint, GDOwner.Profile, null, false, 0);
			}

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				cannon.ForceUpdateController();
			}

			byte winnerUID = 0;
			int winnerFracID = GetFractionID(winner);

			if (winnerFracID == 2) winnerUID = 1;
			if (winnerFracID == 3) winnerUID = 2;
			if (winnerFracID == 4) winnerUID = 3;
			if (winnerFracID == 5) winnerUID = 4;
			if (winnerFracID == 6) winnerUID = 5;

			_server.StartBroadcastAfterGame(winnerUID);
		}

		public override void ExitToMap()
		{
			_server.KillSession();
			_server.Stop();
			MainGame.Inst.SetOverworldScreen();
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished) return new EndGameAutoPlayerController(this, cannon, f);

			if (f == LocalPlayerFraction) return new PlayerController(this, cannon, f);

			if (f.Type == FractionType.NeutralFraction) return new NeutralKIController(this, cannon, f);

			return new RemoteController(this, cannon, f, true, false);
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(musicIdx);
		}
	}
}
