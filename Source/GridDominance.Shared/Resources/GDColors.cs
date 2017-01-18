using System;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;

namespace GridDominance.Shared.Resources
{
	static class GDColors
	{
		public static readonly Color COLOR_DIFFICULTY_0 = FlatColors.PeterRiver;
		public static readonly Color COLOR_DIFFICULTY_1 = FlatColors.Nephritis;
		public static readonly Color COLOR_DIFFICULTY_2 = FlatColors.SunFlower;
		public static readonly Color COLOR_DIFFICULTY_3 = FlatColors.Pomegranate;

		public static Color GetColorForDifficulty(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.KI_EASY:
					return COLOR_DIFFICULTY_0;
				case FractionDifficulty.KI_NORMAL:
					return COLOR_DIFFICULTY_1;
				case FractionDifficulty.KI_HARD:
					return COLOR_DIFFICULTY_2;
				case FractionDifficulty.KI_IMPOSSIBLE:
					return COLOR_DIFFICULTY_3;
				default:
					throw new ArgumentOutOfRangeException(nameof(d), d, null);
			}
		}
	}
}
