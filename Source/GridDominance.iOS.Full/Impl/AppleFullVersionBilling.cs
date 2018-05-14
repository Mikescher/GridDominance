using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.iOS.Impl
{
	class AppleFullVersionBilling : IBillingAdapter
	{
		public bool IsConnected => true;

		public bool IsSynchronized => true;

		public bool Connect()
		{
			return true;
		}
		
		public void Disconnect()
		{
			//
		}

		public PurchaseResult StartPurchase(string id)
		{
			SAMLog.Error("IAB_FULL_IOS::StartPurchase", $"Calling StartPurchase({id}) in full version");
			return PurchaseResult.PurchaseStarted;
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			SAMLog.Error("IAB_FULL_IOS::IsPurchased", $"Calling IsPurchased({id}) in full version");
			return PurchaseQueryResult.Purchased;
		}

		public bool SynchronizePurchases(string[] productIDs)
		{
			SAMLog.Error("IAB_FULL_IOS::SynchronizePurchases", $"Calling SynchronizePurchases() in full version");
            return true;
		}
	}
}