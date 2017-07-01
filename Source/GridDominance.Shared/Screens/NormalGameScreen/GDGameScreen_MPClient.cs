using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_MPClient : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);

		private readonly int musicIdx;
		private readonly GDMultiplayerClient _server;

		public GDGameScreen_MPClient(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, GameSpeedModes speed, int music, GDMultiplayerClient server) 
			: base(game, gdm, bp, FractionDifficulty.KI_NORMAL, false)
		{
			musicIdx = music;
			GameSpeedMode = speed;
			_server = server;
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			base.OnUpdate(gameTime, istate);
			
			_server.Update(gameTime, istate);
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
			if (HasFinished)
			{
				return new EndGameAutoPlayerController(this, cannon, f);
			}
			
			switch (f.Type)
			{
				case FractionType.PlayerFraction:
					return new PlayerController(this, cannon, f);

				case FractionType.ComputerFraction:
					return cannon.CreateKIController(this, f);

				case FractionType.NeutralFraction:
					return new NeutralKIController(this, cannon, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(musicIdx);
		}
	}
}
