using System;
using System.Linq;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using GridDominance.Shared.Screens.WorldMapScreen.Background;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapScreen : GameScreen
	{
		public const int VIEW_WIDTH = 16 * GDConstants.TILE_WIDTH;
		public const int VIEW_HEIGHT = 10 * GDConstants.TILE_WIDTH;

		public bool IsBackgroundPressed = false;
		public BistateProgress ZoomState = BistateProgress.Normal;

		public readonly LevelGraph Graph;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Graph = new LevelGraph(this);

			Initialize();
		}

		protected GDWorldHUD GDHUD => (GDWorldHUD) HUD;

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
			DebugSettings.AddSwitch(null, "DBG", this, KCL.C(SKeys.D, SKeys.AndroidMenu), false);

			DebugSettings.AddTrigger("DBG", "SetQuality_1", this, SKeys.D1, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("DBG", "SetQuality_2", this, SKeys.D2, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("DBG", "SetQuality_3", this, SKeys.D3, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("DBG", "SetQuality_4", this, SKeys.D4, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("DBG", "SetQuality_5", this, SKeys.D5, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.HD));
			DebugSettings.AddTrigger("DBG", "ResetProfile", this, SKeys.R, KeyModifier.Control, x => ResetProfile());
			DebugSettings.AddTrigger("DBG", "ResetProfile", this, SKeys.Z, KeyModifier.None, x => ZoomOut());

			DebugSettings.AddSwitch("DBG", "DebugTextDisplay",      this, SKeys.F2,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugBackground",       this, SKeys.F3,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugHUDBorders",       this, SKeys.F4,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "ShowMatrixTextInfos",   this, SKeys.F6,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowDebugMiniMap",      this, SKeys.F7,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugEntityBoundaries", this, SKeys.F8,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugEntityMouseAreas", this, SKeys.F9,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "ShowOperations",        this, SKeys.F10, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugGestures",         this, SKeys.F11, KeyModifier.None, true);

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
				DebugDisp.AddLine(() => $"GamePointer = ({InputStateMan.GetCurrentState().GamePointerPosition.X:000.0}|{InputStateMan.GetCurrentState().GamePointerPosition.Y:000.0}) | HUDPointer = ({InputStateMan.GetCurrentState().HUDPointerPosition.X:000.0}|{InputStateMan.GetCurrentState().HUDPointerPosition.Y:000.0}) | PointerOnMap = ({InputStateMan.GetCurrentState().GamePointerPositionOnMap.X:000.0}|{InputStateMan.GetCurrentState().GamePointerPositionOnMap.Y:000.0})");
				DebugDisp.AddLine("DebugGestures", () => $"Pinching = {InputStateMan.GetCurrentState().IsGesturePinching} & PinchComplete = {InputStateMan.GetCurrentState().IsGesturePinchComplete} & PinchPower = {InputStateMan.GetCurrentState().LastPinchPower}");
				DebugDisp.AddLine(() => $"OGL Sprites = {LastReleaseRenderSpriteCount:0000} (+ {LastDebugRenderSpriteCount:0000}); OGL Text = {LastReleaseRenderTextCount:0000} (+ {LastDebugRenderTextCount:0000})");
				DebugDisp.AddLine(() => $"Map Offset = {MapOffset} (Map Center = {MapViewportCenter})");
				DebugDisp.AddLine(() => $"CurrentLevelNode = {((GDWorldHUD) HUD).SelectedNode?.Level?.Name ?? "NULL"}; FocusedHUDElement = {HUD.FocusedElement}; ZoomState = {ZoomState}");

				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualGuaranteedSize={VAdapterGame.VirtualGuaranteedSize} || GameAdapter.VirtualGuaranteedSize={VAdapterHUD.VirtualGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealGuaranteedSize={VAdapterGame.RealGuaranteedSize} || GameAdapter.RealGuaranteedSize={VAdapterHUD.RealGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualTotalSize={VAdapterGame.VirtualTotalSize} || GameAdapter.VirtualTotalSize={VAdapterHUD.VirtualTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealTotalSize={VAdapterGame.RealTotalSize} || GameAdapter.RealTotalSize={VAdapterHUD.RealTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualOffset={VAdapterGame.VirtualGuaranteedBoundingsOffset} || GameAdapter.VirtualOffset={VAdapterHUD.VirtualGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealOffset={VAdapterGame.RealGuaranteedBoundingsOffset} || GameAdapter.RealOffset={VAdapterHUD.RealGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.Scale={VAdapterGame.Scale} || GameAdapter.Scale={VAdapterHUD.Scale}");

				DebugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, Entities.Enumerate().SelectMany(e => e.ActiveEntityOperations).Select(o => o.Name)));
				DebugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, HUD.Enumerate().SelectMany(e => e.ActiveHUDOperations).Select(o => o.Name)));

				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);

				DebugDisp.AddLogLines();
				
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

			Graph.Init();

			AddAgent(new WorldMapDragAgent(this, GetEntities<LevelNode>().Select(n => n.Position).ToList()));
			MapOffsetY = VIEW_HEIGHT / -2f;

			((WorldMapBackground)Background).InitBackground(GetEntities<LevelNode>().ToList());
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

			if ((ZoomState == BistateProgress.Expanding || ZoomState == BistateProgress.Expanded) && istate.IsRealJustDown && istate.SwallowConsumer != InputConsumer.HUDElement)
			{
				ZoomIn(istate.GamePointerPositionOnMap);
			}

			if ((ZoomState == BistateProgress.Reverting || ZoomState == BistateProgress.Normal) && istate.IsGesturePinchComplete && istate.LastPinchPower < -10)
			{
				ZoomOut();
			}
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
#if DEBUG
			if (DebugSettings.Get("DebugBackground"))
			{
				sbatch.DrawRectangle(Graph.BoundingRect, Color.OrangeRed, 3f);
				sbatch.DrawRectangle(Graph.BoundingViewport, Color.OrangeRed, 3f);

				DebugRenderHelper.DrawCrossedCircle(sbatch, Color.Lime, MapViewportCenter, 8, 2);
				DebugRenderHelper.DrawHalfCrossedCircle(sbatch, Color.Lime, -MapOffset, 8, 2);
			}
#endif
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		private void ZoomOut()
		{
			if (ZoomState == BistateProgress.Expanding || ZoomState == BistateProgress.Expanded) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			AddAgent(new ZoomOutAgent(this));

			((GDWorldHUD) HUD).SelectedNode?.CloseNode();
		}

		private void ZoomIn(FPoint mapPosCenter)
		{
			if (ZoomState == BistateProgress.Reverting || ZoomState == BistateProgress.Normal) return;
			if (GetAgents<ZoomOutAgent>().Any()) return;
			if (GetAgents<ZoomInAgent>().Any()) return;

			AddAgent(new ZoomInAgent(this, mapPosCenter));
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

#if DEBUG
		private void ResetProfile()
		{
			MainGame.Inst.Profile.InitEmpty();
			MainGame.Inst.SaveProfile();

			DebugDisp.AddDecayLine("Profile reset", 5f, 1f, 1f);
		}
#endif
	}
}
