using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.Fractions
{
	public enum FractionDifficulty
	{
		KI_EASY       = 0x00, // Can be used as Array indizies
		KI_NORMAL     = 0x01,
		KI_HARD       = 0x02,
		KI_IMPOSSIBLE = 0x03,

		NEUTRAL       = 0x10,
		PLAYER        = 0x11,


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

		private const int SCORE_DIFF_0 = 11;
		private const int SCORE_DIFF_1 = 13;
		private const int SCORE_DIFF_2 = 17;
		private const int SCORE_DIFF_3 = 23;

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
					return Textures.TexDifficultyLine0;
				case FractionDifficulty.KI_NORMAL:
					return Textures.TexDifficultyLine1;
				case FractionDifficulty.KI_HARD:
					return Textures.TexDifficultyLine2;
				case FractionDifficulty.KI_IMPOSSIBLE:
					return Textures.TexDifficultyLine3;
			}

			return null;
		}

		public static Color GetColor(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.KI_EASY:
					return GDColors.COLOR_DIFFICULTY_0;
				case FractionDifficulty.KI_NORMAL:
					return GDColors.COLOR_DIFFICULTY_1;
				case FractionDifficulty.KI_HARD:
					return GDColors.COLOR_DIFFICULTY_2;
				case FractionDifficulty.KI_IMPOSSIBLE:
					return GDColors.COLOR_DIFFICULTY_3;
			}

			return Color.Magenta;
		}

		public static string GetDescription(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.KI_EASY:
					return "Easy";
				case FractionDifficulty.KI_NORMAL:
					return "Normal";
				case FractionDifficulty.KI_HARD:
					return "Hard";
				case FractionDifficulty.KI_IMPOSSIBLE:
					return "Realistic";
			}

			return null;
		}

		public static int GetScore(FractionDifficulty d)
		{
			switch (d)
			{
				case FractionDifficulty.KI_EASY:
					return SCORE_DIFF_0;
				case FractionDifficulty.KI_NORMAL:
					return SCORE_DIFF_1;
				case FractionDifficulty.KI_HARD:
					return SCORE_DIFF_2;
				case FractionDifficulty.KI_IMPOSSIBLE:
					return SCORE_DIFF_3;
			}

			SAMLog.Error("EnumSwitch", "GetScore()", "FractionDifficultyHelper.GetScore -> " + d);
			return 0;
		}
	}
}
