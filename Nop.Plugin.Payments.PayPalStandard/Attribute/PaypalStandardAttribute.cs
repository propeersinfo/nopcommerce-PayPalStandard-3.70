using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.PayPalStandard.Attribute
{
	class PaypalStandardAttribute : ActionFilterAttribute, IFilterProvider
	{

		public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		{
			if ((controllerContext.Controller.GetType().FullName.Equals("Nop.Web.Controllers.CheckoutController")) && actionDescriptor.ActionName.Equals("OpcSavePaymentInfo", StringComparison.InvariantCultureIgnoreCase))
			{
				return new List<Filter>() { new Filter(this, FilterScope.Action, 0) };
			}
			return new List<Filter>();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{

			if (filterContext == null || filterContext.HttpContext == null)
				return;
			HttpRequestBase request = filterContext.HttpContext.Request;
			if (request == null)
				return;

			string actionName = filterContext.ActionDescriptor.ActionName;
			if (String.IsNullOrEmpty(actionName))
				return;

			string controllerName = filterContext.Controller.ToString();
			if (String.IsNullOrEmpty(controllerName))
				return;

			//don't apply filter to child methods
			if (filterContext.IsChildAction)
				return;

			if ((controllerName.Equals("Nop.Web.Controllers.CheckoutController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("OpcSavePaymentInfo", StringComparison.InvariantCultureIgnoreCase)))
			{
				var form = filterContext.ActionParameters["form"] as FormCollection;
				var paymentMethodSystemName = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer.GetAttribute<string>(
						 SystemCustomerAttributeNames.SelectedPaymentMethod,
						 EngineContext.Current.Resolve<IGenericAttributeService>(), EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id);
				//var paymentMethod =EngineContext.Current.Resolve<IPaymentService>().LoadPaymentMethodBySystemName(paymentMethodSystemName);
				//var paymentControllerType = paymentMethod.GetControllerType();
				//var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as BasePaymentController;

				//if (paymentController == null)
				//	throw new Exception("Payment controller cannot be loaded");

				//var warnings = paymentController.ValidatePaymentForm(form);

				if(paymentMethodSystemName=="Payments.PayPalStandard")
				{
					var checkoutController = EngineContext.Current.Resolve<Nop.Plugin.Payments.PayPalStandard.Controllers.PaymentPayPalStandardController>();
					filterContext.Controller = checkoutController;
					
			  //	 var routeValues = new RouteValueDictionary {
			  //  { "Controller",checkoutController },
			  //  { "Action", "OpcSavePaymentInfo1"} 
			  //};
			  //	 routeValues.Add("FormCollection", form);

					filterContext.Result = checkoutController.OpcSavePaymentInfo1(form);
				}
			}

			base.OnActionExecuting(filterContext);
		}
	}
}
