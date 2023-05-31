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
    [Authorize]//new categories ko add karne ke liye hamko roles ko bhi add karna padega taaki sirf admin hi categories ko add kar sake or delete kar sake
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var categories = from a in _context.Categories//kabi-2 namespace cnt . se resolve nahi ho pata hai to hamein directly use karna padta hai -> using System.Linq;
                             select new
                             {
                                 Id = a.Id,
                                 Name = a.Title,
                                 ImageUri = a.ImageUri
                             };
            return Ok(categories);
        }
 
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = (from c in _context.Categories
                           where c.Id == id
                           select new
                           {
                               Id = c.Id,
                               Title = c.Title,
                               ImageUri = c.ImageUri
                           }).FirstOrDefault();
            return Ok(category);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            var stream = new MemoryStream(category.ImageArray);//ye ImageArray hamare database ke saath mapped nahi karega to ham in future ispar not mapped bhi likhenge //byte type ka array hota hai MemoryStream ka for Image Uploading
            var guid = Guid.NewGuid().ToString();//Guid for providing unique name to image file -> file(JPEG is actually a data format for compressed photos, rather than a file type)
            var file = $"{guid}.jpg";//file name is defined here using guid
            var folder = "wwwroot";//this is the folder of file
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                category.ImageUri = file;
                _context.Categories.Add(category);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Category category)
        {
            var entity = _context.Categories.Find(id);
            if (entity == null)
            {
                return NotFound("Category Not Found");
            }
            var stream = new MemoryStream(category.ImageArray);
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
                entity.Title = category.Title;
                entity.ImageUri = file;
                _context.SaveChanges();
                return Ok("Category Update");
            }
        }
        
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound("Category not Found");
            }
            else
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Ok("Category Deleted");
            }
        }
    }
}
