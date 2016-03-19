using System;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;
using GridDominance.Shared.Screens.GameScreen.FractionController;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen
{
	class Fraction
	{
		public static readonly Color COLOR_NEUTRAL     = FlatColors.Silver;
		public static readonly Color COLOR_PLAYER      = FlatColors.Nephritis;     
		public static readonly Color COLOR_COMPUTER_01 = FlatColors.Pomegranate;    
		public static readonly Color COLOR_COMPUTER_02 = FlatColors.BelizeHole;  
		public static readonly Color COLOR_COMPUTER_03 = FlatColors.Wisteria;   
		public static readonly Color COLOR_COMPUTER_04 = FlatColors.Orange;
		public static readonly Color COLOR_COMPUTER_05 = FlatColors.GreenSea;

		public const float MULTIPLICATOR_PLAYER     = 1.000f;
		public const float MULTIPLICATOR_NEUTRAL    = 0.600f;
		public const float MULTIPLICATOR_COMPUTER_0 = 0.800f;  // Easy
		public const float MULTIPLICATOR_COMPUTER_1 = 0.875f;  // Normal
		public const float MULTIPLICATOR_COMPUTER_2 = 0.095f;  // Hard
		public const float MULTIPLICATOR_COMPUTER_3 = 1.000f;  // Impossible

		public enum FractionType
		{
			PlayerFraction,
			ComputerFraction,
			NeutralFraction,
		}

		private readonly Fraction neutralFraction;
		public readonly float Multiplicator;

		public readonly Color Color;
		public readonly FractionType Type;

		public bool IsNeutral => (Type == FractionType.NeutralFraction);

		private Fraction(Color c, Fraction nfrac, FractionType type, float mult)
		{
			this.Color = c;
			this.Type = type;
			this.neutralFraction = (Type == FractionType.NeutralFraction) ? this : nfrac;
			this.Multiplicator = mult;
		}
		
		public static Fraction CreatePlayerFraction(Fraction neutral)
		{
			return new Fraction(COLOR_PLAYER, neutral, FractionType.PlayerFraction, MULTIPLICATOR_PLAYER);
		}

		public static Fraction CreateComputerFraction(Color c, Fraction neutral, float difficultyMultiplier)
		{
			return new Fraction(c, neutral, FractionType.ComputerFraction, difficultyMultiplier);
		}

		public static Fraction CreateNeutralFraction()
		{
			return new Fraction(COLOR_NEUTRAL, null, FractionType.NeutralFraction, MULTIPLICATOR_NEUTRAL);
		}

		public Fraction GetNeutral()
		{
			return neutralFraction;
		}

		public AbstractFractionController CreateController(GameScreen owner, Cannon cannon)
		{
			switch (Type)
			{
				case FractionType.PlayerFraction:
					return new PlayerController(owner, cannon, this);
				case FractionType.ComputerFraction:
					return new StandardKIController(owner, cannon, this);
				case FractionType.NeutralFraction:
					return new NeutralKIController(owner, cannon, this);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
