using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayPalStandard.Models;
using Nop.Plugin.Payments.PayPalStandard.Services.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using System.Web;
using Nop.Core.Infrastructure;
using Nop.Web.Models.Checkout;
using System.Web.Routing;
using Nop.Services.Directory;
using Nop.Services.Catalog;
using Nop.Plugin.Payments.PayPalStandard.Domain.Orders;

namespace Nop.Plugin.Payments.PayPalStandard.Controllers
{
	public class PaymentPayPalStandardController : BasePaymentController
	{
		private readonly IWorkContext _workContext;
		private readonly IStoreService _storeService;
		private readonly ISettingService _settingService;
		private readonly IPaymentService _paymentService;
		private readonly IOrderService _orderService;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly ILocalizationService _localizationService;
		private readonly IStoreContext _storeContext;
		private readonly ILogger _logger;
		private readonly IWebHelper _webHelper;
		private readonly PaymentSettings _paymentSettings;
		private readonly PayPalStandardPaymentSettings _payPalStandardPaymentSettings;
		//Customization Propeers info
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly ITempOrder _tempOrderService;
		private readonly HttpContextBase _httpContext;
		private readonly OrderSettings _orderSettings;
		private readonly ICurrencyService _currencyService;
		private readonly IPriceFormatter _priceFormatter;
		private readonly IPaypalStandardOrderProcessingService _payPalOrderProcessingService;
		private readonly IOrderTotalCalculationService _orderTotalCalculationService;
		

		public PaymentPayPalStandardController(IWorkContext workContext,
			 IStoreService storeService,
			 ISettingService settingService,
			 IPaymentService paymentService,
			 IOrderService orderService,
			 IOrderProcessingService orderProcessingService,
			 ILocalizationService localizationService,
			 IStoreContext storeContext,
			 ILogger logger,
			 IWebHelper webHelper,
			 PaymentSettings paymentSettings,
			 PayPalStandardPaymentSettings payPalStandardPaymentSettings,
			 IGenericAttributeService genericAttributeService,
			 HttpContextBase httpContext,
			 OrderSettings orderSettings,
			 ICurrencyService currencyService,
			 IPriceFormatter priceFormatter,
			 IPaypalStandardOrderProcessingService payPalOrderProcessingService,
			 IOrderTotalCalculationService orderTotalCalculationService,
			 ITempOrder tempOrderService
			)
		{
			this._workContext = workContext;
			this._storeService = storeService;
			this._settingService = settingService;
			this._paymentService = paymentService;
			this._orderService = orderService;
			this._orderProcessingService = orderProcessingService;
			this._localizationService = localizationService;
			this._storeContext = storeContext;
			this._logger = logger;
			this._webHelper = webHelper;
			this._paymentSettings = paymentSettings;
			this._payPalStandardPaymentSettings = payPalStandardPaymentSettings;
			this._genericAttributeService = genericAttributeService;
			this._httpContext = httpContext;
			this._orderSettings = orderSettings;
			this._currencyService = currencyService;
			this._priceFormatter = priceFormatter;
			this._payPalOrderProcessingService = payPalOrderProcessingService;
			this._orderTotalCalculationService = orderTotalCalculationService;
			this._tempOrderService = tempOrderService;
		}

		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
			//load settings for a chosen store scope
			var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
			var payPalStandardPaymentSettings = _settingService.LoadSetting<PayPalStandardPaymentSettings>(storeScope);

			var model = new ConfigurationModel();
			model.UseSandbox = payPalStandardPaymentSettings.UseSandbox;
			model.BusinessEmail = payPalStandardPaymentSettings.BusinessEmail;
			model.PdtToken = payPalStandardPaymentSettings.PdtToken;
			model.PdtValidateOrderTotal = payPalStandardPaymentSettings.PdtValidateOrderTotal;
			model.AdditionalFee = payPalStandardPaymentSettings.AdditionalFee;
			model.AdditionalFeePercentage = payPalStandardPaymentSettings.AdditionalFeePercentage;
			model.PassProductNamesAndTotals = payPalStandardPaymentSettings.PassProductNamesAndTotals;
			model.EnableIpn = payPalStandardPaymentSettings.EnableIpn;
			model.IpnUrl = payPalStandardPaymentSettings.IpnUrl;
			model.AddressOverride = payPalStandardPaymentSettings.AddressOverride;
			model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = payPalStandardPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;
			model.AddressOverride = payPalStandardPaymentSettings.AddressOverride;
			model.SelectShippingAddressMode = payPalStandardPaymentSettings.SelectShippingAddressMode;

			model.ActiveStoreScopeConfiguration = storeScope;
			if (storeScope > 0)
			{
				model.UseSandbox_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.UseSandbox, storeScope);
				model.BusinessEmail_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.BusinessEmail, storeScope);
				model.PdtToken_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.PdtToken, storeScope);
				model.PdtValidateOrderTotal_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.PdtValidateOrderTotal, storeScope);
				model.AdditionalFee_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.AdditionalFee, storeScope);
				model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
				model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
				model.EnableIpn_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.EnableIpn, storeScope);
				model.IpnUrl_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.IpnUrl, storeScope);
				model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope);
				model.AddressOverride_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.AddressOverride, storeScope);
				model.SelectShippingAddressMod_OverrideForStore = _settingService.SettingExists(payPalStandardPaymentSettings, x => x.SelectShippingAddressMode, storeScope);
			}

			model.AddressModes.Add(new SelectListItem() { Text = "Optional", Value = "0" });
			model.AddressModes.Add(new SelectListItem() { Text = "Do not prompt", Value = "1" });
			model.AddressModes.Add(new SelectListItem() { Text = "Required", Value = "3" });

			return View("~/Plugins/Payments.PayPalStandard/Views/PaymentPayPalStandard/Configure.cshtml", model);
		}

		[HttpPost]
		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure(ConfigurationModel model)
		{
			if (!ModelState.IsValid)
				return Configure();

			//load settings for a chosen store scope
			var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
			var payPalStandardPaymentSettings = _settingService.LoadSetting<PayPalStandardPaymentSettings>(storeScope);

			//save settings
			payPalStandardPaymentSettings.UseSandbox = model.UseSandbox;
			payPalStandardPaymentSettings.BusinessEmail = model.BusinessEmail;
			payPalStandardPaymentSettings.PdtToken = model.PdtToken;
			payPalStandardPaymentSettings.PdtValidateOrderTotal = model.PdtValidateOrderTotal;
			payPalStandardPaymentSettings.AdditionalFee = model.AdditionalFee;
			payPalStandardPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
			payPalStandardPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
			payPalStandardPaymentSettings.EnableIpn = model.EnableIpn;
			payPalStandardPaymentSettings.IpnUrl = model.IpnUrl;
			payPalStandardPaymentSettings.AddressOverride = model.AddressOverride;
			payPalStandardPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage;

			/* We do not clear cache after each setting update.
			 * This behavior can increase performance because cached settings will not be cleared 
			 * and loaded from database after each update */
			if (model.UseSandbox_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.UseSandbox, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.UseSandbox, storeScope);

			if (model.BusinessEmail_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.BusinessEmail, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.BusinessEmail, storeScope);

			if (model.PdtToken_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.PdtToken, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.PdtToken, storeScope);

			if (model.PdtValidateOrderTotal_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.PdtValidateOrderTotal, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.PdtValidateOrderTotal, storeScope);

			if (model.AdditionalFee_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.AdditionalFee, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.AdditionalFee, storeScope);

			if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

			if (model.PassProductNamesAndTotals_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.PassProductNamesAndTotals, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);

			if (model.EnableIpn_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.EnableIpn, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.EnableIpn, storeScope);

			if (model.IpnUrl_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.IpnUrl, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.IpnUrl, storeScope);

			if (model.AddressOverride_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.AddressOverride, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.AddressOverride, storeScope);

			if (model.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore || storeScope == 0)
				_settingService.SaveSetting(payPalStandardPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope, false);
			else if (storeScope > 0)
				_settingService.DeleteSetting(payPalStandardPaymentSettings, x => x.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage, storeScope);

			//now clear settings cache
			_settingService.ClearCache();

			SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

			return Configure();
		}

		[ChildActionOnly]
		public ActionResult PaymentInfo()
		{
			return View("~/Plugins/Payments.PayPalStandard/Views/PaymentPayPalStandard/PaymentInfo.cshtml");
		}

		[NonAction]
		public override IList<string> ValidatePaymentForm(FormCollection form)
		{
			var warnings = new List<string>();
			return warnings;
		}

		[NonAction]
		public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
		{
			var paymentInfo = new ProcessPaymentRequest();

			return paymentInfo;

		}

		[ValidateInput(false)]
		public ActionResult PDTHandler(FormCollection form)
		{
			var tx = _webHelper.QueryString<string>("tx");
			Dictionary<string, string> values;
			string response;

			var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.PayPalStandard") as PayPalStandardPaymentProcessor;
			if (processor == null ||
				 !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
				throw new NopException("PayPal Standard module cannot be loaded");

			Guid orderGuid = (Guid)this.Session["orderGuid"];
			var processPaymentRequest = this.Session["orderobj"] as ProcessPaymentRequest;

			if (orderGuid != null)
			{
				var resulSaveOrder = _orderProcessingService.PlaceOrder(processPaymentRequest);
				int id = resulSaveOrder.PlacedOrder.Id;
				var tempOrd = _tempOrderService.GetTempOrderByGuid(orderGuid);
				tempOrd.OrderId = id;
				_tempOrderService.UpdateTempOrder(tempOrd);
				return RedirectToRoute("CheckoutCompleted", new { orderId = id });
			}

			if (processor.GetPdtDetails(tx, out values, out response))
			{
				string orderNumber = string.Empty;
				values.TryGetValue("custom", out orderNumber);
				Guid orderNumberGuid = Guid.Empty;
				try
				{
					orderNumberGuid = new Guid(orderNumber);
				}
				catch { }
				Order order = _orderService.GetOrderByGuid(orderNumberGuid);
				if (order != null)
				{
					decimal mc_gross = decimal.Zero;
					try
					{
						mc_gross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
					}
					catch (Exception exc)
					{
						_logger.Error("PayPal PDT. Error getting mc_gross", exc);
					}

					string payer_status = string.Empty;
					values.TryGetValue("payer_status", out payer_status);
					string payment_status = string.Empty;
					values.TryGetValue("payment_status", out payment_status);
					string pending_reason = string.Empty;
					values.TryGetValue("pending_reason", out pending_reason);
					string mc_currency = string.Empty;
					values.TryGetValue("mc_currency", out mc_currency);
					string txn_id = string.Empty;
					values.TryGetValue("txn_id", out txn_id);
					string payment_type = string.Empty;
					values.TryGetValue("payment_type", out payment_type);
					string payer_id = string.Empty;
					values.TryGetValue("payer_id", out payer_id);
					string receiver_id = string.Empty;
					values.TryGetValue("receiver_id", out receiver_id);
					string invoice = string.Empty;
					values.TryGetValue("invoice", out invoice);
					string payment_fee = string.Empty;
					values.TryGetValue("payment_fee", out payment_fee);

					var sb = new StringBuilder();
					sb.AppendLine("Paypal PDT:");
					sb.AppendLine("mc_gross: " + mc_gross);
					sb.AppendLine("Payer status: " + payer_status);
					sb.AppendLine("Payment status: " + payment_status);
					sb.AppendLine("Pending reason: " + pending_reason);
					sb.AppendLine("mc_currency: " + mc_currency);
					sb.AppendLine("txn_id: " + txn_id);
					sb.AppendLine("payment_type: " + payment_type);
					sb.AppendLine("payer_id: " + payer_id);
					sb.AppendLine("receiver_id: " + receiver_id);
					sb.AppendLine("invoice: " + invoice);
					sb.AppendLine("payment_fee: " + payment_fee);

					var newPaymentStatus = PaypalHelper.GetPaymentStatus(payment_status, pending_reason);
					sb.AppendLine("New payment status: " + newPaymentStatus);

					//order note
					order.OrderNotes.Add(new OrderNote
					{
						Note = sb.ToString(),
						DisplayToCustomer = false,
						CreatedOnUtc = DateTime.UtcNow
					});
					_orderService.UpdateOrder(order);

					//load settings for a chosen store scope
					var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
					var payPalStandardPaymentSettings = _settingService.LoadSetting<PayPalStandardPaymentSettings>(storeScope);

					//validate order total
					if (payPalStandardPaymentSettings.PdtValidateOrderTotal && !Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
					{
						string errorStr = string.Format("PayPal PDT. Returned order total {0} doesn't equal order total {1}", mc_gross, order.OrderTotal);
						_logger.Error(errorStr);

						return RedirectToAction("Index", "Home", new { area = "" });
					}

					//mark order as paid
					if (newPaymentStatus == PaymentStatus.Paid)
					{
						if (_orderProcessingService.CanMarkOrderAsPaid(order))
						{
							order.AuthorizationTransactionId = txn_id;
							_orderService.UpdateOrder(order);

							_orderProcessingService.MarkOrderAsPaid(order);
						}
					}
				}

				return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
			}
			else
			{
				string orderNumber = string.Empty;
				values.TryGetValue("custom", out orderNumber);
				Guid orderNumberGuid = Guid.Empty;
				try
				{
					orderNumberGuid = new Guid(orderNumber);
				}
				catch { }
				Order order = _orderService.GetOrderByGuid(orderNumberGuid);
				if (order != null)
				{
					//order note
					order.OrderNotes.Add(new OrderNote
					{
						Note = "PayPal PDT failed. " + response,
						DisplayToCustomer = false,
						CreatedOnUtc = DateTime.UtcNow
					});
					_orderService.UpdateOrder(order);
				}
				return RedirectToAction("Index", "Home", new { area = "" });
			}
		}

		[ValidateInput(false)]
		public ActionResult IPNHandler()
		{
			byte[] param = Request.BinaryRead(Request.ContentLength);
			string strRequest = Encoding.ASCII.GetString(param);
			Dictionary<string, string> values;

			var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.PayPalStandard") as PayPalStandardPaymentProcessor;
			if (processor == null ||
				 !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
				throw new NopException("PayPal Standard module cannot be loaded");

			if (processor.VerifyIpn(strRequest, out values))
			{
				#region values
				decimal mc_gross = decimal.Zero;
				try
				{
					mc_gross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
				}
				catch { }

				string payer_status = string.Empty;
				values.TryGetValue("payer_status", out payer_status);
				string payment_status = string.Empty;
				values.TryGetValue("payment_status", out payment_status);
				string pending_reason = string.Empty;
				values.TryGetValue("pending_reason", out pending_reason);
				string mc_currency = string.Empty;
				values.TryGetValue("mc_currency", out mc_currency);
				string txn_id = string.Empty;
				values.TryGetValue("txn_id", out txn_id);
				string txn_type = string.Empty;
				values.TryGetValue("txn_type", out txn_type);
				string rp_invoice_id = string.Empty;
				values.TryGetValue("rp_invoice_id", out rp_invoice_id);
				string payment_type = string.Empty;
				values.TryGetValue("payment_type", out payment_type);
				string payer_id = string.Empty;
				values.TryGetValue("payer_id", out payer_id);
				string receiver_id = string.Empty;
				values.TryGetValue("receiver_id", out receiver_id);
				string invoice = string.Empty;
				values.TryGetValue("invoice", out invoice);
				string payment_fee = string.Empty;
				values.TryGetValue("payment_fee", out payment_fee);

				#endregion

				var sb = new StringBuilder();
				sb.AppendLine("Paypal IPN:");
				foreach (KeyValuePair<string, string> kvp in values)
				{
					sb.AppendLine(kvp.Key + ": " + kvp.Value);
				}

				var newPaymentStatus = PaypalHelper.GetPaymentStatus(payment_status, pending_reason);
				sb.AppendLine("New payment status: " + newPaymentStatus);

				switch (txn_type)
				{
					case "recurring_payment_profile_created":
						//do nothing here
						break;
					case "recurring_payment":
						#region Recurring payment
						{
							Guid orderNumberGuid = Guid.Empty;
							try
							{
								orderNumberGuid = new Guid(rp_invoice_id);
							}
							catch
							{
							}

							var initialOrder = _orderService.GetOrderByGuid(orderNumberGuid);
							if (initialOrder != null)
							{
								var recurringPayments = _orderService.SearchRecurringPayments(0, 0, initialOrder.Id, null, 0, int.MaxValue);
								foreach (var rp in recurringPayments)
								{
									switch (newPaymentStatus)
									{
										case PaymentStatus.Authorized:
										case PaymentStatus.Paid:
											{
												var recurringPaymentHistory = rp.RecurringPaymentHistory;
												if (recurringPaymentHistory.Count == 0)
												{
													//first payment
													var rph = new RecurringPaymentHistory
													{
														RecurringPaymentId = rp.Id,
														OrderId = initialOrder.Id,
														CreatedOnUtc = DateTime.UtcNow
													};
													rp.RecurringPaymentHistory.Add(rph);
													_orderService.UpdateRecurringPayment(rp);
												}
												else
												{
													//next payments
													_orderProcessingService.ProcessNextRecurringPayment(rp);
												}
											}
											break;
									}
								}

								//this.OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
								_logger.Information("PayPal IPN. Recurring info", new NopException(sb.ToString()));
							}
							else
							{
								_logger.Error("PayPal IPN. Order is not found", new NopException(sb.ToString()));
							}
						}
						#endregion
						break;
					default:
						#region Standard payment
						{
							string orderNumber = string.Empty;
							values.TryGetValue("custom", out orderNumber);
							Guid orderNumberGuid = Guid.Empty;
							try
							{
								orderNumberGuid = new Guid(orderNumber);
							}
							catch
							{
							}

							var order = _orderService.GetOrderByGuid(orderNumberGuid);
							if (order != null)
							{

								//order note
								order.OrderNotes.Add(new OrderNote
								{
									Note = sb.ToString(),
									DisplayToCustomer = false,
									CreatedOnUtc = DateTime.UtcNow
								});
								_orderService.UpdateOrder(order);

								switch (newPaymentStatus)
								{
									case PaymentStatus.Pending:
										{
										}
										break;
									case PaymentStatus.Authorized:
										{
											if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
											{
												_orderProcessingService.MarkAsAuthorized(order);
											}
										}
										break;
									case PaymentStatus.Paid:
										{
											if (_orderProcessingService.CanMarkOrderAsPaid(order))
											{

												order.AuthorizationTransactionId = txn_id;
												_orderService.UpdateOrder(order);

												_orderProcessingService.MarkOrderAsPaid(order);
											}
										}
										break;
									case PaymentStatus.Refunded:
										{
											var totalToRefund = Math.Abs(mc_gross);
											if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
											{
												//refund
												if (_orderProcessingService.CanRefundOffline(order))
												{
													_orderProcessingService.RefundOffline(order);
												}
											}
											else
											{
												//partial refund
												if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
												{
													_orderProcessingService.PartiallyRefundOffline(order, totalToRefund);
												}
											}
										}
										break;
									case PaymentStatus.Voided:
										{
											if (_orderProcessingService.CanVoidOffline(order))
											{
												_orderProcessingService.VoidOffline(order);
											}
										}
										break;
									default:
										break;
								}
							}
							else
							{
								_logger.Error("PayPal IPN. Order is not found", new NopException(sb.ToString()));
							}
						}
						#endregion
						break;
				}
			}
			else
			{
				_logger.Error("PayPal IPN failed.", new NopException(strRequest));
			}

			//nothing should be rendered to visitor
			return Content("");
		}

		public ActionResult CancelOrder(FormCollection form)
		{
			if (_payPalStandardPaymentSettings.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage)
			{
				var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
					 customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
					 .FirstOrDefault();
				if (order != null)
				{
					return RedirectToRoute("OrderDetails", new { orderId = order.Id });
				}
			}

			return RedirectToRoute("ShoppingCart");
			//return RedirectToAction("Index", "Home", new { area = "" });
		}


		[ValidateInput(false)]
		public ActionResult OpcSavePaymentInfo1(FormCollection form)
		{
			try
			{
				//validation
				var cart = _workContext.CurrentCustomer.ShoppingCartItems
				  .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
				  .LimitPerStore(_storeContext.CurrentStore.Id)
				  .ToList();
				if (cart.Count == 0)
					throw new Exception("Your cart is empty");

				if (!_orderSettings.OnePageCheckoutEnabled)
					throw new Exception("One page checkout is disabled");

				if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
					throw new Exception("Anonymous checkout is not allowed");

				var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
				  SystemCustomerAttributeNames.SelectedPaymentMethod,
				  _genericAttributeService, _storeContext.CurrentStore.Id);
				var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
				if (paymentMethod == null)
					throw new Exception("Payment method is not selected");

				var paymentControllerType = paymentMethod.GetControllerType();
				var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as BasePaymentController;
				if (paymentController == null)
					throw new Exception("Payment controller cannot be loaded");

				var warnings = paymentController.ValidatePaymentForm(form);
				foreach (var warning in warnings)
					ModelState.AddModelError("", "error");

				if (ModelState.IsValid)
				{
					//get payment info
					var paymentInfo = paymentController.GetPaymentInfo(form);
					//session save
					_httpContext.Session["OrderPaymentInfo"] = paymentInfo;


					if (paymentMethod != null)
					{
						if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
						{


							return Json(new { redirect = string.Format("{0}PaymentPayPalStandard/OpcCompleteRedirectionPayment1", _webHelper.GetStoreLocation()) });

						}
						else
						{

							var confirmOrderModel = PrepareConfirmOrderModel(cart);
							return Json(new
							{
								update_section = new UpdateSectionJsonModel
								{
									name = "confirm-order",
									html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
								},
								goto_section = "confirm_order"
							});

						}
					}
				}

				//If we got this far, something failed, redisplay form
				var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
				return Json(new
				{
					update_section = new UpdateSectionJsonModel
					{
						name = "payment-info",
						html = this.RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
					}
				});
			}
			catch (Exception exc)
			{
				_logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
				return Json(new { error = 1, message = exc.Message });
			}
		}



		[NonAction]
		protected virtual CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod)
		{
			var model = new CheckoutPaymentInfoModel();
			string actionName;
			string controllerName;
			RouteValueDictionary routeValues;
			paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
			model.PaymentInfoActionName = actionName;
			model.PaymentInfoControllerName = controllerName;
			model.PaymentInfoRouteValues = routeValues;
			model.DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab;
			return model;
		}

		[NonAction]
		protected virtual CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart)
		{
			var model = new CheckoutConfirmModel();
			//terms of service
			model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
			//min order amount validation
			bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
			if (!minOrderTotalAmountOk)
			{
				decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
				model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
			}
			return model;
		}

		[NonAction]
		protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false)
		{
			bool result = true;

			//check whether order total equals zero
			decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, ignoreRewardPoints);
			if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
				result = false;
			return result;
		}


		//Customization Propeers info (changed by ravi)
		public ActionResult OpcCompleteRedirectionPayment1()
		{
			try
			{

				var Orginalcart = _workContext.CurrentCustomer.ShoppingCartItems
			 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
			 .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
			 .ToList();



				if (Orginalcart.Count == 0)
					throw new Exception("Your cart is empty");



				//place order
				var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
				if (processPaymentRequest == null)
				{
					//Check whether payment workflow is required
					if (IsPaymentWorkflowRequired(Orginalcart))
					{
						throw new Exception("Payment information is not entered");
					}
					else
						processPaymentRequest = new ProcessPaymentRequest();
				}




				processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
				processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;

				processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
					 SystemCustomerAttributeNames.SelectedPaymentMethod,
					 _genericAttributeService, _storeContext.CurrentStore.Id);

				TempOrder tempOrderObj = new TempOrder();
				tempOrderObj.TempOrderGuid = Guid.NewGuid();
				this.Session["orderGuid"] = tempOrderObj.TempOrderGuid;
				_tempOrderService.InsertTempOrder(tempOrderObj);
				this.Session["orderobj"] = processPaymentRequest;
				var placeOrderResult = _payPalOrderProcessingService.PaypalOrderDetails(processPaymentRequest);
				TempData["OrderCancelValue"] = 1;


				//_shoppingCartService.AddToCart(, )





				var customer = _workContext.CurrentCustomer;
				var paymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName;/*customer.GetAttribute<string>(
                   SystemCustomerAttributeNames.SelectedPaymentMethod,
                   _genericAttributeService, _storeContext.CurrentStore.Id);   */

				var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
				Guid ogid = processPaymentRequest.OrderGuid;


				//int oid = processPaymentRequest.InitialOrderId;


				//Order odObject = _orderService.GetOrderByGuid(ogid);

				IList<ShoppingCartItem> cart = null;
				cart = Orginalcart.ToList();

				/*cart = _workContext.CurrentCustomer.ShoppingCartItems
						 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
						 .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
						 .ToList();        */



				/*  var postProcessPaymentRequest = new PostProcessPaymentRequest()
				  {
						Order = odObject//new Order { PaymentStatus = PaymentStatus.Pending, PaymentMethodSystemName = paymentMethodSystemName }
				  };     */

				var postProcessPaymentRequest = new PostProcessPaymentRequest();
				Order or = new Order();
				or = placeOrderResult.PlacedOrder;
				or.PaymentMethodSystemName = paymentMethodSystemName;
				or.PaymentStatus = PaymentStatus.Pending;
				or.OrderDiscount = placeOrderResult.PlacedOrder.OrderDiscount;
				or.OrderGuid = tempOrderObj.TempOrderGuid;
				or.Id = _tempOrderService.GetTempOrderByGuid(tempOrderObj.TempOrderGuid).Id;
				postProcessPaymentRequest.Order = or;
				//postProcessPaymentRequest.Cart = Orginalcart;
				// _orderProcessingService.ReOrder(or);




				_paymentService.PostProcessPayment(postProcessPaymentRequest);

				if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
				{
					//redirection or POST has been done in PostProcessPayment
					return Content("Redirected");
				}
				else
				{
					//if no redirection has been done (to a third-party payment page)
					//theoretically it's not possible
					return RedirectToRoute("CheckoutCompleted", new { orderId = 2 });
				}
			}
			catch (Exception exc)
			{
				_logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
				return Content(exc.Message);
			}
		}
	}
}