using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Text;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	/// <summary>
	/// Thin wrapper around SpriteBatch
	/// 
	/// Some methods in this class come from MonoGameExtended.SpritebatchWrapper
	/// </summary>
	class StandardSpriteBatchWrapper : IBatchRenderer, IDebugBatchRenderer
	{
		private readonly SpriteBatch internalBatch;
		private static Texture2D _genTexture;

#if DEBUG
		public int LastRenderSpriteCount { get; private set; }
		public int LastRenderTextCount { get; private set; }

		public int RenderSpriteCount { get; private set; }
		public int RenderTextCount   { get; private set; }
#endif

		public StandardSpriteBatchWrapper(GraphicsDevice device)
		{
			internalBatch = new SpriteBatch(device);
		}
		
		private Texture2D GetGenericTexture()
		{
			if (_genTexture == null)
			{
				_genTexture = new Texture2D(internalBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
				_genTexture.SetData(new[] { Color.White });
			}

			return _genTexture;
		}

		public void Dispose()
		{
			internalBatch.Dispose();
		}

		public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
		{
#if DEBUG
			RenderSpriteCount = 0;
			RenderTextCount = 0;
#endif

			internalBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
		}

		public void End()
		{
#if DEBUG
			LastRenderSpriteCount = RenderSpriteCount;
			LastRenderTextCount = RenderTextCount;
#endif

			internalBatch.End();
		}

		public void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null, Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, position, destinationRectangle, sourceRectangle, origin, rotation, scale, color, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, position, sourceRectangle, color);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
		}

		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, position, color);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(texture, destinationRectangle, color);
		}

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color);
		}

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderTextCount += text.Length;
#endif

			internalBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public void DrawPolygon(Vector2 position, PolygonF polygon, Color color, float thickness = 1f)
		{
			DrawPolygon(position, polygon.Vertices, color, thickness);
		}

		public void DrawPolygon(Vector2 offset, Vector2[] points, Color color, float thickness = 1f)
		{
			if (points.Length == 0)
				return;

			if (points.Length == 1)
			{
				DrawPoint(points[0], color, (int)thickness);
				return;
			}

			var texture = GetGenericTexture();

			for (var i = 0; i < points.Length - 1; i++)
				DrawPolygonEdge(texture, points[i] + offset, points[i + 1] + offset, color, thickness);

			DrawPolygonEdge(texture, points[points.Length - 1] + offset, points[0] + offset, color, thickness);
		}

		private void DrawPolygonEdge(Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			var length = Vector2.Distance(point1, point2);
			var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			var scale = new Vector2(length, thickness);
			internalBatch.Draw(texture, point1, color: color, rotation: angle, scale: scale);
		}

		public void FillRectangle(RectangleF rectangle, Color color)
		{
			FillRectangle(rectangle.Location, rectangle.Size, color);
		}

		public void FillRectangle(Vector2 location, Vector2 size, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(GetGenericTexture(), location, null, color, 0, Vector2.Zero, size, SpriteEffects.None, 0);
		}

		public void FillRectangle(float x, float y, float width, float height, Color color)
		{
			FillRectangle(new Vector2(x, y), new Vector2(width, height), color);
		}

		public void DrawRectangle(RectangleF rectangle, Color color, float thickness = 1f)
		{
#if DEBUG
			RenderSpriteCount+=4;
#endif

			var texture = GetGenericTexture();
			var topLeft = new Vector2(rectangle.X, rectangle.Y);
			var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
			var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
			var horizontalScale = new Vector2(rectangle.Width, thickness);
			var verticalScale = new Vector2(thickness, rectangle.Height);

			internalBatch.Draw(texture, topLeft, scale: horizontalScale, color: color);
			internalBatch.Draw(texture, topLeft, scale: verticalScale, color: color);
			internalBatch.Draw(texture, topRight, scale: verticalScale, color: color);
			internalBatch.Draw(texture, bottomLeft, scale: horizontalScale, color: color);
		}

		public void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1f)
		{
			DrawRectangle(new RectangleF(location.X, location.Y, size.X, size.Y), color, thickness);
		}

		public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
		{
			DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
		}

		public void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
		{
			var distance = Vector2.Distance(point1, point2);
			var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			DrawLine(point1, distance, angle, color, thickness);
		}

		public void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			var origin = new Vector2(0f, 0.5f);
			var scale = new Vector2(length, thickness);
			internalBatch.Draw(GetGenericTexture(), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
		}

		public void DrawPoint(float x, float y, Color color, float size = 1f)
		{
			DrawPoint(new Vector2(x, y), color, size);
		}

		public void DrawPoint(Vector2 position, Color color, float size = 1f)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			var scale = Vector2.One * size;
			var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
			internalBatch.Draw(GetGenericTexture(), position + offset, color: color, scale: scale);
		}
		
		public void DrawCircle(CircleF circle, int sides, Color color, float thickness = 1f)
		{
			DrawCircle(circle.Center, circle.Radius, sides, color, thickness);
		}
		
		public void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1f)
		{
			DrawPolygon(center, CreateCircle(radius, sides), color, thickness);
		}
		
		public void DrawCircle(float x, float y, float radius, int sides, Color color, float thickness = 1f)
		{
			DrawPolygon(new Vector2(x, y), CreateCircle(radius, sides), color, thickness);
		}

		private Vector2[] CreateCircle(double radius, int sides)
		{
			const double max = 2.0 * Math.PI;
			var points = new Vector2[sides];
			var step = max / sides;
			var theta = 0.0;

			for (var i = 0; i < sides; i++)
			{
				points[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
				theta += step;
			}

			return points;
		}

		public void DrawEllipse(RectangleF rectangle, int sides, Color color, float thickness = 1f)
		{
			DrawPolygon(rectangle.Center, CreateEllipse(rectangle.Width, rectangle.Height, sides), color, thickness);
		}

		private Vector2[] CreateEllipse(float width, float height, int sides)
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

		public void Draw(TextureRegion2D textureRegion, Vector2 position, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(textureRegion.Texture, position, textureRegion.Bounds, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		}

		public void Draw(TextureRegion2D textureRegion, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(textureRegion.Texture, position, textureRegion.Bounds, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color)
		{
#if DEBUG
			RenderSpriteCount++;
#endif

			internalBatch.Draw(textureRegion.Texture, destinationRectangle, textureRegion.Bounds, color);
		}
	}
}
