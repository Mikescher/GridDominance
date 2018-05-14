using MonoSAMFramework.Portable.DeviceBridge;
using System.Collections.Generic;

namespace GridDominance.Windows
{
	class WindowsEmulatingBillingAdapter : IBillingAdapter
	{
		public bool IsConnected => true;

		public bool IsSynchronized => true;

		private readonly List<string> _purchased = new List<string>();

		public bool Connect(string[] productIDs)
		{
			return true;
		}

		public void Disconnect()
		{
			//
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			return _purchased.Contains(id) ? PurchaseQueryResult.Purchased : PurchaseQueryResult.NotPurchased;
		}

		public bool SynchronizePurchases(string[] productIDs)
		{
			return true;
		}

		public PurchaseResult StartPurchase(string id)
		{
			_purchased.Add(id);
			return PurchaseResult.PurchaseStarted;
		}
	}
}
