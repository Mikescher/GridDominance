using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class RectangleExtensions
	{
		public static FRectangle ToFRectangle(this Rectangle p)
		{
			return new FRectangle(p);
		}

		public static Rectangle TranslateTruncated(this Rectangle p, Vector2 v)
		{
			return new Rectangle((int) (p.X + v.X), (int) (p.Y + v.Y), p.Width, p.Height);
		}

		public static Vector2 CenterOfSize(this Rectangle p)
		{
			return new Vector2(p.Width/2f, p.Height/2f);
		}
	}
}
