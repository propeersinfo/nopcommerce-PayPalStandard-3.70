using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PayPalStandard
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //PDT
            routes.MapRoute("Plugin.Payments.PayPalStandard.PDTHandler",
                 "Plugins/PaymentPayPalStandard/PDTHandler",
                 new { controller = "PaymentPayPalStandard", action = "PDTHandler" },
                 new[] { "Nop.Plugin.Payments.PayPalStandard.Controllers" }
            );
            //IPN
            routes.MapRoute("Plugin.Payments.PayPalStandard.IPNHandler",
                 "Plugins/PaymentPayPalStandard/IPNHandler",
                 new { controller = "PaymentPayPalStandard", action = "IPNHandler" },
                 new[] { "Nop.Plugin.Payments.PayPalStandard.Controllers" }
            );
            //Cancel
            routes.MapRoute("Plugin.Payments.PayPalStandard.CancelOrder",
                 "Plugins/PaymentPayPalStandard/CancelOrder",
                 new { controller = "PaymentPayPalStandard", action = "CancelOrder" },
                 new[] { "Nop.Plugin.Payments.PayPalStandard.Controllers" }
            );

			  //Customizaton Peopeers info (changed by ravi)
				//var route = routes.MapRoute("OpcSavePaymentInfo",
				//		"checkout/OpcSavePaymentInfo",
				//		new { controller = "checkout", action = "OpcSavePaymentInfo" }
				// );
				//routes.Remove(route);
				//routes.MapRoute("Plugin.Payments.PayPalStandard.OpcCompleteRedirectionPayment1",
				//	  "checkout/OpcSavePaymentInfo",
				//	  new { controller = "PaymentPayPalStandard", action = "OpcCompleteRedirectionPayment1" },
				//	  new[] { "Nop.Plugin.Payments.PayPalStandard.Controllers" }
				//);
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
