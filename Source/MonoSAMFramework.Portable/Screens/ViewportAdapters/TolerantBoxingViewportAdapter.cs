using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Extensions;
using System;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.ViewportAdapters
{
	public class TolerantBoxingViewportAdapter : SAMViewportAdapter
	{
		private float offsetX = 0.0f; // in device ("real") units
		private float offsetY = 0.0f; // in device ("real") units

		private float scaleXY = 1.0f;

		public readonly GameWindow Window;
		public readonly GraphicsDeviceManager GraphicsDeviceManager;

		private float _virtualGuaranteedWidth;
		private float _virtualGuaranteedHeight;

		public override float VirtualGuaranteedWidth => _virtualGuaranteedWidth;
		public override float VirtualGuaranteedHeight => _virtualGuaranteedHeight;

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

		private Matrix cachedScaleMatrix;
		private Matrix cachedShaderMatrix;

		public override Matrix GetScaleMatrix() => cachedScaleMatrix;
		public override Matrix GetShaderMatrix() => cachedShaderMatrix;

		public TolerantBoxingViewportAdapter(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, float virtualWidth, float virtualHeight)
			: base(graphicsDeviceManager.GraphicsDevice)
		{
			Window = window;
			GraphicsDeviceManager = graphicsDeviceManager;

			_virtualGuaranteedWidth = virtualWidth;
			_virtualGuaranteedHeight = virtualHeight;

			window.ClientSizeChanged += OnClientSizeChanged;

			UpdateMatrix();
		}

		public override void ChangeVirtualSize(float virtualWidth, float virtualHeight)
		{
			_virtualGuaranteedWidth = virtualWidth;
			_virtualGuaranteedHeight = virtualHeight;

			UpdateMatrix();
		}

		private void OnClientSizeChanged(object sender, EventArgs eventArgs)
		{
			// Needed for DirectX rendering
			// see http://gamedev.stackexchange.com/questions/68914/issue-with-monogame-resizing

			if (GraphicsDeviceManager.PreferredBackBufferWidth != Window.ClientBounds.Width || GraphicsDeviceManager.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				GraphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
				GraphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
				GraphicsDeviceManager.ApplyChanges();
			}

			UpdateMatrix();
		}

		private void UpdateMatrix()
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

			cachedScaleMatrix = CalculateScaleMatrix();
			cachedShaderMatrix = CalculateShaderMatrix();
		}

		private Matrix CalculateScaleMatrix()
		{
			return MatrixExtensions.CreateScaleTranslation(offsetX, offsetY, scaleXY, scaleXY);
		}

		private Matrix CalculateShaderMatrix()
		{
			return Matrix.CreateOrthographicOffCenter(VirtualTotalBoundingBoxLeft, VirtualTotalBoundingBoxRight, VirtualTotalBoundingBoxBottom, VirtualTotalBoundingBoxTop, -1024, +1024);
		}

		public override Matrix GetFarseerDebugProjectionMatrix()
		{
			var scale = Matrix.CreateScale(new Vector3(scaleXY, scaleXY, 1));
			var trans = Matrix.CreateTranslation(offsetX, offsetY, 0);
			var ortho = Matrix.CreateOrthographicOffCenter(0f, RealTotalWidth, RealTotalHeight, 0f, 0f, 1f);

			return scale * trans * ortho;
		}

		public override SAMViewportAdapter CreateProxyAdapter(IProxyScreenProvider p)
		{
			return new TolerantBoxingProxyViewportAdapter(this, p);
		}
	}
}
