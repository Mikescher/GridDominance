using System;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;

namespace GridDominance.Shared.Screens.ScreenGame.Fractions
{
	public enum FractionDifficulty
	{
		NEUTRAL       = 0x00,
		PLAYER        = 0x01,

		KI_EASY       = 0x10,
		KI_NORMAL     = 0x11,
		KI_HARD       = 0x12,
		KI_IMPOSSIBLE = 0x13,

		DIFF_0        = KI_EASY,
		DIFF_1        = KI_NORMAL,
		DIFF_2        = KI_HARD,
		DIFF_3        = KI_IMPOSSIBLE,
	}

	public static class FractionDifficultyHelper
	{
		public const float MULTIPLICATOR_PLAYER     = 1.000f;

		public const float MULTIPLICATOR_NEUTRAL    = 0.600f;

		public const float MULTIPLICATOR_COMPUTER_0 = 0.800f;  // Easy
		public const float MULTIPLICATOR_COMPUTER_1 = 0.875f;  // Normal
		public const float MULTIPLICATOR_COMPUTER_2 = 0.950f;  // Hard
		public const float MULTIPLICATOR_COMPUTER_3 = 1.000f;  // Impossible

		public static float GetMultiplicator(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.PLAYER:
					return MULTIPLICATOR_PLAYER;

				case FractionDifficulty.NEUTRAL:
					return MULTIPLICATOR_NEUTRAL;

				case FractionDifficulty.KI_EASY:
					return MULTIPLICATOR_COMPUTER_0;

				case FractionDifficulty.KI_NORMAL:
					return MULTIPLICATOR_COMPUTER_1;

				case FractionDifficulty.KI_HARD:
					return MULTIPLICATOR_COMPUTER_2;

				case FractionDifficulty.KI_IMPOSSIBLE:
					return MULTIPLICATOR_COMPUTER_3;

				default:
					throw new ArgumentOutOfRangeException(nameof(d), d, null);
			}
		}

		public static string GetShortIdentifier(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.PLAYER:
					return "P";
				case FractionDifficulty.NEUTRAL:
					return "N";
				case FractionDifficulty.KI_EASY:
					return "C0";
				case FractionDifficulty.KI_NORMAL:
					return "C1";
				case FractionDifficulty.KI_HARD:
					return "C2";
				case FractionDifficulty.KI_IMPOSSIBLE:
					return "C3";
				default:
					throw new ArgumentOutOfRangeException(nameof(d), d, null);
			}
		}

		public static TextureRegion2D GetIcon(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.KI_EASY:
					return Textures.TexDifficulty0;
				case FractionDifficulty.KI_NORMAL:
					return Textures.TexDifficulty1;
				case FractionDifficulty.KI_HARD:
					return Textures.TexDifficulty2;
				case FractionDifficulty.KI_IMPOSSIBLE:
					return Textures.TexDifficulty3;
			}

			return null;
		}
	}
}
