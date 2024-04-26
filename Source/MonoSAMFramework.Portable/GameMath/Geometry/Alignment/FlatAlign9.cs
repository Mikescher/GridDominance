using System.Collections.Generic;

namespace MonoSAMFramework.Portable.GameMath.Geometry.Alignment
{
	public static class FlatAlign9Helper
	{
		public static Dictionary<FlatAlign9, string> LETTERS = new Dictionary<FlatAlign9, string>
		{
			{FlatAlign9.NN, "N"},
			{FlatAlign9.NE, "NE"},
			{FlatAlign9.EE, "E"},
			{FlatAlign9.SE, "SE"},
			{FlatAlign9.SS, "S"},
			{FlatAlign9.SW, "SW"},
			{FlatAlign9.WW, "W"},
			{FlatAlign9.NW, "NW"},
			{FlatAlign9.CC, "C"},
		};
	}

	public enum FlatAlign9
	{

		TOP         = 0x001,
		TOPRIGHT    = 0x020,
		RIGHT       = 0x002,
		BOTTOMRIGHT = 0x040,
		BOTTOM      = 0x004,
		BOTTOMLEFT  = 0x080,
		LEFT        = 0x008,
		TOPLEFT     = 0x100,
		CENTER      = 0x010,


		TL = TOPLEFT,
		TR = TOPRIGHT,
		BL = BOTTOMLEFT,
		BR = BOTTOMRIGHT,


		NORTH     = TOP,
		NORTHEAST = TOPRIGHT,
		EAST      = RIGHT,
		SOUTHEAST = BOTTOMRIGHT,
		SOUTH     = BOTTOM,
		SOUTHWEST = BOTTOMLEFT,
		WEST      = LEFT,
		NORTHWEST = TOPLEFT,


		NN = NORTH,
		NE = NORTHEAST,
		EE = EAST,
		SE = SOUTHEAST,
		SS = SOUTH,
		SW = SOUTHWEST,
		WW = WEST,
		NW = NORTHWEST,
		CC = CENTER,
	}
}
