using System;
using System.Collections.Generic;
using System.Linq;
using Android.Bluetooth;
using Android.Content;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android.Impl
{

	class BTScanReciever : BroadcastReceiver
	{
		private readonly AndroidBluetoothAdapter _adapter;

		public BTScanReciever(AndroidBluetoothAdapter a)
		{
			_adapter = a;
		}

		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				string action = intent.Action;

				if (action == BluetoothDevice.ActionFound)
				{
					BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
					SAMLog.Debug($"BTScanReciver:OnRecieve({device.Name})");

					if (device.BondState != Bond.Bonded) _adapter.ThreadMessage_DeviceFound(device);
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