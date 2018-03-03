using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	public enum GameSpeedModes
	{
		SUPERSLOW = -2,
		SLOW      = -1,
		NORMAL    = 00,
		FAST      = +1,
		SUPERFAST = +2,
	}

	public static class GameSpeedModeHelper
	{
		public static byte ToByte(GameSpeedModes m)
		{
			switch (m)
			{
				case GameSpeedModes.SUPERSLOW:
					return 0x70;
				case GameSpeedModes.SLOW:
					return 0x78;
				case GameSpeedModes.NORMAL:
					return 0x80;
				case GameSpeedModes.FAST:
					return 0x88;
				case GameSpeedModes.SUPERFAST:
					return 0x90;
				default:
					SAMLog.Error("GSM::EnumSwitch_TB", "Unknown value " + m);
					return 0x00;
			}
		}

		public static GameSpeedModes FromByte(byte b)
		{
			if (b == 0x70) return GameSpeedModes.SUPERSLOW;
			if (b == 0x78) return GameSpeedModes.SLOW;
			if (b == 0x80) return GameSpeedModes.NORMAL;
			if (b == 0x88) return GameSpeedModes.FAST;
			if (b == 0x90) return GameSpeedModes.SUPERFAST;

			SAMLog.Error("GSM::EnumSwitch_FB", "Unknown byte " + b);

			return GameSpeedModes.NORMAL;
		}
	}
}
