using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Core.Domain.Customers;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.PayPalStandard.Services.Orders
{
	/// <summary>
	/// Order processing service
	/// </summary>
	public partial class PaypalStandardOrderProcessingService : OrderProcessingService, IPaypalStandardOrderProcessingService
	{
		#region Fields

		private readonly IOrderService _orderService;
		private readonly IWebHelper _webHelper;
		private readonly ILocalizationService _localizationService;
		private readonly ILanguageService _languageService;
		private readonly IProductService _productService;
		private readonly IPaymentService _paymentService;
		private readonly ILogger _logger;
		private readonly IOrderTotalCalculationService _orderTotalCalculationService;
		private readonly IPriceCalculationService _priceCalculationService;
		private readonly IPriceFormatter _priceFormatter;
		private readonly IProductAttributeParser _productAttributeParser;
		private readonly IProductAttributeFormatter _productAttributeFormatter;
		private readonly IGiftCardService _giftCardService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
		private readonly IShippingService _shippingService;
		private readonly IShipmentService _shipmentService;
		private readonly ITaxService _taxService;
		private readonly ICustomerService _customerService;
		private readonly IDiscountService _discountService;
		private readonly IEncryptionService _encryptionService;
		private readonly IWorkContext _workContext;
		private readonly IWorkflowMessageService _workflowMessageService;
		private readonly IVendorService _vendorService;
		private readonly ICustomerActivityService _customerActivityService;
		private readonly ICurrencyService _currencyService;
		private readonly IAffiliateService _affiliateService;
		private readonly IEventPublisher _eventPublisher;
		private readonly IPdfService _pdfService;

		private readonly PaymentSettings _paymentSettings;
		private readonly RewardPointsSettings _rewardPointsSettings;
		private readonly OrderSettings _orderSettings;
		private readonly TaxSettings _taxSettings;
		private readonly LocalizationSettings _localizationSettings;
		private readonly CurrencySettings _currencySettings;
		private readonly IRewardPointService _rewardPointService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly ShippingSettings _shippingSettings;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="orderService">Order service</param>
		/// <param name="webHelper">Web helper</param>
		/// <param name="localizationService">Localization service</param>
		/// <param name="languageService">Language service</param>
		/// <param name="productService">Product service</param>
		/// <param name="paymentService">Payment service</param>
		/// <param name="logger">Logger</param>
		/// <param name="orderTotalCalculationService">Order total calculationservice</param>
		/// <param name="priceCalculationService">Price calculation service</param>
		/// <param name="priceFormatter">Price formatter</param>
		/// <param name="productAttributeParser">Product attribute parser</param>
		/// <param name="productAttributeFormatter">Product attribute formatter</param>
		/// <param name="giftCardService">Gift card service</param>
		/// <param name="shoppingCartService">Shopping cart service</param>
		/// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
		/// <param name="shippingService">Shipping service</param>
		/// <param name="shipmentService">Shipment service</param>
		/// <param name="taxService">Tax service</param>
		/// <param name="customerService">Customer service</param>
		/// <param name="discountService">Discount service</param>
		/// <param name="encryptionService">Encryption service</param>
		/// <param name="workContext">Work context</param>
		/// <param name="workflowMessageService">Workflow message service</param>
		/// <param name="vendorService">Vendor service</param>
		/// <param name="customerActivityService">Customer activity service</param>
		/// <param name="currencyService">Currency service</param>
		/// <param name="affiliateService">Affiliate service</param>
		/// <param name="eventPublisher">Event published</param>
		/// <param name="pdfService">PDF service</param>
		/// <param name="paymentSettings">Payment settings</param>
		/// <param name="rewardPointsSettings">Reward points settings</param>
		/// <param name="orderSettings">Order settings</param>
		/// <param name="taxSettings">Tax settings</param>
		/// <param name="localizationSettings">Localization settings</param>
		/// <param name="currencySettings">Currency settings</param>
		public PaypalStandardOrderProcessingService(IOrderService orderService,
			 IWebHelper webHelper,
			 ILocalizationService localizationService,
			 ILanguageService languageService,
			 IProductService productService,
			 IPaymentService paymentService,
			 ILogger logger,
			 IOrderTotalCalculationService orderTotalCalculationService,
			 IPriceCalculationService priceCalculationService,
			 IPriceFormatter priceFormatter,
			 IProductAttributeParser productAttributeParser,
			 IProductAttributeFormatter productAttributeFormatter,
			 IGiftCardService giftCardService,
			 IShoppingCartService shoppingCartService,
			 ICheckoutAttributeFormatter checkoutAttributeFormatter,
			 IShippingService shippingService,
			 IShipmentService shipmentService,
			 ITaxService taxService,
			 ICustomerService customerService,
			 IDiscountService discountService,
			 IEncryptionService encryptionService,
			 IWorkContext workContext,
			 IWorkflowMessageService workflowMessageService,
			 IVendorService vendorService,
			 ICustomerActivityService customerActivityService,
			 ICurrencyService currencyService,
			 IAffiliateService affiliateService,
			 IEventPublisher eventPublisher,
			 IPdfService pdfService,
			 PaymentSettings paymentSettings,
			 RewardPointsSettings rewardPointsSettings,
			 OrderSettings orderSettings,
			 TaxSettings taxSettings,
			 LocalizationSettings localizationSettings,
			 CurrencySettings currencySettings,
			 IRewardPointService rewardPointService,
			IGenericAttributeService genericAttributeService,
			ShippingSettings shippingSettings
			)
			: base(orderService,
			 webHelper,
			 localizationService,
			 languageService,
			 productService,
			 paymentService,
			 logger,
			 orderTotalCalculationService,
			 priceCalculationService,
			 priceFormatter,
			 productAttributeParser,
			 productAttributeFormatter,
			 giftCardService,
			 shoppingCartService,
			 checkoutAttributeFormatter,
			 shippingService,
			 shipmentService,
			 taxService,
			 customerService,
			 discountService,
			 encryptionService,
			 workContext,
			 workflowMessageService,
			 vendorService,
			 customerActivityService,
			 currencyService,
			 affiliateService,
			 eventPublisher,
			 pdfService,
			 rewardPointService,
			 genericAttributeService,
			 shippingSettings,
			 paymentSettings,
			 rewardPointsSettings,
			 orderSettings,
			 taxSettings,
			 localizationSettings,
			 currencySettings)
		{
			this._orderService = orderService;
			this._webHelper = webHelper;
			this._localizationService = localizationService;
			this._languageService = languageService;
			this._productService = productService;
			this._paymentService = paymentService;
			this._logger = logger;
			this._orderTotalCalculationService = orderTotalCalculationService;
			this._priceCalculationService = priceCalculationService;
			this._priceFormatter = priceFormatter;
			this._productAttributeParser = productAttributeParser;
			this._productAttributeFormatter = productAttributeFormatter;
			this._giftCardService = giftCardService;
			this._shoppingCartService = shoppingCartService;
			this._checkoutAttributeFormatter = checkoutAttributeFormatter;
			this._workContext = workContext;
			this._workflowMessageService = workflowMessageService;
			this._vendorService = vendorService;
			this._shippingService = shippingService;
			this._shipmentService = shipmentService;
			this._taxService = taxService;
			this._customerService = customerService;
			this._discountService = discountService;
			this._encryptionService = encryptionService;
			this._customerActivityService = customerActivityService;
			this._currencyService = currencyService;
			this._affiliateService = affiliateService;
			this._eventPublisher = eventPublisher;
			this._pdfService = pdfService;
			this._paymentSettings = paymentSettings;
			this._rewardPointsSettings = rewardPointsSettings;
			this._orderSettings = orderSettings;
			this._taxSettings = taxSettings;
			this._localizationSettings = localizationSettings;
			this._currencySettings = currencySettings;
			this._rewardPointService = rewardPointService;
		}

		#endregion


		public PlaceOrderResult PaypalOrderDetails(ProcessPaymentRequest processPaymentRequest)
		{
			//think about moving functionality of processing recurring orders (after the initial order was placed) to ProcessNextRecurringPayment() method
			if (processPaymentRequest == null)
				throw new ArgumentNullException("processPaymentRequest");



			var result = new PlaceOrderResult();
			try
			{
				//if (processPaymentRequest.OrderGuid == Guid.Empty)
				//	processPaymentRequest.OrderGuid = Guid.NewGuid();

				//prepare order details
				var details = PreparePlaceOrderDetails(processPaymentRequest);

				#region Payment workflow


				//process payment
				ProcessPaymentResult processPaymentResult = null;
				//skip payment workflow if order total equals zero
				var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
				if (!skipPaymentWorkflow)
				{
					var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(processPaymentRequest.PaymentMethodSystemName);
					if (paymentMethod == null)
						throw new NopException("Payment method couldn't be loaded");

					//ensure that payment method is active
					if (!paymentMethod.IsPaymentMethodActive(_paymentSettings))
						throw new NopException("Payment method is not active");

					if (!processPaymentRequest.IsRecurringPayment)
					{
						if (details.IsRecurringShoppingCart)
						{
							//recurring cart
							var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
							switch (recurringPaymentType)
							{
								case RecurringPaymentType.NotSupported:
									throw new NopException("Recurring payments are not supported by selected payment method");
								case RecurringPaymentType.Manual:
								case RecurringPaymentType.Automatic:
									processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
									break;
								default:
									throw new NopException("Not supported recurring payment type");
							}
						}
						else
						{
							//standard cart
							processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
						}
					}
					else
					{
						if (details.IsRecurringShoppingCart)
						{
							//Old credit card info
							processPaymentRequest.CreditCardType = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardType) : "";
							processPaymentRequest.CreditCardName = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardName) : "";
							processPaymentRequest.CreditCardNumber = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardNumber) : "";
							processPaymentRequest.CreditCardCvv2 = details.InitialOrder.AllowStoringCreditCardNumber ? _encryptionService.DecryptText(details.InitialOrder.CardCvv2) : "";
							try
							{
								processPaymentRequest.CreditCardExpireMonth = details.InitialOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationMonth)) : 0;
								processPaymentRequest.CreditCardExpireYear = details.InitialOrder.AllowStoringCreditCardNumber ? Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationYear)) : 0;
							}
							catch { }

							var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
							switch (recurringPaymentType)
							{
								case RecurringPaymentType.NotSupported:
									throw new NopException("Recurring payments are not supported by selected payment method");
								case RecurringPaymentType.Manual:
									processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
									break;
								case RecurringPaymentType.Automatic:
									//payment is processed on payment gateway site
									processPaymentResult = new ProcessPaymentResult();
									break;
								default:
									throw new NopException("Not supported recurring payment type");
							}
						}
						else
						{
							throw new NopException("No recurring products");
						}
					}
				}
				else
				{
					//payment is not required
					if (processPaymentResult == null)
						processPaymentResult = new ProcessPaymentResult();
					processPaymentResult.NewPaymentStatus = PaymentStatus.Paid;
				}

				if (processPaymentResult == null)
					throw new NopException("processPaymentResult is not available");

				#endregion

				if (processPaymentResult.Success)
				{
					#region Save order details

					var order = new Order
					{
						StoreId = processPaymentRequest.StoreId,
						OrderGuid = processPaymentRequest.OrderGuid,
						CustomerId = details.Customer.Id,
						CustomerLanguageId = details.CustomerLanguage.Id,
						CustomerTaxDisplayType = details.CustomerTaxDisplayType,
						CustomerIp = _webHelper.GetCurrentIpAddress(),
						OrderSubtotalInclTax = details.OrderSubTotalInclTax,
						OrderSubtotalExclTax = details.OrderSubTotalExclTax,
						OrderSubTotalDiscountInclTax = details.OrderSubTotalDiscountInclTax,
						OrderSubTotalDiscountExclTax = details.OrderSubTotalDiscountExclTax,
						OrderShippingInclTax = details.OrderShippingTotalInclTax,
						OrderShippingExclTax = details.OrderShippingTotalExclTax,
						PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
						PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
						TaxRates = details.TaxRates,
						OrderTax = details.OrderTaxTotal,
						OrderTotal = details.OrderTotal,
						RefundedAmount = decimal.Zero,
						OrderDiscount = details.OrderDiscountAmount,
						CheckoutAttributeDescription = details.CheckoutAttributeDescription,
						CheckoutAttributesXml = details.CheckoutAttributesXml,
						CustomerCurrencyCode = details.CustomerCurrencyCode,
						CurrencyRate = details.CustomerCurrencyRate,
						AffiliateId = details.AffiliateId,
						OrderStatus = OrderStatus.Pending,
						AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
						CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
						CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
						CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
						MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
						CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
						CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
						CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
						PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
						AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
						AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
						AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
						CaptureTransactionId = processPaymentResult.CaptureTransactionId,
						CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
						SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
						PaymentStatus = processPaymentResult.NewPaymentStatus,
						PaidDateUtc = null,
						BillingAddress = details.BillingAddress,
						ShippingAddress = details.ShippingAddress,
						ShippingStatus = details.ShippingStatus,
						ShippingMethod = details.ShippingMethodName,
						PickUpInStore = details.PickUpInStore,
						ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
						CustomValuesXml = processPaymentRequest.SerializeCustomValues(),
						VatNumber = details.VatNumber,
						CreatedOnUtc = DateTime.UtcNow
					};
					//_orderService.InsertOrder(order);

					result.PlacedOrder = order;

					if (!processPaymentRequest.IsRecurringPayment)
					{
						//move shopping cart items to order items
						foreach (var sc in details.Cart)
						{
							//prices
							decimal taxRate;
							Discount scDiscount;
							decimal discountAmount;
							decimal scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
							decimal scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out discountAmount, out scDiscount);
							decimal scUnitPriceInclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out taxRate);
							decimal scUnitPriceExclTax = _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out taxRate);
							decimal scSubTotalInclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out taxRate);
							decimal scSubTotalExclTax = _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out taxRate);

							decimal discountAmountInclTax = _taxService.GetProductPrice(sc.Product, discountAmount, true, details.Customer, out taxRate);
							decimal discountAmountExclTax = _taxService.GetProductPrice(sc.Product, discountAmount, false, details.Customer, out taxRate);
							if (scDiscount != null && !details.AppliedDiscounts.ContainsDiscount(scDiscount))
								details.AppliedDiscounts.Add(scDiscount);

							//attributes
							string attributeDescription = _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

							var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);

							//save order item
							var orderItem = new OrderItem
							{
								OrderItemGuid = Guid.NewGuid(),
								Order = order,
								ProductId = sc.ProductId,
								UnitPriceInclTax = scUnitPriceInclTax,
								UnitPriceExclTax = scUnitPriceExclTax,
								PriceInclTax = scSubTotalInclTax,
								PriceExclTax = scSubTotalExclTax,
								OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
								AttributeDescription = attributeDescription,
								AttributesXml = sc.AttributesXml,
								Quantity = sc.Quantity,
								DiscountAmountInclTax = discountAmountInclTax,
								DiscountAmountExclTax = discountAmountExclTax,
								DownloadCount = 0,
								IsDownloadActivated = false,
								LicenseDownloadId = 0,
								ItemWeight = itemWeight,
								RentalStartDateUtc = sc.RentalStartDateUtc,
								RentalEndDateUtc = sc.RentalEndDateUtc
							};
							order.OrderItems.Add(orderItem);
							//_orderService.UpdateOrder(order);

							//gift cards
							if (sc.Product.IsGiftCard)
							{
								string giftCardRecipientName, giftCardRecipientEmail,
									 giftCardSenderName, giftCardSenderEmail, giftCardMessage;
								_productAttributeParser.GetGiftCardAttribute(sc.AttributesXml,
									 out giftCardRecipientName, out giftCardRecipientEmail,
									 out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

								for (int i = 0; i < sc.Quantity; i++)
								{
									var gc = new GiftCard
									{
										GiftCardType = sc.Product.GiftCardType,
										PurchasedWithOrderItem = orderItem,
										Amount = sc.Product.OverriddenGiftCardAmount.HasValue ? sc.Product.OverriddenGiftCardAmount.Value : scUnitPriceExclTax,
										IsGiftCardActivated = false,
										GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
										RecipientName = giftCardRecipientName,
										RecipientEmail = giftCardRecipientEmail,
										SenderName = giftCardSenderName,
										SenderEmail = giftCardSenderEmail,
										Message = giftCardMessage,
										IsRecipientNotified = false,
										CreatedOnUtc = DateTime.UtcNow
									};
									// _giftCardService.InsertGiftCard(gc);
								}
							}

							//inventory
							_productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml);
						}

						//clear shopping cart
						//details.Cart.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
					}
					else
					{
						//recurring payment
						var initialOrderItems = details.InitialOrder.OrderItems;
						foreach (var orderItem in initialOrderItems)
						{
							//save item
							var newOrderItem = new OrderItem
							{
								OrderItemGuid = Guid.NewGuid(),
								Order = order,
								ProductId = orderItem.ProductId,
								UnitPriceInclTax = orderItem.UnitPriceInclTax,
								UnitPriceExclTax = orderItem.UnitPriceExclTax,
								PriceInclTax = orderItem.PriceInclTax,
								PriceExclTax = orderItem.PriceExclTax,
								OriginalProductCost = orderItem.OriginalProductCost,
								AttributeDescription = orderItem.AttributeDescription,
								AttributesXml = orderItem.AttributesXml,
								Quantity = orderItem.Quantity,
								DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
								DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
								DownloadCount = 0,
								IsDownloadActivated = false,
								LicenseDownloadId = 0,
								ItemWeight = orderItem.ItemWeight,
								RentalStartDateUtc = orderItem.RentalStartDateUtc,
								RentalEndDateUtc = orderItem.RentalEndDateUtc
							};
							order.OrderItems.Add(newOrderItem);
							//_orderService.UpdateOrder(order);

							//gift cards
							if (orderItem.Product.IsGiftCard)
							{
								string giftCardRecipientName, giftCardRecipientEmail,
									 giftCardSenderName, giftCardSenderEmail, giftCardMessage;
								_productAttributeParser.GetGiftCardAttribute(orderItem.AttributesXml,
									 out giftCardRecipientName, out giftCardRecipientEmail,
									 out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

								for (int i = 0; i < orderItem.Quantity; i++)
								{
									var gc = new GiftCard
									{
										GiftCardType = orderItem.Product.GiftCardType,
										PurchasedWithOrderItem = newOrderItem,
										Amount = orderItem.UnitPriceExclTax,
										IsGiftCardActivated = false,
										GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
										RecipientName = giftCardRecipientName,
										RecipientEmail = giftCardRecipientEmail,
										SenderName = giftCardSenderName,
										SenderEmail = giftCardSenderEmail,
										Message = giftCardMessage,
										IsRecipientNotified = false,
										CreatedOnUtc = DateTime.UtcNow
									};
									//_giftCardService.InsertGiftCard(gc);
								}
							}

							//inventory
							_productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml);
						}
					}

					//discount usage history
					if (!processPaymentRequest.IsRecurringPayment)
						foreach (var discount in details.AppliedDiscounts)
						{
							var duh = new DiscountUsageHistory
							{
								Discount = discount,
								Order = order,
								CreatedOnUtc = DateTime.UtcNow
							};
							//_discountService.InsertDiscountUsageHistory(duh);
						}

					//gift card usage history
					if (!processPaymentRequest.IsRecurringPayment)
						if (details.AppliedGiftCards != null)
							foreach (var agc in details.AppliedGiftCards)
							{
								decimal amountUsed = agc.AmountCanBeUsed;
								var gcuh = new GiftCardUsageHistory
								{
									GiftCard = agc.GiftCard,
									UsedWithOrder = order,
									UsedValue = amountUsed,
									CreatedOnUtc = DateTime.UtcNow
								};
								agc.GiftCard.GiftCardUsageHistory.Add(gcuh);
								//_giftCardService.UpdateGiftCard(agc.GiftCard);
							}

					//reward points history
					if (details.RedeemedRewardPointsAmount > decimal.Zero)
					{
						_rewardPointService.AddRewardPointsHistoryEntry(details.Customer,
							 -details.RedeemedRewardPoints, order.StoreId,
							 string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.Id),
							 order, details.RedeemedRewardPointsAmount);
						//_customerService.UpdateCustomer(details.Customer);
					}

					//recurring orders
					if (!processPaymentRequest.IsRecurringPayment && details.IsRecurringShoppingCart)
					{
						//create recurring payment (the first payment)
						var rp = new RecurringPayment
						{
							CycleLength = processPaymentRequest.RecurringCycleLength,
							CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
							TotalCycles = processPaymentRequest.RecurringTotalCycles,
							StartDateUtc = DateTime.UtcNow,
							IsActive = true,
							CreatedOnUtc = DateTime.UtcNow,
							InitialOrder = order,
						};
						//_orderService.InsertRecurringPayment(rp);


						var recurringPaymentType = _paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName);
						switch (recurringPaymentType)
						{
							case RecurringPaymentType.NotSupported:
								{
									//not supported
								}
								break;
							case RecurringPaymentType.Manual:
								{
									//first payment
									var rph = new RecurringPaymentHistory
									{
										RecurringPayment = rp,
										CreatedOnUtc = DateTime.UtcNow,
										OrderId = order.Id,
									};
									rp.RecurringPaymentHistory.Add(rph);
									_orderService.UpdateRecurringPayment(rp);
								}
								break;
							case RecurringPaymentType.Automatic:
								{
									//will be created later (process is automated)
								}
								break;
							default:
								break;
						}
					}

					#endregion

					#region Notifications & notes

					//notes, messages
					if (_workContext.OriginalCustomerIfImpersonated != null)
					{
						//this order is placed by a store administrator impersonating a customer
						order.OrderNotes.Add(new OrderNote
						{
							Note = string.Format("Order placed by a store owner ('{0}'. ID = {1}) impersonating the customer.",
								 _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
							DisplayToCustomer = false,
							CreatedOnUtc = DateTime.UtcNow
						});
						//_orderService.UpdateOrder(order);
					}
					else
					{
						order.OrderNotes.Add(new OrderNote
						{
							Note = "Order placed",
							DisplayToCustomer = false,
							CreatedOnUtc = DateTime.UtcNow
						});
						//_orderService.UpdateOrder(order);
					}


					//send email notifications
					//int orderPlacedStoreOwnerNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
					//if (orderPlacedStoreOwnerNotificationQueuedEmailId > 0)
					//{
					//	order.OrderNotes.Add(new OrderNote
					//	{
					//		Note = string.Format("\"Order placed\" email (to store owner) has been queued. Queued email identifier: {0}.", orderPlacedStoreOwnerNotificationQueuedEmailId),
					//		DisplayToCustomer = false,
					//		CreatedOnUtc = DateTime.UtcNow
					//	});
					//	//_orderService.UpdateOrder(order);
					//}

					var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
						 _pdfService.PrintOrderToPdf(order, order.CustomerLanguageId) : null;
					//var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
					//	 "order.pdf" : null;
					//int orderPlacedCustomerNotificationQueuedEmailId = _workflowMessageService
					//	 .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
					//if (orderPlacedCustomerNotificationQueuedEmailId > 0)
					//{
					//	order.OrderNotes.Add(new OrderNote
					//	{
					//		Note = string.Format("\"Order placed\" email (to customer) has been queued. Queued email identifier: {0}.", orderPlacedCustomerNotificationQueuedEmailId),
					//		DisplayToCustomer = false,
					//		CreatedOnUtc = DateTime.UtcNow
					//	});
					//	//_orderService.UpdateOrder(order);
					//}

					//var vendors = GetVendorsInOrder(order);
					//foreach (var vendor in vendors)
					//{
					//	int orderPlacedVendorNotificationQueuedEmailId = _workflowMessageService.SendOrderPlacedVendorNotification(order, vendor, order.CustomerLanguageId);
					//	if (orderPlacedVendorNotificationQueuedEmailId > 0)
					//	{
					//		order.OrderNotes.Add(new OrderNote
					//		{
					//			Note = string.Format("\"Order placed\" email (to vendor) has been queued. Queued email identifier: {0}.", orderPlacedVendorNotificationQueuedEmailId),
					//			DisplayToCustomer = false,
					//			CreatedOnUtc = DateTime.UtcNow
					//		});
					//		//_orderService.UpdateOrder(order);
					//	}
					//}

					//check order status
					CheckOrderStatus(order);

					//reset checkout data
					//if (!processPaymentRequest.IsRecurringPayment)
					//	_customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);

					//if (!processPaymentRequest.IsRecurringPayment)
					//{
					//	_customerActivityService.InsertActivity(
					//		 "PublicStore.PlaceOrder",
					//		 _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"),
					//		 order.Id);
					//}

					//raise event       
					//_eventPublisher.Publish(new OrderPlacedEvent(order));

					if (order.PaymentStatus == PaymentStatus.Paid)
					{
						ProcessOrderPaid(order);
					}
					#endregion
				}
				else
				{
					//payment errors
					foreach (var paymentError in processPaymentResult.Errors)
						result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
				}
			}
			catch (Exception exc)
			{
				_logger.Error(exc.Message, exc);
				result.AddError(exc.Message);
			}

			#region Process errors

			string error = "";
			for (int i = 0; i < result.Errors.Count; i++)
			{
				error += string.Format("Error {0}: {1}", i + 1, result.Errors[i]);
				if (i != result.Errors.Count - 1)
					error += ". ";
			}
			if (!String.IsNullOrEmpty(error))
			{
				//log it
				string logError = string.Format("Error while placing order. {0}", error);
				var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
				_logger.Error(logError, customer: customer);
			}

			#endregion

			return result;
		}




	}
}
