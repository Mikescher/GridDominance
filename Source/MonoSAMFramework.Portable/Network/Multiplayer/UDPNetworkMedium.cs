using System;
using MonoSAMFramework.Portable.DeviceBridge;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	class UDPNetworkMedium : INetworkMedium, IDisposable
	{
		private readonly IUDPClient _client;
		
		public UDPNetworkMedium(string ip, int port)
		{
			_client = MonoSAMGame.CurrentInst.Bridge.CreateUPDClient();
			_client.Connect(ip, port);
		}

		public void Dispose()
		{
			_client.Disconnect();
		}
	}
}
