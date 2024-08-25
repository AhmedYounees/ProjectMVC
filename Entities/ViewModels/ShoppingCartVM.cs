using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShopingCart> CartList { get; set; }
        public decimal TotalCarts{ get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
