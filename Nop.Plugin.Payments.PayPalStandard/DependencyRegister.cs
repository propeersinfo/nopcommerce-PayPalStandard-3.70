using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.PayPalStandard.Attribute;
using Nop.Plugin.Payments.PayPalStandard.Data;
using Nop.Plugin.Payments.PayPalStandard.Domain.Orders;
using Nop.Plugin.Payments.PayPalStandard.Services.Orders;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalStandard
{
	class DependencyRegistrar : IDependencyRegistrar
	{
		/// <summary>
		/// Register services and interfaces
		/// </summary>
		/// <param name="builder">Container builder</param>
		/// <param name="typeFinder">Type finder</param>
		/// <param name="config">Config</param>
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			builder.RegisterType<PaypalStandardAttribute>().As<IFilterProvider>();
			builder.RegisterType<PaypalStandardOrderProcessingService>().As<IPaypalStandardOrderProcessingService>().InstancePerLifetimeScope();
			builder.RegisterType<TempOrderService>().As<ITempOrder>().InstancePerLifetimeScope();

			//data context
			this.RegisterPluginDataContext<PaypalStandardObjectContext>(builder, "nop_object_context_payment_paypalstandard");

			//override required repository with our custom context
			builder.RegisterType<EfRepository<TempOrder>>()
				 .As<IRepository<TempOrder>>()
				 .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_payment_paypalstandard"))
				 .InstancePerLifetimeScope();
		}

		/// <summary>
		/// Order of this dependency registrar implementation
		/// </summary>
		public int Order
		{
			get { return 1; }
		}

	}
}
