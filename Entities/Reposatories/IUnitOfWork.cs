using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Reposatories
{
    public interface IUnitOfWork:IDisposable
    {
        ICategoryRepo Category { get; }
        IProductRepo Product { get; }
        IShopingCartRepo ShoppingCart { get; }

        IOrderHeaderRepo OrderHeader { get; }
        IOrderDetailRepo OrderDetail { get; }
        IApplicationUserRepo ApplicationUser { get; }
       

        int complete();
    }
}
