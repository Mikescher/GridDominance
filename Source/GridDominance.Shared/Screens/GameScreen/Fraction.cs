using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen
{
	public class Fraction
	{
		public static readonly Color COLOR_NEUTRAL     = Color.LightGray;
		public static readonly Color COLOR_PLAYER      = Color.Green;
		public static readonly Color COLOR_COMPUTER_01 = Color.Red;
		public static readonly Color COLOR_COMPUTER_02 = Color.Blue;
		public static readonly Color COLOR_COMPUTER_03 = Color.Yellow;
		public static readonly Color COLOR_COMPUTER_04 = Color.Cyan;
		public static readonly Color COLOR_COMPUTER_05 = Color.Orange;
		public static readonly Color COLOR_COMPUTER_06 = Color.Pink;

		public const float MULTIPLICATOR_PLAYER     = 1.000f;
		public const float MULTIPLICATOR_NEUTRAL    = 0.500f;
		public const float MULTIPLICATOR_COMPUTER_0 = 0.800f;  // Easy
		public const float MULTIPLICATOR_COMPUTER_1 = 0.875f;  // Normal
		public const float MULTIPLICATOR_COMPUTER_2 = 0.095f;  // Hard
		public const float MULTIPLICATOR_COMPUTER_3 = 1.000f;  // Impossible

		private readonly Fraction neutralFraction;
		public readonly float Multiplicator;

		public readonly Color Color;
		public readonly bool IsNeutral;

		private Fraction(Color c, Fraction nfrac, bool neutral, float mult)
		{
			this.Color = c;
			this.IsNeutral = neutral;
			this.neutralFraction = neutral ? this : nfrac;
			this.Multiplicator = mult;
		}


		public static Fraction CreatePlayerFraction(Fraction neutral)
		{
			return new Fraction(COLOR_PLAYER, neutral, false, MULTIPLICATOR_PLAYER);
		}

		public static Fraction CreateComputerFraction(Color c, Fraction neutral, float difficulty_multiplier)
		{
			return new Fraction(c, neutral, false, difficulty_multiplier);
		}

		public static Fraction CreateNeutralFraction()
		{
			return new Fraction(COLOR_NEUTRAL, null, true, MULTIPLICATOR_NEUTRAL);
		}

		public Fraction GetNeutral()
		{
			return neutralFraction;
		}
	}
}
