
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
		// ######## OTHER ########

		void OnBegin();
		void OnEnd();

		// ######## MONOGAME METHODS ########

		void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null);
		void End();

		void DrawStretched(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color);
		void DrawSimple(TextureRegion2D texture, Vector2 centerTarget, float height, float width, Color color, float rotation, float layerDepth = 0);

		void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
		void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

		void FillRectangle(FRectangle rectangle, Color color);
		void FillRectangle(Vector2 location, Vector2 size, Color color);
		void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1f);
		void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1f);
		void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f);
		void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f);
		void FillCircle(Vector2 center, float radius, int sides, Color color);
		void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1f);
		void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1f);
		void DrawCirclePiece(Vector2 center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f);
		void DrawPath(Vector2 posVector2, VectorPath path, int segments, Color color, float thickness = 1f);
		void DrawShape(IFShape shape, Color color, float thickness = 1f);

		void DrawRot000(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot090(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot180(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot270(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);

		void DrawRaw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		void DrawRaw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		void DrawRaw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);

#if DEBUG
		IDisposable BeginDebugDraw();

		int LastReleaseRenderSpriteCount { get; }
		int LastReleaseRenderTextCount { get; }
		int LastDebugRenderSpriteCount { get; }
		int LastDebugRenderTextCount { get; }
#endif
	}
}
