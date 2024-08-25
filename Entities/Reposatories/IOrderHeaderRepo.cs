using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Reposatories
{
    public interface IOrderHeaderRepo : IGenaricRepo<OrderHeader> 
    {
        void update(OrderHeader orderHeader);

        void updateOrderStates(int id , string OrderStates,string PaymentStates);

    }
}
