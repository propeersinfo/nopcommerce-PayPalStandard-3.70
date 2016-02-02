using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

//Added by Propeersinfo Team
namespace Nop.Plugin.Payments.PayPalStandard.Domain.Orders
 {
    public partial class TempOrder : BaseEntity
     {
         public Guid TempOrderGuid { get; set; }
         public int OrderId { get; set; }
         public string PaypalTransactionId { get; set; }
         public bool? Deleted { get; set; }
     }
 }
