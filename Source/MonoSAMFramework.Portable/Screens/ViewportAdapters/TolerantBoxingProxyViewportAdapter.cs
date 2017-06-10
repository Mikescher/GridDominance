using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.ViewportAdapters
{
	public class TolerantBoxingProxyViewportAdapter : SAMViewportAdapter
	{
		private readonly SAMViewportAdapter _owner;
		private readonly IProxyScreenProvider _proxy;

		private FRectangle _boundsCache;

		private float offsetX = 0.0f; // in device ("real") units
		private float offsetY = 0.0f; // in device ("real") units
		
		private float suboffsetX = 0.0f; // in device ("real") units
		private float suboffsetY = 0.0f; // in device ("real") units

		private float scaleXY = 1.0f; // scale from subviewport to virtual
		private float scaleDX = 1.0f; // scale drom deviceviewport to subviewport
		private float scaleDY = 1.0f; // scale drom deviceviewport to subviewport

		private float _virtualGuaranteedWidth;
		private float _virtualGuaranteedHeight;

		public override float VirtualGuaranteedWidth => _virtualGuaranteedWidth;
		public override float VirtualGuaranteedHeight => _virtualGuaranteedHeight;

		public override float RealGuaranteedWidth => VirtualGuaranteedWidth * scaleXY;
		public override float RealGuaranteedHeight => VirtualGuaranteedHeight * scaleXY;

		public override float VirtualTotalWidth => VirtualGuaranteedBoundingsOffsetX * 2 + VirtualGuaranteedWidth;
		public override float VirtualTotalHeight => VirtualGuaranteedBoundingsOffsetY * 2 + VirtualGuaranteedHeight;

		public override float RealTotalWidth => VirtualTotalWidth * scaleXY;
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

		public TolerantBoxingProxyViewportAdapter(SAMViewportAdapter owner, IProxyScreenProvider provider) 
			: base(owner.GraphicsDevice)
		{
			_owner = owner;
			_proxy = provider;

			_virtualGuaranteedWidth = owner.VirtualGuaranteedWidth;
			_virtualGuaranteedHeight = owner.VirtualGuaranteedHeight;

			_boundsCache = _proxy.ProxyTargetBounds;
			UpdateMatrix();
		}

		public override void Update()
		{
			if (_boundsCache != _proxy.ProxyTargetBounds)
			{
				_boundsCache = _proxy.ProxyTargetBounds;
				UpdateMatrix();
			}

			if (Math.Abs(_virtualGuaranteedWidth - _owner.VirtualGuaranteedWidth) > FloatMath.EPSILON)
			{
				_virtualGuaranteedWidth  = _owner.VirtualGuaranteedWidth;
				_virtualGuaranteedHeight = _owner.VirtualGuaranteedHeight;
				
				UpdateMatrix();
			}

			if (Math.Abs(_virtualGuaranteedWidth - _owner.VirtualGuaranteedWidth) > FloatMath.EPSILON)
			{
				_virtualGuaranteedWidth  = _owner.VirtualGuaranteedWidth;
				_virtualGuaranteedHeight = _owner.VirtualGuaranteedHeight;
				
				UpdateMatrix();
			}
		}

		public override void ChangeVirtualSize(float virtualWidth, float virtualHeight)
		{
			_virtualGuaranteedWidth = virtualWidth;
			_virtualGuaranteedHeight = virtualHeight;

			UpdateMatrix();
		}

		protected void UpdateMatrix()
		{
			var deviceviewport = GraphicsDevice.Viewport;
			var subviewport = _boundsCache;
			
			var aspectRatio = VirtualGuaranteedWidth * 1f / VirtualGuaranteedHeight;
			var viewportRatio = subviewport.Width * 1f / subviewport.Height;

			if (aspectRatio < viewportRatio)
			{
				scaleXY = subviewport.Height * 1f / VirtualGuaranteedHeight;
			}
			else
			{
				scaleXY = subviewport.Width * 1f / VirtualGuaranteedWidth;
			}

			scaleDX = subviewport.Width / deviceviewport.Width;
			scaleDY = subviewport.Height / deviceviewport.Height;

			offsetX = (subviewport.Width - VirtualGuaranteedWidth * scaleXY) / 2;
			offsetY = (subviewport.Height - VirtualGuaranteedHeight * scaleXY) / 2;

			suboffsetX = subviewport.X;
			suboffsetY = subviewport.Y;

			cachedScaleMatrix = CalculateScaleMatrix();
			cachedShaderMatrix = CalculateShaderMatrix();
		}

		private Matrix CalculateScaleMatrix()
		{
			return MatrixExtensions.CreateScaleTranslation(scaleDX * offsetX + suboffsetX, scaleDY * offsetY + suboffsetY, scaleXY, scaleXY);
		}

		private Matrix CalculateShaderMatrix()
		{
			return Matrix.CreateOrthographicOffCenter(VirtualTotalBoundingBoxLeft, VirtualTotalBoundingBoxRight, VirtualTotalBoundingBoxBottom, VirtualTotalBoundingBoxTop, -1024, +1024);
		}

		public override Matrix GetFarseerDebugProjectionMatrix()
		{
			return Matrix.CreateScale(new Vector3(scaleXY, scaleXY, 1)) * Matrix.CreateTranslation(offsetX, offsetY, 0) * Matrix.CreateOrthographicOffCenter(0f, RealTotalWidth, RealTotalHeight, 0f, 0f, 1f);
		}

		public override SAMViewportAdapter CreateProxyAdapter(IProxyScreenProvider p)
		{
			return new TolerantBoxingProxyViewportAdapter(this, p);
		}
	}
}
