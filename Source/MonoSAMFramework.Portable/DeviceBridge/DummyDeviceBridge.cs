using System;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public class DummyDeviceBridge : IOperatingSystemBridge
	{
		public string FullDeviceInfoString { get; } = "";
		public string DeviceName { get; } = "";
		public string DeviceVersion { get; } = "";
		public FSize DeviceResolution { get; } = FSize.Empty;

		public FileHelper FileHelper { get; } = new DummyFileHelper();
		public IBillingAdapter IAB { get; } = new DummyIAB();

		public string DoSHA256(string input) => "";

		public void OpenURL(string url) { }
		public void Sleep(int milsec) {}
		public void ExitApp() {}
	}

	public class DummyFileHelper : FileHelper
	{
		public override void WriteData(string fileid, string data) { }
		public override string ReadDataOrNull(string fileid) => null;
	}

	public class DummyIAB : IBillingAdapter
	{
		public bool IsConnected => false;
		public bool Connect(string[] productIDs) => false;
		public void Disconnect() { }
		public PurchaseQueryResult IsPurchased(string id) => PurchaseQueryResult.NotConnected;
		public PurchaseResult StartPurchase(string id) => PurchaseResult.NotConnected;
	}
}
