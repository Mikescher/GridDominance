using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Android.Bluetooth;
using Java.Lang;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Android.Impl
{
	/// <summary>
	/// This thread runs during a connection with a remote device.
	/// It handles all incoming and outgoing transmissions.
	/// </summary>
	class BTTransferSendThread : Thread
	{
		private readonly BluetoothSocket mmSocket;
		private readonly Stream mmOutStream;
		private readonly XamarinBluetooth _adapter;

		private readonly ConcurrentQueue<byte[]> _sendData = new ConcurrentQueue<byte[]>();

		public BTTransferSendThread(BluetoothSocket socket, XamarinBluetooth a)
		{
			mmSocket = socket;
			_adapter = a;

			// Get the BluetoothSocket input and output streams
			mmOutStream = socket.OutputStream;
		}

		public override void Run()
		{
			Name = "ConnectedThread";
			try
			{
				SAMLog.Debug("ABTA::StartSendThread()");
				ThreadRun();
			}
			catch (Exception e)
			{
				_adapter.ThreadMessage_ConnectionError();
				SAMLog.Error("ABTA::ConnectedThread_Run", e);
			}
		}

		private void ThreadRun()
		{
			byte[] lenbuffer = new byte[2];
			
			while (true)
			{
				try
				{
					byte[] d;
					if (_sendData.TryDequeue(out d))
					{
						NetworkDataTools.SetUInt16(out lenbuffer[0], out lenbuffer[1], (ushort)d.Length);
						mmOutStream.Write(lenbuffer, 0, 2);
						mmOutStream.Write(d, 0, d.Length);
					}
					else
					{
						Thread.Sleep(0);
					}
				}
				catch (Java.IO.IOException e)
				{
					SAMLog.Warning("ABTA::ThreadSend_ConnLost", e.Message);
					_adapter.ThreadMessage_ConnectionLost();
					break;
				}
			}
		}

		/// <summary>
		/// Write to the connected OutStream.
		/// </summary>
		/// <param name='buffer'>
		/// The bytes to write
		/// </param>
		public void Write(byte[] buffer)
		{
			_sendData.Enqueue(buffer.ToArray());
		}

		public void Cancel()
		{
			try
			{
				mmSocket.Close();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Error("ABTA::ThreadSend_Cancel", e.Message);
			}
		}
	}
}