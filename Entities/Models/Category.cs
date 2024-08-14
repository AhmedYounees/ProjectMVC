using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Category
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        public DateTime CreatedTime { get; set; }= DateTime.Now;
    }
}
