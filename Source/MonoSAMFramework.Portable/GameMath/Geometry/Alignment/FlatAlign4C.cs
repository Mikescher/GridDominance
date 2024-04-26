namespace MonoSAMFramework.Portable.GameMath.Geometry.Alignment
{
	public enum FlatAlign4C
	{
		TOPRIGHT    = 0x020,
		BOTTOMRIGHT = 0x040,
		BOTTOMLEFT  = 0x080,
		TOPLEFT     = 0x100,


		NORTHEAST = TOPRIGHT,
		SOUTHEAST = BOTTOMRIGHT,
		SOUTHWEST = BOTTOMLEFT,
		NORTHWEST = TOPLEFT,


		NE = NORTHEAST,
		SE = SOUTHEAST,
		SW = SOUTHWEST,
		NW = NORTHWEST,
	}
}
