using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userReprository, IMapper mapper)
        {
            _userRepository = userReprository;
            _mapper = mapper;
        }


        //Enpoint to get all users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] //Successful response with user data
        [ProducesResponseType(StatusCodes.Status403Forbidden)] //Forbidden response if the user does not have permission]

        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers(); //Get all user from the repository (database)
            var usersDto = _mapper.Map<List<UserDTO>>(users); // Take the list of users and map it to a list of UserDTO objects using AutoMapper

            return Ok(usersDto); // Return a 200 OK response with the list of UserDTO objects
        }



    }
}
