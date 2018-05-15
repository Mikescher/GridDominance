using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.LogProtocol;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

namespace GridDominance.iOS.Impl
{
	/// <summary>
	/// https://jamesmontemagno.github.io/InAppBillingPlugin/GettingStarted.html
	/// </summary>
	class AppleIABVersionBilling : IBillingAdapter
	{
		private const long TRYCONNECT_TIMEOUT = 16 * 1000; // 16s

		private const string VERIFY_PAYLOAD = "{1F0F9EFA-A23F-4CD1-8AC4-5AA8008532E8}";

		private const string PREFIX = "blackforestbytes.";

		public bool IsConnected { get; set; } = false;
		public bool IsSynchronized { get; set; } = false;

		private PurchaseError? LastConnectError = null;

		private readonly IInAppBilling _iab = CrossInAppBilling.Current;
      
		private readonly Dictionary<string, InAppBillingProduct> _products = new Dictionary<string, InAppBillingProduct>();
		private readonly Dictionary<string, InAppBillingPurchase> _purchases = new Dictionary<string, InAppBillingPurchase>();
      
		public bool Connect(string[] productIDs)
		{
			DoConnnect().Wait();
			return true;
		}

        private async Task DoConnnect()
		{
            try
            {
                SAMLog.Debug("AppleIABVersionBilling.TryConnect");

                var sucess = await _iab.ConnectAsync();

                if (!sucess) { LastConnectError = null; IsConnected = false; return; }
            
                LastConnectError = null;
                IsConnected = true;
            }
            catch (InAppBillingPurchaseException e)
            {
                SAMLog.Info("IAB_IOS::TryConnect_Start_1", e.PurchaseError.ToString(), e.ToString());

                LastConnectError = e.PurchaseError;
                IsConnected = false;
            }
            catch (Exception e)
            {
                SAMLog.Info("IAB_IOS::TryConnect_Start_2", e);

                LastConnectError = null;
                IsConnected = false;
            }
		}

		public bool SynchronizePurchases(string[] productIDs)
		{
			if (IsSynchronized) return false;
			Resync(productIDs.Select(p => PREFIX+p).ToArray()).EnsureNoError();
			return true;
		}

		private async Task Resync(string[] productIDs)
		{         
            if (LastConnectError != null) SAMLog.Debug("TryConnect: [[LastConnectError]]: " + LastConnectError);

            try
            {
                SAMLog.Debug("AppleIABVersionBilling.TryConnect");

                var sucess = await _iab.ConnectAsync();

                if (!sucess) { LastConnectError = null; IsConnected = false; return; }

				await QueryStatesAsync(productIDs);

                LastConnectError = null;
                IsConnected = true;
				IsSynchronized = true;
                return;
            }
            catch (InAppBillingPurchaseException e)
            {
                SAMLog.Info("IAB_IOS::TryConnect_Start_1", e.PurchaseError.ToString(), e.ToString());

                LastConnectError = e.PurchaseError;
                IsConnected = false;
                return;
            }
            catch (Exception e)
            {
                SAMLog.Info("IAB_IOS::TryConnect_Start_2", e);

                LastConnectError = null;
                IsConnected = false;
                return;
            }
		}
      
		private async Task QueryStatesAsync(string[] productIDs)
		{
			try
			{ 
				var items = await _iab.GetProductInfoAsync(ItemType.InAppPurchase, productIDs);

				lock (_products)
				{
					foreach(var item in items)
					{
						_products[item.ProductId.ToLower()] = item;
					}
				}

				var purchases = await _iab.GetPurchasesAsync(ItemType.InAppPurchase);
				
				lock (_purchases)
				{
					foreach(var item in purchases)
					{
						_purchases[item.ProductId.ToLower()] = item;
					}
				}
			}
			catch(InAppBillingPurchaseException e)
			{
				switch (e.PurchaseError)
				{
					case PurchaseError.BillingUnavailable:
						SAMLog.Error("IAB_IOS::QSA_1", "BillingUnavailable", e.ToString());
						break;

					case PurchaseError.DeveloperError:
						SAMLog.Error("IAB_IOS::QSA_1", "DeveloperError", e.ToString());
						break;

					case PurchaseError.ItemUnavailable:
						SAMLog.Error("IAB_IOS::QSA_1", "ItemUnavailable", e.ToString());
						break;

					case PurchaseError.GeneralError:
						SAMLog.Error("IAB_IOS::QSA_1", "GeneralError", e.ToString());
						break;

					case PurchaseError.UserCancelled:
						SAMLog.Info("IAB_IOS::QSA_1", "UserCancelled", e.ToString());
						break;

					case PurchaseError.AppStoreUnavailable:
						SAMLog.Warning("IAB_IOS::QSA_1", "AppStoreUnavailable", e.ToString());
						break;

					case PurchaseError.PaymentNotAllowed:
						SAMLog.Error("IAB_IOS::QSA_1", "PaymentNotAllowed", e.ToString());
						break;

					case PurchaseError.PaymentInvalid:
						SAMLog.Error("IAB_IOS::QSA_1", "PaymentInvalid", e.ToString());
						break;

					case PurchaseError.InvalidProduct:
						SAMLog.Error("IAB_IOS::QSA_1", "InvalidProduct", e.ToString());
						break;

					case PurchaseError.ProductRequestFailed:
						SAMLog.Error("IAB_IOS::QSA_1", "ProductRequestFailed", e.ToString());
						break;

					case PurchaseError.RestoreFailed:
						SAMLog.Warning("IAB_IOS::QSA_1", "RestoreFailed", e.ToString());
						break;

					case PurchaseError.ServiceUnavailable:
						SAMLog.Info("IAB_IOS::QSA_1", "ServiceUnavailable", e.ToString());
						break;

					case PurchaseError.AlreadyOwned:
						SAMLog.Warning("IAB_IOS::QSA_1", "AlreadyOwned", e.ToString());
						break;

					case PurchaseError.NotOwned:
						SAMLog.Error("IAB_IOS::QSA_1", "NotOwned", e.ToString());
						break;

					default:
						SAMLog.Error("IAB_IOS::QSA_EnumSwitch", "e.PurchaseError = " + e.PurchaseError);
						break;
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("IAB_IOS::QSA_Exception", e);
			}
		}

		public void Disconnect()
		{
			if (IsConnected) _iab.DisconnectAsync().Wait();
		}

		public PurchaseResult StartPurchase(string id)
		{
            if (!IsConnected) return PurchaseResult.NotConnected;

			DoPurchase(id).EnsureNoError();

			return PurchaseResult.PurchaseStarted;
		}

		private async Task DoPurchase(string id)
		{
			id = PREFIX + id;

			var purchase = await _iab.PurchaseAsync(id, ItemType.InAppPurchase, VERIFY_PAYLOAD, null);

			lock (_purchases)
			{
				SAMLog.Debug("AppleIABVersionBilling.DoPurchase1");
				
				if (purchase == null)
				{
					SAMLog.Debug("AppleIABVersionBilling.DoPurchase: returned null");
					_purchases.Remove(id.ToLower());
					return;
				}
				else
				{
					SAMLog.Debug("AppleIABVersionBilling.DoPurchase: returned " + purchase.State);
					_purchases[id.ToLower()] = purchase;
					return;
				}
			}
		}

		public PurchaseQueryResult IsPurchased(string id)
		{
			if (!IsConnected) return PurchaseQueryResult.NotConnected;

			id = PREFIX + id;

			lock (_purchases)
			{
				if (_purchases.TryGetValue(id.ToLower(), out var purchase))
				{
					switch (purchase.State)
					{
						case PurchaseState.Purchased:
						case PurchaseState.FreeTrial:
						case PurchaseState.Restored:
							return PurchaseQueryResult.Purchased;

						case PurchaseState.Canceled:
							return PurchaseQueryResult.Cancelled;

						case PurchaseState.Refunded:
							return PurchaseQueryResult.Refunded;

						case PurchaseState.PaymentPending:
						case PurchaseState.Purchasing:
						case PurchaseState.Deferred:
							return PurchaseQueryResult.Pending;

						case PurchaseState.Failed:
						case PurchaseState.Unknown:
							return PurchaseQueryResult.Error;

						default:
							SAMLog.Error("IAB_IOS::IP_EnumSwitch", "purchase.State := " + purchase.State);
							return PurchaseQueryResult.Error;
					}
				}
				else
				{
					return PurchaseQueryResult.NotPurchased;
				}
			}
		}

	}
}