using System;

namespace GridDominance.Shared.Resources
{
	public static class GDConstants
	{
		public static readonly Version Version = new Version(0,23,0,0);

		public const int TILE_WIDTH = 64;

		public const int DEFAULT_GRID_WIDTH = 16;
		public const int DEFAULT_GRID_HEIGHT = 10;

		public const int VIEW_WIDTH  = DEFAULT_GRID_WIDTH  * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = DEFAULT_GRID_HEIGHT * TILE_WIDTH; //  640

		public const bool USE_IAB = true;

		public const string LOGO_STRING = "CANNON\nCONQUEST";

		//public const string SERVER_URL = "http://localhost:666";
		public const string SERVER_URL = "http://gdapi.mikescher.com";
		public const string SERVER_SECRET = __Secrets.SERVER_SECRET;

		public const float PHYSICS_CONVERSION_FACTOR = 50;

		public const int ORDER_GAME_BLACKHOLE      = 100;
		public const int ORDER_GAME_LASER          = 150;
		public const int ORDER_GAME_WALL           = 200;
		public const int ORDER_GAME_BULLETS        = 300;
		public const int ORDER_GAME_PORTAL         = 400;
		public const int ORDER_GAME_PORTALPARTICLE = 410;
		public const int ORDER_GAME_CANNON         = 500;
		public const int ORDER_GAME_BULLETPARTICLE = 600;

		public const int ORDER_MAP_PIPE          = 100;
		public const int ORDER_MAP_ORB           = 110;
		public const int ORDER_MAP_NODE          = 200;
		public const int ORDER_MAP_NODEPARTICLES = 210;

		public const int ORDER_WORLD_NODE = 100;
		public const int ORDER_WORLD_LOGO = 200;
	}
}
