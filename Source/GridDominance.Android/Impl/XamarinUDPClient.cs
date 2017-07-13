using System;
using System.Net;
using System.Net.Sockets;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

// ReSharper disable once CheckNamespace
namespace GridDominance.Generic.Impl
{
	class XamarinUDPClient : IUDPClient
	{
		private readonly UdpClient _client;

		private string _addr;
		private int _port;

		private IPEndPoint tmp = new IPEndPoint(IPAddress.None, 0);
		
		public XamarinUDPClient()
		{
#if __DESKTOP__
			_client = new UdpClient(new Random().Next(5000, 27000));
#else
			try
			{
				_client = new UdpClient();
			}
			catch (SocketException e)
			{
				throw new Exception($"SocketException thrown ({e.Message}) with SocketErrorCode={e.SocketErrorCode}", e);
			}
#endif
		}

		public void Connect(string host, int port)
		{
			_addr = host;
			_port = port;
			_client.Connect(_addr, _port);
		}

		public void Disconnect()
		{
			_client.Close();
		}

		public void BeginRecieve(AsyncCallback callback, object data)
		{
			_client.BeginReceive(callback, data);
		}

		public void EndRecieve(IAsyncResult callback)
		{
			var ep = new IPEndPoint(IPAddress.None, 0);
			_client.EndReceive(callback, ref ep);
		}

		public byte[] RecieveOrNull()
		{
			if (_client.Available > 0)
			{
				try
				{
					return _client.Receive(ref tmp);
				}
				catch (Exception e)
				{
					SAMLog.Debug(e.Message);
					return null;
				}
			}
			return null;
		}

		public int Send(byte[] data, int length)
		{
			return _client.Send(data, length);
		}
	}
}