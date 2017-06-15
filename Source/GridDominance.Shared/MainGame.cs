using System;
using System.Diagnostics;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.GlobalAgents;
using GridDominance.Shared.Network;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Sound;
using MonoSAMFramework.Portable.Localization;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.OverworldScreen.Entities;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : MonoSAMGame
	{
		public const string PROFILE_FILENAME = "USERPROFILE";

		public const string IAB_WORLD2 = @"gd_world_002";
		public const string IAB_WORLD3 = @"gd_world_003";
		public static readonly string[] IABList = new[] 
		{
#if DEBUG
			AndroidBillingHelper.PID_CANCELED,
			AndroidBillingHelper.PID_PURCHASED,
			AndroidBillingHelper.PID_REFUNDED,
			AndroidBillingHelper.PID_UNAVAILABLE,
#endif
			IAB_WORLD2,
			IAB_WORLD3,
		};

		public readonly PlayerProfile Profile;
		public readonly IGDServerAPI Backend;

		public static MainGame Inst;

		public readonly GDSounds GDSound = new GDSounds();
		public override SAMSoundPlayer Sound => GDSound;

		public MainGame(IOperatingSystemBridge b) : base(b)
		{
			Backend = new GDServerAPI(b);
			//Backend = new DummyGDServerAPI();

			if (GDConstants.USE_IAB) Bridge.IAB.Connect(IABList);

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

			L10NImpl.Init(Profile.Language);
			
			if (Profile.OnlineUserID >= 0)
			{
				Backend.Ping(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			}
			else
			{
				Backend.CreateUser(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			}
			
			AddAgent(new HighscoreAgent());

			Inst = this;
		}

		private void SAMLogOnLogEvent(object sender, SAMLog.LogEventArgs args)
		{
#if !DEBUG
			if (args.Level == SAMLogLevel.ERROR || args.Level == SAMLogLevel.FATAL_ERROR)
			{
				Backend.LogClient(Profile, args.Entry).EnsureNoError();
			}
#endif
		}

		protected override void OnInitialize()
		{
#if __DESKTOP__

//			const double ZOOM = 0.925;
			const double ZOOM = 0.525;
//			const double ZOOM = 0.325;

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
//			SetTutorialLevelScreen();
			SetOverworldScreen(false);
//			SetWorldMapScreen();
//			SetLevelScreen(Levels.LEVEL_DBG, FractionDifficulty.KI_EASY, Levels.WORLD_001);
		}

		protected override void OnUpdate(SAMTime gameTime)
		{
			GDSound.IsEffectsMuted = !Profile.SoundsEnabled;
		}

		public void SetLevelScreen(LevelBlueprint blueprint, FractionDifficulty d, GraphBlueprint source)
		{
			SetCurrentScreen(new GDGameScreen_SP(this, Graphics, blueprint, d, source));
		}

		public void SetWorldMapScreen(GraphBlueprint g, Guid focus)
		{
			SetCurrentScreen(new GDWorldMapScreen(this, Graphics, g, focus));
		}

		public void SetWorldMapScreenZoomedOut(GraphBlueprint g, Guid focus)
		{
			var screen = new GDWorldMapScreen(this, Graphics, g, focus);
			SetCurrentScreen(screen);
//			screen.ZoomInstantOut();
			screen.ZoomOut();
		}

		public void SetWorldMapScreenWithTransition(GraphBlueprint g)
		{
			var screen = new GDWorldMapScreen(this, Graphics, g, null);
			SetCurrentScreen(screen);
			screen.AddAgent(new InitialTransitionAgent(screen));
			screen.ColorOverdraw = 1f;
		}

		public void SetOverworldScreen(bool noflicker = true)
		{
			var ovs = new GDOverworldScreen(this, Graphics);
			SetCurrentScreen(ovs);

			if (noflicker)
			{
				foreach (var node in ovs.GetEntities<OverworldNode>())
				{
					node.FlickerTime = OverworldNode.COLLAPSE_TIME * 10; // no flicker - for sure
				}
			}
		}

		public void SetOverworldScreenWithTransition(GraphBlueprint bp)
		{
			var screen = new GDOverworldScreen(this, Graphics);
			SetCurrentScreen(screen);
			screen.AddAgent(new ReappearTransitionAgent(screen, bp));
		}
		
		public void SetTutorialLevelScreen()
		{
			SetCurrentScreen(new GDGameScreen_Tutorial(this, Graphics, Levels.LEVEL_TUTORIAL));
		}

		public void SetDebugLevelScreen()
		{
			SetLevelScreen(Levels.LEVEL_DBG, FractionDifficulty.KI_EASY, Levels.WORLD_001);
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
			SAMLog.Debug($"Profile saved ({sdata.Length})");

			try
			{
				var p = new PlayerProfile();
				p.DeserializeFromString(sdata);
				var sdata2 = p.SerializeToString();

				if (sdata2 != sdata)
				{
					SAMLog.Error("Serialization", "Serialization test mismatch", $"Data_1:\n{sdata}\n\n----------------\n\nData_2:\n{sdata2}");
					Debugger.Break();
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("Serialization", "Serialization test mismatch", e.ToString());
				Debugger.Break();
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

