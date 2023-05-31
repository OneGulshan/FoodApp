using FoodApp.Data;
using FoodApp.Models;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace FoodApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Products);
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_context.Products.Find(id));
        }
        
        [HttpGet("[action]/{Categoryid}")]
        public IActionResult ProductsByCategoryId(int Categoryid)
        {
            var products = from x in _context.Products
                           where x.CategoryId == Categoryid
                           select new
                           {
                               Id = x.Id,
                               Title = x.Title,
                               Price = x.Price,
                               Description = x.Description,
                               CategoryId = x.CategoryId,
                               ImageUri = x.ImageUri,
                           };
            return Ok(products);
        }
        
        [HttpGet("[action]")]
        public IActionResult PopularProducts()
        {
            var products = from x in _context.Products
                           where x.IsPopular == true
                           select new
                           {
                               Id = x.Id,
                               Title = x.Title,
                               Price = x.Price,
                               ImageUri = x.ImageUri
                           };
            return Ok(products);
        }
        
        [Authorize(Roles = "Admin")]//Products sirf admin hi post kar sakta hai isliye hamne yahan Admin Authorize kara liya
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            var stream = new MemoryStream(product.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                product.ImageUri = file;
                _context.Products.Add(product);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }
        
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            var entity = _context.Products.Find(id);
            if(entity == null)
            {
                return NotFound("Product Not Found");
            }
            var stream = new MemoryStream(product.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                entity.CategoryId = product.CategoryId;//here entity means Product
                entity.Title = product.Title;
                entity.ImageUri = file;
                entity.Price = product.Price;
                entity.Description = product.Description;
                _context.SaveChanges();
                return Ok("Product Details Updated");                
            }
        }
        
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if(product == null)
            {
                return NotFound("Product Not Found");
            }
            else
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Ok("Product deleted");
            }
        }
    }
}
