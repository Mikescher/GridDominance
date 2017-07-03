using MonoSAMFramework.Portable.DeviceBridge;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class UDPNetworkMedium : INetworkMedium
	{
		private readonly IUDPClient _client;
		
		public UDPNetworkMedium(string ip, int port)
		{
			_client = MonoSAMGame.CurrentInst.Bridge.CreateUPDClient();
			_client.Connect(ip, port);
		}

		public byte[] RecieveOrNull()
		{
			return _client.RecieveOrNull();
		}

		public void Send(byte[] data)
		{
			_client.Send(data, data.Length);
		}

		public void Send(byte[] data, int len)
		{
			_client.Send(data, len);
		}

		public void Dispose()
		{
			_client.Disconnect();
		}
	}
}
