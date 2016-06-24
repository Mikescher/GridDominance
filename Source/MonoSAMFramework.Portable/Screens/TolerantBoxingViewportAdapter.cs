using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Screens
{
	public class TolerantBoxingViewportAdapter : ViewportAdapter
	{
		public override int VirtualWidth { get; }
		public override int VirtualHeight { get; }

		public override int ViewportWidth => GraphicsDevice.Viewport.Width;
		public override int ViewportHeight => GraphicsDevice.Viewport.Height;

		public BoxingMode BoxingMode { get; private set; }
		public float RealWidth => VirtualWidth * scaleXY;
		public float RealHeight => VirtualHeight * scaleXY;

		public FSize RealSize => new FSize(RealWidth, RealHeight);

		private float offsetX = 0.0f;
		private float offsetY = 0.0f;

		private float scaleXY = 1.0f;
		public float Scale => scaleXY;

		public float RealOffsetX => offsetX;
		public float RealOffsetY => offsetY;
		public float VirtualOffsetX => offsetX / scaleXY;
		public float VirtualOffsetY => offsetY / scaleXY;
		
		public TolerantBoxingViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
			:base(graphicsDevice)
		{
			VirtualWidth = virtualWidth;
			VirtualHeight = virtualHeight;

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

		private void UpdateMatrix()
		{
			var viewport = GraphicsDevice.Viewport;
			var aspectRatio = VirtualWidth * 1f / VirtualHeight;
			var viewportRatio = viewport.Width * 1f / viewport.Height;

			if (aspectRatio < viewportRatio)
			{
				BoxingMode = BoxingMode.Pillarbox;

				scaleXY = viewport.Height * 1f / VirtualHeight;
			}
			else
			{
				BoxingMode = BoxingMode.Letterbox;

				scaleXY = viewport.Width * 1f / VirtualWidth;
			}

			offsetX = (viewport.Width - VirtualWidth * scaleXY) / 2;
			offsetY = (viewport.Height - VirtualHeight * scaleXY) / 2;

			GraphicsDevice.Viewport = new Viewport(0, 0, viewport.Width, viewport.Height);
		}

		public override Point PointToScreen(int x, int y)
		{
			var viewport = GraphicsDevice.Viewport;
			return base.PointToScreen(x - viewport.X, y - viewport.Y);
		}

		public override Matrix GetScaleMatrix()
		{
			/*
			 * result matrix := 
			 *
			 * | scaleX  0      0       RealOffsetX |
			 * | 0       scaleY 0       offsetY |
			 * | 0       0      1       0       |
			 * | 0       0      0       1       |
			 *
			 *  = Scale(scaleX, scaleY) * translate(RealOffsetX, offsetY)
			*/

			return new Matrix(scaleXY, 0f, 0f, 0, 0, scaleXY, 0, 0, 0, 0, 1, 0, offsetX, offsetY, 0, 1);
		}

		public Matrix GetFarseerDebugProjectionMatrix()
		{
			return Matrix.CreateScale(new Vector3(scaleXY, scaleXY, 1)) * Matrix.CreateTranslation(offsetX, offsetY, 0) * Matrix.CreateOrthographicOffCenter(0f, ViewportWidth, ViewportHeight, 0f, 0f, 1f);
		}
	}
}
