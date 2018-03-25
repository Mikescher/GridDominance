using System;
using MonoSAMFramework.Portable.DeviceBridge;

namespace GridDominance.iOS.Impl
{
	/// <summary>
	/// https://docs.microsoft.com/en-us/xamarin/ios/platform/in-app-purchasing/
	/// </summary>
	class AppleIABVersionBilling : IBillingAdapter
	{
		public bool IsConnected => true;

		public bool Connect(string[] productIDs)
		{
			return true;
		}
		
		public void Disconnect()
		{
			//
		}

		public PurchaseResult StartPurchase(string id)
		{
			throw new NotImplementedException();
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			throw new NotImplementedException();
		}
	}
}