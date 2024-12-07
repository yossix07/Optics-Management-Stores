using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Models.Store;
using OMSAPI.Services;
using OMSAPI.Services.ServicesInterfaces;


namespace OMSAPI.Controllers.StoreControllers
{
    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    [Authorize(Policy = Roles.Roles.User)]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductServices productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        [HttpGet]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<List<Product>>> GetAllProductsAsync([FromRoute] string tenantId)
        {
            // Validate that tenant Acess only to his messages.
            if (User.IsInRole(Roles.Roles.Tenant) &&
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var list = await _productService.GetAllProducts(tenantId);

            if (list == null)
            {
                _logger.LogError($"Failed to execute GetAllProducts for {tenantId}");
                return BadRequest("Database or collection was not found".ToJson());
            }
            _logger.LogInformation($"GetAll operation finished successfully");
            return Ok(list);
        }

        [HttpGet("get/{productId}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<Product>> Get([FromRoute] string tenantId, [FromRoute] string productId)
        {
            // Validate access.
            if (User.IsInRole(Roles.Roles.Tenant) &&
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var product = await _productService.GetProduct(tenantId, productId);

            if (product == null)
            {
                _logger.LogError($"GetProduct opration failed for product {productId} in {tenantId} database.");
                return NotFound($"Product with Id {productId} was not found".ToJson());
            }
            _logger.LogInformation($"GetProduct operation finished successfully");
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Product>> CreateProductAsync([FromRoute] string tenantId, [FromBody] Product product)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var res = await _productService.CreateProduct(tenantId, product);
            if (res == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to create product {product.Name}".ToJson());
            }
            _logger.LogInformation($"Create product finished successfully.");
            return Ok(product);
        }

        [HttpPut("put/{productId}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Product>> PutProductAsync([FromRoute] string tenantId,[FromRoute] string productId, [FromBody] Product newProduct)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var product = await _productService.GetProduct(tenantId, productId);
            if (product == null)
            {
                _logger.LogError($"Failed to execute update for {productId}. Product was not found");
                return BadRequest("Product was not found".ToJson());
            }

            var res = await _productService.UpdateProduct(tenantId,productId, newProduct);
            if (res == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to execute update for {newProduct.Name}".ToJson());
            }
            _logger.LogInformation($"Update product finished successfully.");
            return Ok();
        }

        [HttpDelete("delete/{productId}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Product>> DeleteProductAsync([FromRoute] string tenantId, [FromRoute] string productId)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var product = await _productService.GetProduct(tenantId, productId);
            if (product == null)
            {
                _logger.LogError($"Failed to execute delete for {productId}. Product was not found");
                return BadRequest("Product was not found".ToJson());
            }

            var res = await _productService.DeleteProduct(tenantId, product);
            if (res == null)
            {
                _logger.LogError($"Failed to execute delete for {productId}");
                return BadRequest($"Failed to execute delete product".ToJson());
            }
            _logger.LogInformation($"Delete product finished successfully.");
            return Ok();
        }
    }
}
