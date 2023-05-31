using FoodApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): 
            base(options)//DbContextOptions passing here for passing connection string here and Providing DbContext by using DbContextOptions
        {

        }//jitne bhi hamare models hai unhe yahan hamein int karna hai set karna hai/use karna hai
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
