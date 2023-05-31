using System;
using System.Collections.Generic;

namespace FoodApp.Models
{
    public class Order//here we have two types of Order, Order and Order detail, Order detail about products information and Order is declared here you can see that about User Information, means order placed or not and on what's date order are placed, userid etc. informations
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsCompleated { get; set; }//here checking order is compleated or not using bool prop IsCompleated
        public int UserId { get; set; }//a user have multiple orders and in Order has single User
        public ICollection<OrderDetail> OrderDetails { get; set; }//1 Order multiple orderdetails hold kar sakta hai kunki OrderDetail ke ander product hai hamare to yahan Product ki detail aaegi, Means ham yaha khe sakte hai ki ek single Order ke ander Multiple OrderDetail ho sakti hai
    }
}
