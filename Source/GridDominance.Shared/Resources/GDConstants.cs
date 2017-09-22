using System;

namespace GridDominance.Shared.Resources
{
	public enum GDFlavor { FREE, IAB, FULL, FULL_NOMP }

	public static class GDConstants
	{
		public static readonly Version Version = new Version(1,1,2,0);
		public static ulong IntVersion { get; } = (ulong)((((((Version.Major << 12) | Version.Minor) << 12) | Version.Build) << 12) | Version.Revision);

		public const int TILE_WIDTH = 64;

		public const int DEFAULT_GRID_WIDTH = 16;
		public const int DEFAULT_GRID_HEIGHT = 10;

		public const int VIEW_WIDTH  = DEFAULT_GRID_WIDTH  * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = DEFAULT_GRID_HEIGHT * TILE_WIDTH; //  640

#if GD_FULL_NOMP
		public const GDFlavor FLAVOR = GDFlavor.FULL_NOMP;
#endif
#if GD_FULL
		public const GDFlavor FLAVOR = GDFlavor.FULL;
#endif
#if GD_IAB
        public const GDFlavor FLAVOR = GDFlavor.IAB;
#endif
#if GD_FREE
		public const GDFlavor FLAVOR = GDFlavor.FREE;
#endif

        public const string IAB_WORLD2      = @"gd_world_002";
		public const string IAB_WORLD3      = @"gd_world_003";
		public const string IAB_WORLD4      = @"gd_world_004";
		public const string IAB_MULTIPLAYER = @"gd_multiplayer";

		public static readonly string[] IABList =
		{
#if DEBUG
			MonoSAMFramework.Portable.DeviceBridge.AndroidBillingHelper.PID_CANCELED,
			MonoSAMFramework.Portable.DeviceBridge.AndroidBillingHelper.PID_PURCHASED,
			MonoSAMFramework.Portable.DeviceBridge.AndroidBillingHelper.PID_REFUNDED,
			MonoSAMFramework.Portable.DeviceBridge.AndroidBillingHelper.PID_UNAVAILABLE,
#endif

			GDConstants.IAB_WORLD2,
			GDConstants.IAB_WORLD3,
			GDConstants.IAB_WORLD4,
			GDConstants.IAB_MULTIPLAYER,
		};

		public const string LOGO_STRING = "CANNON\nCONQUEST";
		public const string BFB_URL     = @"http://blackforestbytes.de/";
		public const string PROFILE_FILENAME = "USERPROFILE";

		public const string URL_REDDIT = @"https://www.reddit.com/r/CannonConquest/";
		public const string URL_BLACKFORESTBYTES = @"http://blackforestbytes.de/";

#if __ANDROID__ && !DEBUG
		public const string MULTIPLAYER_SERVER_HOST = "mikescher.com";
		public const int    MULTIPLAYER_SERVER_PORT = 28023;
#else
		public const string MULTIPLAYER_SERVER_HOST = "localhost";
		public const int    MULTIPLAYER_SERVER_PORT = 28023;
#endif

		public const string BLUETOOTH_NAME = "GridDominance.BluetoothSocket";
		public const string BLUETOOTH_UUID = "4748A5F0-0AC7-44F7-BE49-30F5FD2A08ED";
		public const string BLUETOOTH_LE_SERVICE_UUID = "64BBAB15-2F82-4789-996F-0691B65E5E0E";
		public const string BLUETOOTH_LE_CHRCTR_UUID = "33AB15F8-45CE-49C3-ACFC-96F6B510C989";

#if __ANDROID__ && !DEBUG
		public const string SERVER_URL = "http://gdapi.mikescher.com";
#else
		public const string SERVER_URL = "http://localhost:666";
#endif
		public const string SERVER_SECRET = __Secrets.SERVER_SECRET;

		public const float PHYSICS_CONVERSION_FACTOR = 50;

		public const int ORDER_GAME_BACKGROUNDTEXT =  50;
		public const int ORDER_GAME_BLACKHOLE      = 100;
		public const int ORDER_GAME_LASER          = 150;
		public const int ORDER_GAME_WALL           = 200;
		public const int ORDER_GAME_BULLETS        = 300;
		public const int ORDER_GAME_PORTAL         = 400;
		public const int ORDER_GAME_PORTALPARTICLE = 410;
		public const int ORDER_GAME_CANNON         = 500;
		public const int ORDER_GAME_BULLETPARTICLE = 600;

		public const int ORDER_MAP_PIPE_OFF      = 101;
		public const int ORDER_MAP_PIPE_ON       = 102;
		public const int ORDER_MAP_ORB           = 110;
		public const int ORDER_MAP_NODE          = 200;
		public const int ORDER_MAP_NODEPARTICLES = 210;

		public const int ORDER_WORLD_NODE = 100;
		public const int ORDER_WORLD_LOGO = 200;

		
		public const float MULTIPLICATOR_BULLET_PLAYER     = 1.000f;
		public const float MULTIPLICATOR_BULLET_NEUTRAL    = 0.600f;
		public const float MULTIPLICATOR_BULLET_SUPERSLOW  = 0.500f;  // Easy on 1-1
		public const float MULTIPLICATOR_BULLET_COMPUTER_0 = 0.800f;  // Easy
		public const float MULTIPLICATOR_BULLET_COMPUTER_1 = 0.875f;  // Normal
		public const float MULTIPLICATOR_BULLET_COMPUTER_2 = 0.950f;  // Hard
		public const float MULTIPLICATOR_BULLET_COMPUTER_3 = 1.000f;  // Impossible

		public const float MULTIPLICATOR_LASER_PLAYER      = 1.000f;
		public const float MULTIPLICATOR_LASER_NEUTRAL     = 0.300f;
		public const float MULTIPLICATOR_LASER_COMPUTER_0  = 0.500f;  // Easy
		public const float MULTIPLICATOR_LASER_COMPUTER_1  = 0.650f;  // Normal
		public const float MULTIPLICATOR_LASER_COMPUTER_2  = 0.800f;  // Hard
		public const float MULTIPLICATOR_LASER_COMPUTER_3  = 1.000f;  // Impossible

		public const int SCORE_DIFF_0 = 11; // also specified in SQL (server)
		public const int SCORE_DIFF_1 = 13;
		public const int SCORE_DIFF_2 = 17;
		public const int SCORE_DIFF_3 = 23;

		public const int FREE_SCORE_PER_WORLD = 100;
	}
}
