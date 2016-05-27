using Microsoft.Xna.Framework.Graphics;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FontRenderHelper
	{
		public static float GetFontScale(SpriteFont fnt, float targetSize)
		{
			return targetSize / fnt.MeasureString("M").Y;
		}
	}
}
