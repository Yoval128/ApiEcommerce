using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models
{
    public class Product
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set}
        public string Description { get; set; }

        [Range(0, double.MaxValue)] // Ensure price is non-negative
        public decimal Price { get; set; }

        public string imgUrl { get; set; }

        [Required]
        public string SKU { get; set; } = string.Empty; // String.Empty to avoid null reference issues

        [Range(0, int.MaxValue)] // Ensure stock is non-negative]
        public int Stock { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow; // Default to current UTC time
        public DateTime? UpdateDate { get; set; } // Nullable to allow for products that haven't been updated yet

        //Relation with model Category
        public int CategoryId { get; set; }
        [ForeignKey] // Specify that this is a foreign key

    }
}
