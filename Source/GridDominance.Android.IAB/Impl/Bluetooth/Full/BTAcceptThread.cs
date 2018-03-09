using Android.Bluetooth;
using Java.Lang;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using Exception = Java.Lang.Exception;

namespace GridDominance.Android.Impl
{
	class BTAcceptThread : Thread
	{
		// The local server socket
		private readonly BluetoothServerSocket mmServerSocket;
		private readonly XamarinBluetooth _adapter;

		private bool _stopped = false;

		public BTAcceptThread(XamarinBluetooth a)
		{
			_adapter = a;
			BluetoothServerSocket tmp = null;

			// Create a new listening server socket

			tmp = _adapter.Adapter.ListenUsingRfcommWithServiceRecord(XamarinBluetooth.NAME, XamarinBluetooth.UUID);

			mmServerSocket = tmp;
		}

		public override void Run()
		{
			Name = "AcceptThread";
			try
			{
				ThreadRun();
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::AcceptThread_Run", e);
			}
		}

		private void ThreadRun()
		{
			// Listen to the server socket if we're not connected
			while (_adapter.State != BluetoothAdapterState.Connected)
			{
				if (_adapter.State == BluetoothAdapterState.AdapterNotFound) return;
				if (_adapter.State == BluetoothAdapterState.Error) return;
				if (_adapter.State == BluetoothAdapterState.PermissionNotGranted) return;
				if (_adapter.State == BluetoothAdapterState.ConnectionFailed) return;
				if (_adapter.State == BluetoothAdapterState.ConnectionLost) return;
				if (_adapter.State == BluetoothAdapterState.NotEnabledByUser) return;

				BluetoothSocket socket = null;
				try
				{
					// This is a blocking call and will only return on a
					// successful connection or an exception
					socket = mmServerSocket.Accept();
				}
				catch (Java.IO.IOException e)
				{
					SAMLog.Warning("ABTA::AcceptFailed", e);
					continue;
				}

				// If a connection was accepted
				if (socket != null)
				{
					lock (this)
					{
						SAMLog.Debug("ABTA::Accept()=>" + _adapter.State);

						switch (_adapter.State)
						{
							case BluetoothAdapterState.Listen:
							case BluetoothAdapterState.Connecting:
								// Situation normal. Start the connected thread.
								_stopped = true;
								_adapter.ThreadMessage_Connected(socket, socket.RemoteDevice);
								return;
							case BluetoothAdapterState.Active:
							case BluetoothAdapterState.Connected:
								// Either not ready or already connected. Terminate new socket.
								try
								{
									socket.Close();
								}
								catch (Java.IO.IOException e)
								{
									SAMLog.Warning("ABTA::CNC", "Could not close unwanted socket", e.Message);
								}
								break;
							case BluetoothAdapterState.AdapterNotFound:
							case BluetoothAdapterState.PermissionNotGranted:
							case BluetoothAdapterState.Created:
							case BluetoothAdapterState.RequestingEnable:
							case BluetoothAdapterState.NotEnabledByUser:
							case BluetoothAdapterState.Scanning:
							case BluetoothAdapterState.ConnectionLost:
							case BluetoothAdapterState.ConnectionFailed:
							case BluetoothAdapterState.Error:
							default:
								SAMLog.Error("ABTA::EnumSwitch_TR", "value: " + _adapter.State);
								try
								{
									socket.Close();
								}
								catch (Java.IO.IOException e)
								{
									SAMLog.Warning("ABTA::CNC", "Could not close unwanted socket", e.Message);
								}
								return;
						}
					}
				}
			}
		}

		public void Cancel()
		{
			if (_stopped) return;

			try
			{
				mmServerSocket.Close();
			}
			catch (Java.IO.IOException e)
			{
				SAMLog.Error("ABTA::Thread3_Cancel", e);
			}
		}
	}
}