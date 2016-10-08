using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapScreen : GameScreen
	{
		public const int VIEW_WIDTH = GDGameScreen.VIEW_WIDTH;
		public const int VIEW_HEIGHT = GDGameScreen.VIEW_HEIGHT;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Initialize();
		}

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);
		protected override GameBackground CreateBackground() => new WorldMapBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new GDWorldMapDebugMinimap(this);

		private void Initialize()
		{

#if DEBUG
			DebugSettings.AddTrigger("SetQuality_1", this, Keys.D1, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("SetQuality_2", this, Keys.D2, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("SetQuality_3", this, Keys.D3, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("SetQuality_4", this, Keys.D4, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("SetQuality_5", this, Keys.D5, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.HD));

			DebugSettings.AddSwitch("DebugTextDisplay", this, Keys.F2, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("ShowMatrixTextInfos", this, Keys.F3, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("ShowDebugMiniMap", this, Keys.F4, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("DebugEntityBoundaries", this, Keys.F5, KeyboardModifiers.None, false);

			DebugSettings.AddPush("ShowDebugShortcuts", this, Keys.Tab, KeyboardModifiers.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} (d = {FPSCounter.MaximumDelta * 1000:0000.0} ) | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} (d = {UPSCounter.MaximumDelta * 1000:0000.0} ) | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00}");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | Particles = {Entities.Enumerate().OfType<ParticleEmitter>().Sum(p => p.ParticleDataCount),3}/{Entities.Enumerate().OfType<ParticleEmitter>().Sum(p => p.ParticleRenderCount),3} (Visible: {Entities.Enumerate().OfType<ParticleEmitter>().Where(p=>p.IsInViewport).Sum(p => p.ParticleDataCount),3}/{Entities.Enumerate().OfType<ParticleEmitter>().Where(p => p.IsInViewport).Sum(p => p.ParticleRenderCount),3})");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {LastRenderSpriteCount:0000}; OGL Text = {LastRenderTextCount:0000}");
				DebugDisp.AddLine(() => $"Map Offset = {MapOffset}");

				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualGuaranteedSize={VAdapter.VirtualGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealGuaranteedSize={VAdapter.RealGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualTotalSize={VAdapter.VirtualTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealTotalSize={VAdapter.RealTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualOffset={VAdapter.VirtualGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealOffset={VAdapter.RealGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Scale={VAdapter.Scale}");

				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);
			}
#endif

			Entities.AddEntity(new LevelNode(this, new Vector2(GDGameScreen.TILE_WIDTH * 0.5f, GDGameScreen.TILE_WIDTH * 4.5f)));
			//Entities.AddEntity(new LevelNode(this, new Vector2(GDGameScreen.TILE_WIDTH * 8.5f, GDGameScreen.TILE_WIDTH * 4.5f)));
			Entities.AddEntity(new LevelNode(this, new Vector2(GDGameScreen.TILE_WIDTH * 8.5f, GDGameScreen.TILE_WIDTH * -4.5f)));
			Entities.AddEntity(new LevelNode(this, new Vector2(GDGameScreen.TILE_WIDTH * 16.5f, GDGameScreen.TILE_WIDTH * 4.5f)));
			//			Entities.AddEntity(new LevelNode(this, new Vector2(GDGameScreen.TILE_WIDTH * 16.5f, GDGameScreen.TILE_WIDTH * 12.5f)));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 1.5f, GDGameScreen.TILE_WIDTH * 8.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 40f,
					ParticleLifetime = 1f,
					ParticleVelocity = 45f,
					ParticleSize = 48,
					Color = Color.CornflowerBlue,
				}.Build()));

			Entities.AddEntity(new PathParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 5.5f, GDGameScreen.TILE_WIDTH * 4.0f),
				new VectorPath(new List<VectorPathSegment> { new EllipsePieceSegment(FPoint.Zero, 128f, 64f, FloatMath.RAD_NEG_225, FloatMath.RAD_POS_045) }),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 380f,
					ParticleLifetime = 0.5f,
					ParticleVelocity = 15f,
					ParticleSize = 16,
					Color = Color.Black,
				}.Build()));

			Entities.AddEntity(new PathParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 5.5f, GDGameScreen.TILE_WIDTH * 5.5f),
				PathPresets.PATH_LETTER_A.AsScaled(150f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 500,
					ParticleLifetimeMin = 0.8f,
					ParticleLifetimeMax = 1.2f,
					ParticleVelocityMax = 15f,
					ParticleVelocityMin = 7f,
					ParticleSizeInitial = 16,
					ParticleSizeFinal = 12,
					ParticleAlphaInitial = 1f,
					ParticleAlphaFinal = 0.25f,
					Color = Color.Black,
				}.Build()));

			Entities.AddEntity(new PathParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 13.5f, GDGameScreen.TILE_WIDTH * 5.5f),
				PathPresets.PATH_LETTER_B.AsScaled(150f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 500,
					ParticleLifetimeMin = 0.8f,
					ParticleLifetimeMax = 1.2f,
					ParticleVelocityMax = 15f,
					ParticleVelocityMin = 7f,
					ParticleSizeInitial = 16,
					ParticleSizeFinal = 12,
					ParticleAlphaInitial = 1f,
					ParticleAlphaFinal = 0.25f,
					Color = Color.Black,
				}.Build()));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 5.5f, GDGameScreen.TILE_WIDTH * 8.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 80f,
					ParticleLifetime = 0.5f,
					ParticleVelocity = 45f,
					ParticleSize = 16,
					ColorInitial = Color.Yellow,
					ColorFinal = Color.Red,
				}.Build()));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 6.5f, GDGameScreen.TILE_WIDTH * 8.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 80f,
					ParticleLifetime = 0.5f,
					ParticleVelocity = 45f,
					ParticleSize = 16,
					ColorInitial = Color.Yellow,
					ColorFinal = Color.Red,
				}.Build()));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 7.5f, GDGameScreen.TILE_WIDTH * 8.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[0],
					SpawnRate = 80f,
					ParticleLifetime = 0.5f,
					ParticleVelocity = 45f,
					ParticleSize = 16,
					ColorInitial = Color.Yellow,
					ColorFinal = Color.Red,
				}.Build()));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 9.5f, GDGameScreen.TILE_WIDTH * 1.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[12],
					SpawnRate = 55f,
					ParticleLifetimeMin = 1.8f,
					ParticleLifetimeMax = 3.2f,
					ParticleVelocityMin = 45f,
					ParticleVelocityMax = 70f,
					ParticleSizeInitialMin = 8,
					ParticleSizeInitialMax = 24,
					ParticleSizeFinalMin = 32,
					ParticleSizeFinalMax = 64,
					ParticleAlphaInitial = 1f,
					ParticleAlphaFinal = 0f,
					ParticleSpawnAngleMin = FloatMath.ToRadians(-35),
					ParticleSpawnAngleMax = FloatMath.ToRadians(+35),
					Color = Color.Red,
				}.Build()));

			Entities.AddEntity(new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 9.5f, GDGameScreen.TILE_WIDTH * 1.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[12],
					SpawnRate = 55f,
					ParticleLifetimeMin = 1.8f,
					ParticleLifetimeMax = 3.2f,
					ParticleVelocityMin = 45f,
					ParticleVelocityMax = 70f,
					ParticleSizeInitialMin = 8,
					ParticleSizeInitialMax = 24,
					ParticleSizeFinalMin = 32,
					ParticleSizeFinalMax = 64,
					ParticleAlphaInitial = 1f,
					ParticleAlphaFinal = 0f,
					ParticleSpawnAngleMin = FloatMath.ToRadians(145),
					ParticleSpawnAngleMax = FloatMath.ToRadians(215),
					Color = Color.Red,
				}.Build()));

			for (char chr = 'A'; chr <= 'Z'; chr++)
			{
				var i = chr - 'A';
				var x = GDGameScreen.TILE_WIDTH * 3.5f + GDGameScreen.TILE_WIDTH * 3.5f * (i % 5);
				var y = GDGameScreen.TILE_WIDTH * 13.5f + GDGameScreen.TILE_WIDTH * 3.5f * (int)(i / 5);

				Entities.AddEntity(new PathParticleEmitter(this,
					new Vector2(x, y),
					PathPresets.LETTERS[chr].AsScaled(150f),
					new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						Texture = Textures.TexParticle[10],
						SpawnRate = 500,
						ParticleLifetimeMin = 0.8f,
						ParticleLifetimeMax = 1.2f,
						ParticleVelocityMax = 15f,
						ParticleVelocityMin = 7f,
						ParticleSizeInitial = 16,
						ParticleSizeFinal = 12,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0.25f,
						ColorInitial = Color.Beige,
						ColorFinal = Color.Orange,
					}.Build()));
			}



			Entities.AddEntity(PPE1 = new PointParticleEmitter(this,
				new Vector2(GDGameScreen.TILE_WIDTH * 1.5f, GDGameScreen.TILE_WIDTH * 1.5f),
				new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[10],
					SpawnRate = 150f,
					ParticleLifetime = 1.5f,
					ParticleVelocity = 25f,
					ParticleSizeInitialMin = 16,
					ParticleSizeInitialMax = 32,
					ParticleSizeFinalMin = 6,
					ParticleSizeFinalMax = 8,
					ParticleAlphaInitial = 1f,
					ParticleAlphaFinal = 0f,
					ColorInitial = Color.White,
					ColorFinal = Color.CornflowerBlue,
				}.Build()));

			AddAgent(new WorldMapDragAgent(this));
		}

		private PointParticleEmitter PPE1;

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif

			var p = new Vector2(GDGameScreen.TILE_WIDTH * 13.5f, GDGameScreen.TILE_WIDTH * 5.5f) + new Vector2(GDGameScreen.TILE_WIDTH * 2.0f, 0).Rotate(FloatMath.NormalizeAngle((float)gameTime.TotalGameTime.TotalSeconds));

			PPE1.SetPosition(p);
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

#if DEBUG
			var newQuality = Textures.GetPreferredQuality(Game.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Game.Content, newQuality);
			}
#endif
		}
	}
}
