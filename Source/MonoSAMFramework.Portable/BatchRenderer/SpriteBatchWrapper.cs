using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.RenderHelper;
using System;
using System.Linq;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	/// <summary>
	/// Thin wrapper around SpriteBatch
	/// 
	/// Some methods in this class come from MonoGameExtended.SpritebatchWrapper
	/// </summary>
	class SpriteBatchWrapper : IBatchRenderer
	{
		private class DebugDrawDisposable : IDisposable
		{
			private readonly SpriteBatchWrapper sbatch;
			public DebugDrawDisposable(SpriteBatchWrapper sbc) { sbatch = sbc; sbatch.isDebugDrawCounter++; }
			public void Dispose() { sbatch.isDebugDrawCounter--; }
		}

		private readonly SpriteBatch internalBatch;

		private int isDebugDrawCounter = 0;
		public bool IsDebugDraw => isDebugDrawCounter > 0;

#if DEBUG
		public int LastReleaseRenderSpriteCount { get; private set; }
		public int LastReleaseRenderTextCount { get; private set; }
		public int LastDebugRenderSpriteCount { get; private set; }
		public int LastDebugRenderTextCount { get; private set; }

		private int renderSpriteCountRelease;
		private int renderTextCountRelease;
		private int renderSpriteCountDebug;
		private int renderTextCountDebug;
#endif

		protected float TexScale; // SpriteBatch Default texturescale

		public void OnBegin(float defaultTexScale)
		{
			TexScale = defaultTexScale;

#if DEBUG
			renderSpriteCountRelease = 0;
			renderTextCountRelease = 0;
			renderSpriteCountDebug = 0;
			renderTextCountDebug = 0;
#endif
		}

		public void OnEnd()
		{
#if DEBUG
			LastReleaseRenderSpriteCount = renderSpriteCountRelease;
			LastReleaseRenderTextCount = renderTextCountRelease;
			LastDebugRenderSpriteCount = renderSpriteCountDebug;
			LastDebugRenderTextCount = renderTextCountDebug;
#endif
		}

		public IDisposable BeginDebugDraw()
		{
			return new DebugDrawDisposable(this);
		}

#if DEBUG
		protected void IncRenderSpriteCount(int v = 1)
		{
			if (IsDebugDraw) renderSpriteCountDebug += v;
			else renderSpriteCountRelease += v;
		}
		protected void IncRenderTextCount(int v = 1)
		{
			if (IsDebugDraw) renderTextCountDebug += v;
			else renderTextCountRelease += v;
		}
#endif

		public SpriteBatchWrapper(GraphicsDevice device)
		{
			internalBatch = new SpriteBatch(device);
		}

		public SpriteBatchWrapper(SpriteBatch sbatch)
		{
			internalBatch = sbatch;
		}

		public void Dispose()
		{
			internalBatch.Dispose();
		}

		public void Begin(float defTexScale, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
		{
			OnBegin(defTexScale);

			internalBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
		}

		public void End()
		{
			internalBatch.End();

			OnEnd();
		}

		protected FPoint[] CreateCircle(double radius, int sides)
		{
			const double max = 2.0 * Math.PI;
			var points = new FPoint[sides];
			var step = max / sides;
			var theta = 0.0;

			for (var i = 0; i < sides; i++)
			{
				points[i] = new FPoint((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
				theta += step;
			}

			return points;
		}

		protected FPoint[] CreateCirclePiece(double radius, float aMin, float aMax, int sides)
		{
			var points = new FPoint[sides+1];
			var step = (aMax - aMin) / sides;
			var theta = aMin;

			for (var i = 0; i <= sides; i++)
			{
				points[i] = new FPoint((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
				theta += step;
			}

			return points;
		}

		protected FPoint[] CreateEllipse(float width, float height, int sides)
		{
			const double max = 2.0 * Math.PI;
			var points = new FPoint[sides];
			var step = max / sides;
			var theta = 0.0;

			for (var i = 0; i < sides; i++)
			{
				points[i] = new FPoint((float)(width * Math.Cos(theta) / 2), (float)(height * Math.Sin(theta) / 2));
				theta += step;
			}

			return points;
		}

		public void DrawRot000(TextureRegion2D texture, FRectangle destRect, Color color, float layerDepth = 0f)
		{
			var rect = destRect.AsRotated(PerpendicularRotation.DEGREE_CW_000);

			DrawStretched(texture, rect, color, FloatMath.RAD_POS_000, layerDepth);
		}

		public void DrawRot090(TextureRegion2D texture, FRectangle destRect, Color color, float layerDepth = 0f)
		{
			var rect = destRect.AsRotated(PerpendicularRotation.DEGREE_CW_090);

			DrawStretched(texture, rect, color, FloatMath.RAD_POS_090, layerDepth);
		}

		public void DrawRot180(TextureRegion2D texture, FRectangle destRect, Color color, float layerDepth = 0f)
		{
			var rect = destRect.AsRotated(PerpendicularRotation.DEGREE_CW_180);

			DrawStretched(texture, rect, color, FloatMath.RAD_POS_180, layerDepth);
		}

		public void DrawRot270(TextureRegion2D texture, FRectangle destRect, Color color, float layerDepth = 0f)
		{
			var rect = destRect.AsRotated(PerpendicularRotation.DEGREE_CW_270);

			DrawStretched(texture, rect, color, FloatMath.RAD_POS_270, layerDepth);
		}

		public void DrawShape(IFShape shape, Color color, float thickness = 1)
		{
			if (shape is FRectangle)
			{
				DrawRectangle((FRectangle)shape, color, thickness);
				return;
			}

			if (shape is FCircle)
			{
				DrawCircle((FCircle)shape, 32, color, thickness);
				return;
			}
		}

		public void DrawCircle(FCircle circle, int sides, Color color, float thickness = 1f)
		{
			DrawCircle(circle.Center, circle.Radius, sides, color, thickness);
		}

		public void DrawString(SpriteFont spriteFont, string text, FPoint position, Color color)
		{
#if DEBUG
			IncRenderTextCount(text.Length);
#endif

			internalBatch.DrawString(spriteFont, text, position.ToVec2D(), color);
		}

		public void DrawString(SpriteFont spriteFont, string text, FPoint position, Color color, float rotation, FPoint origin, float scale, SpriteEffects effects, float layerDepth)
		{
#if DEBUG
			IncRenderTextCount(text.Length);
#endif

			internalBatch.DrawString(spriteFont, text, position.ToVec2D(), color, rotation, origin.ToVec2D(), scale, effects, layerDepth);
		}
		
		public void DrawPolygon(FPoint offset, FPoint[] points, Color color, bool closed, float thickness)
		{
			if (points.Length == 0)
				return;

			if (points.Length == 1)
			{
				DrawPoint(points[0], color, (int)thickness);
				return;
			}

			var texture = StaticTextures.SinglePixel;

			for (var i = 0; i < points.Length - 1; i++)
				DrawPolygonEdge(texture, points[i].WithOrigin(offset), points[i + 1].WithOrigin(offset), color, thickness);

			if (closed) DrawPolygonEdge(texture, points[points.Length - 1].WithOrigin(offset), points[0].WithOrigin(offset), color, thickness);
		}

		private void DrawPolygonEdge(TextureRegion2D texture, FPoint point1, FPoint point2, Color color, float thickness)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			var length = FPoint.Distance(point1, point2);
			var angle = FloatMath.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			var scale = new Vector2(length, thickness);

			internalBatch.Draw(
				texture: texture.Texture,
				position: point1.ToVec2D(), 
				sourceRectangle: texture.Bounds, 
				color: color, 
				rotation: angle, 
				scale: scale);
		}

		public void FillRectangle(FRectangle rectangle, Color color)
		{
			FillRectangle(rectangle.Location, rectangle.Size, color);
		}

		public void FillRectangle(FPoint location, FSize size, Color color)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			internalBatch.Draw(
				StaticTextures.SinglePixel.Texture,
				location.ToVec2D(),
				StaticTextures.SinglePixel.Bounds,
				color,
				0,
				Vector2.Zero,
				size.ToVec2D(),
				SpriteEffects.None,
				0);
		}

		public void FillRectangleRot(FPoint center, FSize size, Color color, float rotation)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			internalBatch.Draw(
				StaticTextures.SinglePixel.Texture,
				center.ToVec2D(),
				StaticTextures.SinglePixel.Bounds,
				color,
				rotation,
				StaticTextures.SinglePixel.VecCenter(),
				size.ToVec2D(),
				SpriteEffects.None,
				0);
		}

		public void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1f)
		{
#if DEBUG
			IncRenderSpriteCount(4);
#endif

			var pixel = StaticTextures.SinglePixel;
			var topLeft = new Vector2(rectangle.X, rectangle.Y);
			var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
			var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
			var horizontalScale = new Vector2(rectangle.Width, thickness);
			var verticalScale = new Vector2(thickness, rectangle.Height);

			internalBatch.Draw(pixel.Texture, topLeft, sourceRectangle: pixel.Bounds, scale: horizontalScale, color: color);
			internalBatch.Draw(pixel.Texture, topLeft, sourceRectangle: pixel.Bounds, scale: verticalScale, color: color);
			internalBatch.Draw(pixel.Texture, topRight, sourceRectangle: pixel.Bounds, scale: verticalScale, color: color);
			internalBatch.Draw(pixel.Texture, bottomLeft, sourceRectangle: pixel.Bounds, scale: horizontalScale, color: color);
		}

		public void DrawRectangleRot(FRectangle rectangle, Color color, float rotation, float thickness = 1f)
		{
			var center = rectangle.Center;

			var tl = rectangle.TopLeft.RotateAround(center, rotation);
			var tr = rectangle.TopRight.RotateAround(center, rotation);
			var bl = rectangle.BottomLeft.RotateAround(center, rotation);
			var br = rectangle.BottomRight.RotateAround(center, rotation);

			DrawLine(tl, tr, color, thickness);
			DrawLine(tr, br, color, thickness);
			DrawLine(br, bl, color, thickness);
			DrawLine(bl, tl, color, thickness);
		}

		public void DrawRectangle(FPoint location, FSize size, Color color, float thickness = 1f)
		{
			DrawRectangle(new FRectangle(location, size), color, thickness);
		}

		public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
		{
			DrawLine(new FPoint(x1, y1), new FPoint(x2, y2), color, thickness);
		}

		public void DrawLine(FPoint point1, FPoint point2, Color color, float thickness = 1f)
		{
			var distance = FPoint.Distance(point1, point2);
			var angle = FloatMath.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			DrawLine(point1, distance, angle, color, thickness);
		}

		public void DrawLine(FPoint point, float length, float angle, Color color, float thickness = 1f)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			var origin = new Vector2(0f, 0.5f);
			var scale = new Vector2(length, thickness);
			internalBatch.Draw(StaticTextures.SinglePixel.Texture, point.ToVec2D(), StaticTextures.SinglePixel.Bounds, color, angle, origin, scale, SpriteEffects.None, 0);
		}
		
		public void DrawPoint(FPoint position, Color color, float size = 1f)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			var scale = Vector2.One * size;
			var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
			internalBatch.Draw(StaticTextures.SinglePixel.Texture, (position + offset).ToVec2D(), sourceRectangle: StaticTextures.SinglePixel.Bounds, color: color, scale: scale);
		}

		public void DrawCircle(FPoint center, float radius, int sides, Color color, float thickness = 1f)
		{
			DrawPolygon(center, CreateCircle(radius, sides), color, true, thickness);
		}

		public void DrawCirclePiece(FPoint center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f)
		{
			FPoint[] poly = CreateCirclePiece(radius, angleMin, angleMax, sides);

			DrawPolygon(center, poly, color, false, thickness);
		}

		public void DrawPiePiece(FPoint center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f)
		{
			FPoint[] poly = CreateCirclePiece(radius, angleMin, angleMax, sides);

			DrawPolygon(center, poly, color, false, thickness);
			DrawLine(center, poly.First().WithOrigin(center), color, thickness);
			DrawLine(center, poly.Last().WithOrigin(center), color, thickness);
		}

		public void DrawPath(FPoint pos, VectorPath path, int segments, Color color, float thickness = 1)
		{
			foreach (var segment in path.Segments)
			{
				if (segment is LineSegment)
				{
					DrawPathSegment(pos, segment, 1, color, thickness);
				}
				else
				{
					DrawPathSegment(pos, segment, segments, color, thickness);
				}
			}
		}

		public void DrawPathSegment(FPoint pos, VectorPathSegment path, int segments, Color color, float thickness = 1)
		{
			var last = path.Get(0).WithOrigin(pos);
			for (int i = 1; i <= segments; i++)
			{
				var next = path.Get((path.Length * i) / segments).WithOrigin(pos);

				DrawPolygonEdge(StaticTextures.SinglePixel, last, next, color, thickness);

				last = next;
			}
		}

		public void FillCircle(FPoint center, float radius, int sides, Color color)
		{
#if DEBUG
			IncRenderSpriteCount(sides);
#endif

			float angle = FloatMath.TAU / sides;
			float sideWidth = FloatMath.Asin(angle / 2) * 2 * radius;
			float rectHeight = FloatMath.Sin(FloatMath.RAD_POS_090 - angle / 2) * radius;

			
			var r = new FRectangle(center.X - sideWidth/2, center.Y, sideWidth, rectHeight);
			
			for (int i = 0; i < sides; i++)
			{
				internalBatch.Draw(
					StaticTextures.SinglePixel.Texture,
					null,
					r.Round(),
					StaticTextures.SinglePixel.Bounds,
					new Vector2(0.5f, 0),
					angle * i,
					Vector2.One, 
					color);
			}
		}

		public void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1f)
		{
			DrawPolygon(rectangle.Center, CreateEllipse(rectangle.Width, rectangle.Height, sides), color, true, thickness);
		}
		
		public void DrawStretched(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color, float rotation = 0f, float layerDepth = 0f)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			internalBatch.Draw(
				textureRegion.Texture, 
				destinationRectangle.VecCenter, 
				textureRegion.Bounds, 
				color,
				rotation, 
				textureRegion.VecCenter(), 
				new Vector2(destinationRectangle.Width / textureRegion.Width, destinationRectangle.Height / textureRegion.Height), 
				SpriteEffects.None,
				layerDepth);
		}

		public void DrawCentered(TextureRegion2D texture, FPoint centerTarget, float width, float height, Color color, float rotation = 0f, float layerDepth = 0f)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			var scale = new Vector2(width / texture.Bounds.Width, height / texture.Bounds.Height);

			internalBatch.Draw(
				texture.Texture,
				centerTarget.ToVec2D(),
				texture.Bounds,
				color,
				rotation,
				texture.VecCenter(),
				scale,
				SpriteEffects.None,
				layerDepth);
		}

		public void DrawScaled(TextureRegion2D texture, FPoint centerTarget, float scale, Color color, float rotation = 0, float layerDepth = 0)
		{
#if DEBUG
			IncRenderSpriteCount();
#endif

			internalBatch.Draw(
				texture.Texture,
				centerTarget.ToVec2D(),
				texture.Bounds,
				color,
				rotation,
				texture.VecCenter(),
				TexScale * scale,
				SpriteEffects.None,
				0);
		}
	}
}
