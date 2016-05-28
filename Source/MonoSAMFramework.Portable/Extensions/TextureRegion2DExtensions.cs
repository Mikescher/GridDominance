using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

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
	}
}
