namespace FoodApp.Models
{
    public class OrderDetail//OrderDetail is related to Products and Orders
    {//Both of Combination of Navigations of Product and Order we can get details both of Tables in/for OrderDetail Class/Table
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }//navigate kar dia taaki agar Product par bhi information chaiye ho to get ki ja sake
        public int OrderId { get; set; }//konse order ki hamein details chaiye uske liye ye OrderId prop declared hai
        public Order Order { get; set; }
        public double OrderTotal { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
