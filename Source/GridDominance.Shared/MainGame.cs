using System;
using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Network;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Sound;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : MonoSAMGame
	{
		public const string PROFILE_FILENAME = "USERPROFILE";

		public readonly PlayerProfile Profile;
		public readonly GDServerAPI Backend;

		public static MainGame Inst;

		public readonly GDSounds GDSound = new GDSounds();
		public override SAMSoundPlayer Sound => GDSound;

		public MainGame(IOperatingSystemBridge b) : base(b)
		{
			Backend = new GDServerAPI(b);
			Profile = new PlayerProfile();

			var sdata = FileHelper.Inst.ReadDataOrNull(PROFILE_FILENAME);
			if (sdata != null)
			{
				try
				{
#if DEBUG
					var starttime = Environment.TickCount;
#endif
					Profile.DeserializeFromString(sdata);
#if DEBUG
					SAMLog.Debug($"Deserialized profile in {Environment.TickCount - starttime}ms");
#endif
				}
				catch (Exception e)
				{
					SAMLog.Error("Deserialization", e);

					Profile = new PlayerProfile();
					SaveProfile();
				}
			}
			else
			{
				SaveProfile();
			}
			
			SAMLog.LogEvent += SAMLogOnLogEvent;

			//TODO Reenable
			//if (Profile.OnlineUserID >= 0)
			//{
			//	Backend.Ping(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			//}
			//else
			//{
			//	Backend.CreateUser(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			//}

			Inst = this;
		}

		private void SAMLogOnLogEvent(object sender, SAMLog.LogEventArgs args)
		{
			if (args.Level == SAMLogLevel.ERROR || args.Level == SAMLogLevel.FATAL_ERROR)
			{
				Backend.LogClient(Profile, args.Entry).EnsureNoError();
			}
		}

		protected override void OnInitialize()
		{
//			const double ZOOM = 0.925;
			const double ZOOM = 0.625;
//			const double ZOOM = 0.325;

#if __DESKTOP__
			IsMouseVisible = true;
			Graphics.IsFullScreen = false;

			Graphics.PreferredBackBufferWidth  = (int)(1920 * ZOOM);
			Graphics.PreferredBackBufferHeight = (int)(1080 * ZOOM);
			Window.AllowUserResizing = true;

#if DEBUG
			Graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;
			TargetElapsedTime = TimeSpan.FromMilliseconds(1);
#endif

			Graphics.ApplyChanges();
			Window.Position = new Point((1920 - Graphics.PreferredBackBufferWidth) / 2, (1080 - Graphics.PreferredBackBufferHeight) / 2);

#else
			Graphics.IsFullScreen = true;
			Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
			Graphics.ApplyChanges();
#endif
		}

		protected override void OnAfterInitialize()
		{
			//SetLevelScreen(Levels.LEVEL_001, FractionDifficulty.KI_EASY);
			//SetWorldMapScreen();
			SetOverworldScreen();
		}

		protected override void OnUpdate(SAMTime gameTime)
		{
			GDSound.IsMuted = !Profile.SoundsEnabled;
		}

		public void SetLevelScreen(LevelFile blueprint, FractionDifficulty d, WorldGraphFile source)
		{
			SetCurrentScreen(new GDGameScreen(this, Graphics, blueprint, d, source));
		}

		public void SetWorldMapScreen(WorldGraphFile g)
		{
			SetCurrentScreen(new GDWorldMapScreen(this, Graphics, g));
		}

		public void SetWorldMapScreenWithTransition(WorldGraphFile g)
		{
			var screen = new GDWorldMapScreen(this, Graphics, g);
			SetCurrentScreen(screen);
			screen.AddAgent(new InitialTransitionAgent(screen));
			screen.ColorOverdraw = 1f;
		}

		public void SetOverworldScreen()
		{
			SetCurrentScreen(new GDOverworldScreen(this, Graphics));
		}

		protected override void LoadContent()
		{
			Textures.Initialize(Content, GraphicsDevice);
			Levels.LoadContent(Content);
			Sound.Initialize(Content);
		}

		protected override void UnloadContent()
		{
			// NOP
		}


		public void SaveProfile()
		{
			var sdata = Profile.SerializeToString();
			FileHelper.Inst.WriteData(PROFILE_FILENAME, sdata);


#if DEBUG
			SAMLog.Debug("Profile saved");

			try
			{
				var p = new PlayerProfile();
				p.DeserializeFromString(sdata);
				var sdata2 = p.SerializeToString();

				if (sdata2 != sdata)
				{
					SAMLog.Warning("Serialization", "Serialization test mismatch", $"Data_1:\n{sdata}\n\n----------------\n\nData_2:\n{sdata2}");
				}
			}
			catch (Exception e)
			{
				SAMLog.Warning("Serialization", "Serialization test mismatch", e.ToString());
			}
#endif
		}


#if DEBUG
		public void ResetProfile()
		{
			Profile.InitEmpty();
			SaveProfile();

			DebugDisplay.AddDecayLine("Profile reset", 5f, 1f, 1f);
		}
#endif

	}
}

