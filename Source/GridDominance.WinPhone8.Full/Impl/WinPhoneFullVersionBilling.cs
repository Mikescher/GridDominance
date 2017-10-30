using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android.Impl
{
	class WinPhoneFullVersionBilling : IBillingAdapter
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
			SAMLog.Error("IAB_WP8FULL::StartPurchase", $"Calling StartPurchase({id}) in full version");
			return PurchaseResult.PurchaseStarted;
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			SAMLog.Error("IAB_WP8FULL::IsPurchased", $"Calling IsPurchased({id}) in full version");
			return PurchaseQueryResult.Purchased;
		}
	}
}