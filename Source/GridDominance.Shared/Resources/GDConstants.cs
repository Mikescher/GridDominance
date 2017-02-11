using System;

namespace GridDominance.Shared.Resources
{
	public static class GDConstants
	{
		public static readonly Version Version = new Version(0,19,0,0);

		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640

		//TODO prod
		public const string SERVER_URL = "http://localhost:666";
		public const string SERVER_SECRET = "OZothFoshCiHyPhebMyGheVushNopTyg";
	}
}
