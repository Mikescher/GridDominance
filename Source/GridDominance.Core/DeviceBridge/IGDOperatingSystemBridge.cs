using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.DeviceBridge
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
