using DataAccessLayer.Data;
using Entities.Models;
using Entities.Reposatories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class OredrHeaderRepo : GenaricRepo<OrderHeader>,IOrderHeaderRepo
    {
        private readonly ApplicationDbContext _context;
        public OredrHeaderRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

      

        public void updateOrderStates(int id, string OrderStates, string PaymentStates)
        {
            var orderFromDB = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDB != null) {
            
                orderFromDB.OrderStatus = OrderStates;
                orderFromDB.OrderDate = DateTime.Now;
                if (PaymentStates != null)
                {

                    orderFromDB.PaymentSatuts = PaymentStates;

                }
            
            }
        }
    }
}
