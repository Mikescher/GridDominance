
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using System;
using System.Text;

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
		void Draw(Texture2D texture, Vector2? position = null, Rectangle? destinationRectangle = null, Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0);
		void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
		void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
		void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
		void Draw(Texture2D texture, Vector2 position, Color color);
		void Draw(Texture2D texture, Rectangle destinationRectangle, Color color);
		void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
		void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color);
		void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
		void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

		// ######## MONOGAME METHODS WITH FLOAT TYPES ########

		void Draw(Texture2D texture, Vector2? position = null, FRectangle? destinationRectangle = null, Rectangle? sourceRectangle = null, Vector2? origin = null, float rotation = 0, Vector2? scale = null, Color? color = null, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0);
		void Draw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
		void Draw(Texture2D texture, FRectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
		void Draw(Texture2D texture, FRectangle destinationRectangle, Color color);

		void DrawCircle(FCircle circle, int sides, Color color, float thickness = 1);

		// ######## MONOGAME.EXTENDED PRIMITIVE METHODS ########

		void DrawPolygon(Vector2 position, PolygonF polygon, Color color, float thickness = 1f);
		void DrawPolygon(Vector2 offset, Vector2[] points, Color color, bool closed, float thickness);
		void FillRectangle(FRectangle rectangle, Color color);
		void FillRectangle(Vector2 location, Vector2 size, Color color);
		void FillRectangle(float x, float y, float width, float height, Color color);
		void DrawRectangle(FRectangle rectangle, Color color, float thickness = 1f);
		void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1f);
		void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f);
		void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f);
		void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f);
		void DrawPoint(float x, float y, Color color, float size = 1f);
		void DrawPoint(Vector2 position, Color color, float size = 1f);
		void DrawCircle(CircleF circle, int sides, Color color, float thickness = 1f);
		void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1f);
		void DrawCircle(float x, float y, float radius, int sides, Color color, float thickness = 1f);
		void FillCircle(Vector2 center, float radius, int sides, Color color);

		// ######## MONOGAME.EXTENDED EXTENSION METHODS ########

		void Draw(TextureRegion2D textureRegion, Vector2 position, Color color);
		void Draw(TextureRegion2D textureRegion, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
		void Draw(TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color);

		// ######## MONOGAME.EXTENDED EXTENSION METHODS WITH FLOAT TYPES ########

		void Draw(TextureRegion2D textureRegion, FRectangle destinationRectangle, Color color);

		// ######## MONOSAMFRAMEWORK METHODS ########

		void DrawEllipse(FRectangle rectangle, int sides, Color color, float thickness = 1f);
		void DrawCirclePiece(Vector2 center, float radius, float angleMin, float angleMax, int sides, Color color, float thickness = 1f);
		void DrawPath(Vector2 posVector2, VectorPath path, int segments, Color color, float thickness = 1f);
		void Draw(IFShape shape, Color color, float thickness = 1f);

		void DrawRot000(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot090(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot180(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);
		void DrawRot270(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float layerDepth);

		// ######## OTHER ########

#if DEBUG
		IDisposable BeginDebugDraw();

		int LastReleaseRenderSpriteCount { get; }
		int LastReleaseRenderTextCount { get; }
		int LastDebugRenderSpriteCount { get; }
		int LastDebugRenderTextCount { get; }
#endif
	}
}
