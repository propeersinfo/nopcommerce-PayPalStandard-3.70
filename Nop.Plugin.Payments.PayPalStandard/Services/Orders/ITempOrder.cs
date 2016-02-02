using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Payments.PayPalStandard.Domain.Orders;

//Added by Propeersinfo Team
namespace Nop.Plugin.Payments.PayPalStandard.Services.Orders
    {
         public partial interface ITempOrder
            {
             void InsertTempOrder(TempOrder temporder);
             void UpdateTempOrder(TempOrder temporder);
             void DeleteTempOrder(TempOrder temporder);

             List<TempOrder> GetAllTempOrders();
             TempOrder GetTempOrderById(int id);
             TempOrder GetTempOrderByGuid(Guid guid);

            }
    }
