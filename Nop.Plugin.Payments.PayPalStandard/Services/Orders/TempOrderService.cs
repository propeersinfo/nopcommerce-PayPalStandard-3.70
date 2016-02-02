using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Payments.PayPalStandard.Domain.Orders;
using Nop.Services.Events;

//Added by Propeersinfo Team
namespace Nop.Plugin.Payments.PayPalStandard.Services.Orders
    {
    public partial class TempOrderService:ITempOrder
        {
        private readonly IRepository<TempOrder> _temporderRepository;
        private readonly IEventPublisher _eventPublisher;

        public TempOrderService(IRepository<TempOrder> temporderRepository,
            IEventPublisher eventPublisher)
        {
            this._temporderRepository = temporderRepository;
            this._eventPublisher = eventPublisher;
        }
        #region ITempOrder Members

        public void InsertTempOrder(TempOrder temporder)
            {
            if (temporder == null)
                throw new ArgumentNullException("temp order");

            _temporderRepository.Insert(temporder);

            //event notification
            _eventPublisher.EntityInserted(temporder);
            }

        public void UpdateTempOrder(TempOrder temporder)
            {
            if (temporder == null)
                throw new ArgumentNullException("temp order");

            _temporderRepository.Update(temporder);

            //event notification
            _eventPublisher.EntityUpdated(temporder);
            }

        public void DeleteTempOrder(TempOrder temporder)
            {
            if (temporder == null)
                throw new ArgumentNullException("temp order");

            temporder.Deleted = true;
            UpdateTempOrder(temporder);

            }

        public List<TempOrder> GetAllTempOrders()
        {
            var query = from o in _temporderRepository.Table
                            select  o;
            var temporder = query.ToList();
            return temporder;
        }

        public TempOrder GetTempOrderById(int id)
            {
            var query = from o in _temporderRepository.Table
                        where  o.Id==id
                        select o;

            var temporder = query.FirstOrDefault();
            return temporder;
            }

        public TempOrder GetTempOrderByGuid(Guid guid)
            {
            var query = from o in _temporderRepository.Table
                        where o.TempOrderGuid == guid
                        select o;

            var temporder = query.FirstOrDefault();
            return temporder;
            }

        #endregion
        }
    }
