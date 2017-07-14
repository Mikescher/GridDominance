using System.IO;
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
	class BTConnectedThread : Thread
	{
		private readonly BluetoothSocket mmSocket;
		private readonly Stream mmInStream;
		private readonly Stream mmOutStream;
		private readonly AndroidBluetoothAdapter _adapter;

		public BTConnectedThread(BluetoothSocket socket, AndroidBluetoothAdapter a)
		{
			mmSocket = socket;
			_adapter = a;
			Stream tmpIn = null;
			Stream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			tmpIn = socket.InputStream;
			tmpOut = socket.OutputStream;

			mmInStream = tmpIn;
			mmOutStream = tmpOut;
		}

		public override void Run()
		{
			byte[] buffer = new byte[SAMNetworkConnection.MAX_PACKAGE_SIZE_BYTES];

			// Keep listening to the InputStream while connected
			while (true)
			{
				try
				{
					// Read from the InputStream
					var bytes = mmInStream.Read(buffer, 0, buffer.Length);

					_adapter.ThreadMessage_DataRead(buffer, bytes);
				}
				catch (Java.IO.IOException e)
				{
					SAMLog.Warning("ABTA::BTCT_ConnLost", e.Message);
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
			mmOutStream.Write(buffer, 0, buffer.Length);
		}

		public void Cancel()
		{
			try
			{
				mmSocket.Close();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Error("ABTA::Thread2_Cancel", e.Message);
			}
		}
	}
}