namespace MonoSAMFramework.Portable.GameMath.Geometry.Alignment
{
	public enum FlatAlign8
	{
		TOP,
		TOPRIGHT,
		RIGHT,
		BOTTOMRIGHT,
		BOTTOM,
		BOTTOMLEFT,
		LEFT,
		TOPLEFT,


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
