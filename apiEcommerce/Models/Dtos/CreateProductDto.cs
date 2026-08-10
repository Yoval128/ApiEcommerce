namespace apiEcommerce.Models.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImgUrl { get; set; }

        public string? ImgUrlLocal { get; set; }
        public IFormFile? Image { get; set; } // Property for uploaded image file

        public string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int CategoryId { get; set; }

    }
}
