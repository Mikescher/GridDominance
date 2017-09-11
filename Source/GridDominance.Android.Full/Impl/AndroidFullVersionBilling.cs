using Android.App;
using Android.Content;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android.Impl
{
	class AndroidFullVersionBilling : IBillingAdapter
	{
		public bool IsConnected => true;

		public bool Connect(string[] productIDs)
		{
			return true;
		}

		public void HandleActivityResult(int requestCode, Result resultCode, Intent data)
		{
			//
		}

		public void Disconnect()
		{
			//
		}

		public PurchaseResult StartPurchase(string id)
		{
			SAMLog.Error("IAB_FULL::StartPurchase", $"Calling StartPurchase({id}) in full version");
			return PurchaseResult.PurchaseStarted;
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			SAMLog.Error("IAB_FULL::IsPurchased", $"Calling IsPurchased({id}) in full version");
			return PurchaseQueryResult.Purchased;
		}
	}
}