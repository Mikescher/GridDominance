using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.UpdateAgents;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class GameScreen : ILifetimeObject, IUpdateOperationOwner
	{
		public bool IsShown { get; private set; }
		public bool IsRemoved { get; private set; }

		public bool Alive => !IsRemoved;

		
		private float _mapOffsetX = 0f;
		private float _mapOffsetY = 0f;

		public GraphicsDevice GraphicsDevice => Graphics.GraphicsDevice;

		public readonly GraphicsDeviceManager Graphics;
		public readonly MonoSAMGame Game;
		public SAMViewportAdapter VAdapterGame;
		public SAMViewportAdapter VAdapterHUD;
		public GameHUD HUD => GameHUD;

		public InputStateManager InputStateMan;

		protected GameHUD GameHUD;
		public    EntityManager Entities;
		public    GameBackground Background;
		public    IDebugTextDisplay DebugDisp;

		protected SpriteBatch InternalBatch;
		protected IBatchRenderer FixedBatch;         // no translation          (for HUD)
		protected IBatchRenderer TranslatedBatch;    // translated by MapOffset (for everything else)

#if DEBUG
		public    RealtimeAPSCounter FPSCounter;
		public    RealtimeAPSCounter UPSCounter;
		public    GCMonitor GCMonitor;
		protected DebugMinimap DebugMap;
		
		public static readonly TimingCounter TIMING_DRAW_BACKGROUND     = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_ENTITIES       = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_SCREEN         = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_BACKGROUNDPOST = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_ENTITIESPOST   = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_HUD            = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_DEBUGSCREEN    = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_PROXIES        = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_BATCH_GAME     = new TimingCounter(32);
		public static readonly TimingCounter TIMING_DRAW_BATCH_HUD      = new TimingCounter(32);
#endif

		public float GameSpeed = 1f;

		public float MapOffsetX { get { return _mapOffsetX; } set { _mapOffsetX = value; } }
		public float MapOffsetY { get { return _mapOffsetY; } set { _mapOffsetY = value; } }
		public FPoint MapOffset => new FPoint(_mapOffsetX, _mapOffsetY);
		public float MapViewportCenterX { get { return VAdapterGame.VirtualTotalWidth  / 2 - MapOffsetX - VAdapterGame.VirtualGuaranteedBoundingsOffsetX; } set { MapOffsetX = VAdapterGame.VirtualTotalWidth / 2 - VAdapterGame.VirtualGuaranteedBoundingsOffsetX - value; } }
		public float MapViewportCenterY { get { return VAdapterGame.VirtualTotalHeight / 2 - MapOffsetY - VAdapterGame.VirtualGuaranteedBoundingsOffsetY; } set { MapOffsetY = VAdapterGame.VirtualTotalHeight / 2 - VAdapterGame.VirtualGuaranteedBoundingsOffsetY - value; } }
		public FPoint MapViewportCenter => new FPoint(MapViewportCenterX, MapViewportCenterY);
		public FRectangle GuaranteedMapViewport => new FRectangle(-MapOffsetX, -MapOffsetY, VAdapterGame.VirtualGuaranteedWidth, VAdapterGame.VirtualGuaranteedHeight);
		public FRectangle CompleteMapViewport => new FRectangle(-MapOffsetX - VAdapterGame.VirtualGuaranteedBoundingsOffsetX, -MapOffsetY - VAdapterGame.VirtualGuaranteedBoundingsOffsetY, VAdapterGame.VirtualTotalWidth, VAdapterGame.VirtualTotalHeight);
		public FRectangle MapFullBounds { get; protected set; }

		public float PixelWidth => VAdapterGame.VirtualGuaranteedWidth / VAdapterGame.RealGuaranteedWidth;

		private readonly List<IUpdateOperation> agents = new List<IUpdateOperation>();
		private readonly List<IProxyScreenProvider> _proxyScreens = new List<IProxyScreenProvider>();
		private bool _clearScreenOnDraw = true;
		private bool _updateDebugSettings = true;
		
#if DEBUG
		public int LastDebugRenderSpriteCount   => FixedBatch.LastDebugRenderSpriteCount   + TranslatedBatch.LastDebugRenderSpriteCount + DebugDisp.LastRenderSpriteCount;
		public int LastReleaseRenderSpriteCount => FixedBatch.LastReleaseRenderSpriteCount + TranslatedBatch.LastReleaseRenderSpriteCount;
		public int LastDebugRenderTextCount     => FixedBatch.LastDebugRenderTextCount     + TranslatedBatch.LastDebugRenderTextCount + DebugDisp.LastRenderTextCount;
		public int LastReleaseRenderTextCount   => FixedBatch.LastReleaseRenderTextCount   + TranslatedBatch.LastReleaseRenderTextCount;
#endif

		protected GameScreen(MonoSAMGame game, GraphicsDeviceManager gdm)
		{
			Graphics = gdm;
			Game = game;
			
			Initialize();
		}

		public void Show()
		{
#if DEBUG
			if (IsRemoved) throw new Exception("You cannot reuse screens");
#endif

			IsShown = true;

			OnShow();
		}

		public void Remove()
		{
			IsRemoved = true;
			IsShown = false;

			OnRemove();
		}

		private void Initialize()
		{
			MapFullBounds = CreateMapFullBounds();
			VAdapterGame = CreateViewport();
			VAdapterHUD = CreateViewport(); // later perhaps diff adapters

			InternalBatch   = new SpriteBatch(Graphics.GraphicsDevice);
			FixedBatch      = new SpriteBatchWrapper(InternalBatch);
			TranslatedBatch = new SpriteBatchWrapper(InternalBatch);

			InputStateMan = new InputStateManager(VAdapterGame, VAdapterHUD, MapOffsetX, MapOffsetY);
			GameHUD = CreateHUD();
			Background = CreateBackground();

			Entities = CreateEntityManager();

			DebugDisp = new DummyDebugTextDisplay();

			GameHUD.Validate();

#if DEBUG
			FPSCounter = new RealtimeAPSCounter();
			UPSCounter = new RealtimeAPSCounter();
			GCMonitor = new GCMonitor();

			DebugMap = CreateDebugMinimap();
#endif
		}

		protected virtual void OnShow() { }
		
		protected virtual void OnRemove()
		{
			FixedBatch.Dispose();
			TranslatedBatch.Dispose();
		}

		public virtual void Update(SAMTime gameTime)
		{
#if DEBUG
			UPSCounter.StartCycle(gameTime);
#endif
			var state = InputStateMan.GetNewState(MapOffsetX, MapOffsetY);
			
#if DEBUG
			if (_updateDebugSettings) DebugSettings.Update(state);
			GCMonitor.Update(gameTime, state);
#endif

			if (FloatMath.IsZero(GameSpeed))
			{
				// render these two always
				DebugDisp.Update(gameTime, state);
				GameHUD.Update(gameTime, state);
				return;
			}
			else if (FloatMath.IsOne(GameSpeed))
			{
				InternalUpdate(gameTime, state, gameTime);
			}
			else if (GameSpeed < 1f)
			{
				var internalTime = gameTime.Stretch(GameSpeed, 1);

				InternalUpdate(internalTime, state, gameTime);
			}
			else if (GameSpeed > 1f)
			{
				int runCount = (int) GameSpeed;

				var timeVirtual = gameTime.Stretch(GameSpeed / runCount, 1f / runCount);
				var timeReal    = gameTime.Stretch(1f / runCount, 1f / runCount);
				
				for (int i = 0; i < runCount; i++)
				{
					InternalUpdate(timeVirtual, state, timeReal);
				}
			}
			else
			{
				// okay - dafuq
				throw new ArgumentException(nameof(GameSpeed) + " = " + GameSpeed, nameof(GameSpeed));
			}

#if DEBUG
			UPSCounter.EndCycle();
#endif
		}

		private void InternalUpdate(SAMTime timeVirtual, InputState state, SAMTime timeReal)
		{
			// Update Top Down  (Debug -> HUD -> Entities -> BG)
			// Render Bottom Up (BG -> Entities -> HUD -> Debug)

			DebugDisp.Update(timeReal, state);

			GameHUD.Update(timeReal, state);

			Entities.Update(timeVirtual, state);

			Background.Update(timeVirtual, state);

			UpdateAgents(timeVirtual, state);

			OnUpdate(timeVirtual, state);
		}

		private void UpdateAgents(SAMTime timeVirtual, InputState istate)
		{
			for (int i = agents.Count - 1; i >= 0; i--)
			{
				if (!agents[i].UpdateUnchecked(this, timeVirtual, istate))
				{
					agents.RemoveAt(i);
				}
			}
		}

		public virtual void Draw(SAMTime gameTime)
		{
			InternalDraw(gameTime, null);
		}
		
		private void InternalDraw(SAMTime gameTime, Rectangle? scissor)
		{
#if DEBUG
			FPSCounter.StartCycle(gameTime);
#endif
			VAdapterGame.Update();
			VAdapterHUD.Update();
			
			if (_clearScreenOnDraw) Graphics.GraphicsDevice.Clear(Color.Magenta);

			// Update Top Down  (Debug -> HUD -> Entities -> BG)
			// Render Bottom Up (BG -> Entities -> GPU_Particle -> HUD -> Debug)

			var bts = GetBaseTextureScale();
			var mat = Matrix.CreateTranslation(MapOffsetX, MapOffsetY, 0) * VAdapterGame.GetScaleMatrix();

			// ======== GAME =========

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_BATCH_GAME.Start();
#endif

			TranslatedBatch.OnBegin(bts);
			if (scissor == null)
			{
				InternalBatch.Begin(transformMatrix: mat);
			}
			else
			{
				GraphicsDevice.ScissorRectangle = scissor.Value;
				InternalBatch.Begin(transformMatrix: mat, rasterizerState: new RasterizerState{ScissorTestEnable = true});
			}
			try
			{
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_BACKGROUND.Start();
#endif
				Background.Draw(TranslatedBatch);
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_BACKGROUND.Stop();
#endif

#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_ENTITIES.Start();
#endif
				Entities.Draw(TranslatedBatch);
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_ENTITIES.Stop();
#endif

#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_SCREEN.Start();
#endif
				OnDrawGame(TranslatedBatch);
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_SCREEN.Stop();
#endif

#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_BACKGROUNDPOST.Start();
#endif
				Background.DrawOverlay(TranslatedBatch);
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_BACKGROUNDPOST.Stop();
#endif

#if DEBUG
				DrawScreenDebug(TranslatedBatch);
#endif
			}
			finally
			{
				InternalBatch.End();
				TranslatedBatch.OnEnd();
			}

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_BATCH_GAME.Stop();
#endif

			// ======== STUFF ========

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_ENTITIESPOST.Start();
#endif
			Entities.PostDraw();
#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_ENTITIESPOST.Stop();
#endif

			// ======== HUD ==========

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_BATCH_HUD.Start();
#endif

			FixedBatch.OnBegin(bts);

			if (scissor == null)
			{
				InternalBatch.Begin(transformMatrix: VAdapterHUD.GetScaleMatrix());
			}
			else
			{
				GraphicsDevice.ScissorRectangle = scissor.Value;
				InternalBatch.Begin(transformMatrix: VAdapterHUD.GetScaleMatrix(), rasterizerState: new RasterizerState { ScissorTestEnable = true });
			}
			try
			{
#if DEBUG
				if (_updateDebugSettings) TIMING_DRAW_HUD.Start();
#endif
				GameHUD.Draw(FixedBatch);
#if DEBUG
				using (FixedBatch.BeginDebugDraw()) DebugMap.Draw(FixedBatch);
				if (_updateDebugSettings) TIMING_DRAW_HUD.Stop();
#endif
				OnDrawHUD(TranslatedBatch);
			}
			finally
			{
				InternalBatch.End();
				FixedBatch.OnEnd();
			}

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_BATCH_HUD.Stop();
#endif

			// =======================

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_DEBUGSCREEN.Start();
			using (FixedBatch.BeginDebugDraw())
			using (TranslatedBatch.BeginDebugDraw())
			{
				Entities.DrawOuterDebug();
				DebugDisp.Draw();
			}
			if (_updateDebugSettings) TIMING_DRAW_DEBUGSCREEN.Stop();
#endif

#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_PROXIES.Start();
#endif
			foreach (var proxy in _proxyScreens)
			{
				if (proxy.ProxyTargetBounds.IsEmpty) continue;
				proxy.Proxy._clearScreenOnDraw = false;
				proxy.Proxy._updateDebugSettings = false;

				proxy.Proxy.InternalDraw(gameTime, proxy.ProxyTargetBounds.CeilOutwards());
			}
#if DEBUG
			if (_updateDebugSettings) TIMING_DRAW_PROXIES.Stop();
#endif

#if DEBUG
			FPSCounter.EndCycle();
#endif
		}
		
#if DEBUG
		private void DrawScreenDebug(IBatchRenderer sbatch)
		{
			if (DebugSettings.Get("DebugBackground"))
			{
				DebugRenderHelper.DrawCrossedCircle(sbatch, Color.Red, MapViewportCenter, 8, 2);
				DebugRenderHelper.DrawHalfCrossedCircle(sbatch, Color.Red, MapOffset.Negate(), 8, 2);

				var rTop = new FRectangle(CompleteMapViewport.X, CompleteMapViewport.Y, CompleteMapViewport.Width, GuaranteedMapViewport.Y - CompleteMapViewport.Y);
				var rBot = new FRectangle(CompleteMapViewport.X, GuaranteedMapViewport.Bottom, CompleteMapViewport.Width, CompleteMapViewport.Bottom - GuaranteedMapViewport.Bottom);
				var rLef = new FRectangle(CompleteMapViewport.X, CompleteMapViewport.Y, GuaranteedMapViewport.X - CompleteMapViewport.X, CompleteMapViewport.Height);
				var rRig = new FRectangle(GuaranteedMapViewport.Right, CompleteMapViewport.Y, CompleteMapViewport.Right - GuaranteedMapViewport.Right, CompleteMapViewport.Height);

				if (rTop.Area > 0.001f) sbatch.FillRectangle(rTop, Color.DarkRed * 0.35f);
				if (rBot.Area > 0.001f) sbatch.FillRectangle(rBot, Color.DarkRed * 0.35f);
				if (rLef.Area > 0.001f) sbatch.FillRectangle(rLef, Color.DarkRed * 0.35f);
				if (rRig.Area > 0.001f) sbatch.FillRectangle(rRig, Color.DarkRed * 0.35f);

				sbatch.DrawRectangle(GuaranteedMapViewport, Color.Red * 0.8f, 1f);
			}
		}
#endif

		public virtual void Resize(int width, int height)
		{
			GameHUD.RecalculateAllElementPositions();
		}

		public void PushNotification(string text)
		{
			DebugDisp.AddDecayLine(text);
		}

		void IUpdateOperationOwner.AddOperation(IUpdateOperation o) { AddAgent(o); }

		public void AddAgent(IUpdateOperation a)
		{
			agents.Add(a);
			a.InitUnchecked(this);
		}

		public IEnumerable<T> GetAgents<T>()
		{
			return agents.OfType<T>();
		}

		public IEnumerable<T> GetEntities<T>()
		{
			return Entities.Enumerate().OfType<T>();
		}

		public IEnumerable<GameEntity> GetAllEntities()
		{
			return Entities.Enumerate();
		}

		public FPoint TranslateGameToHUDCoordinates(float x, float y)
		{
			return VAdapterHUD.PointToScreen(VAdapterGame.ScreenToPoint(x, y));
		}

		public FPoint TranslateHUDToGameCoordinates(float x, float y)
		{
			return VAdapterGame.PointToScreen(VAdapterHUD.ScreenToPoint(x, y));
		}

		public void RegisterProxyScreenProvider(IProxyScreenProvider p)
		{
			_proxyScreens.Add(p);
		}

		public void DeregisterProxyScreenProvider(IProxyScreenProvider p)
		{
			_proxyScreens.Remove(p);
		}

		public virtual void Pause() { }
		public virtual void Resume() { }

		protected abstract void OnUpdate(SAMTime gameTime, InputState istate);

		protected abstract EntityManager CreateEntityManager();
		protected abstract GameHUD CreateHUD();
		protected abstract GameBackground CreateBackground();
		protected abstract SAMViewportAdapter CreateViewport();
		protected abstract DebugMinimap CreateDebugMinimap();
		protected abstract FRectangle CreateMapFullBounds();
		protected abstract float GetBaseTextureScale();
		protected abstract void OnDrawGame(IBatchRenderer sbatch);
		protected abstract void OnDrawHUD(IBatchRenderer sbatch);
	}
}
