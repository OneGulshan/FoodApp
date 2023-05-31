using AuthenticationPlugin;
using FoodApp.Data;
using FoodApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;//3 dependencies declared here and int also in ctor
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public AccountsController(ApplicationDbContext context,
            IConfiguration configuration, AuthService auth)
        {
            _context = context;
            _configuration = configuration;
            _auth = auth;
        }

        [HttpPost]
        [AllowAnonymous]//ye koi bhi access kar sakta hai by AllowAnonymous
        public async Task<IActionResult> Register(User user)//Account ki jagah yahan Register use kia hai with user as a parameter //or password ko # me algorithm me use kar ke yahan use kar rahe hai
        {
            var EmailAleadyExists = _context.Users.SingleOrDefault(//ise use kar ke ham chk kar sakte hai ki data already exist karta hai ke nahi karta hai //here we chk Email exist or not in database using lambda expression easily by using context
                x => x.Email == user.Email);
            if (EmailAleadyExists != null) return BadRequest("User's Email Already Exists");//if email already found then we send an badrequest that Email Already Exist
            var userObj = new User//if not exist then we can push data in database by using hash(using Authentication plugin jo hamne use me liya hai).
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),//here we converting our pass is hash Using Authentication Plugin that's we defined in our Startup Class //Hash is a method of SecurePasswordHasherHelper
                Role = "User"
            };
            _context.Users.Add(userObj);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(User email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email.Email);//jo data api url se aega usme se email chk ho jaegi
            if (user == null) return StatusCode(StatusCodes.Status404NotFound);
            var password = user.Password;
            if (!SecurePasswordHasherHelper.Verify(user.Password, password))//In Verify Password(source) and password(destination)
                return Unauthorized();//if not matched password
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),//ek array hai claims ka jisme hamne Email ka claim daal dia
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token =token.AccessToken,
                token_type=token.TokenType,
                user_Id=email.Id,
                user_name=email.Name,
                expires_in=token.ExpiresIn,
                creation_Time=token.ValidFrom,
                expiration_Time=token.ValidTo
            });
        }
    }
}
