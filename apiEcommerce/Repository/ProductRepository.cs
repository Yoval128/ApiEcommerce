using apiEcommerce.Models;
using apiEcommerce.Repository.IRepository;

namespace apiEcommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        // Declare a private read-only field of type
        // ApplicationDbContext to interact with the
        // database private readonly A
        // pplicationDbContext _db;

        private readonly ApplicationDbContext _db;

        //Constructor that initializes the database context for the repository
        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool BuyProduct(String name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return false;
            }
            //First, indentify the product withaout spaces and lowercase is the same as the name parameter
            var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());

            if (product == null || product.Stock < quantity)
            {
                return false;
            }

            // Update the Stock in the database
            product.Stock -= quantity; // This is the same that product.Stock = Stock - quantity

            // 
            _db.Products.Update(product);

            return Save();

        }

        public bool CreateProduct(Product product)
        {
            // Validate the product object
            if (product == null)
            {
                return false;
            }


            product.CreationDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;

            _db.Products.Add(product);

            return Save();

        }

        public bool DeleteProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }

            _db.Products.Remove(product);
            return Save();
        }

        public Product? GetProduct(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _db.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public ICollection<Product> GetProductForCategory(int categoryId)
        {
            // Validate the categoryId  
            if (categoryId <= 0)
            {
                return new List<Product>(); // Return an empty list if the categoryId is invalid
            }
            return _db.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
        }

        public ICollection<Product> GetProducts()
        {
            return _db.Products.OrderBy(p => p.Name).ToList();
        }

        public bool ProductExists(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return _db.Products.Any(p => p.ProductId == id);
        }

        public bool ProductExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public ICollection<Product> SearchProduct(string name)
        {
            IQueryable<Product> query = _db.Products;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.ToLower().Trim() == p.Name.ToLower().Trim());
            }

            return query.OrderBy(p => p.Name).ToList();

        }

        public bool UpdateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }

            product.UpdateDate = DateTime.Now;

            _db.Products.Update(product);

            return Save();
        }
    }
}
