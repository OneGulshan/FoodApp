using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FoodApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]//userId wise ham cart ki information get karenge
        public IActionResult Get(int userId)
        {
            var user = _context.CartItems.Where(x => x.CoustomerId == userId);//yahan user ka matlab kart item me CoustomerId se hota hai //hamein yahan vo cartitems chaiye actually me jo UserId se match karta hoga, yahan 1 user ki detail hamare paas aa jaegi, here CartItem means a User
            if(user == null)
            {
                return NotFound();
            }
            var cartItems = from a in _context.CartItems.Where(
                x => x.CoustomerId == userId)//x=>x.CustomerId <- x tence(=>) to CustomerId
                            join b in _context.Products on a.ProductId equals b.Id//yahan ham products ke saath cartitems join kar rahe hai
                            select new
                            {
                                Id = a.Id,
                                Price = a.Price,
                                TotalAmount = a.TotalAmount,
                                Quantity = a.Quantity,
                                ProductName = b.Title
                            };
            return Ok(cartItems);//yahan hamne Product ki detail wise saare cartitems nikal liye hai jo userid wise match karte honge
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] Cart carts)//Cart me data ko insert karne ke liye method, jo cartitem me phele se data insert hai to uski quantity increase honi chaiye so ham log wahi chize kar rahe hain
        {
            var cart = _context.CartItems.FirstOrDefault(x=>x.ProductId
            ==carts.ProductId && x.CoustomerId==carts.CoustomerId);//ProductId carts.ProductId and UserId dono se match karni chahiye, isse customer ki sari details hamare pass aa jaegi
            if (cart != null)
            {
                cart.Quantity += cart.Quantity;//isse main Quentity database me update ho jaegi Cart ki
                cart.TotalAmount = cart.Price * cart.Quantity;//TotalAmount = price*quentity
            }
            else//agar cart hamara null hoga to yahan 1 new Shopping cart ban(add ho jaega) jaega with items
            {
                var shoppingCart = new Cart()
                {//phele se hamare pass cart ki agar details hongi to ham isme increment kar lenge and total amount calculate kar lenge or agar phele se nahi hai carts me user ki koi details to vo item hamein cart ke ander dalna hai to ham yahan wahi cheez kar rahe hain
                    CoustomerId = cart.CoustomerId,
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                    TotalAmount = cart.TotalAmount
                };
                _context.CartItems.Add(shoppingCart);
            }
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        //here we know how to get total items userby
        [HttpGet("[action]/{userId}")]
        public IActionResult TotalItems(int userId)
        {
            var cartItems = (from cart in _context.CartItems
                             where cart.CoustomerId == userId
                             select cart.Quantity).Sum();//Sum means Total
            return Ok(new { TotalItems = cartItems });
        }
        
        [HttpGet("[action]/{userId}")]
        public IActionResult TotalAmount(int userId)
        {
            var totalAmount = (from cart in _context.CartItems
                             where cart.CoustomerId == userId
                             select cart.TotalAmount).Sum();//Sum means Total
            return Ok(new { TotalAmount = totalAmount });
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            var cart = _context.CartItems.Where(x => x.CoustomerId == userId);
            _context.CartItems.RemoveRange(cart);//RemoveRange user because we get list for Removing
            _context.SaveChanges();
            return Ok();
        }
    }
}
