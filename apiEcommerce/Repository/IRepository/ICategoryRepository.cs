using apiEcommerce.Models;

namespace apiEcommerce.Repository.IRepository
{
    public interface ICategoryRepository
    {
        //<T> where T is a class
        ICollection<Category> GetCategories(); // Get list of all categories

        Category? GetCategory(int id); // Get a category by its ID

        bool CategoryExists(int id); // Check if a category exists by its ID

        bool CategoryExists(String name); // Check if a category exists by its name

        bool CreateCategory(Category category); // Create a new category

        bool UpdateCategory(Category category); //Update a category

        bool DeleteCategory(Category category); //Delete a category

        bool Save(); // Save changes to the database



    }
}
