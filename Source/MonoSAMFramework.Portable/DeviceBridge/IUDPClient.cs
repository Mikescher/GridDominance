using System;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IUDPClient
	{
		void Connect(string host, int port);
		void Disconnect();

		void BeginRecieve(AsyncCallback callback, object data);
		void EndRecieve(IAsyncResult callback);
		byte[] RecieveOrNull();
		int Send(byte[] data, int length);
	}
}
