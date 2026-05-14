using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiEcommerce.Models
{ //Create the model Prodcut and relationship with model Category
    public class Product
    {
        [Required]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Fix the property Price 

        [Range(0, double.MaxValue)] // Ensure price is non-negative
        [Column(TypeName = "decimal(18,2)")] // Specify precision and scale for decimal)]
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
        [ForeignKey("CategoryId")] // Foreign key to Category model

        public required Category Category { get; set; } // Navigation property to Category

    }
}
