using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapScreen : GameScreen
	{
		public const int VIEW_WIDTH  = 1024;
		public const int VIEW_HEIGHT = 640;

		public GDWorldMapScreen(MonoSAMGame game, GraphicsDeviceManager gdm) : base(game, gdm)
		{
			Initialize();
		}

		protected override EntityManager CreateEntityManager() => new GDWorldMapEntityManager(this);
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);
		protected override GameBackground CreateBackground() => new SolidColorBackground(this, Color.DarkGray);
		protected override ViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
#endif
			//
		}

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

			DebugSettings.AddPush("ShowDebugShortcuts", this, Keys.Tab, KeyboardModifiers.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00}");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {((IDebugBatchRenderer)MainBatch).LastRenderSpriteCount:0000}; OGL Text = {((IDebugBatchRenderer)MainBatch).LastRenderTextCount:0000}");

				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealSize=[{((TolerantBoxingViewportAdapter)Viewport).RealWidth}|{((TolerantBoxingViewportAdapter)Viewport).RealHeight}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Offset=[{((TolerantBoxingViewportAdapter)Viewport).RealOffsetX}|{((TolerantBoxingViewportAdapter)Viewport).RealOffsetY}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualOffset=[{((TolerantBoxingViewportAdapter)Viewport).VirtualOffsetX}|{((TolerantBoxingViewportAdapter)Viewport).VirtualOffsetY}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Scale={((TolerantBoxingViewportAdapter)Viewport).Scale}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualSize=[{Viewport.VirtualWidth}|{Viewport.VirtualHeight}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.ViewportSize=[{Viewport.ViewportWidth}|{Viewport.ViewportHeight}]");

				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);
			}
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
