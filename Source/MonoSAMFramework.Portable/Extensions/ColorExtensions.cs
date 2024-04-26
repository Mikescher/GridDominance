using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class ColorExtensions
	{
		public static Vector4 ToVector4(this Color c, float alphaMult)
		{
			return new Vector4
			(
				c.R / 255f, 
				c.G / 255f, 
				c.B / 255f, 
				(c.A / 255f) * alphaMult
			);
		}
	}
}
