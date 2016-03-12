using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Framework
{
	static class ColorMath
	{
		public static Color Darken(this Color c, float perc = 0.5f)
		{
			return new Color(
				perc*(c.R / 255f),
				perc*(c.G / 255f),
				perc*(c.B / 255f),
				c.A);
		}

		public static Color Lighten(this Color c, float perc = 0.5f)
		{
			return new Color(
				1f - perc*(1f - c.R/255f),
				1f - perc*(1f - c.G/255f),
				1f - perc*(1f - c.B/255f),
				c.A);
		}
	}
}
