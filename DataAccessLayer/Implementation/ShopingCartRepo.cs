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
    public class ShoppingCartRepo : GenaricRepo<ShopingCart>, IShopingCartRepo
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecreaseCount(ShopingCart shopinCart, int Count)
        {
            shopinCart.Count-= Count;
            return shopinCart.Count;
        }

        public int IncreaseCount(ShopingCart shopinCart, int Count)
        {
            shopinCart.Count += Count;
            return shopinCart.Count;
        }

        public void update(ShopingCart ShopingCart)
        {
            var ShopingCartFromDatabase = _context.shopingCarts.FirstOrDefault(x => x.ID == ShopingCart.ID);
            if (ShopingCartFromDatabase != null)
            {
                ShopingCartFromDatabase.Count = ShopingCart.Count;

            }

            //public void update(Category category)
            //{
            //    var categoryinDB=_context.categories.FirstOrDefault(x=>x.id==category.id);
            //    if (categoryinDB != null) { 
            //    categoryinDB.name = category.name;
            //    categoryinDB.description = category.description;
            //    categoryinDB.CreatedTime=DateTime.Now;
            //    }
            //}
        }
    }
}
