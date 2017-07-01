using System;
using System.Net;
using System.Net.Sockets;
using MonoSAMFramework.Portable.DeviceBridge;

// ReSharper disable once CheckNamespace
namespace GridDominance.Generic.Impl
{
	class XamarinUDPClient : IUDPClient
	{
		private readonly UdpClient _client;

		private IPAddress _addr;
		private int _port;

		public XamarinUDPClient()
		{
			_client = new UdpClient();
		}

		public void Connect(string ip, int port)
		{
			_addr = IPAddress.Parse(ip);
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
			var ep = new IPEndPoint(_addr, _port);
			_client.EndReceive(callback, ref ep);
		}

		public int Send(byte[] data, int length)
		{
			return _client.Send(data, length);
		}
	}
}