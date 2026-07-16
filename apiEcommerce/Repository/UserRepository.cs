using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apiEcommerce.Repository
{
    //Metho
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

            // Validate the username
            var user = await _db.Users.FirstOrDefaultAsync(
                u => u.Username.ToLower().Trim() == userLoginDTO.Username.ToLower().Trim()
                );

            // Check if the user is null and return a UserLoginResponseDTO object with an error message
            if (user == null)
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Username not found"
                };
            }

            //Check if the password is null or empty and return a UserLoginResponseDTO object with an error message
            if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.Password))
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Password is incorrect"
                };
            }

            // declare a new instance of JwtSecurityTokenHandler to create a JWT token
            var handler = new JwtSecurityTokenHandler();

            //check if the secret key is null or empty and throw an exception if it is
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            // declare a byte array to hold the secret key for signing the JWT token
            var key = Encoding.UTF8.GetBytes(secretKey!);

            //check if the key is null or empty and throw an exception if it is
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //The Subject property is set to a new ClaimsIdentity object that contains the user's ID,
                //username, and role as claims.
                //The Expires property is set to 2 hours from the current UTC time,
                //and the SigningCredentials property is set to use the HMAC SHA256 algorithm with the secret key for signing the token.
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim("id",user.Id.ToString()), // Add the user's ID as a claim
                   new Claim("username",user.Username), // Add the user's username as a claim
                   new Claim(ClaimTypes.Role,user.Role ?? string.Empty) // Add the user's role as a claim, or an empty string if the role is null

                }),

                Expires = DateTime.UtcNow.AddHours(2), // Set the token expiration time to 2 hour from now
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                // Set the signing credentials to use the HMAC SHA256 algorithm with the secret key
            };

            // Create a new JWT token using the token descriptor
            var token = handler.CreateToken(tokenDescriptor);

            // Return a UserLoginResponseDTO object with the generated token, user information, and a success message
            return new UserLoginResponseDTO()
            {
                Token = handler.WriteToken(token), // Write the token to a string and set it as the Token property

                User = new UserRegisterDTO() // Create a new UserRegisterDTO object and set its properties to the user's information
                {
                    Username = user.Username,
                    Name = user.Name,
                    Role = user.Role,
                    Password = user.Password ?? "",
                },
                Message = "Login successful",
            };
        }

        //Get a CreateUserDto object and return a object User
        public async Task<User> Register(CreateUserDTO createUserDTO)
        {
            //Encrypt the password using BCrypt and store it in the database
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);

            //Create a new User object and set its properties to the values from the CreateUserDTO object
            var user = new User()
            {
                Username = createUserDTO.Username ?? "",
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
