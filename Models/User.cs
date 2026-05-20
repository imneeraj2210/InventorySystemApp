using System.ComponentModel.DataAnnotations;

namespace InventorySystemApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(4,
           ErrorMessage = "Password must be at least 4 characters")]
        public string? Password { get; set; }
        public string? Role { get; set; } = "Customer";

        [Required(ErrorMessage ="Email is required!")]
        public string? Email { get; set; }
    }
}
