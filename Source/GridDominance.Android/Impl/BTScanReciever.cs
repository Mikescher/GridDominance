using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
			string action = intent.Action;

			// When discovery finds a device
			if (action == BluetoothDevice.ActionFound)
			{
				// Get the BluetoothDevice object from the Intent
				BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
				// If it's already paired, skip it, because it's been listed already
				if (device.BondState != Bond.Bonded)
				{
					_adapter.ThreadMessage_DeviceFound(device);
				}
				// When discovery is finished, change the Activity title
			}
			else if (action == BluetoothAdapter.ActionDiscoveryFinished)
			{
				_adapter.ThreadMessage_DiscoveryFinished();
			}
		}
	}
}