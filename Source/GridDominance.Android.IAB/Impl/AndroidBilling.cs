using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;
using Plugin.InAppBilling;

namespace GridDominance.Android.Impl
{
	// https://components.xamarin.com/gettingstarted/xamarin.inappbilling
	class AndroidBilling : IBillingAdapter
	{
		public bool IsConnected => CrossInAppBilling.Current.IsConnected;

		public bool IsSynchronized {get; private set;} = false;

		private List<InAppBillingProduct> _products;
		private List<InAppBillingPurchase> _purchases;
		private string[] _productIDs = null;
		private bool _isInitializing = false;

		public AndroidBilling()
		{
			SAMLog.Debug("AndroidBilling.ctr");
		}

		public async Task<bool> Connect(string[] productIDs)
		{
			try
			{
				SAMLog.Debug("AndroidBilling.Connect");

                if (!CrossInAppBilling.IsSupported)
                {
                    SAMLog.Info("IAB::Connect::NotSupported", "CrossInAppBilling.IsSupported == false");
                    return false;
                }

                _purchases = null;
				_productIDs = productIDs;

				if (IsConnected) await Disconnect();

				var connected = await CrossInAppBilling.Current.ConnectAsync();
				if (!connected) return false;

				await OnConnected();

				return true;
			}
			catch (Exception e)
			{
				SAMLog.Info("IAB::Connect", e);
				return false;
			}
		}

		public bool SynchronizePurchases(string[] productIDs)
		{
			return true;
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			try
			{
				if (_isInitializing) return PurchaseQueryResult.CurrentlyInitializing;

				if (!IsConnected || _purchases == null) return PurchaseQueryResult.NotConnected;

				PurchaseState result = PurchaseState.Unknown;
				foreach (var purch in _purchases.Where(p => p.ProductId == id).OrderBy(p => p.TransactionDateUtc))
				{
					result = purch.State;
                    if (purch.State == PurchaseState.Purchased) return PurchaseQueryResult.Purchased;
                    if (purch.State == PurchaseState.Restored) return PurchaseQueryResult.Purchased;
                }

				if (result == PurchaseState.Unknown) return PurchaseQueryResult.NotPurchased;
                if (result == PurchaseState.Deferred) return PurchaseQueryResult.NotPurchased;
                if (result == PurchaseState.Purchasing) return PurchaseQueryResult.NotPurchased;
                if (result == PurchaseState.PaymentPending) return PurchaseQueryResult.NotPurchased;

                if (result == PurchaseState.Canceled) return PurchaseQueryResult.Cancelled;
                if (result == PurchaseState.Failed) return PurchaseQueryResult.Cancelled;


                SAMLog.Error("IAB::IsPurchased-Inv", "result has invalid value: " + result);
				return PurchaseQueryResult.Error;
			}
			catch (Exception e)
			{
				SAMLog.Error("IAB::IsPurchased-Ex", e);
				return PurchaseQueryResult.Error;
			}
		}

		public PurchaseResult StartPurchase(string id)
		{
			SAMLog.Debug($"AndroidBilling.StartPurchase({id})");

			//  You must make the call to BuyProduct from the main thread of your Activity or it will fail.

			if (_isInitializing) return PurchaseResult.CurrentlyInitializing;
			if (!IsConnected) return PurchaseResult.NotConnected;

			var prod = _products.FirstOrDefault(p => p.ProductId == id);
			if (prod == null) return PurchaseResult.ProductNotFound;

            PurchaseInBackground(prod);
            return PurchaseResult.PurchaseStarted;
        }

        private async void PurchaseInBackground(InAppBillingProduct prod)
        {
            try
            {
                var purch = await CrossInAppBilling.Current.PurchaseAsync(prod.ProductId, ItemType.InAppPurchase);
				this._purchases.Add(purch);
            }
            catch (InAppBillingPurchaseException e)
            {
                SAMLog.Info("IAB::PurchaseInBackground::IABPE", e);
            }
            catch (Exception e)
            {
                SAMLog.Info("IAB::PurchaseInBackground::EX", e);
            }
        }

        public async Task Disconnect()
		{
			try
			{
				SAMLog.Debug($"AndroidBilling.Disconnect");

				if (IsConnected) await CrossInAppBilling.Current.DisconnectAsync();
			}
			catch (Exception e)
			{
				SAMLog.Error("IAB::Disconnect", e);
			}
		}

		private async Task OnConnected()
		{
			try
			{
				_isInitializing = true;
				SAMLog.Debug($"AndroidBilling.OnConnected[1]");

				_purchases = (await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase)).ToList();
                IsSynchronized = true;

                SAMLog.Debug($"AndroidBilling.OnConnected[2]");

                _products = (await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, _productIDs)).ToList();

                SAMLog.Debug($"AndroidBilling.OnConnected[3]");
			}
			catch (Exception e)
			{
				SAMLog.Info("IAB::OnConnected", e);
			}
			finally
			{
				_isInitializing = false;
			}
		}
	}
}