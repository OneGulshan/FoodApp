using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUri { get; set; }
        [NotMapped]
        public byte[] ImageArray { get; set; }
        public ICollection<Product> Products { get; set; }//here we defined/declared that in one Category can multiple products ex. our Category is fastfood than in my category has pizza, burger etc.
    }
}
