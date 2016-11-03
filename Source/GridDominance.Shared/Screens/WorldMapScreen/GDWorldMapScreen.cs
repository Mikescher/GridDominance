using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
		public const int VIEW_WIDTH = 16 * GDGameScreen.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDGameScreen.TILE_WIDTH;

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
			DebugSettings.AddSwitch("ShowMatrixTextInfos", this, Keys.F3, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("ShowDebugMiniMap", this, Keys.F4, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("DebugEntityBoundaries", this, Keys.F5, KeyboardModifiers.None, false);

			DebugSettings.AddPush("ShowDebugShortcuts", this, Keys.Tab, KeyboardModifiers.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} (d = {FPSCounter.MaximumDelta * 1000:0000.0} ) | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} (d = {UPSCounter.MaximumDelta * 1000:0000.0} ) | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"GC = Time since GC:{GCMonitor.TimeSinceLastGC:00.00}s ({GCMonitor.TimeSinceLastGC0:000.00}s | {GCMonitor.TimeSinceLastGC1:000.00}s | {GCMonitor.TimeSinceLastGC2:000.00}s) Memory = {GCMonitor.TotalMemory:000.0}MB Frequency = {GCMonitor.GCFrequency:0.000}");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00}");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | Particles = {Entities.Enumerate().OfType<ParticleEmitter>().Sum(p => p.ParticleDataCount),3} (Visible: {Entities.Enumerate().OfType<ParticleEmitter>().Where(p=>p.IsInViewport).Sum(p => p.ParticleDataCount),3})");
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
			AddLetter('B', 1.0f, 100 + 20, 256, 1);
			AddLetter('L', 0.5f, 100 + 120, 256, 2);
			AddLetter('A', 0.5f, 100 + 190, 256, 3);
			AddLetter('C', 0.5f, 100 + 260, 256, 4);
			AddLetter('K', 0.5f, 100 + 330, 256, 5);

			AddLetter('F', 1.0f, 100 + 500, 256, 6);
			AddLetter('O', 0.5f, 100 + 570, 256, 7);
			AddLetter('R', 0.5f, 100 + 640, 256, 8);
			AddLetter('E', 0.5f, 100 + 710, 256, 9);
			AddLetter('S', 0.5f, 100 + 780, 256, 10);
			AddLetter('T', 0.5f, 100 + 850, 256, 11);

			AddLetter('B', 1.0f, 100 + 260 + 20, 512, 12);
			AddLetter('Y', 0.5f, 100 + 260 + 120, 512, 13);
			AddLetter('T', 0.5f, 100 + 260 + 190, 512, 14);
			AddLetter('E', 0.5f, 100 + 260 + 260, 512, 15);
			AddLetter('S', 0.5f, 100 + 260 + 330, 512, 16);
		}
		
		private int currentConfig = 0;

		private void AddLetter(char chr, float size, float x, float y, int index)
		{
			ParticleEmitterConfig cfg = null;

			switch (currentConfig % 8)
			{
				case 0:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// red fire
						Texture = Textures.TexParticle[12],
						SpawnRate = 100 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 0.5f,
						ParticleLifetimeMax = 1.8f,
						ParticleVelocityMin = 4f * size,
						ParticleVelocityMax = 24f * size,
						ParticleSizeInitial = 24 * size,
						ParticleSizeFinalMin = 0 * size,
						ParticleSizeFinalMax = 24 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0f,
						ColorInitial = Color.DarkOrange,
						ColorFinal = Color.DarkRed,
					}.Build();
					break;
				case 1:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						//blue lines
						Texture = Textures.TexParticle[7],
						SpawnRate = 75 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 0.5f,
						ParticleLifetimeMax = 1.8f,
						ParticleVelocityMin = 4f * size,
						ParticleVelocityMax = 8f * size,
						ParticleSizeInitial = 24 * size,
						ParticleSizeFinalMin = 0 * size,
						ParticleSizeFinalMax = 24 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0.5f,
						ColorInitial = Color.DeepSkyBlue,
						ColorFinal = Color.Turquoise,
					}.Build();
					break;
				case 2:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// gray letters
						Texture = Textures.TexParticle[14],
						SpawnRate = 175 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 0.5f,
						ParticleLifetimeMax = 1.8f,
						ParticleVelocityMin = 0f * size,
						ParticleVelocityMax = 8f * size,
						ParticleSizeInitial = 0 * size,
						ParticleSizeFinalMin = 24 * size,
						ParticleSizeFinalMax = 24 * size,
						ParticleAlphaInitial = 0.2f,
						ParticleAlphaFinal = 1f,
						ColorInitial = Color.DarkGray,
						ColorFinal = Color.DarkSlateGray,
					}.Build();
					break;
				case 3:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// golden bubbles 
						Texture = Textures.TexParticle[11],
						SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 2f,
						ParticleLifetimeMax = 4f,
						ParticleVelocityMin = 4f * size,
						ParticleVelocityMax = 8f * size,
						ParticleSizeInitial = 24 * size,
						ParticleSizeFinalMin = 4 * size,
						ParticleSizeFinalMax = 16 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0f,
						ColorInitial = Color.DimGray,
						ColorFinal = Color.Gold,
					}.Build();
					break;
				case 4:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// star stuff
						Texture = Textures.TexParticle[3],
						SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 8f,
						ParticleLifetimeMax = 10f,
						ParticleVelocityMin = 1f * size,
						ParticleVelocityMax = 2f * size,
						ParticleSizeInitial = 24 * size,
						ParticleSizeFinalMin = 4 * size,
						ParticleSizeFinalMax = 16 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 1f,
						ColorInitial = Color.Black,
						ColorFinal = Color.Gold,
					}.Build();
					break;
				case 5:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// green stars
						Texture = Textures.TexParticle[5],
						SpawnRate = 125 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 0.8f,
						ParticleLifetimeMax = 1.4f,
						ParticleVelocityMin = 0f * size,
						ParticleVelocityMax = 24f * size,
						ParticleSizeInitial = 24 * size,
						ParticleSizeFinalMin = 24 * size,
						ParticleSizeFinalMax = 24 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0f,
						ColorInitial = Color.DarkGreen,
						ColorFinal = Color.GreenYellow,
					}.Build();
					break;
				case 6:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// smokey fire
						Texture = Textures.TexParticle[12],
						SpawnRate = 125 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 1.0f,
						ParticleLifetimeMax = 1.5f,
						ParticleVelocityMin = 0f * size,
						ParticleVelocityMax = 32f * size,
						ParticleSizeInitial = 8 * size,
						ParticleSizeFinalMin = 24 * size,
						ParticleSizeFinalMax = 32 * size,
						ParticleAlphaInitial = 1f,
						ParticleAlphaFinal = 0f,
						ColorInitial = Color.DarkRed,
						ColorFinal = Color.SlateGray,
					}.Build();
					break;
				case 7:
					cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
					{
						// fine lines
						Texture = Textures.TexParticle[7],
						SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
						ParticleLifetimeMin = 4f,
						ParticleLifetimeMax = 4f,
						ParticleVelocityMin = 0f * size,
						ParticleVelocityMax = 0f * size,
						ParticleSizeInitial = 64 * size,
						ParticleSizeFinalMin = 0 * size,
						ParticleSizeFinalMax = 0 * size,
						ParticleAlphaInitial = 0f,
						ParticleAlphaFinal = 1f,
						ColorInitial = Color.DimGray,
						ColorFinal = Color.Goldenrod,
					}.Build();
					break;
			}
			
			var em = new AnimatedPathParticleEmitter(this, new Vector2(x, y - (size * 150) / 2), PathPresets.LETTERS[chr].AsScaled(size * 150), cfg, 0.5f + index * 0.3f, 0.3f);
			Entities.AddEntity(em);
		}
		
		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif
			if (istate.IsJustDown)
			{
				foreach (var e in Entities.Enumerate()) e.Alive = false;
				
				AddLetter('B', 1.0f, 100 + 20, 256, 1);
				AddLetter('L', 0.5f, 100 + 120, 256, 2);
				AddLetter('A', 0.5f, 100 + 190, 256, 3);
				AddLetter('C', 0.5f, 100 + 260, 256, 4);
				AddLetter('K', 0.5f, 100 + 330, 256, 5);
			
				AddLetter('F', 1.0f, 100 + 500, 256, 6);
				AddLetter('O', 0.5f, 100 + 570, 256, 7);
				AddLetter('R', 0.5f, 100 + 640, 256, 8);
				AddLetter('E', 0.5f, 100 + 710, 256, 9);
				AddLetter('S', 0.5f, 100 + 780, 256, 10);
				AddLetter('T', 0.5f, 100 + 850, 256, 11);
			
				AddLetter('B', 1.0f, 100 + 260 + 20, 512, 12);
				AddLetter('Y', 0.5f, 100 + 260 + 120, 512, 13);
				AddLetter('T', 0.5f, 100 + 260 + 190, 512, 14);
				AddLetter('E', 0.5f, 100 + 260 + 260, 512, 15);
				AddLetter('S', 0.5f, 100 + 260 + 330, 512, 16);
			
				currentConfig++;
			}
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
