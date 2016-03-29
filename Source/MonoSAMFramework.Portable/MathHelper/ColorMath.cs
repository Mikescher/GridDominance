namespace MonoSAMFramework.Portable.MathHelper
{
	public static class ColorMath
	{
		public static Microsoft.Xna.Framework.Color Darken(this Microsoft.Xna.Framework.Color c, float perc = 0.5f)
		{
			return new Microsoft.Xna.Framework.Color(
				perc*(c.R / 255f),
				perc*(c.G / 255f),
				perc*(c.B / 255f),
				c.A);
		}

		public static Microsoft.Xna.Framework.Color Lighten(this Microsoft.Xna.Framework.Color c, float perc = 0.5f)
		{
			return new Microsoft.Xna.Framework.Color(
				1f - perc*(1f - c.R/255f),
				1f - perc*(1f - c.G/255f),
				1f - perc*(1f - c.B/255f),
				c.A);
		}

		public static Microsoft.Xna.Framework.Color Blend(Microsoft.Xna.Framework.Color a, Microsoft.Xna.Framework.Color b, float perc)
		{
			var cr = (1 - perc)*a.R + perc*b.R;
			var cg = (1 - perc)*a.G + perc*b.G;
			var cb = (1 - perc)*a.B + perc*b.B;

			return new Microsoft.Xna.Framework.Color((int)cr, (int)cg, (int)cb);
		}
	}
}
