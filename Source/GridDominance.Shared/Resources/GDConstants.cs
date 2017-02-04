namespace GridDominance.Shared.Resources
{
	public static class GDConstants
	{
		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640

		//TODO prod
		public const string SERVER_URL = "localhost:666";
		public const string SERVER_SECRET = "OZothFoshCiHyPhebMyGheVushNopTyg";
		public const string SERVER_PUBKEY = "<RSAKeyValue><Modulus>z9t6M0Bco5wRtqlSXxahayA1TWPt50Hh7yInq72RXZ/a+wpkbZMgqzEg8DYXNcX2jcIwUGztZWQXHjaLUlLmbQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
		public const int SERVER_PUBKEYSIZE = 512;
	}
}
