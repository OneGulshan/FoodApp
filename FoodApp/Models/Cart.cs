namespace FoodApp.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }//In cart have mentioned ProductId then we mentioned here
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public int CoustomerId { get; set; }
    }
}
