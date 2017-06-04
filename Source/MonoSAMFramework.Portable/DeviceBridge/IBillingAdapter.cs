
namespace MonoSAMFramework.Portable.DeviceBridge
{
	public enum PurchaseQueryResult { Purchased, NotPurchased, Refunded, Cancelled, Error, NotConnected, CurrentlyInitializing }
	public enum PurchaseResult { ProductNotFound, NotConnected, CurrentlyInitializing, PurchaseStarted }

	public static class AndroidBillingHelper
	{
		public const string PID_PURCHASED   = @"android.test.purchased";
		public const string PID_CANCELED    = @"android.test.canceled";
		public const string PID_REFUNDED    = @"android.test.refunded";
		public const string PID_UNAVAILABLE = @"android.test.item_unavailable";

		public const int STATE_PURCHASED = 0;
		public const int STATE_REFUNDED  = 1;
		public const int STATE_CANCELLED = 2;
	}

	public interface IBillingAdapter
	{
		bool IsConnected { get; }

		bool Connect(string[] productIDs);
		void Disconnect();

		PurchaseResult StartPurchase(string id);
		PurchaseQueryResult IsPurchased(string id);
	}
}
