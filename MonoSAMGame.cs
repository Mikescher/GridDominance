using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Sound;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MonoSAMFramework.Portable.UpdateAgents;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game, ILifetimeObject, IUpdateOperationOwner
	{
		private const int NOLAGFRAMES_FOR_INITLAG = 6;
		private const float MAX_DELTA_FOR_NOLAGFRAME = 1/30f;

		protected ScreenManager screens;
		private readonly CustomDispatcher gameDispatcher = new CustomDispatcher();
		private readonly List<IUpdateOperation> agents = new List<IUpdateOperation>();
		private static int _initialNoLagFrameCounter= 0;

		public readonly GraphicsDeviceManager Graphics;

		public static ulong GameCycleCounter { get; private set; }
		public static SAMTime CurrentTime { get; private set; }
		public static bool IsInitializationLag = true;

		public static MonoSAMGame CurrentInst { get; private set; }

		public bool Alive { get; private set; } = true;

		public readonly ISAMOperatingSystemBridge Bridge;

		public abstract SAMSoundPlayer Sound { get; }

		public IDebugTextDisplay DebugDisplay => (screens?.CurrentScreen as GameScreen)?.DebugDisp;

		public static ISAMOperatingSystemBridge StaticBridge;

		protected MonoSAMGame()
		{
			try
			{
				FileHelper.RegisterSystemSecificHandler(StaticBridge.FileHelper);

				Bridge = StaticBridge;
				CurrentInst = this;

				CurrentTime = new SAMTime();

				Graphics = new GraphicsDeviceManager(this);
				Content.RootDirectory = "Content";

				CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
				CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Constructor", e);
			}
		}

		protected override void Initialize()
		{
			try
			{
				base.Initialize();

				StaticBridge.OnNativeInitialize(this);

				OnInitialize();

				screens = new ScreenManager(this);

				OnAfterInitialize();
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Initialize", e);
			}
		}

		public void CleanExit()
		{
			Alive = false;
			Bridge?.ExitApp();
			Exit();
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			base.OnExiting(sender, args);
			Alive = false;
		}

		protected void SetCurrentScreen(GameScreen gdGameScreen)
		{
			screens.CurrentScreen = gdGameScreen;
		}

		public GameScreen GetCurrentScreen()
		{
			return screens?.CurrentScreen;
		}

		protected override void Update(GameTime gameTime)
		{
			try
			{
				var time = new SAMTime(gameTime);

				UpdateAgents(time);

				OnUpdate(time);

				screens.Update(time);

				Sound.Update(time);

				gameDispatcher.Work();

				base.Update(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Update", e);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			try
			{
				var time = new SAMTime(gameTime);

				GameCycleCounter++;
				CurrentTime = time;

				screens.Draw(time);

				base.Draw(gameTime);

				if (IsInitializationLag) UpdateInitLag(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Draw", e);
			}
		}

		private static void UpdateInitLag(GameTime gameTime)
		{
			if (GameCycleCounter > 30)
			{
				IsInitializationLag = false;
				return;
			}

			if (gameTime.IsRunningSlowly)
			{
				IsInitializationLag = true;
				return;
			}

			if (gameTime.ElapsedGameTime.TotalSeconds < MAX_DELTA_FOR_NOLAGFRAME)
			{
				_initialNoLagFrameCounter++;

				if (_initialNoLagFrameCounter > NOLAGFRAMES_FOR_INITLAG) IsInitializationLag = true;
				return;
			}
			else
			{
				_initialNoLagFrameCounter = 0;
				return;
			}
		}

		private void UpdateAgents(SAMTime time)
		{
			for (int i = agents.Count - 1; i >= 0; i--)
			{
				if (!agents[i].UpdateUnchecked(this, time, null))
				{
					agents.RemoveAt(i);
				}
			}
		}

		public void DispatchBeginInvoke(Action a)
		{
			gameDispatcher.BeginInvoke(a);
		}

		public void DispatchInvoke(Action a)
		{
			gameDispatcher.Invoke(a);
		}

		void IUpdateOperationOwner.AddOperation(IUpdateOperation o) { AddAgent(o); }

		public void AddAgent(IUpdateOperation a)
		{
			agents.Add(a);
			a.InitUnchecked(this);
		}

		public IEnumerable<IUpdateOperation> GetAllAgents()
		{
			return agents;
		}

		public IEnumerable<T> GetAgents<T>()
		{
			return agents.OfType<T>();
		}

		public void ShowToast(string id, string text, int size, Color background, Color foreground, float lifetime)
		{
			screens?.CurrentScreen?.HUD?.ShowToast(id, text, size, background, foreground, lifetime);
		}

		public static bool IsDesktop() => CurrentInst.Bridge.SystemType == SAMSystemType.MONOGAME_DESKTOP;
		public static bool IsPhone()   => CurrentInst.Bridge.SystemType == SAMSystemType.MONOGAME_ANDROID || CurrentInst.Bridge.SystemType == SAMSystemType.MONOGAME_IOS;
		public static bool IsAndroid() => CurrentInst.Bridge.SystemType == SAMSystemType.MONOGAME_ANDROID;
		public static bool IsIOS()     => CurrentInst.Bridge.SystemType == SAMSystemType.MONOGAME_IOS;

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
		protected virtual void OnUpdate(SAMTime gameTime) { /* override */}
	}
}
