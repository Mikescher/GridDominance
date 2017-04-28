using System;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.Fractions
{
	public class Fraction
	{
		public static readonly Color COLOR_NEUTRAL     = FlatColors.Silver;
		public static readonly Color COLOR_PLAYER      = FlatColors.Nephritis;     
		public static readonly Color COLOR_COMPUTER_01 = FlatColors.Pomegranate;    
		public static readonly Color COLOR_COMPUTER_02 = FlatColors.BelizeHole;  
		public static readonly Color COLOR_COMPUTER_03 = FlatColors.Wisteria;   
		public static readonly Color COLOR_COMPUTER_04 = FlatColors.Orange;
		public static readonly Color COLOR_COMPUTER_05 = FlatColors.GreenSea;


		private readonly Fraction neutralFraction;
		public readonly float Multiplicator;

		public readonly Color Color;
		public readonly Color BackgroundColor;
		public readonly FractionType Type;

		public readonly FractionDifficulty Difficulty;

		public bool IsNeutral  => (Type == FractionType.NeutralFraction);
		public bool IsPlayer   => (Type == FractionType.PlayerFraction);
		public bool IsComputer => (Type == FractionType.ComputerFraction);

		private Fraction(Color c, Fraction nfrac, FractionType type, float mult, FractionDifficulty diff)
		{
			Color = c;
			Type = type;
			Difficulty = diff;
			neutralFraction = (Type == FractionType.NeutralFraction) ? this : nfrac;
			Multiplicator = mult;
			BackgroundColor = IsNeutral ? Color.Magenta : ColorMath.Blend(FlatColors.Background, c, 0.25f);
		}
		
		public static Fraction CreatePlayerFraction(Fraction neutral)
		{
			return new Fraction(COLOR_PLAYER, neutral, FractionType.PlayerFraction, FractionDifficultyHelper.MULTIPLICATOR_PLAYER, FractionDifficulty.PLAYER);
		}

		public static Fraction CreateComputerFraction(Color c, Fraction neutral, FractionDifficulty diff)
		{
			return new Fraction(c, neutral, FractionType.ComputerFraction, FractionDifficultyHelper.GetMultiplicator(diff), diff);
		}

		public static Fraction CreateNeutralFraction()
		{
			return new Fraction(COLOR_NEUTRAL, null, FractionType.NeutralFraction, FractionDifficultyHelper.MULTIPLICATOR_NEUTRAL, FractionDifficulty.NEUTRAL);
		}

		public Fraction GetNeutral()
		{
			return neutralFraction;
		}

		public override string ToString()
		{
			return FractionDifficultyHelper.GetShortIdentifier(Difficulty);
		}

		public AbstractFractionController CreateController(GDGameScreen owner, Cannon cannon)
		{
			switch (Type)
			{
				case FractionType.PlayerFraction:
					if (owner.HasFinished)
						return new EndGameAutoPlayerController(owner, cannon, this);
					else
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
