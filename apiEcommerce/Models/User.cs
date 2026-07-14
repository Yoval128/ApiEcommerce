using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        // The username property is required and cannot be null or empty
        public string Username { get; set; } = String.Empty;
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
