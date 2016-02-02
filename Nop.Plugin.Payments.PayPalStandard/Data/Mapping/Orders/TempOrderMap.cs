using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.PayPalStandard.Domain.Orders;

//Added by Propeersinfo Team
namespace Nop.Plugin.Payments.PayPalStandard.Data.Mapping.Orders
{
	public partial class TempOrderMap : NopEntityTypeConfiguration<TempOrder>
	{
		public TempOrderMap()
		{
			this.ToTable("TempOrder");
			this.HasKey(o => o.Id);
		}
	}
}
