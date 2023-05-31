using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApp.Models
{
    public class Product//1 category ke ander multiple projects aa sakte hai
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public double Price { get; set; }
        public bool IsPopular { get; set; }//konsa product popular hai janne ke liye by bool prop
        public int CategoryId { get; set; }//1 Product ke ander single category hi hogi/ho sakti hai to yahan many to one ka relationship ban rha hai
        [NotMapped]//means database me ye fiel nahi banegi by NotMapped
        public byte[] ImageArray { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }//1 Product ke ander multiple OrderDetail ho sakti hai, iske liye hamein not mapped karna hoga/Json ignore bhi ham log kar sakte hai ye ham latter videos me disscuss karenge jo Data Notations hai vo ham log baad me disscuss ka lenge
        public ICollection<Cart> CartItems { get; set; }//1 product ke ander multiple cart items ho sakte hai
    }
}
