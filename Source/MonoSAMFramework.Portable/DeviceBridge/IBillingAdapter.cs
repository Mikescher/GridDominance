using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public enum PurchaseQueryResult { Purchased, NotPurchased, Refunded, Cancelled, Error, NotConnected, CurrentlyInitializing, Pending }
	public enum PurchaseResult { ProductNotFound, NotConnected, CurrentlyInitializing, PurchaseStarted }

	public static class AndroidBillingHelper
	{
		public const string PID_PURCHASED   = @"android.test.purchased";
		public const string PID_CANCELED    = @"android.test.canceled";
		public const string PID_REFUNDED    = @"android.test.refunded";
		public const string PID_UNAVAILABLE = @"android.test.item_unavailable";
	}

	public interface IBillingAdapter
	{
		bool IsConnected { get; }
        bool IsSynchronized { get; }

        Task<bool> Connect(string[] productIDs);
		Task Disconnect();

        PurchaseResult StartPurchase(string id);
		PurchaseQueryResult IsPurchased(string id);
		bool SynchronizePurchases(string[] productIDs);

	}
}
