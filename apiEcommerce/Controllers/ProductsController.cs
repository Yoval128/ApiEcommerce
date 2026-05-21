using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        //Endpoint to get all products

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission

        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        //Endpoint to get product by id

        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist

        public IActionResult GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId); // Retrieve the category with the specified id from the repository
            if (product == null)
            {
                return NotFound($"El producto con el id {productId} no existe"); // Return a 404 Not Found response if the category does not exist
            }
            var productDto = _mapper.Map<ProductDto>(product); // Map the category entity to a CategoryDto
            return Ok(productDto);
        }

        //Endpoint to create a new product
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Successful response with the created category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized response if the user is not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error response if there is an error while creating the category 

        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El producto ya existe");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {createProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(createProductDto);
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productoDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productoDto);
        }

        //Endpoint Get products by category id
        [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist

        public IActionResult GetProductForCategory(int categoryId)
        {
            var products = _productRepository.GetProductForCategory(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"Los productos con la categoría {categoryId} no existen");
            }
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }


        //Endpoint Get products by name and description
        [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist

        public IActionResult SearchProducts(string searchTerm)
        {
            var products = _productRepository.SearchProducts(searchTerm);
            if (products.Count == 0)
            {
                return NotFound($"El producto con el nombre '{searchTerm}' no existe");
            }
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        //Endpoint to buy a product
        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response if the category does not exist

        public IActionResult SearchProducts(string name, int quantity)
        {
            var products = _productRepository.BuyProduct(name, quantity);

            if (String.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest($"El nombre del producto no puede estar vacío y la cantidad debe ser mayor a cero");
            }
            var foundProduct = _productRepository.ProductExists(name);

            if (!foundProduct)
            {
                return NotFound($"El producto con el nombre '{name}' no existe");
            }

            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No hay suficiente stock para comprar {quantity} unidades del producto '{name}'");
                return BadRequest(ModelState);
            }

            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"Has comprado {quantity} {units} del producto '{name}' exitosamente");

        }

    }
}
