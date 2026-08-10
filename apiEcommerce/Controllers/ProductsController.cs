using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using apiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ApiVersionNeutral]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        private void UploadProductImage(dynamic productDto, Product product)
        {
            // Generate a unique file name using the product ID and a GUID
            string fileName = product.ProductId +
                Guid.NewGuid().ToString() +
                Path.GetExtension(productDto.Image.FileName);

            // Set the ImgUrl property of the product to the relative path of the image
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");

            // validate if the folder exists, if not create it
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            // Combine the folder path and the file name to get the full file path
            var filePath = Path.Combine(imagesFolder, fileName);

            FileInfo file = new FileInfo(filePath); // Create a FileInfo object for the file path

            // validate if the file exists, if so delete it
            if (file.Exists)
            {
                file.Delete();
            }
            using var fileStream = new FileStream(filePath, FileMode.Create); // Create a new file stream to write the image to the server

            productDto.Image.CopyTo(fileStream); // Copy the image to the file stream

            // Set the ImgUrl property of the product to the absolute URL of the image
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            // Set the ImgUrl property of the product to the absolute URL of the image
            product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
            product.ImgUrlLocal = filePath; // Set the ImgUrlLocal property of the product to the local file path of the image
        }

        public ProductsController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        //Endpoint to get all products

        [HttpGet]
        [AllowAnonymous] // Allow anonymous access to this endpoint
        [ProducesResponseType(StatusCodes.Status200OK)] // Successful response with category data
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission

        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        //Endpoint to get product by id
        [AllowAnonymous]
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

        //public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto) old version without image upload
        public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
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

            // validate if the image is not null, then save it to the server and set the ImgUrl property of the product
            if (createProductDto.Image != null)
            {
                UploadProductImage(createProductDto, product);
            }
            else
            {
                product.ImgUrl = "https://placehold.co/300x300";
            }

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productoDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new
            {
                productId = product.ProductId
            }, productoDto);
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

        //Endpoint to Update a product
        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Successful response with the created category data
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad request response if the id is invalid
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized response if the user is not authenticated
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Forbidden response if the user does not have permission
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal server error response if there is an error while creating the category 

        public IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {updateProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;

            // validate if the image is not null, then save it to the server and set the ImgUrl property of the product
            if (updateProductDto.Image != null)
            {
                UploadProductImage(updateProductDto, product);
            }
            else
            {
                product.ImgUrl = "https://placehold.co/300x300";
            }

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult DeleteProduct(int productId)
        {

            if (productId == 0)
            {
                return BadRequest();
            }

            var product = _productRepository.GetProduct(productId);

            if (product == null)
            {
                return NotFound($"El producto con el id {productId} No existe");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustumerError", $"Algo salio mal al eliminar el producto {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
