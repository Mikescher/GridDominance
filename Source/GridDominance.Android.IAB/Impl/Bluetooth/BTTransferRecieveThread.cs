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
	class BTTransferRecieveThread : Thread
	{
		private readonly BluetoothSocket mmSocket;
		private readonly Stream mmInStream;
		private readonly XamarinBluetooth _adapter;

		public BTTransferRecieveThread(BluetoothSocket socket, XamarinBluetooth a)
		{
			mmSocket = socket;
			_adapter = a;
			Stream tmpIn = null;

			// Get the BluetoothSocket input and output streams
			mmInStream = socket.InputStream;
		}

		public override void Run()
		{
			Name = "ConnectedThread";
			try
			{
				SAMLog.Debug("ABTA::StartRecieveThread()");
				ThreadRun();
			}
			catch (Exception e)
			{
				_adapter.ThreadMessage_ConnectionError();
				SAMLog.Error("ABTA::RecieveThread_Run", e);
			}
		}

		private void ThreadRun()
		{
			byte[] buffer = new byte[SAMNetworkConnection.MAX_PACKAGE_SIZE_BYTES * 3];

			ushort currentLen = 0;
			int currentOffset = 0;

			// Keep listening to the InputStream while connected
			while (true)
			{
				try
				{
					// Read from the InputStream
					var bytes = mmInStream.Read(buffer, currentOffset, SAMNetworkConnection.MAX_PACKAGE_SIZE_BYTES);

					if (currentLen == 0)
					{
						if (bytes >= 2)
						{
							currentLen = NetworkDataTools.GetUInt16(buffer[0], buffer[1]);
							if (currentLen == 0)
							{
								SAMLog.Warning("ABTA::NullLen", "BT recieved 0 len package");
								currentOffset = 0;
							}
						}
						currentOffset += bytes;
					}
					else
					{
						currentOffset += bytes;
					}


					while (currentLen > 0 && currentLen + 2 <= currentOffset)
					{
						_adapter.ThreadMessage_DataRead(buffer, 2, currentLen);
						for (int i = currentLen + 2; i < currentOffset; i++) buffer[i - (currentLen + 2)] = buffer[i];
						currentOffset -= (currentLen + 2);

						if (currentOffset > 2)
						{
							currentLen = NetworkDataTools.GetUInt16(buffer[0], buffer[1]);
						}
						else
						{
							currentLen = 0;
						}
					}
				}
				catch (Java.IO.IOException e)
				{
					SAMLog.Warning("ABTA::ThreadRecieve_ConnLost", e.Message);
					_adapter.ThreadMessage_ConnectionLost();
					break;
				}
			}
		}

		public void Cancel()
		{
			try
			{
				mmSocket.Close();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Error("ABTA::ThreadRecieve_Cancel", e.Message);
			}
		}
	}
}