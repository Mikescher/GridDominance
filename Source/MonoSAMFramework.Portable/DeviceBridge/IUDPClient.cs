using System;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IUDPClient
	{
		void Connect(string ip, int port);
		void Disconnect();

		void BeginRecieve(AsyncCallback callback, object data);
		void EndRecieve(IAsyncResult callback);
		int Send(byte[] data, int length);
	}
}
