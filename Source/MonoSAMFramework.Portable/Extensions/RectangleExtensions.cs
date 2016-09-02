using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.FloatClasses;

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
	}
}
