using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class SAMSpriteBatchExtensions
	{
		public static void DrawEllipse(this SpriteBatch spriteBatch, RectangleF rectangle, int sides, Color color, float thickness = 1f)
		{
			spriteBatch.DrawPolygon(rectangle.Center, CreateEllipse(rectangle.Width, rectangle.Height, sides), color, thickness);
		}
		
		private static Vector2[] CreateEllipse(float width, float height, int sides)
		{
			const double max = 2.0 * Math.PI;
			var points = new Vector2[sides];
			var step = max / sides;
			var theta = 0.0;

			for (var i = 0; i < sides; i++)
			{
				points[i] = new Vector2((float)(width * Math.Cos(theta) / 2), (float)(height * Math.Sin(theta) / 2));
				theta += step;
			}

			return points;
		}
	}
}
