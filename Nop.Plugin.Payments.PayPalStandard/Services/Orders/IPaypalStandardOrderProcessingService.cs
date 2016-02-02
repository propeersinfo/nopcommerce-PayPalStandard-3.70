using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.PayPalStandard.Services.Orders
{
    /// <summary>
    /// Order processing service interface
    /// </summary>
    public partial interface IPaypalStandardOrderProcessingService
    {
        /// <summary>
        /// PaypalOrderDetails
        /// </summary>
        /// <param name="processPaymentRequest">PaypalOrderDetails</param>
        /// <returns>PaypalOrderDetails</returns>
		 PlaceOrderResult PaypalOrderDetails(ProcessPaymentRequest processPaymentRequest);
    }
}
