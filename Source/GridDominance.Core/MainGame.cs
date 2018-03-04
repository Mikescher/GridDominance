using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.GlobalAgents;
using GridDominance.Shared.Network;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.OverworldScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Sound;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.OverworldScreen.Entities;
using System.Text;
using GridDominance.Shared.Screens.EndGameScreen;
using GridDominance.Shared.Screens.LevelEditorScreen;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared
{
	public class MainGame : MonoSAMGame
	{
		public const float MAX_LOG_SEND_DELTA = 25f; // Max send 5 logs in 25sec
		public const int   MAX_LOG_SEND_COUNT = 5;

		public PlayerProfile Profile;
		public IGDServerAPI Backend;

		public static MainGame Inst;

		public readonly GDSounds GDSound = new GDSounds();
		public override SAMSoundPlayer Sound => GDSound;

		public static GDFlavor Flavor => Inst?.GDBridge?.Flavor ?? ((IGDOperatingSystemBridge)StaticBridge).Flavor;

		public readonly float[] LastSendLogTimes = new float[MAX_LOG_SEND_COUNT];

		public IGDOperatingSystemBridge GDBridge => (IGDOperatingSystemBridge)Bridge;
		
		public static bool IsShaderless() => StaticBridge.SystemType == SAMSystemType.MONOGAME_IOS;

		public MainGame() : base()
		{
			Backend = new GDServerAPI(GDBridge);
			//Backend = new DummyGDServerAPI();

			if (MainGame.Flavor == GDFlavor.IAB) GDBridge.IAB.Connect(GDConstants.IABList);

			Profile = new PlayerProfile();

			var sdata = FileHelper.Inst.ReadDataOrNull(GDConstants.PROFILE_FILENAME); // %LOCALAPPDATA%\IsolatedStorage\...
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
			SAMLog.AdditionalLogInfo.Add(GetLogInfo);

			for (int i = 0; i < MAX_LOG_SEND_COUNT; i++) LastSendLogTimes[i] = float.MinValue;

			L10NImpl.Init(Profile.Language);
			
			AddAgent(new HighscoreAgent());

			Inst = this;
		}

		private void SAMLogOnLogEvent(object sender, SAMLog.LogEventArgs args)
		{
			if (args.Level == SAMLogLevel.ERROR || args.Level == SAMLogLevel.FATAL_ERROR)
			{
				//Prevent sending logs too fast
				if (CurrentTime.TotalElapsedSeconds - LastSendLogTimes[0] < MAX_LOG_SEND_DELTA)
				{
					SAMLog.Info("Backend::LogSpam", $"Do not send log '{args.Entry.MessageShort}', cause too many online logs in last time");
					return;
				}


				Backend.LogClient(Profile, args.Entry).EnsureNoError();


				for (int i = 0; i < MAX_LOG_SEND_COUNT-1; i++)
				{
					LastSendLogTimes[i] = LastSendLogTimes[i + 1];
				}
				LastSendLogTimes[MAX_LOG_SEND_COUNT - 1] = CurrentTime.TotalElapsedSeconds;
			}
		}

		protected override void OnInitialize()
		{
			if (IsDesktop())
			{
#if DEBUG
//			const double ZOOM = 0.925;
				const double ZOOM = 0.525;
//			const double ZOOM = 0.325;

				IsMouseVisible = true;
				Graphics.IsFullScreen = false;

				Graphics.PreferredBackBufferWidth = (int) (1920 * ZOOM);
				Graphics.PreferredBackBufferHeight = (int) (1080 * ZOOM);
				Window.AllowUserResizing = true;

				Graphics.SynchronizeWithVerticalRetrace = false;
				IsFixedTimeStep = false;
				TargetElapsedTime = TimeSpan.FromMilliseconds(1);

				Graphics.ApplyChanges();
				Window.Position = new Point((1920 - Graphics.PreferredBackBufferWidth) / 2, (1080 - Graphics.PreferredBackBufferHeight) / 2);
#endif
			}
			else
			{
				Graphics.IsFullScreen = true;
				Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
				Graphics.ApplyChanges();
			}
		}

		protected override void OnAfterInitialize()
		{
//			SetTutorialLevelScreen();
			SetOverworldScreen(false);
//			SetWorldMapScreen();
//			SetLevelScreen(Levels.LEVEL_DBG, FractionDifficulty.KI_EASY, Levels.WORLD_001);

			
			if (Profile.OnlineUserID >= 0)
			{
				Backend.Ping(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			}
			else
			{
				Backend.CreateUser(Profile).ContinueWith(t => Backend.DownloadHighscores(Profile)).EnsureNoError();
			}
		}

		protected override void OnUpdate(SAMTime gameTime)
		{
			GDSound.IsEffectsMuted = !Profile.SoundsEnabled;
			GDSound.IsMusicMuted = !(Profile.MusicEnabled && Profile.SoundsEnabled);
		}

		public void SetLevelScreen(LevelBlueprint blueprint, FractionDifficulty d, GraphBlueprint source)
		{
			var scrn = new GDGameScreen_SP(this, Graphics, blueprint, d, source);
			SetCurrentScreen(scrn);
		}

		public void SetEditorTestLevel(LevelBlueprint blueprint, FractionDifficulty d, SCCMLevelData source, GameSpeedModes speed = GameSpeedModes.NORMAL)
		{
			var scrn = new GDGameScreen_SCCMTest(this, Graphics, blueprint, d, source, speed);
			SetCurrentScreen(scrn);
		}

		public void SetEditorUploadLevel(LevelBlueprint blueprint, SCCMLevelData source, bool first, GameSpeedModes speed = GameSpeedModes.NORMAL)
		{
			var scrn = new GDGameScreen_SCCMUpload(this, Graphics, blueprint, source, speed, first);
			SetCurrentScreen(scrn);
		}

		public void SetWorldMapScreen(GraphBlueprint g, Guid focus)
		{
			SetCurrentScreen(new GDWorldMapScreen(this, Graphics, g, focus));
		}

		public void SetWorldMapScreenZoomedOut(GraphBlueprint g, LevelBlueprint initFocus = null)
		{
			var screen = new GDWorldMapScreen(this, Graphics, g, initFocus?.UniqueID);
			SetCurrentScreen(screen);
//			screen.ZoomInstantOut();
			screen.ZoomOut();
		}

		public void SetWorldMapScreenWithTransition(GraphBlueprint g)
		{
			var screen = new GDWorldMapScreen(this, Graphics, g, null);
			SetCurrentScreen(screen);
			screen.AddAgent(new InitialTransitionOperation());
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

				ovs.GDHUD.ScoreDispMan.FinishCounter();
			}
		}
		
		public void SetOverworldScreenWithSCCM(SCCMMainPanel.SCCMTab tab, bool noflicker = true)
		{
			var ovs = new GDOverworldScreen(this, Graphics);
			SetCurrentScreen(ovs);

			if (noflicker)
			{
				foreach (var node in ovs.GetEntities<OverworldNode>())
				{
					node.FlickerTime = OverworldNode.COLLAPSE_TIME * 10; // no flicker - for sure
				}

				ovs.GDHUD.ScoreDispMan.FinishCounter();
			}
			
			var pnl = new SCCMMainPanel();
			ovs.HUD.AddModal(pnl, true, 0.5f, 1f);
			pnl.SelectTab(tab);
		}

		public void SetOverworldScreenCopy(GDOverworldScreen s)
		{
			if (s == null) { SetOverworldScreen(); return; }

			var ovs = new GDOverworldScreen(this, Graphics);
			SetCurrentScreen(ovs);

			foreach (var node in ovs.GetEntities<OverworldNode>())
			{
				node.FlickerTime = OverworldNode.COLLAPSE_TIME * 10; // no flicker - for sure
			}

			ovs.ScrollAgent.CopyState(s.ScrollAgent);
		}

		public void SetOverworldScreenWithTransition(GraphBlueprint bp)
		{
			var screen = new GDOverworldScreen(this, Graphics);
			SetCurrentScreen(screen);
			screen.ScrollAgent.ScrollTo(bp);
			screen.AddAgent(new ReappearTransitionOperation(bp));

			screen.GDHUD.ScoreDispMan.FinishCounter();
		}
		
		public void SetTutorialLevelScreen()
		{
			SetCurrentScreen(new GDGameScreen_Tutorial(this, Graphics));
		}

		public void SetDebugLevelScreen()
		{
			SetLevelScreen(Levels.LEVEL_DBG, FractionDifficulty.KI_EASY, Levels.WORLD_001);
		}

		public void SetMultiplayerServerLevelScreen(LevelBlueprint level, GameSpeedModes speed, int music, GDMultiplayerServer server)
		{
			var scrn = new GDGameScreen_MPServer(this, Graphics, level, speed, music, server);
			SetCurrentScreen(scrn);
		}

		public void SetMultiplayerClientLevelScreen(LevelBlueprint level, GameSpeedModes speed, int music, GDMultiplayerClient server)
		{
			var scrn = new GDGameScreen_MPClient(this, Graphics, level, speed, music, server);
			SetCurrentScreen(scrn);
		}

		public void SetGameEndScreen()
		{
			var scrn = new GDEndGameScreen(this, Graphics);
			SetCurrentScreen(scrn);
		}

		public void SetLevelEditorScreen(SCCMLevelData dat)
		{
			var scrn = new LevelEditorScreen(this, Graphics, dat);
			SetCurrentScreen(scrn);
		}
		
		public void SetCustomLevelScreen(LevelBlueprint blueprint, FractionDifficulty d)
		{
			var scrn = new GDGameScreen_SCCMPlay(this, Graphics, blueprint, d);
			SetCurrentScreen(scrn);
		}

		protected override void LoadContent()
		{
			Textures.Initialize(Content, GraphicsDevice);
			Levels.LoadContent(Content);
			try
			{
				Sound.Initialize(Content);
			}
			catch (Exception e)
			{
				SAMLog.Error("MG::LC", "Initializing sound failed", e.ToString());
				Sound.InitErrorState = true;
			}
		}

		protected override void UnloadContent()
		{
			// NOP
		}

		public void SaveProfile()
		{
			var sdata = Profile.SerializeToString();

			try
			{
				FileHelper.Inst.WriteData(GDConstants.PROFILE_FILENAME, sdata);
			}
			catch (IOException e)
			{
				if (e.Message.Contains("Disk full"))
				{
					DispatchBeginInvoke(() =>
					{
						ShowToast("MG::OOM", L10N.T(L10NImpl.STR_ERR_OUTOFMEMORY), 32, FlatColors.Flamingo, FlatColors.Foreground, 3f);
					});
				}
				else
				{
					SAMLog.Error("MG::WRITE", e);
				}
			}

#if DEBUG
			SAMLog.Debug($"Profile saved ({sdata.Length})");

			try
			{
				var p = new PlayerProfile();
				p.NoAfterSerializeFixes = true;
				p.DeserializeFromString(sdata);
				var sdata2 = p.SerializeToString();

				if (sdata2 != sdata)
				{
					SAMLog.Error("Serialization_mismatch", "Serialization test mismatch", $"Data_1:\n{sdata}\n\n----------------\n\nData_2:\n{sdata2}");
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("Serialization-Ex", "Serialization test mismatch", e.ToString());
			}
#endif
		}

		public string GetLogInfo()
		{
			StringBuilder b = new StringBuilder();

			b.AppendLine("GameCycleCounter: " + GameCycleCounter);
			b.AppendLine("IsInitializationLag: " + IsInitializationLag);
			b.AppendLine("MainGame.Alive: " + Alive);
			b.AppendLine("AppType: " + GDBridge.AppType);

			var scrn = screens?.CurrentScreen;

			if (scrn != null)
			{
				b.AppendLine("MainGame.CurrentScreen:    " + scrn.GetType());
				b.AppendLine("GameScreen.Entities.Count: " + scrn.Entities?.Count());
				b.AppendLine("GameScreen.HUD.DeepCount:  " + scrn.HUD?.DeepCount());
				b.AppendLine("GameScreen.IsRemoved:      " + scrn.IsRemoved);
				b.AppendLine("GameScreen.IsShown:        " + scrn.IsShown);
				b.AppendLine("GameScreen.GameSpeed:      " + scrn.GameSpeed);
			}

			if (scrn is GDGameScreen)
			{
				b.AppendLine("GDGameScreen.HasFinished:  " + ((GDGameScreen)scrn).HasFinished);
				b.AppendLine("GDGameScreen.Blueprint.ID: " + ((GDGameScreen)scrn).Blueprint?.UniqueID);
				b.AppendLine("GDGameScreen.Difficulty:   " + ((GDGameScreen)scrn).Difficulty);
				b.AppendLine("GDGameScreen.IsPaused:     " + ((GDGameScreen)scrn).IsPaused);
			}

			if (scrn is GDWorldMapScreen)
			{
				b.AppendLine("GDWorldMapScreen.GraphBlueprint.ID: " + ((GDWorldMapScreen)scrn)?.GraphBlueprint?.ID);
			}


			b.AppendLine("Profile.Language:         " + Profile.Language);
			b.AppendLine("Profile.MusicEnabled:     " + Profile.MusicEnabled);
			b.AppendLine("Profile.SoundsEnabled:    " + Profile.SoundsEnabled);
			b.AppendLine("Profile.NeedsReupload:    " + Profile.NeedsReupload);
			b.AppendLine("Profile.EffectsEnabled:   " + Profile.EffectsEnabled);
			b.AppendLine("Profile.AccountType:      " + Profile.AccountType);
			b.AppendLine("Textures.TEXTURE_QUALITY: " + Textures.TEXTURE_QUALITY);

			return b.ToString();
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

