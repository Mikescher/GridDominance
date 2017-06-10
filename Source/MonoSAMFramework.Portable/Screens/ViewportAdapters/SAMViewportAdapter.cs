using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.ViewportAdapters
{
	/// <summary>
	/// Explanations:
	/// 
	/// Virtual units are in-engine
	/// Real units are device-units (mostly pixel)
	/// Use should use pretty much always virtual units
	/// 
	/// The conversion factor is the Scale property
	/// virtual * scale = real
	/// 
	/// 
	/// </summary>
	public abstract class SAMViewportAdapter
	{
		protected SAMViewportAdapter(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice;
		}

		public GraphicsDevice GraphicsDevice { get; }
		public Viewport Viewport => GraphicsDevice.Viewport;

		public abstract float Scale { get; }

		public abstract float VirtualGuaranteedWidth { get; }
		public abstract float VirtualGuaranteedHeight { get; }
		public FSize VirtualGuaranteedSize => new FSize(VirtualGuaranteedWidth, VirtualGuaranteedHeight);

		public abstract float RealGuaranteedWidth { get; }
		public abstract float RealGuaranteedHeight { get; }
		public FSize RealGuaranteedSize => new FSize(RealGuaranteedWidth, RealGuaranteedHeight);

		public abstract float VirtualTotalWidth { get; }
		public abstract float VirtualTotalHeight { get; }
		public FSize VirtualTotalSize => new FSize(VirtualTotalWidth, VirtualTotalHeight);

		public abstract float RealTotalWidth { get; }
		public abstract float RealTotalHeight { get; }
		public FSize RealTotalSize => new FSize(RealTotalWidth, RealTotalHeight);

		//public int ViewportWidth => Viewport.Width; // Real units !
		//public int ViewportHeight => Viewport.Height;
		//public FSize ViewportSize => new FSize(ViewportWidth, ViewportHeight);

		public abstract float VirtualGuaranteedBoundingsOffsetX { get; }
		public abstract float VirtualGuaranteedBoundingsOffsetY { get; }
		public Vector2 VirtualGuaranteedBoundingsOffset => new Vector2(VirtualGuaranteedBoundingsOffsetX, VirtualGuaranteedBoundingsOffsetY);

		public abstract float RealGuaranteedBoundingsOffsetX { get; }
		public abstract float RealGuaranteedBoundingsOffsetY { get; }
		public Vector2 RealGuaranteedBoundingsOffset => new Vector2(RealGuaranteedBoundingsOffsetX, RealGuaranteedBoundingsOffsetY);

		public float VirtualTotalBoundingBoxLeft => -VirtualGuaranteedBoundingsOffsetX;
		public float VirtualTotalBoundingBoxRight => VirtualTotalWidth - VirtualGuaranteedBoundingsOffsetX;
		public float VirtualTotalBoundingBoxTop => -VirtualGuaranteedBoundingsOffsetY;
		public float VirtualTotalBoundingBoxBottom =>  VirtualTotalHeight - VirtualGuaranteedBoundingsOffsetY;

		public FRectangle VirtualTotalBoundingBox => new FRectangle(VirtualTotalBoundingBoxLeft, VirtualTotalBoundingBoxTop, VirtualTotalWidth, VirtualTotalHeight);
		public FRectangle VirtualGuaranteedBoundingBox => new FRectangle(0, 0, VirtualGuaranteedWidth, VirtualGuaranteedHeight);

		public abstract Matrix GetScaleMatrix();
		public abstract Matrix GetShaderMatrix();
		public abstract Matrix GetFarseerDebugProjectionMatrix();
		public abstract void ChangeVirtualSize(float virtualWidth, float virtualHeight);

		//public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
		//public Point Center => BoundingRectangle.Center;

		public FRectangle ScreenToRect(FRectangle point) // virtual to real
		{
			var p1 = ScreenToPoint(point.TopLeft);
			var p2 = ScreenToPoint(point.BottomRight);
			return new FRectangle(p1, p2);
		}
		
		public FPoint PointToScreen(Point point) // real to virtual
		{
			return PointToScreen(point.X, point.Y);
		}

		public FPoint PointToScreen(FPoint point)
		{
			return PointToScreen(point.X, point.Y);
		}

		public virtual FPoint PointToScreen(float x, float y)
		{
			var scaleMatrix = GetScaleMatrix();
			var invertedMatrix = Matrix.Invert(scaleMatrix);
			return Vector2.Transform(new Vector2(x, y), invertedMatrix).ToFPoint();
		}

		public virtual FPoint ScreenToPoint(float x, float y) // virtual to real
		{
			var scaleMatrix = GetScaleMatrix();
			return Vector2.Transform(new Vector2(x, y), scaleMatrix).ToFPoint();
		}

		public virtual FPoint ScreenToPoint(FPoint fp) // virtual to real
		{
			var scaleMatrix = GetScaleMatrix();
			return Vector2.Transform(new Vector2(fp.X, fp.Y), scaleMatrix).ToFPoint();
		}

		public abstract SAMViewportAdapter CreateProxyAdapter(IProxyScreenProvider p);
		
		public virtual void Update() { }
	}
}
