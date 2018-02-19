using FarseerPhysics;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.Physics
{
	public static class ConvertUnits2
	{
		public static FPoint ToDisplayUnitsPoint(Vector2 simUnits)
		{
			return new FPoint(simUnits.X * ConvertUnits._displayUnitsToSimUnitsRatio, simUnits.Y * ConvertUnits._displayUnitsToSimUnitsRatio);
		}

		public static Vector2 ToSimUnits(FPoint displayUnits)
		{
			return new Vector2(displayUnits.X * ConvertUnits._simUnitsToDisplayUnitsRatio, displayUnits.Y * ConvertUnits._simUnitsToDisplayUnitsRatio); ;
		}
	}
}
