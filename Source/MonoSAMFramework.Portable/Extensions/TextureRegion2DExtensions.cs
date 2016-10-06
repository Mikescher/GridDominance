using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class TextureRegion2DExtensions
	{
		public static Vector2 Center(this TextureRegion2D source)
		{
			return new Vector2(source.Width/2f, source.Height/2f);
		}

		public static FSize Size(this TextureRegion2D source)
		{
			return new FSize(source.Width, source.Height);
		}

		public static Matrix GetShaderProjectionMatrix(this TextureRegion2D source)
		{
			var bounds = source.Bounds;

			float sx = bounds.Width * 1f / source.Texture.Width;
			float sy = bounds.Height * 1f / source.Texture.Height;

			float px = bounds.X * 1f / source.Texture.Width;
			float py = bounds.Y * 1f / source.Texture.Height;

			return Matrix.Identity;
			//return MatrixExtensions.CreateScaleTranslation(px, py, sx, sy);
		}
	}
}
