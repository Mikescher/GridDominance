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
		public SAMViewportAdapter VAdapter;
		public GameHUD HUD => GameHUD;

		protected InputStateManager InputStateMan;

		protected GameHUD GameHUD;
		protected EntityManager Entities;
		protected GameBackground Background;
		protected IDebugTextDisplay DebugDisp;

		protected SpriteBatch InternalBatch;
		protected IBatchRenderer FixedBatch;                  // no translation          (for HUD)
		protected ITranslateBatchRenderer TranslatedBatch;    // translated by MapOffset (for everything else)

#if DEBUG
		protected RealtimeAPSCounter FPSCounter;
		protected RealtimeAPSCounter UPSCounter;
		protected GCMonitor GCMonitor;
		protected DebugMinimap DebugMap;
#endif

		public float GameSpeed = 1f;

		public float MapOffsetX { get { return _mapOffsetX; } set { _mapOffsetX = value; TranslatedBatch.VirtualOffsetX = value; } }
		public float MapOffsetY { get { return _mapOffsetY; } set { _mapOffsetY = value; TranslatedBatch.VirtualOffsetY = value; } }
		public Vector2 MapOffset => new Vector2(_mapOffsetX, _mapOffsetY);
		public float MapViewportCenterX { get { return VAdapter.VirtualTotalWidth  / 2 - MapOffsetX - VAdapter.VirtualGuaranteedBoundingsOffsetX; } set { MapOffsetX = VAdapter.VirtualTotalWidth / 2 - VAdapter.VirtualGuaranteedBoundingsOffsetX - value; } }
		public float MapViewportCenterY { get { return VAdapter.VirtualTotalHeight / 2 - MapOffsetY - VAdapter.VirtualGuaranteedBoundingsOffsetY; } set { MapOffsetY = VAdapter.VirtualTotalHeight / 2 - VAdapter.VirtualGuaranteedBoundingsOffsetY - value; } }
		public Vector2 MapViewportCenter => new Vector2(MapViewportCenterX, MapViewportCenterY);
		public FRectangle GuaranteedMapViewport => new FRectangle(-MapOffsetX, -MapOffsetY, VAdapter.VirtualGuaranteedWidth, VAdapter.VirtualGuaranteedHeight);
		public FRectangle CompleteMapViewport => new FRectangle(-MapOffsetX - VAdapter.VirtualGuaranteedBoundingsOffsetX, -MapOffsetY - VAdapter.VirtualGuaranteedBoundingsOffsetY, VAdapter.VirtualTotalWidth, VAdapter.VirtualTotalHeight);
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
			VAdapter = CreateViewport();

			InternalBatch   = new SpriteBatch(Graphics.GraphicsDevice);
			FixedBatch      = new StandardSpriteBatchWrapper(InternalBatch);
			TranslatedBatch = new TranslatingSpriteBatchWrapper(InternalBatch);

			InputStateMan = new InputStateManager(VAdapter, MapOffsetX, MapOffsetY);
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

			foreach (var agent in agents) agent.Update(timeVirtual, state);

			OnUpdate(timeVirtual, state);
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

			FixedBatch.OnBegin(bts);
			TranslatedBatch.OnBegin(bts);
			InternalBatch.Begin(transformMatrix: VAdapter.GetScaleMatrix());
			{
				Background.Draw(TranslatedBatch);

				Entities.Draw(TranslatedBatch);

				GameHUD.Draw(FixedBatch);

#if DEBUG
				using (FixedBatch.BeginDebugDraw()) DebugMap.Draw(FixedBatch);
#endif
			}
			InternalBatch.End();
			TranslatedBatch.OnEnd();
			FixedBatch.OnEnd();

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
	}
}
