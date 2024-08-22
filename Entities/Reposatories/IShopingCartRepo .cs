using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Reposatories
{
    public interface IShopingCartRepo : IGenaricRepo<ShopingCartVM> 
    {
         void update(ShopingCartVM ShopingCart);
        public int IncreaseCount(ShopingCartVM shopinCart, int Count);
        public int DecreaseCount(ShopingCartVM shopinCart, int Count);
    }
}
