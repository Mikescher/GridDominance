using System;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Particles;

namespace GridDominance.Shared.Screens
{
	public static class DebugUtils
	{
#if DEBUG
		public static DebugTextDisplay CreateDisplay(GameScreen scrn)
		{
			var debugDisp = new DebugTextDisplay(scrn.Graphics.GraphicsDevice, Textures.DebugFont);
			var gdg = scrn as GDGameScreen;

			debugDisp.AddLine(() => $"Device = {scrn.Game.Bridge.DeviceName} | Version = {scrn.Game.Bridge.DeviceVersion}");
			debugDisp.AddLine(() => $"FPS={scrn.FPSCounter.AverageAPS:0000.0} (curr={scrn.FPSCounter.CurrentAPS:0000.0} delta={scrn.FPSCounter.AverageDelta * 1000:000.00} min={scrn.FPSCounter.MinimumAPS:0000.0} (d={scrn.FPSCounter.MaximumDelta * 1000:0000.0}) cycletime={scrn.FPSCounter.AverageCycleTime * 1000:000.00} (max={scrn.FPSCounter.MaximumCycleTime * 1000:000.00} curr={scrn.FPSCounter.CurrentCycleTime * 1000:000.00}) total={scrn.FPSCounter.TotalActions:000000})");
			debugDisp.AddLine(() => $"UPS={scrn.UPSCounter.AverageAPS:0000.0} (curr={scrn.UPSCounter.CurrentAPS:0000.0} delta={scrn.UPSCounter.AverageDelta * 1000:000.00} min={scrn.UPSCounter.MinimumAPS:0000.0} (d={scrn.UPSCounter.MaximumDelta * 1000:0000.0}) cycletime={scrn.UPSCounter.AverageCycleTime * 1000:000.00} (max={scrn.UPSCounter.MaximumCycleTime * 1000:000.00} curr={scrn.UPSCounter.CurrentCycleTime * 1000:000.00}) total={scrn.UPSCounter.TotalActions:000000})");
			debugDisp.AddLine(() => $"GC = Time since GC:{scrn.GCMonitor.TimeSinceLastGC:00.00}s ({scrn.GCMonitor.TimeSinceLastGC0:000.00}s | {scrn.GCMonitor.TimeSinceLastGC1:000.00}s | {scrn.GCMonitor.TimeSinceLastGC2:000.00}s) Memory = {scrn.GCMonitor.TotalMemory:000.0}MB Frequency = {scrn.GCMonitor.GCFrequency:0.000}");
			debugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(scrn.Game.GraphicsDevice):#.00}");
			debugDisp.AddLine(() => $"Entities = {scrn.Entities.Count(),3} | EntityOps = {scrn.Entities.Enumerate().Sum(p => p.ActiveEntityOperations.Count()):00} | Particles = {scrn.Entities.Enumerate().OfType<IParticleOwner>().Sum(p => p.ParticleCount),3} (Visible: {scrn.Entities.Enumerate().Where(p => p.IsInViewport).OfType<IParticleOwner>().Sum(p => p.ParticleCount),3})");
			debugDisp.AddLine(() => $"GamePointer = ({scrn.InputStateMan.GetCurrentState().GamePointerPosition.X:000.0}|{scrn.InputStateMan.GetCurrentState().GamePointerPosition.Y:000.0}) | HUDPointer = ({scrn.InputStateMan.GetCurrentState().HUDPointerPosition.X:000.0}|{scrn.InputStateMan.GetCurrentState().HUDPointerPosition.Y:000.0}) | PointerOnMap = ({scrn.InputStateMan.GetCurrentState().GamePointerPositionOnMap.X:000.0}|{scrn.InputStateMan.GetCurrentState().GamePointerPositionOnMap.Y:000.0})");
			debugDisp.AddLine("DebugGestures", () => $"Pinching = {scrn.InputStateMan.GetCurrentState().IsGesturePinching} & PinchComplete = {scrn.InputStateMan.GetCurrentState().IsGesturePinchComplete} & PinchPower = {scrn.InputStateMan.GetCurrentState().LastPinchPower}");
			debugDisp.AddLine(() => $"OGL Sprites = {scrn.LastReleaseRenderSpriteCount:0000} (+ {scrn.LastDebugRenderSpriteCount:0000}); OGL Text = {scrn.LastReleaseRenderTextCount:0000} (+ {scrn.LastDebugRenderTextCount:0000})");
			debugDisp.AddLine(() => $"Map Offset = {scrn.MapOffset} (Map Center = {scrn.MapViewportCenter})");
			if (gdg != null) debugDisp.AddLine(() => $"LevelTime = {gdg.LevelTime:000.000} (finished={gdg.HasFinished})");

			if (scrn is GDWorldMapScreen) debugDisp.AddLine(() => $"CurrentLevelNode = {((GDWorldHUD)scrn.HUD).SelectedNode?.Blueprint?.Name ?? "NULL"}; FocusedHUDElement = {scrn.HUD.FocusedElement}; ZoomState = {((GDWorldMapScreen)scrn).ZoomState}");

			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{scrn.Game.GraphicsDevice.Viewport.Width}|{scrn.Game.GraphicsDevice.Viewport.Height}]");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualGuaranteedSize={scrn.VAdapterGame.VirtualGuaranteedSize} || GameAdapter.VirtualGuaranteedSize={scrn.VAdapterHUD.VirtualGuaranteedSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealGuaranteedSize={scrn.VAdapterGame.RealGuaranteedSize} || GameAdapter.RealGuaranteedSize={scrn.VAdapterHUD.RealGuaranteedSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualTotalSize={scrn.VAdapterGame.VirtualTotalSize} || GameAdapter.VirtualTotalSize={scrn.VAdapterHUD.VirtualTotalSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealTotalSize={scrn.VAdapterGame.RealTotalSize} || GameAdapter.RealTotalSize={scrn.VAdapterHUD.RealTotalSize}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.VirtualOffset={scrn.VAdapterGame.VirtualGuaranteedBoundingsOffset} || GameAdapter.VirtualOffset={scrn.VAdapterHUD.VirtualGuaranteedBoundingsOffset}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.RealOffset={scrn.VAdapterGame.RealGuaranteedBoundingsOffset} || GameAdapter.RealOffset={scrn.VAdapterHUD.RealGuaranteedBoundingsOffset}");
			debugDisp.AddLine("ShowMatrixTextInfos", () => $"GameAdapter.Scale={scrn.VAdapterGame.Scale} || GameAdapter.Scale={scrn.VAdapterHUD.Scale}");

			debugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, scrn.Entities.Enumerate().SelectMany(e => e.ActiveEntityOperations).Select(o => o.Name).GroupBy(p=>p).Select(p=>p.Count()==1 ? p.Key : $"{p.Key} (x{p.Count()})")));
			debugDisp.AddLine("ShowOperations", () => string.Join(Environment.NewLine, scrn.HUD.Enumerate().SelectMany(e => e.ActiveHUDOperations).Select(o => o.Name).GroupBy(p => p).Select(p => p.Count() == 1 ? p.Key : $"{p.Key} (x{p.Count()})")));

			debugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);

			debugDisp.AddLogLines();

			debugDisp.AddLine("ShowSerializedProfile", () => MainGame.Inst.Profile.SerializeToString(128));

			debugDisp.AddLine("FALSE", () => scrn.InputStateMan.GetCurrentState().GetFullDebugSummary());
			debugDisp.AddLine("FALSE", () => scrn.Game.Bridge.FullDeviceInfoString);

			return debugDisp;
		}

		public static void CreateShortcuts(GameScreen scrn)
		{
#if __DESKTOP__
			DebugSettings.AddSwitch(null, "DBG", scrn, KCL.C(SKeys.D, SKeys.AndroidMenu), true);
#else
			DebugSettings.AddSwitch(null, "DBG", scrn, KCL.C(SKeys.D, SKeys.AndroidMenu), false);
#endif

			DebugSettings.AddTrigger("DBG", "SetQuality_1",    scrn, SKeys.D1, KeyModifier.Control,                    x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("DBG", "SetQuality_2",    scrn, SKeys.D2, KeyModifier.Control,                    x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("DBG", "SetQuality_3",    scrn, SKeys.D3, KeyModifier.Control,                    x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("DBG", "SetQuality_4",    scrn, SKeys.D4, KeyModifier.Control,                    x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("DBG", "SetQuality_5",    scrn, SKeys.D5, KeyModifier.Control,                    x => Textures.ChangeQuality(scrn.Game.Content, TextureQuality.HD));
			DebugSettings.AddTrigger("DBG", "ResetProfile",    scrn, SKeys.R, KeyModifier.Control,                     x => MainGame.Inst.ResetProfile());
			DebugSettings.AddTrigger("DBG", "ClearMessages",   scrn, SKeys.C, KeyModifier.None,                        x => scrn.DebugDisp.Clear());
			DebugSettings.AddTrigger("DBG", "StartDebugLevel", scrn, SKeys.D, KeyModifier.Control | KeyModifier.Shift, x => MainGame.Inst.SetDebugLevelScreen());
			DebugSettings.AddTrigger("DBG", "ShowOverworld",   scrn, SKeys.O, KeyModifier.Control | KeyModifier.Shift, x => MainGame.Inst.SetOverworldScreen());

			if (scrn is GDWorldMapScreen) DebugSettings.AddTrigger("TRUE", "ZoomOut", scrn, SKeys.Z, KeyModifier.None, x => ((GDWorldMapScreen)scrn).ZoomOut());

			DebugSettings.AddSwitch("DBG", "PhysicsDebugView",      scrn, SKeys.F1,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugTextDisplay",      scrn, SKeys.F2,  KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugBackground",       scrn, SKeys.F3,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugHUDBorders",       scrn, SKeys.F4,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "?",                     scrn, SKeys.F5,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowMatrixTextInfos",   scrn, SKeys.F6,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowDebugMiniMap",      scrn, SKeys.F7,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugEntityBoundaries", scrn, SKeys.F8,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugEntityMouseAreas", scrn, SKeys.F9,  KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowOperations",        scrn, SKeys.F10, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugGestures",         scrn, SKeys.F11, KeyModifier.None, false);

			DebugSettings.AddPush("DBG",  "ShowDebugShortcuts",     scrn, SKeys.Tab,       KeyModifier.None);
			DebugSettings.AddPush("DBG",  "ShowSerializedProfile",  scrn, SKeys.O,         KeyModifier.None);
			if (scrn is GDGameScreen)     DebugSettings.AddPush("TRUE",  "AssimilateCannon",       scrn, SKeys.A,         KeyModifier.None);
			if (scrn is GDWorldMapScreen) DebugSettings.AddPush("TRUE",  "UnlockNode",             scrn, SKeys.A,         KeyModifier.None);
			if (scrn is GDGameScreen)     DebugSettings.AddPush("TRUE",  "AbandonCannon",          scrn, SKeys.S,         KeyModifier.None);
			if (scrn is GDWorldMapScreen) DebugSettings.AddPush("TRUE", "LeaveScreen",            scrn, SKeys.Backspace, KeyModifier.Control);
			DebugSettings.AddPush("TRUE", "HideHUD", scrn, SKeys.H, KeyModifier.None);
		}

#endif
		}
}
