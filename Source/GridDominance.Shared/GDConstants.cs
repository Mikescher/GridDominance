using System;
using System.Collections.Generic;
using System.Text;

namespace GridDominance.Shared
{
	public static class GDConstants
	{
		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640
	}
}
