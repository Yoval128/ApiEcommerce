using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        // Repository for category data access
        private readonly ICategoryRepository _categoryRepository;

        // Mapper to map between DTOs and entities
        private readonly IMapper _mapper;

        // Constructor
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // Endpoint to get all categories
        [HttpGet] // GET: api/Category
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission

        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories(); // Retrieve categories from the repository
            var categoriesDto = new List<CategoryDto>(); // Create a list to hold the category DTOs

            // Map each category entity to a CategoryDto and add it to the list
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category)); // Add each category DTO to the list
            }

            return Ok(categoriesDto);
        }

        //Endpoint to get category by id
        [HttpGet("{id:int}", Name = "GetCategory")] // Get: api/Category/{id} where id is an integer
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist

        public IActionResult GetCategories(int id)
        {
            var categories = _categoryRepository.GetCategory(id); // Retrieve the category with the specified id from the repository
            if (categories == null)
            {
                return NotFound($"La categoria con el id {id} no existe"); // Return a 404 Not Found response if the category does not exist
            }
            var categoriesDto = _mapper.Map<CategoryDto>(categories); // Map the category entity to a CategoryDto

            return Ok(categoriesDto);
        }


        //Endpoint to create a new category
        [HttpPost] // Post: api/Category
        [ProducesResponseType(StatusCodes.Status201Created)] // Successful response with the created category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized response if the user is not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error response if there is an error while creating the category 

        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            // Validate the input data
            if (createCategoryDto == null)
            {
                return BadRequest(ModelState); // 
            }

            // Validate that the category name is unique
            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoria ya existe");
            }

            // create var category entity from the CreateCategoryDto using AutoMapper
            var category = _mapper.Map<Category>(createCategoryDto); // Map the CreateCategoryDto to a Category entity

            // Validate the model state after mapping
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {category.Name}");
                return StatusCode(500, ModelState); // Return a 500 Internal Server Error response if there is an error while creating the category
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
            // Return a 201 Created response with the created category data)
        }

        //Endpoint to update an existing category
        [HttpPatch("{id:int}", Name = "UpdateCategory")] // PATCH: api/Category/{id} where id is an integer
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized response if the user is not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error response if there is an error while creating the category 

        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {

            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoria con el id {id} no existe"); // Return a 404 Not Found response if the category does not exist
            }
            // Validate the input data
            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState); // return a 400 Bad Request response if the data is invalid
            }

            // Validate that the category name is unique
            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoria ya existe");
            }

            // create var category entity from the CreateCategoryDto using AutoMapper
            var category = _mapper.Map<Category>(updateCategoryDto); // Map the CreateCategoryDto to a Category entity

            category.Id = id; // Set the category Id to the id from the route

            // Validate the model state after mapping
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {category.Name}");
                return StatusCode(500, ModelState); // Return a 500 Internal Server Error response if there is an error while creating the category
            }

            return NoContent(); // Return a 204 No Content response if the category was updated successfully
        }

        // Endpoint to delete a category
        [HttpDelete("{id:int}", Name = "DeleteCategory")] // Delete : api/Category/{id} where id is an integer
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized response if the user is not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error response if there is an error while creating the category 

        public IActionResult DeleteCategory(int id)
        {

            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoria con el id {id} no existe"); // Return a 404 Not Found response if the category does not exist
            }

            var category = _categoryRepository.GetCategory(id);

            if (category == null)
            {
                return NotFound($"La categoria con el {id} no existe");
            }

            // 
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {category.Name}");
                return StatusCode(500, ModelState); // Return a 500 Internal Server Error response if there is an error while creating the category
            }

            return NoContent(); // Return a 204 No Content response if the category was updated successfully
        }

    }
}
