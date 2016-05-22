using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class RectangleExtensions
	{
		public static Rectangle AsInflated(this Rectangle rect, int horizontalAmount, int verticalAmount)
		{
			return new Rectangle(
				rect.X + horizontalAmount,
				rect.Y + verticalAmount,
				rect.Width - horizontalAmount * 2,
				rect.Height - verticalAmount * 2);
		}

		public static Rectangle AsInflated(this Rectangle rect, float horizontalAmount, float verticalAmount)
		{
			return new Rectangle(
				(int)(rect.X + horizontalAmount),
				(int)(rect.Y + verticalAmount),
				(int)(rect.Width - horizontalAmount * 2),
				(int)(rect.Height - verticalAmount * 2));
		}

		public static Point TopLeft(this Rectangle rect) => new Point(rect.Left, rect.Top);
		public static Point TopRight(this Rectangle rect) => new Point(rect.Right, rect.Top);
		public static Point BottomLeft(this Rectangle rect) => new Point(rect.Left, rect.Bottom);
		public static Point BottomRight(this Rectangle rect) => new Point(rect.Right, rect.Bottom);

		public static Vector2 VectorTopLeft(this Rectangle rect) => new Vector2(rect.Left, rect.Top);
		public static Vector2 VectorTopRight(this Rectangle rect) => new Vector2(rect.Right, rect.Top);
		public static Vector2 VectorBottomLeft(this Rectangle rect) => new Vector2(rect.Left, rect.Bottom);
		public static Vector2 VectorBottomRight(this Rectangle rect) => new Vector2(rect.Right, rect.Bottom);

		public static Rectangle Resize(this Rectangle rect, Size size)
		{
			return new Rectangle(
				rect.X + rect.Width/2 - size.Width/2,
				rect.Y + rect.Height / 2 - size.Height / 2,
				size.Width,
				size.Height);
		}
	}
}
