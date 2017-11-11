using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web.Routing;
using CCA.Util;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.CCAvenue.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.CCAvenue
{
    /// <summary>
    /// CCAvenue payment processor
    /// </summary>
    public class CCAvenuePaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CCAvenuePaymentSettings _ccAvenuePaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly CCACrypto _ccaCrypto;
        #endregion

        #region Ctor

        public CCAvenuePaymentProcessor(CCAvenuePaymentSettings ccAvenuePaymentSettings,
            ISettingService settingService, ICurrencyService currencyService,
            CurrencySettings currencySettings, IWebHelper webHelper)
        {
            this._ccAvenuePaymentSettings = ccAvenuePaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._ccaCrypto = new CCACrypto();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var remotePostHelperData = new Dictionary<string, string>();
            var remotePostHelper = new RemotePost
            {
                FormName = "CCAvenueForm",
                Url = _ccAvenuePaymentSettings.PayUri
            };

            //use TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            remotePostHelperData.Add("Merchant_Id", _ccAvenuePaymentSettings.MerchantId);
            remotePostHelperData.Add("Amount", postProcessPaymentRequest.Order.OrderTotal.ToString(new CultureInfo("en-US", false).NumberFormat));
            remotePostHelperData.Add("Currency", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            remotePostHelperData.Add("Order_Id", postProcessPaymentRequest.Order.Id.ToString());
            remotePostHelperData.Add("Redirect_Url", _webHelper.GetStoreLocation(false) + "Plugins/PaymentCCAvenue/Return");

            remotePostHelperData.Add("cancel_url", _webHelper.GetStoreLocation(false) + "Plugins/PaymentCCAvenue/Return");
            remotePostHelperData.Add("language", "EN");

            //var myUtility = new CCAvenueHelper();
            //remotePostHelperData.Add("Checksum", myUtility.getchecksum(_ccAvenuePaymentSettings.MerchantId.ToString(), postProcessPaymentRequest.Order.Id.ToString(), postProcessPaymentRequest.Order.OrderTotal.ToString(), _webHelper.GetStoreLocation(false) + "Plugins/PaymentCCAvenue/Return", _ccAvenuePaymentSettings.Key));
            
            //Billing details
            remotePostHelperData.Add("billing_name", postProcessPaymentRequest.Order.BillingAddress.FirstName );
            //remotePostHelperData.Add("billing_address", postProcessPaymentRequest.Order.BillingAddress.Address1 + " " + postProcessPaymentRequest.Order.BillingAddress.Address2);
           
            remotePostHelperData.Add("billing_address", postProcessPaymentRequest.Order.BillingAddress.Address1);
            remotePostHelperData.Add("billing_tel", postProcessPaymentRequest.Order.BillingAddress.PhoneNumber);
            remotePostHelperData.Add("billing_email", postProcessPaymentRequest.Order.BillingAddress.Email);

            remotePostHelperData.Add("billing_city", postProcessPaymentRequest.Order.BillingAddress.City);
            var billingStateProvince = postProcessPaymentRequest.Order.BillingAddress.StateProvince;
            remotePostHelperData.Add("billing_state", billingStateProvince != null ? billingStateProvince.Abbreviation : string.Empty);
            remotePostHelperData.Add("billing_zip", postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode);
            var billingCountry = postProcessPaymentRequest.Order.BillingAddress.Country;
            remotePostHelperData.Add("billing_country", billingCountry != null ? billingCountry.Name : string.Empty);

            //Delivery details

            if (postProcessPaymentRequest.Order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                remotePostHelperData.Add("delivery_name", postProcessPaymentRequest.Order.ShippingAddress.FirstName );
                //remotePostHelperData.Add("delivery_address", postProcessPaymentRequest.Order.ShippingAddress.Address1 + " " + postProcessPaymentRequest.Order.ShippingAddress.Address2);
                remotePostHelperData.Add("delivery_address", postProcessPaymentRequest.Order.ShippingAddress.Address1);
                //   remotePostHelper.Add("delivery_cust_notes", string.Empty);
                remotePostHelperData.Add("delivery_tel", postProcessPaymentRequest.Order.ShippingAddress.PhoneNumber);
                remotePostHelperData.Add("delivery_city", postProcessPaymentRequest.Order.ShippingAddress.City);
                var deliveryStateProvince = postProcessPaymentRequest.Order.ShippingAddress.StateProvince;
                remotePostHelperData.Add("delivery_state", deliveryStateProvince != null ? deliveryStateProvince.Abbreviation : string.Empty);
                remotePostHelperData.Add("delivery_zip", postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode);
                var deliveryCountry = postProcessPaymentRequest.Order.ShippingAddress.Country;
                remotePostHelperData.Add("delivery_country", deliveryCountry != null ? deliveryCountry.Name : string.Empty);
            }

            remotePostHelperData.Add("Merchant_Param", _ccAvenuePaymentSettings.MerchantParam);

            var strPOSTData = string.Empty;
            foreach (var item in remotePostHelperData)
            {
                //strPOSTData = strPOSTData +  item.Key.ToLower() + "=" + item.Value.ToLower() + "&";
                strPOSTData = strPOSTData + item.Key.ToLower() + "=" + item.Value + "&";
            }

            try
            {
                var strEncPOSTData = _ccaCrypto.Encrypt(strPOSTData, _ccAvenuePaymentSettings.Key);
                remotePostHelper.Add("encRequest", strEncPOSTData);
                remotePostHelper.Add("access_code", _ccAvenuePaymentSettings.AccessCode);
                
                remotePostHelper.Post();
            }
            catch (Exception ep)
            {
                throw new Exception(ep.Message);
            }
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _ccAvenuePaymentSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //CCAvenue is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice

            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;

            //let's ensure that at least 1 minute passed after order is placed
            return !((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1);
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentCCAvenue";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.CCAvenue.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentCCAvenue";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.CCAvenue.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentCCAvenueController);
        }

        public override void Install()
        {
            var settings = new CCAvenuePaymentSettings()
            {
                MerchantId = "",
                Key = "",
                AccessCode = "",
                MerchantParam = "",

                //PayUri = "https://www.ccavenue.com/shopzone/cc_details.jsp",
                PayUri = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction",
                AdditionalFee = 0,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.RedirectionTip", "You will be redirected to CCAvenue site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantId", "Merchant ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantId.Hint", "Enter merchant ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.Key", "Working Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.Key.Hint", "Enter working key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantParam", "Merchant Param");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantParam.Hint", "Enter merchant param.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.PayUri", "Pay URI");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.PayUri.Hint", "Enter Pay URI.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.AdditionalFee.Hint", "Enter additional fee to charge your customers.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.AccessCode", "Access Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.CCAvenue.AccessCode.Hint", "Enter Access Code.");

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantId");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.Key.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantParam");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.MerchantParam.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.PayUri");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.PayUri.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.AdditionalFee.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.AccessCode");
            this.DeletePluginLocaleResource("Plugins.Payments.CCAvenue.AccessCode.Hint");
            base.Uninstall();
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        #endregion
    }
}
