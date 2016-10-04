using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoSAMFramework.Portable.Screens.ViewportAdapters
{
	public class TolerantBoxingViewportAdapter : SAMViewportAdapter
	{
		private float offsetX = 0.0f; // in device ("real") units
		private float offsetY = 0.0f; // in device ("real") units

		private float scaleXY = 1.0f;
		
		public override float VirtualGuaranteedWidth { get; }
		public override float VirtualGuaranteedHeight { get; }

		public override float RealGuaranteedWidth  => VirtualGuaranteedWidth * scaleXY;
		public override float RealGuaranteedHeight => VirtualGuaranteedHeight * scaleXY;

		public override float VirtualTotalWidth  => VirtualGuaranteedBoundingsOffsetX * 2 + VirtualGuaranteedWidth;
		public override float VirtualTotalHeight => VirtualGuaranteedBoundingsOffsetY * 2 + VirtualGuaranteedHeight;

		public override float RealTotalWidth  => VirtualTotalWidth * scaleXY;
		public override float RealTotalHeight => VirtualTotalHeight * scaleXY;

		public override float VirtualGuaranteedBoundingsOffsetX => offsetX / scaleXY;
		public override float VirtualGuaranteedBoundingsOffsetY => offsetY / scaleXY;

		public override float RealGuaranteedBoundingsOffsetX => offsetX;
		public override float RealGuaranteedBoundingsOffsetY => offsetY;

		public override float Scale => scaleXY;
		
		public TolerantBoxingViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
			:base(graphicsDevice)
		{
			VirtualGuaranteedWidth = virtualWidth;
			VirtualGuaranteedHeight = virtualHeight;

			window.ClientSizeChanged += OnClientSizeChanged;

			UpdateMatrix();
		}
		
		public TolerantBoxingViewportAdapter(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, int virtualWidth, int virtualHeight)
			: this(window, graphicsDeviceManager.GraphicsDevice, virtualWidth, virtualHeight)
		{
		}

		private void OnClientSizeChanged(object sender, EventArgs eventArgs)
		{
			UpdateMatrix();
		}

		private void UpdateMatrix() //TODO does not wotk under DirectX
		{
			var viewport = GraphicsDevice.Viewport;
			var aspectRatio = VirtualGuaranteedWidth * 1f / VirtualGuaranteedHeight;
			var viewportRatio = viewport.Width * 1f / viewport.Height;

			if (aspectRatio < viewportRatio)
			{
				scaleXY = viewport.Height * 1f / VirtualGuaranteedHeight;
			}
			else
			{
				scaleXY = viewport.Width * 1f / VirtualGuaranteedWidth;
			}

			offsetX = (viewport.Width - VirtualGuaranteedWidth * scaleXY) / 2;
			offsetY = (viewport.Height - VirtualGuaranteedHeight * scaleXY) / 2;

			GraphicsDevice.Viewport = new Viewport(0, 0, viewport.Width, viewport.Height);
		}

		public override Matrix GetScaleMatrix()
		{
			/*
			 * result matrix := 
			 *
			 * | scaleX  0      0       offsetx |
			 * | 0       scaleY 0       offsety |
			 * | 0       0      1       0       |
			 * | 0       0      0       1       |
			 *
			 *  = Scale(scaleX, scaleY) * translate(RealOffsetX, offsetY)
			*/

			return new Matrix(scaleXY, 0f, 0f, 0, 0, scaleXY, 0, 0, 0, 0, 1, 0, offsetX, offsetY, 0, 1);
		}

		public Matrix GetFarseerDebugProjectionMatrix()
		{
			return Matrix.CreateScale(new Vector3(scaleXY, scaleXY, 1)) * Matrix.CreateTranslation(offsetX, offsetY, 0) * Matrix.CreateOrthographicOffCenter(0f, RealTotalWidth, RealTotalHeight, 0f, 0f, 1f);
		}
	}
}
