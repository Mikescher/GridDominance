
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using System;

namespace MonoSAMFramework.Portable.BatchRenderer
{
	// ReSharper disable UnusedMemberInSuper.Global
	// ReSharper disable UnusedMember.Global
	public interface IBatchRenderer : IDisposable
	{
		void Begin(float defTexScale, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);
		void End();

		void DrawStretched(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color, float rotation = 0f, float layerDepth = 0f);
		void DrawCentered(TextureRegion2D texture, FPoint centerTarget, float width, float height, Color color, float rotation = 0f, float layerDepth = 0f);
		void DrawScaled(TextureRegion2D texture, FPoint centerTarget, float scale, Color color, float rotation = 0f, float layerDepth = 0f);

		void DrawString(SpriteFont spriteFont, string text, FPoint position, Color color);
		void DrawString(SpriteFont spriteFont, string text, FPoint position, Color color, float rotation, FPoint origin, float scale, SpriteEffects effects, float layerDepth);

		void FillRectangle(FRectangle rectangle, Color color);
		void FillRectangle(FPoint location, FSize size, Color color);
		void FillRectangleRot(FPoint center, FSize size, Color color, float rotation);
		void DrawRectangle(FPoint location, FSize size, Color color, float thickness = 1f);
		void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1f);
		void DrawRectangleRot(FRectangle rectangle, Color color, float rotation, float thickness = 1f);
		void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f);
		void DrawLine(FPoint point1, FPoint point2, Color color, float thickness = 1f);
		void FillCircle(FPoint center, float radius, int sides, Color color);
		void DrawCircle(FPoint center, float radius, int sides, Color color, float thickness = 1f);
		void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1f);
		void DrawCirclePiece(FPoint center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f);
		void DrawPiePiece(FPoint center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f);
		void DrawPath(FPoint posVector2, VectorPath path, int segments, Color color, float thickness = 1f);
		void DrawShape(IFShape shape, Color color, float thickness = 1f);

		void DrawRot000(TextureRegion2D texture, FRectangle destinationRectangle, Color color, float layerDepth);
		void DrawRot090(TextureRegion2D texture, FRectangle destinationRectangle, Color color, float layerDepth);
		void DrawRot180(TextureRegion2D texture, FRectangle destinationRectangle, Color color, float layerDepth);
		void DrawRot270(TextureRegion2D texture, FRectangle destinationRectangle, Color color, float layerDepth);

		//void DrawRaw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		//void DrawRaw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
		
#if DEBUG
		IDisposable BeginDebugDraw();

		int LastReleaseRenderSpriteCount { get; }
		int LastReleaseRenderTextCount { get; }
		int LastDebugRenderSpriteCount { get; }
		int LastDebugRenderTextCount { get; }
#endif

		void OnBegin(float defaultTexScale);
		void OnEnd();
	}
}
