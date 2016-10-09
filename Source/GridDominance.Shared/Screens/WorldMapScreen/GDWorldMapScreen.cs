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
		}

		private static readonly Dictionary<string, Color> dictionary =
	typeof(Color).GetProperties(BindingFlags.Public |
								BindingFlags.Static)
				 .Where(prop => prop.PropertyType == typeof(Color))
				 .ToDictionary(prop => prop.Name,
							   prop => (Color)prop.GetValue(null, null));

		private int currentConfig = 0;

		private void AddLetterFromConfig(char chr, float size, float x, float y, int index)
		{
			ParticleEmitterConfig cfg = null;

			var file = filecfgcache.Split('\r', '\n').Where(p => p != "").Take(11).Select(p => float.Parse(string.Join("", p.Split('=')[1].Split('*')[0].Trim().Replace("Textures.TexParticle", "").ToCharArray().Where(c => c == '.' || char.IsDigit(c))), NumberStyles.Float, CultureInfo.InvariantCulture)).ToList();
			var col = filecfgcache.Split('\r', '\n').Where(p => p != "").Skip(11).Take(2).Select(p => p.Split('=')[1].Replace("Color.", "").TrimEnd(',').Trim()).ToList();
			try
			{
				cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[(int)file[0]],
					SpawnRate = file[1] * PathPresets.LETTERS[chr].Length,
					ParticleLifetimeMin = file[2],
					ParticleLifetimeMax = file[3],
					ParticleVelocityMin = file[4] * size,
					ParticleVelocityMax = file[5] * size,
					ParticleSizeInitial = file[6] * size,
					ParticleSizeFinalMin = file[7] * size,
					ParticleSizeFinalMax = file[8] * size,
					ParticleAlphaInitial = file[9],
					ParticleAlphaFinal = file[10],
					ColorInitial = dictionary[col[0]],
					ColorFinal = dictionary[col[1]],
				}.Build();


				var em = new AnimatedPathParticleEmitter(this, new Vector2(x, y - (size * 150) / 2), PathPresets.LETTERS[chr].AsScaled(size * 150), cfg, index * 0.3f, 0.3f);
				Entities.AddEntity(em);
			}
			catch (Exception)
			{
				cfg = new ParticleEmitterConfig.ParticleEmitterConfigBuilder
				{
					Texture = Textures.TexParticle[12],
					SpawnRate = 100 * PathPresets.LETTERS['E'].Length,
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


				var em = new AnimatedPathParticleEmitter(this, new Vector2(x, y - (size * 150) / 2), PathPresets.LETTERS[chr].AsScaled(size * 150), cfg, index * 0.3f, 0.3f);
				Entities.AddEntity(em);
			}
			
		}

		private string filecfgcache = "";

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif
			string newt = filecfgcache;
			try
			{
				newt = File.ReadAllText(@"C:\Users\Mike\Desktop\config.txt");
			}
			catch (Exception)
			{
			}

			//if (istate.IsJustDown)
			if (newt != filecfgcache)
			{
				filecfgcache = newt;

				foreach (var e in Entities.Enumerate()) e.Alive = false;
				
				AddLetterFromConfig('B', 1.0f, 100 + 20, 256, 1);
				AddLetterFromConfig('L', 0.5f, 100 + 120, 256, 2);
				AddLetterFromConfig('A', 0.5f, 100 + 190, 256, 3);
				AddLetterFromConfig('C', 0.5f, 100 + 260, 256, 4);
				AddLetterFromConfig('K', 0.5f, 100 + 330, 256, 5);

				AddLetterFromConfig('F', 1.0f, 100 + 500, 256, 6);
				AddLetterFromConfig('O', 0.5f, 100 + 570, 256, 7);
				AddLetterFromConfig('R', 0.5f, 100 + 640, 256, 8);
				AddLetterFromConfig('E', 0.5f, 100 + 710, 256, 9);
				AddLetterFromConfig('S', 0.5f, 100 + 780, 256, 10);
				AddLetterFromConfig('T', 0.5f, 100 + 850, 256, 11);

				AddLetterFromConfig('B', 1.0f, 100 + 260 + 20, 512, 12);
				AddLetterFromConfig('Y', 0.5f, 100 + 260 + 120, 512, 13);
				AddLetterFromConfig('T', 0.5f, 100 + 260 + 190, 512, 14);
				AddLetterFromConfig('E', 0.5f, 100 + 260 + 260, 512, 15);
				AddLetterFromConfig('S', 0.5f, 100 + 260 + 330, 512, 16);

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
