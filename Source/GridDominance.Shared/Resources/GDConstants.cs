using System;

namespace GridDominance.Shared.Resources
{
	public static class GDConstants
	{
		public static readonly Version Version = new Version(0,21,0,0);

		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640


		//TODO prod
		//public const string SERVER_URL = "http://localhost:666";
		public const string SERVER_URL = "http://192.168.0.200:666";
		public const string SERVER_SECRET = "OZothFoshCiHyPhebMyGheVushNopTyg";


		public const int ORDER_GAME_WALL           = 100;
		public const int ORDER_GAME_CANNON         = 110;
		public const int ORDER_GAME_BULLETPARTICLE = 200;
		public const int ORDER_GAME_BULLETS        = 210; //TODO Bullets behind cannons ?

		public const int ORDER_MAP_PIPE          = 100;
		public const int ORDER_MAP_ORB           = 110;
		public const int ORDER_MAP_NODE          = 200;
		public const int ORDER_MAP_NODEPARTICLES = 210;

		public const int ORDER_WORLD_NODE = 100;
		public const int ORDER_WORLD_LOGO = 200;
	}
}
