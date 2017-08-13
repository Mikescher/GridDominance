using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using GridDominance.Shared.Resources;

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

		public static readonly int NAME_NEUTRAL     = L10NImpl.STR_FRAC_N0;
		public static readonly int NAME_PLAYER      = L10NImpl.STR_FRAC_P1;
		public static readonly int NAME_COMPUTER_01 = L10NImpl.STR_FRAC_A2;
		public static readonly int NAME_COMPUTER_02 = L10NImpl.STR_FRAC_A3;
		public static readonly int NAME_COMPUTER_03 = L10NImpl.STR_FRAC_A4;
		public static readonly int NAME_COMPUTER_04 = L10NImpl.STR_FRAC_A5;
		public static readonly int NAME_COMPUTER_05 = L10NImpl.STR_FRAC_A6;

		private readonly Fraction neutralFraction;
		public readonly float BulletMultiplicator;
		public readonly float LaserMultiplicator;

		public readonly Color Color;
		public readonly Color BackgroundColor;
		public readonly FractionType Type;

		public readonly FractionDifficulty Difficulty;

		public ulong LastKiSingleCycle = 0;
		public readonly Queue<byte> KICycleWaitQueue = new Queue<byte>();
		
		public bool IsNeutral  => (Type == FractionType.NeutralFraction);
		public bool IsPlayer   => (Type == FractionType.PlayerFraction);
		public bool IsComputer => (Type == FractionType.ComputerFraction);

		private Fraction(Color c, Fraction nfrac, FractionType type, float multBullet, float multLaser, FractionDifficulty diff)
		{
			Color = c;
			Type = type;
			Difficulty = diff;
			neutralFraction = (Type == FractionType.NeutralFraction) ? this : nfrac;
			BulletMultiplicator = multBullet;
			LaserMultiplicator = multLaser;
			BackgroundColor = IsNeutral ? Color.Magenta : ColorMath.Blend(FlatColors.Background, c, 0.25f);
		}
		
		public static Fraction CreatePlayerFraction(Fraction neutral)
		{
			return new Fraction(
				COLOR_PLAYER, 
				neutral, 
				FractionType.PlayerFraction, 
				FractionDifficultyHelper.MULTIPLICATOR_B_PLAYER, 
				FractionDifficultyHelper.MULTIPLICATOR_L_PLAYER, 
				FractionDifficulty.PLAYER);
		}

		public static Fraction CreateComputerFraction(Color c, Fraction neutral, FractionDifficulty diff, bool slow)
		{
			return new Fraction(
				c, 
				neutral, 
				FractionType.ComputerFraction, 
				FractionDifficultyHelper.GetBulletMultiplicator(diff, slow),
				FractionDifficultyHelper.GetLaserMultiplicator(diff),
				diff);
		}

		public static Fraction CreateNeutralFraction()
		{
			return new Fraction(
				COLOR_NEUTRAL,
				null, 
				FractionType.NeutralFraction, 
				FractionDifficultyHelper.MULTIPLICATOR_B_NEUTRAL, 
				FractionDifficultyHelper.MULTIPLICATOR_L_NEUTRAL,
				FractionDifficulty.NEUTRAL);
		}

		public Fraction GetNeutral()
		{
			return neutralFraction;
		}

		public override string ToString()
		{
			return FractionDifficultyHelper.GetShortIdentifier(Difficulty);
		}
	}
}
