using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class GameScreen : Screen
	{
		private float _mapOffsetX = 0f;
		private float _mapOffsetY = 0f;

		public GraphicsDevice GraphicsDevice => Graphics.GraphicsDevice;

		public readonly GraphicsDeviceManager Graphics;
		public readonly MonoSAMGame Game;
		public SAMViewportAdapter VAdapterGame;
		public SAMViewportAdapter VAdapterHUD;
		public GameHUD HUD => GameHUD;

		protected InputStateManager InputStateMan;

		protected GameHUD GameHUD;
		public    EntityManager Entities;
		public    GameBackground Background;
		public    IDebugTextDisplay DebugDisp;

		protected SpriteBatch InternalBatch;
		protected IBatchRenderer FixedBatch;         // no translation          (for HUD)
		protected IBatchRenderer TranslatedBatch;    // translated by MapOffset (for everything else)

#if DEBUG
		protected RealtimeAPSCounter FPSCounter;
		protected RealtimeAPSCounter UPSCounter;
		protected GCMonitor GCMonitor;
		protected DebugMinimap DebugMap;
#endif

		public float GameSpeed = 1f;

		public float MapOffsetX { get { return _mapOffsetX; } set { _mapOffsetX = value; } }
		public float MapOffsetY { get { return _mapOffsetY; } set { _mapOffsetY = value; } }
		public Vector2 MapOffset => new Vector2(_mapOffsetX, _mapOffsetY);
		public float MapViewportCenterX { get { return VAdapterGame.VirtualTotalWidth  / 2 - MapOffsetX - VAdapterGame.VirtualGuaranteedBoundingsOffsetX; } set { MapOffsetX = VAdapterGame.VirtualTotalWidth / 2 - VAdapterGame.VirtualGuaranteedBoundingsOffsetX - value; } }
		public float MapViewportCenterY { get { return VAdapterGame.VirtualTotalHeight / 2 - MapOffsetY - VAdapterGame.VirtualGuaranteedBoundingsOffsetY; } set { MapOffsetY = VAdapterGame.VirtualTotalHeight / 2 - VAdapterGame.VirtualGuaranteedBoundingsOffsetY - value; } }
		public Vector2 MapViewportCenter => new Vector2(MapViewportCenterX, MapViewportCenterY);
		public FRectangle GuaranteedMapViewport => new FRectangle(-MapOffsetX, -MapOffsetY, VAdapterGame.VirtualGuaranteedWidth, VAdapterGame.VirtualGuaranteedHeight);
		public FRectangle CompleteMapViewport => new FRectangle(-MapOffsetX - VAdapterGame.VirtualGuaranteedBoundingsOffsetX, -MapOffsetY - VAdapterGame.VirtualGuaranteedBoundingsOffsetY, VAdapterGame.VirtualTotalWidth, VAdapterGame.VirtualTotalHeight);
		public FRectangle MapFullBounds { get; private set; }

		private List<GameScreenAgent> agents;

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
			agents = new List<GameScreenAgent>();

			DebugDisp = new DummyDebugTextDisplay();

#if DEBUG
			FPSCounter = new RealtimeAPSCounter();
			UPSCounter = new RealtimeAPSCounter();
			GCMonitor = new GCMonitor();

			DebugMap = CreateDebugMinimap();
#endif
		}

		protected override void OnRemove()
		{
			base.OnRemove();

			FixedBatch.Dispose();
			TranslatedBatch.Dispose();
		}

		public override void Update(SAMTime gameTime)
		{
			var state = InputStateMan.GetNewState(MapOffsetX, MapOffsetY);

			if (state.IsKeyDown(SKeys.Escape) || state.IsKeyDown(SKeys.AndroidBack)) Game.Exit();

#if DEBUG
			DebugSettings.Update(state);
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
		}

		private void InternalUpdate(SAMTime timeVirtual, InputState state, SAMTime timeReal)
		{
#if DEBUG
			UPSCounter.Update(timeReal);
#endif
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
				agents[i].Update(timeVirtual, istate);

				if (!agents[i].Alive)
				{
					agents.RemoveAt(i);
				}
			}
		}

		public override void Draw(SAMTime gameTime)
		{
#if DEBUG
			FPSCounter.Update(gameTime);
#endif
			// Update Top Down  (Debug -> HUD -> Entities -> BG)
			// Render Bottom Up (BG -> Entities -> HUD -> Debug)

			var bts = GetBaseTextureScale();

			Graphics.GraphicsDevice.Clear(Color.Magenta);

			var mat = Matrix.CreateTranslation(MapOffsetX, MapOffsetY, 0) * VAdapterGame.GetScaleMatrix();


			// ======== GAME =========

			TranslatedBatch.OnBegin(bts);
			InternalBatch.Begin(transformMatrix: mat);
			{
				Background.Draw(TranslatedBatch);
				Entities.Draw(TranslatedBatch);
				OnDrawGame(TranslatedBatch);
			}
			InternalBatch.End();
			TranslatedBatch.OnEnd();

			// ======== HUD ==========

			FixedBatch.OnBegin(bts);
			InternalBatch.Begin(transformMatrix: VAdapterHUD.GetScaleMatrix());
			{
				GameHUD.Draw(FixedBatch);
#if DEBUG
				using (FixedBatch.BeginDebugDraw()) DebugMap.Draw(FixedBatch);
#endif
				OnDrawHUD(TranslatedBatch);
			}
			InternalBatch.End();
			FixedBatch.OnEnd();

			// =======================

			Entities.PostDraw();

#if DEBUG
			using (FixedBatch.BeginDebugDraw())
			using (TranslatedBatch.BeginDebugDraw())
			{
				Entities.DrawOuterDebug();
				DebugDisp.Draw();
			}
#endif
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

			GameHUD.RecalculateAllElementPositions();
		}

		public void PushNotification(string text)
		{
			DebugDisp.AddDecayLine(text);
		}

		public void AddAgent(GameScreenAgent a)
		{
			agents.Add(a);
		}

		public bool RemoveAgent(GameScreenAgent a)
		{
			return agents.Remove(a);
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
