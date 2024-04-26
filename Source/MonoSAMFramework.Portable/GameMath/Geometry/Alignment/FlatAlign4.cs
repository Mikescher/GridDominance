using System;

namespace MonoSAMFramework.Portable.GameMath.Geometry.Alignment
{
	[Flags]
	public enum FlatAlign4
	{
		TOP    = 0x01,
		RIGHT  = 0x02,
		BOTTOM = 0x04,
		LEFT   = 0x08,

		
		NORTH = TOP,
		EAST  = RIGHT,
		SOUTH = BOTTOM,
		WEST  = LEFT,


		NN = NORTH,
		EE = EAST,
		SS = SOUTH,
		WW = WEST,
	}
}
