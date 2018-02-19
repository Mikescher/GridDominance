﻿using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IGDOperatingSystemBridge : ISAMOperatingSystemBridge
	{
		string AppType { get; }
		GDFlavor Flavor { get; }

		IBillingAdapter IAB { get; }

		IBluetoothAdapter BluetoothFull { get; }
		IUDPClient CreateUPDClient();

		void OpenURL(string url);
		void ShareAppLink();
	}
}