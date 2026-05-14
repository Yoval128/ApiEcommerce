using apiEcommerce.Models;

namespace apiEcommerce.Repository.IRepository
{
    public interface IProductReository
    {
        // Get list of all products
        ICollection<Product> GetProducts();
        // Get a category by id and return all the products in that category
        ICollection<Product> GetProductForCategory(int categoryId);

        // Get a product by id and return all the products that been equal to that product
        ICollection<Product> SearchProduct(string name);

        //Get a id and return a product
        Product? GetProduct(int id);

        // Get a name and quantity and 
        // return a bool to check if the buy is successful
        bool BuyProduct(String name, int quantity);

        // Check if a product exists by its ID
        bool ProductExists(int id);

        // Check if a product exists by its name
        bool ProductExists(String name);

        //Get a object of product and return a bool to check if the create is successful
        bool CreateProduct(Product product);

        //Get a object product and return a bool to check if the update is successful
        bool UpdateProduct(Product product);

        //Get a object Porduct and return a bool to check if the delete is successful
        bool DeleteProduct(Product product);

        // Get a bool to check if the save is successful
        bool Save();

    }
}
