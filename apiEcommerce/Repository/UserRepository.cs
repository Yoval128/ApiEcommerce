using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;

namespace apiEcommerce.Repository
{
    public class UserRepository : IUserRepository
    {
        // Declare a private read-only field of type
        // ApplicationDbContext to interact with the
        // database private readonly A
        // pplicationDbContext _db;

        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
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

        public Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO) { }

        public async Task<User> Register(CreateUserDTO createUserDTO)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);

            var User = new User
            {
                Username = createUserDTO.Username,
                Name = createUserDTO.Name,
                Role = createUserDTO.Role,
                Password = encryptedPassword,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return User;
        }
    }
}
