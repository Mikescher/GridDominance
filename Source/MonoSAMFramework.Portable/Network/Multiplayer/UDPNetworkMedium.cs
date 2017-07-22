using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.DeviceBridge;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class UDPNetworkMedium : INetworkMedium
	{
		private readonly IUDPClient _client;

		public BufferedLambdaString DebugDisplayString { get; }

		public bool IsP2PConnected => false;
		public bool IsP2PListening => false;

		private readonly string _ip;
		private readonly int _port;

		public UDPNetworkMedium(string ip, int port)
		{
			_client = MonoSAMGame.CurrentInst.Bridge.CreateUPDClient();

			_ip = ip;
			_port = port;

			DebugDisplayString = new BufferedLambdaString(() => _client.DebugDisplayString, null);
		}

		public void Init(out SAMNetworkConnection.ErrorType error)
		{
			_client.Connect(_ip, _port);
			error = SAMNetworkConnection.ErrorType.None;
		}

		public void Update(out SAMNetworkConnection.ErrorType err)
		{
			err = SAMNetworkConnection.ErrorType.None;
		}

		public void StartServer()
		{
			//
		}

		public void StartClient()
		{
			//
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
