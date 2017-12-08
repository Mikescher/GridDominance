using System;
using MonoSAMFramework.Portable.DebugTools;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public interface INetworkMedium : IDisposable
	{
		void Init(out SAMNetworkConnection.ErrorType error);
		void Update(out SAMNetworkConnection.ErrorType updateError);

		bool IsP2PConnected { get; }
		bool IsP2PListening { get; }

		void StartServer();
		void StartClient();

		byte[] RecieveOrNull();
		void Send(byte[] data);
		void Send(byte[] data, int len);

		BufferedLambdaString DebugDisplayString { get; }
	}
}
