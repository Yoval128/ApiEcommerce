using apiEcommerce.Models;
using apiEcommerce.Repository.IRepository;

//Logic for creating, reading, updating and deleting categories in the database
namespace apiEcommerce.Repository
{
    //Implementation of the ICategoryRepository interface for managing category data in the database
    public class CategoryRepository : ICategoryRepository
    {

        //Private field to hold the database context for accessing the database
        private readonly ApplicationDbContext _db;


        //Constructor that initializes the database context for the repository
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public bool CategoryExists(int id)
        {
            return _db.Categories.Any(c => c.Id == id);
        }

        public bool CategoryExists(string name)
        {
            //Checks if a category with the specified name exists in the database, ignoring case and whitespace
            return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool CreateCategory(Category category)
        {
            //Sets the creation date
            category.CreationDate = DateTime.Now;
            //Adds the new category to the database
            _db.Categories.Add(category);
            //Saves the changes to the database and returns the result
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            //Removes the specified category from the database
            _db.Categories.Remove(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            //Retrieves all categories from the database, ordered by name, and returns them as a list

            return _db.Categories.OrderBy(c.Name).ToList();
        }

        public Category GetCategory(int id)
        {
            return _db.Categories.FirstOrDefault(c.Id == id); ?? throw new InvalidOperationException($"La categoria con el id {id} no existe");
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Categories.Update(category);
            return Save();
        }
    }
}
