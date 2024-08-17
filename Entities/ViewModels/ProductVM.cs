using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }
    }
}
