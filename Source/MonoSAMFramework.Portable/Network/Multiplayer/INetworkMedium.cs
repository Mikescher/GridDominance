using System;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public interface INetworkMedium : IDisposable
	{
		void Init(out SAMNetworkConnection.ErrorType error);

		byte[] RecieveOrNull();
		void Send(byte[] data);
		void Send(byte[] data, int len);
	}
}
