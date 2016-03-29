using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class TextureRegion2DExtensions
	{
		public static Vector2 Center(this TextureRegion2D source)
		{
			return new Vector2(source.Width/2f, source.Height/2f);
		}
	}
}
