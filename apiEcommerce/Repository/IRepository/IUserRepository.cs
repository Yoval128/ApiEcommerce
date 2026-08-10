using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;

namespace apiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {

        // Get a list of all users
        //   ICollection<User> GetUsers(); old method signature
        ICollection<ApplicationUser> GetUsers();

        // Get a user by ID and return a user
        // User? GetUser(int id); Old method signature
        ApplicationUser? GetUser(String id);

        // Get a username and return a bool indicating whether it is unique
        bool IsUniqueUser(string username);
        // Get a UserLoginDto object and return a UserLoginResponseDto
        Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO);
        // Get a CreateUserDto object and return a object User 

        // Task<User> Register(CreateUserDTO createUserDTO); Old method signature
        Task<UserDataDTO> Register(CreateUserDTO createUserDTO);
    }
}
