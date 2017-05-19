using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
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

		// http://stackoverflow.com/a/1879223/1761622
		public static bool Intersects(this FRectangle rectangle, FCircle circle)
		{
			// clamp(value, min, max) - limits value to the range min..max

			// Find the closest point to the circle within the rectangle
			float closestX = FloatMath.Clamp(circle.X, rectangle.Left, rectangle.Right);
			float closestY = FloatMath.Clamp(circle.Y, rectangle.Top, rectangle.Bottom);

			// Calculate the distance between the circle's center and this closest point
			float distanceX = circle.X - closestX;
			float distanceY = circle.Y - closestY;

			// If the distance is less than the circle's radius, an intersection occurs
			float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
			return distanceSquared < (circle.Radius * circle.Radius);
		}
	}
}
