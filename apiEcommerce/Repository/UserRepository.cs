using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace apiEcommerce.Repository
{
    public class UserRepository : IUserRepository
    {
        // Declare a private read-only field of type
        // ApplicationDbContext to interact with the
        // database private readonly A
        // pplicationDbContext _db;

        private readonly ApplicationDbContext _db;
        private string? secretKey; // Declare a private field to store the secret key for JWT token generation

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        }

        //Get a list of all users
        public ICollection<User> GetUsers()
        {
            //return a list of all users ordered by username
            return _db.Users.OrderBy(u => u.Username).ToList();
        }

        //Get a user by id
        public User? GetUser(int id)
        {
            return _db.Users.FirstOrDefault(u => u.Id == id);
        }

        //Get a username and return a bool indicating whether it is unique
        public bool GetUser(string UserName)
        {
            //ignoring case and whitespace, and return true if no user with the same username exists, otherwise return false
            return !_db.Users.Any(u => u.Username.ToLower().Trim() == UserName.ToLower().Trim());
        }

        public async Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO)
        {
            // Check if the username is null or empty and return a UserLoginResponseDTO object with an error message
            if (string.IsNullOrEmpty(userLoginDTO.Username))
            {
                // Return a UserLoginResponseDTO object with an error message if the username is null or empty
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Username is required"
                };
            }
            var user = await _db.Users.FirstOrDefaultAsync(
                u => u.Username.ToLower().Trim() == userLoginDTO.Username.ToLower().Trim()
                );

            if (user == null)
            {
                Token = "",
                    User = null,
                    Message = "Username not found"


            }
        }

        //Get a CreateUserDto object and return a object User
        public async Task<User> Register(CreateUserDTO createUserDTO)
        {
            //Encrypt the password using BCrypt and store it in the database
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);

            //Create a new User object and set its properties to the values from the CreateUserDTO object
            var user = new User()
            {
                Username = createUserDTO.Username,
                Name = createUserDTO.Name,
                Role = createUserDTO.Role,
                Password = encryptedPassword,
            };


            _db.Users.Add(user); // Add the new user to the database
            await _db.SaveChangesAsync(); // Save the changes to the database
            return user; // Return the newly created user
        }
    }
}
