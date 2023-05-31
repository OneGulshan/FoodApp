using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FoodApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            order.IsCompleated = false;
            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();


            var cartItems = _context.CartItems.Where
                (x => x.CoustomerId == order.UserId);
            foreach (var item in cartItems)
            {
                var orderDetails = new OrderDetail()
                {
                    Price = item.Price,
                    OrderTotal = item.TotalAmount,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    OrderId = order.Id
                };
                _context.OrderDetails.Add(orderDetails);
            }
            _context.SaveChanges();
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
            return Ok(new { OrderId = order.Id });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult PendingOrders()
        {
            var orders = _context.Orders.Where(x => x.IsCompleated == false);//IsCompleated == false orders are bydefault uncompleate
            return Ok(orders);//ye orders pendings hai ye hamne return kar die
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult CompleatedOrders()
        {
            var orders = _context.Orders.Where(x => x.IsCompleated == true);
            return Ok(orders);
        }
        
        [HttpGet("[action]/{orderId}")]//Order/OrderHeader is Consumer information and OrderDetail is Product information
        public IActionResult OrderDetails(int orderId)//ham yahan orderId wise OrderDetails findout kar rahe hain
        {
            var orders = _context.Orders.Where(x => x.Id == orderId)
                .Include(y => y.OrderDetails).ThenInclude(z => z.Product);//where laga kar ham Order ki puri ki puri info nikal sakte hai or include laga kar OrderDetail findout kar sakte hai or even ThenInclude laga kar pure product ki information bhi findout kar sakte hai, using lazyloding in ef core using Include and ThenInclude
            return Ok(orders);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult TotalOrders()
        {
            var orders = (from order in _context.Orders
                          where order.IsCompleated == false
                          select order.IsCompleated).Count();//Pending orders gotted
            return Ok(new { PendingOrders = orders });
        }
        
        [HttpGet("[action]/{id}")]
        public IActionResult OrdersByUser(int UserId)//Orders userid wize get karne ke liye //Ajj kal to amazon me phone no. wize hi orders get ho jate hai or emailid wize, 1-1 module wize hi kaam hota hai
        {
            var orders = _context.Orders.Where(x => x.UserId == UserId).OrderByDescending(x => x.OrderDate);//OrderByDescending <- OrderDate wize Orders dikhane ke liye linq Sorting Operator is OrderByDescending
            return Ok(orders);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("[action]/{orderId}")]
        public IActionResult MarkOrderCompleated(int orderId, [FromBody] Order order)//pending orders ko mark karne ke liye method orderId wize, FromBody <- using API Formater
        {
            var orderfromDatabase = _context.Orders.Find(orderId);//jo database se orders aenge unhe mark kar rahe hai
            if(orderfromDatabase == null)
            {
                return NotFound("Not Found");
            }
            else
            {
                orderfromDatabase.IsCompleated = order.IsCompleated;//jo bhi order ka status aaega vo isme chala jaega ki Order Compleate hua ya nahi hua
                _context.SaveChanges();//isse order ka status change ho jaega, mark ho jaega ki vo order Compleate ho chuka hai
                return Ok("Order Compleated");
            }
        }
    }
}
