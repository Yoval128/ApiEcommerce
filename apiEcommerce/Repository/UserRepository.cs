using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolMannager;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
            _userManager = userManager;
            _rolMannager = roleManager;
            _mapper = mapper;
        }

        //Get a list of all users

        //--old method without Identity
        //public ICollection<User> GetUsers()
        //{
        //    //return a list of all users ordered by username
        //    return _db.Users.OrderBy(u => u.Username).ToList();
        //}

        public ICollection<ApplicationUser> GetUsers()
        {
            //return a list of all users ordered by username
            return _db.ApplicationUsers.OrderBy(u => u.UserName).ToList();
        }

        //Get a user by id

        //--old method whitout Identity
        //public User? GetUser(int id)
        //{
        // return _db.Users.FirstOrDefault(u => u.Id == id);
        //}

        public ApplicationUser? GetUser(String id)
        {
            // return the user with the specified id, or null if no user is found
            return _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
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
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(
                u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDTO.Username.ToLower().Trim()
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

            //This method was added (implementation of Identity) 
            // Check if the password is null and return a UserLoginResponseDTO object with an error message
            if (userLoginDTO.Password == null)
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Password is required"
                };
            }
            // Validation manually the password, but now we are using Identity
            ////Check if the password is null or empty and return a UserLoginResponseDTO object with an error message
            //if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.Password))
            //{
            //    return new UserLoginResponseDTO()
            //    {
            //        Token = "",
            //        User = null,
            //        Message = "Password is incorrect"
            //    };
            //}

            bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDTO.Password);
            if (!isValid)
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

            // Get the user's role using the UserManager's GetRolesAsync method
            var role = await _userManager.GetRolesAsync(user);

            // declare a byte array to hold the secret key for signing the JWT token
            var key = Encoding.UTF8.GetBytes(secretKey!);

            //check if the key is null or empty and throw an exception if it is
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //The Subject property is set to a new ClaimsIdentity object that contains the user's ID,
                //username, and role as claims.
                //The Expires property is set to 2 hours from the current UTC time,
                //and the SigningCredentials property is set to use the HMAC SHA256 algorithm with the secret key for signing the token.
                //Subject = new ClaimsIdentity(new[]
                //{
                //   new Claim("id",user.Id.ToString()), // Add the user's ID as a claim
                //   new Claim("username",user.Username), // Add the user's username as a claim
                //   new Claim(ClaimTypes.Role,user.Role ?? string.Empty) // Add the user's role as a claim, or an empty string if the role is null

                //}),

                //New implementation using Identity to get the user's role
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()), // Add the user's ID as a claim
                    new Claim("username", user.UserName ?? string.Empty), // Add the user's username as a claim
                    new Claim(ClaimTypes.Role, role.FirstOrDefault() ?? string.Empty) // Add the user's role as a claim, or an empty string if the role is null
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
                //Method old 
                //User = new UserRegisterDTO() // Create a new UserRegisterDTO object and set its properties to the user's information
                //{
                //    Username = user.Username,
                //    Name = user.Name,
                //    Role = user.Role,
                //    Password = user.Password ?? "",
                //},
                User = _mapper.Map<UserDataDTO>(user), // Use AutoMapper to map the ApplicationUser object to a UserRegisterDTO object
                Message = "Login successful",
            };
        }

        //Get a CreateUserDto object and return a object User


        //public async Task<User> Register(CreateUserDTO createUserDTO)
        // {
        //OLD METHOD WITHOUT IDENTITY
        ////Encrypt the password using BCrypt and store it in the database
        //var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);

        ////Create a new User object and set its properties to the values from the CreateUserDTO object
        //var user = new User()
        //{
        //    Username = createUserDTO.Username ?? "",
        //    Name = createUserDTO.Name,
        //    Role = createUserDTO.Role,
        //    Password = encryptedPassword,
        //};


        //_db.Users.Add(user); // Add the new user to the database
        //await _db.SaveChangesAsync(); // Save the changes to the database
        //return user; // Return the newly created user



        //   }
        //New method using Identity
        public async Task<UserDataDTO> Register(CreateUserDTO createUserDTO)
        {
            // Validate the input data and throw an exception if any required field is missing
            if (string.IsNullOrEmpty(createUserDTO.Username))
            {
                throw new ArgumentNullException("Username is required");
            }

            // Validate the input data and throw an exception if any required field is missing
            if (createUserDTO.Password == null)
            {
                throw new ArgumentNullException("Password is required");
            }

            // Validate the input data and throw an exception if any required field is missing
            var user = new ApplicationUser()
            {
                UserName = createUserDTO.Username,
                Email = createUserDTO.Username,
                NormalizedEmail = createUserDTO.Username.ToUpper(),
                Name = createUserDTO.Name,
            };

            // Create the user using the UserManager's CreateAsync method and pass in the user object and password
            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (result.Succeeded)
            {
                var userRole = createUserDTO.Role ?? "User"; // Default role is "User" if not provided
                var roleExists = await _rolMannager.RoleExistsAsync(userRole); // Check if the role exists in the database

                // If the role does not exist, create it using the RoleManager's CreateAsync method
                if (!roleExists)
                {
                    var identityRole = new IdentityRole(userRole); // Create a new IdentityRole object with the specified role name
                    await _rolMannager.CreateAsync(identityRole); // Create the role in the database
                }

                await _userManager.AddToRoleAsync(user, userRole); // Assign the role to the user using the UserManager's AddToRoleAsync method

                var createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDTO.Username); // Retrieve the created user from the database
                return _mapper.Map<UserDataDTO>(createdUser); // Use AutoMapper to map the ApplicationUser object to a UserDataDTO object and return it
            }
            throw new ApplicationException("No se puedo completar el registro");

        }
        //Get a username and return a bool indicating whether it is unique
        public bool IsUniqueUser(string username)
        {
            //ignoring case and whitespace, and return true if no user with the same username exists, otherwise return false
            return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
        }

    }
}
