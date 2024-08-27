using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class review
    {

        public int id { get; set; }
        public string comment { get; set; }
        [Range(0,5)]
        public int Rate { get; set; }
        [ForeignKey("product")]
        public int productID { get; set; }
        public Product product { get; set; }
      
    }
}
