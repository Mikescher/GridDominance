using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using System;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	public abstract class SpriteBatchCommon : IBatchRenderer
	{
		private class DebugDrawDisposable : IDisposable
		{
			private readonly SpriteBatchCommon sbatch;
			public DebugDrawDisposable(SpriteBatchCommon sbc) { sbatch = sbc; sbatch.isDebugDrawCounter++;}
			public void Dispose() { sbatch.isDebugDrawCounter--; }
		}

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

		#region CreatePrimitive

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

		#endregion

		#region DrawRot

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

		#endregion


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

		#region abstracts

		public abstract void DrawStretched(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color, float rotation = 0f, float layerDepth = 0f);
		public abstract void DrawCentered(TextureRegion2D texture, Vector2 centerTarget, float width, float height, Color color, float rotation = 0f, float layerDepth = 0f);
		public abstract void DrawScaled(TextureRegion2D texture, Vector2 centerTarget, float scale, Color color, float rotation = 0f, float layerDepth = 0f);

		public abstract void Begin(float defTexScale, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);
		public abstract void End();

		public abstract void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
		public abstract void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

		public abstract void FillRectangle(FRectangle rectangle, Color color);
		public abstract void FillRectangle(Vector2 location, Vector2 size, Color color);
		public abstract void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1);
		public abstract void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1);
		public abstract void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1);
		public abstract void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1);
		public abstract void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1);
		public abstract void FillCircle(Vector2 center, float radius, int sides, Color color);
		public abstract void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1);
		public abstract void DrawCirclePiece(Vector2 center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1);
		public abstract void DrawPath(Vector2 posVector2, VectorPath path, int segments, Color color, float thickness = 1);

		public abstract void Dispose();

		#endregion
	}
}
