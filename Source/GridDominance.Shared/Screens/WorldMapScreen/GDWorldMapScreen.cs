using System.Linq;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Network;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.REST;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapScreen : GameScreen
	{
		public const int VIEW_WIDTH = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Initialize();
		}

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new GDWorldHUD(this);
		protected override GameBackground CreateBackground() => new WorldMapBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new GDWorldMapDebugMinimap(this);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(-8, -8, 48, 48) * GDConstants.TILE_WIDTH;
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private void Initialize()
		{
#if DEBUG
			DebugSettings.AddSwitch(null, "DBG", this, KCL.C(SKeys.D, SKeys.AndroidMenu), true);

			DebugSettings.AddTrigger("DBG", "SetQuality_1", this, SKeys.D1, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("DBG", "SetQuality_2", this, SKeys.D2, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("DBG", "SetQuality_3", this, SKeys.D3, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("DBG", "SetQuality_4", this, SKeys.D4, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("DBG", "SetQuality_5", this, SKeys.D5, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.HD));

			DebugSettings.AddSwitch("DBG", "DebugTextDisplay",      this, SKeys.F2, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugBackground",       this, SKeys.F3, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugHUDBorders",       this, SKeys.F4, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "ShowMatrixTextInfos",   this, SKeys.F6, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowDebugMiniMap",      this, SKeys.F7, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugEntityBoundaries", this, SKeys.F8, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugEntityMouseAreas", this, SKeys.F9, KeyModifier.None, true);

			DebugSettings.AddPush("DBG", "ShowDebugShortcuts",      this, SKeys.Tab, KeyModifier.None);
			DebugSettings.AddPush("DBG", "ShowSerializedProfile",   this, SKeys.O, KeyModifier.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"Device = {Game.Bridge.DeviceName} | Version = {Game.Bridge.DeviceVersion}");
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} (d = {FPSCounter.MaximumDelta * 1000:0000.0} ) | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} (d = {UPSCounter.MaximumDelta * 1000:0000.0} ) | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"GC = Time since GC:{GCMonitor.TimeSinceLastGC:00.00}s ({GCMonitor.TimeSinceLastGC0:000.00}s | {GCMonitor.TimeSinceLastGC1:000.00}s | {GCMonitor.TimeSinceLastGC2:000.00}s) Memory = {GCMonitor.TotalMemory:000.0}MB Frequency = {GCMonitor.GCFrequency:0.000}");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00}");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | EntityOps = {Entities.Enumerate().Sum(p => p.ActiveEntityOperations.Count()):00} | Particles = {Entities.Enumerate().OfType<IParticleOwner>().Sum(p => p.ParticleCount),3} (Visible: {Entities.Enumerate().Where(p => p.IsInViewport).OfType<IParticleOwner>().Sum(p => p.ParticleCount),3})");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0}) | PointerOnMap = ({InputStateMan.GetCurrentState().PointerPositionOnMap.X:000.0}|{InputStateMan.GetCurrentState().PointerPositionOnMap.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {LastReleaseRenderSpriteCount:0000} (+ {LastDebugRenderSpriteCount:0000}); OGL Text = {LastReleaseRenderTextCount:0000} (+ {LastDebugRenderTextCount:0000})");
				DebugDisp.AddLine(() => $"Map Offset = {MapOffset} (Map Center = {MapViewportCenter})");
				DebugDisp.AddLine(() => $"CurrentLevelNode = {((GDWorldHUD) HUD).SelectedNode?.Level?.Name ?? "NULL"}");
				
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualGuaranteedSize={VAdapter.VirtualGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealGuaranteedSize={VAdapter.RealGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualTotalSize={VAdapter.VirtualTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealTotalSize={VAdapter.RealTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualOffset={VAdapter.VirtualGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealOffset={VAdapter.RealGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Scale={VAdapter.Scale}");
				
				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);
				
				DebugDisp.AddLogLines(SAMLogLevel.DEBUG);
				
				DebugDisp.AddLine("ShowSerializedProfile", () => MainGame.Inst.Profile.SerializeToString(128));
				
				DebugDisp.AddLine("FALSE", () => InputStateMan.GetCurrentState().GetFullDebugSummary());

				DebugDisp.AddLine("FALSE", () => Game.Bridge.FullDeviceInfoString);
			}
#endif
			//AddLetter('B', 1.0f, 100 + 20, 256, 1);
			//AddLetter('L', 0.5f, 100 + 120, 256, 2);
			//AddLetter('A', 0.5f, 100 + 190, 256, 3);
			//AddLetter('C', 0.5f, 100 + 260, 256, 4);
			//AddLetter('K', 0.5f, 100 + 330, 256, 5);
			//
			//AddLetter('F', 1.0f, 100 + 500, 256, 6);
			//AddLetter('O', 0.5f, 100 + 570, 256, 7);
			//AddLetter('R', 0.5f, 100 + 640, 256, 8);
			//AddLetter('E', 0.5f, 100 + 710, 256, 9);
			//AddLetter('S', 0.5f, 100 + 780, 256, 10);
			//AddLetter('T', 0.5f, 100 + 850, 256, 11);
			//
			//AddLetter('B', 1.0f, 100 + 260 + 20, 512, 12);
			//AddLetter('Y', 0.5f, 100 + 260 + 120, 512, 13);
			//AddLetter('T', 0.5f, 100 + 260 + 190, 512, 14);
			//AddLetter('E', 0.5f, 100 + 260 + 260, 512, 15);
			//AddLetter('S', 0.5f, 100 + 260 + 330, 512, 16);

			AddLevelNode(4,  10, Levels.LEVEL_001);
			AddLevelNode(10, 10, Levels.LEVEL_002);
			AddLevelNode(16, 10, Levels.LEVEL_003);

			AddAgent(new WorldMapDragAgent(this));
			MapOffsetY = VIEW_HEIGHT / -2f;
		}

		private void AddLevelNode(float x, float y, LevelFile f)
		{
			var data = MainGame.Inst.Profile.GetLevelData(f.UniqueID);
			var pos = new Vector2(GDConstants.TILE_WIDTH * (x + 0.5f), GDConstants.TILE_WIDTH * (y + 0.5f));

			var node = new LevelNode(this, pos, f, data);

			Entities.AddEntity(node);
		}

		private void AddLetter(char chr, float size, float x, float y, int index)
		{
			//*
			var em = new AnimatedPathGPUParticleEmitter(
				this, 
				new Vector2(x, y - (size * 150) / 2), 
				PathPresets.LETTERS[chr].AsScaled(size * 150), 
				ParticlePresets.GetConfigLetterFireRed(size, chr), 
				0.5f + index * 0.3f, 
				0.3f);
			/*/

			var em = new AnimatedPathCPUParticleEmitter(
				this,
				new Vector2(x, y - (size * 150) / 2),
				PathPresets.LETTERS[chr].AsScaled(size * 150),
				ParticlePresets.GetConfigLetterFireRed(size, chr),
				0.5f + index * 0.3f,
				0.3f);
			//*/

			Entities.AddEntity(em);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;

			if (SAMLog.Entries.Any()) DebugSettings.SetManual("DebugTextDisplay", true);
#endif
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
