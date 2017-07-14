namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public interface IBluetoothDevice
	{
		string Name { get; }
		string Address { get; }

		string DeviceClass { get; }
		string Type { get; }

		bool IsBonded { get; }
		bool IsBonding { get; }
	}
}
