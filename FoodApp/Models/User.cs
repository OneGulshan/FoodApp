using System.Collections.Generic;

namespace FoodApp.Models
{
    public class User//User is a temporary Class using Role we can handle User Class using Employee/Admin/Coustomer or diff-2 time, on diff tome diff Persons use this User Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public ICollection<Order> Orders { get; set; }//1 user 1 se zyada orders kar sakta hai uske liye ye relationsip hamne declare kari hai
    }
}
