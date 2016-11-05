using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using System;
using System.Text;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	public abstract class SpriteBatchCommon : IBatchRenderer
	{
		protected Vector2[] CreateCircle(double radius, int sides)
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

		protected Vector2[] CreateCirclePiece(double radius, float aMin, float aMax, int sides)
		{
			var points = new Vector2[sides];
			var step = (aMax - aMin) / sides;
			var theta = aMin;

			for (var i = 0; i < sides; i++)
			{
				points[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
				theta += step;
			}

			return points;
		}

		protected Vector2[] CreateEllipse(float width, float height, int sides)
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

		public void DrawRot000(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth)
		{
			Draw(texture, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0);
		}
		
		public void DrawRot090(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth)
		{
			Draw(
				texture, 
				new Rectangle(
					destinationRectangle.X + destinationRectangle.Width, 
					destinationRectangle.Y, 
					destinationRectangle.Height, 
					destinationRectangle.Width), 
				sourceRectangle, 
				color,
				90 * FloatMath.DegreesToRadians, 
				Vector2.Zero, 
				SpriteEffects.None, 
				layerDepth);
		}

		public void DrawRot180(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth)
		{
			Draw(
				texture,
				new Rectangle(
					destinationRectangle.X + destinationRectangle.Width,
					destinationRectangle.Y + destinationRectangle.Height,
					destinationRectangle.Width, 
					destinationRectangle.Height),
				sourceRectangle,
				color,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				SpriteEffects.None,
				layerDepth);
		}

		public void DrawRot270(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth)
		{
			Draw(
				texture,
				new Rectangle(
					destinationRectangle.X,
					destinationRectangle.Y + destinationRectangle.Height,
					destinationRectangle.Height,
					destinationRectangle.Width),
				sourceRectangle,
				color,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				SpriteEffects.None,
				layerDepth);
		}

		#region abstracts

		public abstract void OnBegin();
		public abstract void OnEnd();
		public abstract void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);
		public abstract void End();
		public abstract void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null, Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0);
		public abstract void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		public abstract void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
		public abstract void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
		public abstract void Draw(Texture2D texture, Vector2 position, Color color);
		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Color color);
		public abstract void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
		public abstract void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		public abstract void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		public abstract void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color);
		public abstract void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		public abstract void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		public abstract void Draw(Texture2D texture, Vector2? position = null, FRectangle? destinationRectangle = null, Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0);
		public abstract void Draw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
		public abstract void Draw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
		public abstract void Draw(Texture2D texture, FRectangle destinationRectangle, Color color);
		public abstract void DrawPolygon(Vector2 position, PolygonF polygon, Color color, float thickness = 1);
		public abstract void DrawPolygon(Vector2 offset, Vector2[] points, Color color, bool closed, float thickness);
		public abstract void FillRectangle(FRectangle rectangle, Color color);
		public abstract void FillRectangle(Vector2 location, Vector2 size, Color color);
		public abstract void FillRectangle(float x, float y, float width, float height, Color color);
		public abstract void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1);
		public abstract void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1);
		public abstract void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1);
		public abstract void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1);
		public abstract void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1);
		public abstract void DrawPoint(float x, float y, Color color, float size = 1);
		public abstract void DrawPoint(Vector2 position, Color color, float size = 1);
		public abstract void DrawCircle(CircleF circle, int sides, Color color, float thickness = 1);
		public abstract void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1);
		public abstract void DrawCircle(float x, float y, float radius, int sides, Color color, float thickness = 1);
		public abstract void FillCircle(Vector2 center, float radius, int sides, Color color);
		public abstract void Draw(TextureRegion2D textureRegion, Vector2 position, Color color);
		public abstract void Draw(TextureRegion2D textureRegion, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		public abstract void Draw(TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color);
		public abstract void Draw(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color);
		public abstract void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1);
		public abstract void DrawCirclePiece(Vector2 center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1);
		public abstract void DrawPath(Vector2 posVector2, VectorPath path, int segments, Color color, float thickness = 1);
		public abstract void Dispose();

		#endregion
	}
}
