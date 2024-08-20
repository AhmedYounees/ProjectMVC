using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public  class ShopingCartVM
    {
        public Product Product { get; set; }
        public int  Count { get; set; }
    }
}
