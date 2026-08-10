using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userReprository)
        {
            _userRepository = userReprository;
        }


        //Enpoint to get all users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] //Successful response with user data
        [ProducesResponseType(StatusCodes.Status403Forbidden)] //Forbidden response if the user does not have permission]

        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers(); //Get all user from the repository (database)
            var usersDto = users.Adapt<List<UserDTO>>(); // Map users to DTOs using Mapster

            return Ok(usersDto); // Return a 200 OK response with the list of UserDTO objects
        }

        //Endpoint to get a user by id
        //[HttpGet("{id:int}", Name = "GetUser")] old version of the endpoint
        [HttpGet("{id}", Name = "GetUser")] // new version that implement Indentity
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //public IActionResult GetUser(int id)
        //{
        //    var user = _userRepository.GetUser(id); // Get the user from the repository by id

        //    if (user == null)
        //    {
        //        return NotFound(); // Return a 404 Not Found response if the user does not exist
        //    }

        //    var userDto = _mapper.Map<List<UserDTO>>(user); // Map the user object to a UserDTO object using AutoMapper

        //    return Ok(userDto);
        //}

        public IActionResult GetUser(string id)
        {
            var user = _userRepository.GetUser(id); // Get the user from the repository by id
            if (user == null)
            {
                return NotFound(); // Return a 404 Not Found response if the user does not exist
            }
            var userDto = user.Adapt<UserDTO>(); // Map the user object to a UserDTO object using Mapster
            return Ok(userDto);
        }


        //Enpoint to create a new user
        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegisterUser(CreateUserDTO createUserDTO)
        {
            // Validate the input data and check if the input data is null or the model state is invalid
            if (createUserDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return a 400 Bad Request response with the model state errors
            }

            //Validate if the username is null or empty
            if (string.IsNullOrEmpty(createUserDTO.Username))
            {
                return BadRequest("Username is required.");
            }

            //Validate if username is unique
            if (!_userRepository.IsUniqueUser(createUserDTO.Username)) //check if the username already exists in the repository and negates the result to check if it is not unique
            {
                return BadRequest("The username already exists.");
            }

            var result = await _userRepository.Register(createUserDTO); // Call the Register method of the user repository to create a new user

            //Validate if the user creation was successful
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user."); // Return a 500 Internal Server Error response if the user creation fails
            }

            return CreatedAtRoute("GetUser", new { id = result.Id }, result); // Return a 201 Created response with the location of the newly created user
        }

        //Endpoint to login a user  
        [AllowAnonymous]
        [HttpPost("login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState); //return a 400 Bad Request response with the model state errors
            }

            var user = await _userRepository.Login(userLoginDTO); // Call the Login method of the user repository to authenticate the user

            if (user == null)
            {
                return Unauthorized(); // Return a 401 Unauthorized response if the user authentication fails
            }

            return Ok(user); // Return a 200 OK response with the authenticated user data
        }
    }
}
