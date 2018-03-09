using System;
using Android.Bluetooth;
using Android.Content;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android.Impl
{

	class BTScanReciever : BroadcastReceiver
	{
		private readonly XamarinBluetooth _adapter;

		public BTScanReciever(XamarinBluetooth a)
		{
			_adapter = a;
		}

		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				SAMLog.Debug("BTScanReciever::OnRecieve(" + intent.Action + ")");

				string action = intent.Action;

				if (action == BluetoothDevice.ActionFound)
				{
					BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
					if (device != null)
					{
						SAMLog.Debug($"BTScanReciver:ActionFound({device.Name}|{device.Type}|{device.BluetoothClass.DeviceClass}|{device.BondState})");

						_adapter.ThreadMessage_DeviceFound(device);
					}
					else
					{
						SAMLog.Warning("ABTA::NULLDEVICE", "BTScanReciver:ActionFound(device == null)");
					}
				}
				else if (action == BluetoothAdapter.ActionDiscoveryFinished)
				{
					_adapter.ThreadMessage_DiscoveryFinished();
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::ScanRecieve", e);
			}
		}
	}
}