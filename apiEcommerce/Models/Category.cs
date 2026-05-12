using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models
{
    public class Category
    {
        [Key] // This attribute indicates that the Id property is the primary key of the Category entity
        public int Id { get; set; }
        [Required] // This attribute indicates that the Name property is required and cannot be null
        public string Name { get; set; } = string.Empty; // string.Empty is used to avoid null reference issues
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
