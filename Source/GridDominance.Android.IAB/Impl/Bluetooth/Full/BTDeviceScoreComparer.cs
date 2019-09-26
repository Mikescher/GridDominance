using System.Collections.Generic;
using Android.Bluetooth;
using MonoSAMFramework.Portable.DeviceBridge;

namespace GridDominance.Android.Impl
{
	internal class BTDeviceScoreComparer : IComparer<IBluetoothDevice>
	{
		/// <summary>
		/// 
		/// A < B  -1
		/// A = B  00
		/// A > B  +1
		/// 
		/// </summary>
		public int Compare(IBluetoothDevice x, IBluetoothDevice y)
		{
			var xx = (x as BTDeviceWrapper)?.InternalDevice;
			var yy = (y as BTDeviceWrapper)?.InternalDevice;

			if (x == null || y == null || xx == null || yy == null) return 0;

			if (xx.BluetoothClass == null || yy.BluetoothClass == null) return 0;

			if (IsPhone(xx.BluetoothClass.DeviceClass) && !IsPhone(yy.BluetoothClass.DeviceClass)) return -1;
			if (IsPhone(yy.BluetoothClass.DeviceClass) && !IsPhone(xx.BluetoothClass.DeviceClass)) return +1;

			if (IsBT(xx.Type) && !IsBT(yy.Type)) return -1;
			if (IsBT(yy.Type) && !IsBT(xx.Type)) return +1;

			if (xx.BondState == Bond.Bonded && yy.BondState != Bond.Bonded) return -1;
			if (yy.BondState == Bond.Bonded && xx.BondState != Bond.Bonded) return +1;

			return 0;
		}

		private bool IsBT(BluetoothDeviceType t)
		{
			switch (t)
			{
				case BluetoothDeviceType.Classic:
				case BluetoothDeviceType.Dual:
					return true;
				case BluetoothDeviceType.Le:
				case BluetoothDeviceType.Unknown:
				default:
					return false;
			}
		}

		private bool IsPhone(DeviceClass d)
		{
			switch (d)
			{
				case DeviceClass.PhoneCellular:
				case DeviceClass.PhoneCordless:
				case DeviceClass.PhoneIsdn:
				case DeviceClass.PhoneModemOrGateway:
				case DeviceClass.PhoneSmart:
				case DeviceClass.PhoneUncategorized:
				case DeviceClass.ComputerHandheldPcPda:
				case DeviceClass.ComputerPalmSizePcPda:
				case DeviceClass.ComputerWearable:
					return true;

				case DeviceClass.ToyVehicle:
				case DeviceClass.AudioVideoCamcorder:
				case DeviceClass.AudioVideoCarAudio:
				case DeviceClass.AudioVideoHandsfree:
				case DeviceClass.AudioVideoHeadphones:
				case DeviceClass.AudioVideoHifiAudio:
				case DeviceClass.AudioVideoLoudspeaker:
				case DeviceClass.AudioVideoMicrophone:
				case DeviceClass.AudioVideoPortableAudio:
				case DeviceClass.AudioVideoSetTopBox:
				case DeviceClass.AudioVideoUncategorized:
				case DeviceClass.AudioVideoVcr:
				case DeviceClass.AudioVideoVideoCamera:
				case DeviceClass.AudioVideoVideoConferencing:
				case DeviceClass.AudioVideoVideoDisplayAndLoudspeaker:
				case DeviceClass.AudioVideoVideoGamingToy:
				case DeviceClass.AudioVideoVideoMonitor:
				case DeviceClass.AudioVideoWearableHeadset:
				case DeviceClass.ComputerDesktop:
				case DeviceClass.ComputerLaptop:
				case DeviceClass.ComputerServer:
				case DeviceClass.ComputerUncategorized:
				case DeviceClass.HealthBloodPressure:
				case DeviceClass.HealthDataDisplay:
				case DeviceClass.HealthGlucose:
				case DeviceClass.HealthPulseOximeter:
				case DeviceClass.HealthPulseRate:
				case DeviceClass.HealthThermometer:
				case DeviceClass.HealthUncategorized:
				case DeviceClass.HealthWeighing:
				case DeviceClass.ToyController:
				case DeviceClass.ToyDollActionFigure:
				case DeviceClass.ToyGame:
				case DeviceClass.ToyRobot:
				case DeviceClass.ToyUncategorized:
				case DeviceClass.WearableGlasses:
				case DeviceClass.WearableHelmet:
				case DeviceClass.WearableJacket:
				case DeviceClass.WearablePager:
				case DeviceClass.WearableUncategorized:
				case DeviceClass.WearableWristWatch:
				default:
					return false;
			}
		}
	}
}