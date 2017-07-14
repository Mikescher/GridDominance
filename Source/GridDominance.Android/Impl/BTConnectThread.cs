using Android.Bluetooth;
using Java.Lang;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android.Impl
{
	class BTConnectThread : Thread
	{
		private readonly BluetoothSocket mmSocket;
		private readonly BluetoothDevice mmDevice;
		private readonly AndroidBluetoothAdapter _adapter;

		public BTConnectThread(BluetoothDevice device, AndroidBluetoothAdapter a)
		{
			mmDevice = device;
			_adapter = a;
			BluetoothSocket tmp = null;

			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			tmp = device.CreateRfcommSocketToServiceRecord(AndroidBluetoothAdapter.UUID);

			mmSocket = tmp;
		}

		public override void Run()
		{
			Name = "ConnectThread";

			// Always cancel discovery because it will slow down a connection
			_adapter.Adapter.CancelDiscovery();

			// Make a connection to the BluetoothSocket
			try
			{
				// This is a blocking call and will only return on a
				// successful connection or an exception
				mmSocket.Connect();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Warning("ABTA::CONNFAILED", e);
				_adapter.ThreadMessage_ConnectionFailed();
				// Close the socket
				try
				{
					mmSocket.Close();
				}
				catch (Java.IO.IOException e2)
				{
					SAMLog.Error("ABTA::NOCLOSE2", e2.Message);
				}
				return;
			}

			// Reset the ConnectThread because we're done
			lock (this)
			{
				_adapter.SetConnectThreadCancelled();
			}

			// Start the connected thread
			_adapter.ThreadMessage_Connected(mmSocket, mmDevice);
		}

		public void Cancel()
		{
			try
			{
				mmSocket.Close();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Error("ABTA::Thread1_Cancel", e);
			}
		}
	}
}