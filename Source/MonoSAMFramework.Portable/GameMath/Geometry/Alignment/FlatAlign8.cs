namespace MonoSAMFramework.Portable.GameMath.Geometry.Alignment
{
	public enum FlatAlign8
	{
		TOP         = 0x001,
		TOPRIGHT    = 0x020,
		RIGHT       = 0x002,
		BOTTOMRIGHT = 0x040,
		BOTTOM      = 0x004,
		BOTTOMLEFT  = 0x080,
		LEFT        = 0x008,
		TOPLEFT     = 0x100,


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
	}
}
