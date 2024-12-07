using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSAPI.Models.Store;
using OMSAPI.Services;
using OMSAPI.Dto.StoreDto;
using OMSAPI.Services.ServicesInterfaces;
using MongoDB.Bson;

namespace OMSAPI.Controllers.StoreControllers
{

    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly IOrderServices _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IEmailServices _emailServices;
        private readonly IUserServices _userServices;
        private readonly ITenantServices _tenantServices;
        private readonly IProductServices _productServices;

        public OrderController(IOrderServices orderService, ILogger<OrderController> logger, IEmailServices emailServices, IUserServices userServices,ITenantServices tenantServices, IProductServices productServices)
        {
            _orderService = orderService;
            _logger = logger;
            _emailServices = emailServices;
            _userServices = userServices;
            _tenantServices = tenantServices;
            _productServices = productServices;
        }

        [HttpGet("getAll/{status}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<List<OrderDeliveryDto>>> GetAllOrdersAsync([FromRoute] string tenantId, [FromRoute] string status)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            // validate status 
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Status {status} is not valid".ToJson());
            }

            var list = await _orderService.GetAllOrders(tenantId, status);
            var users = await _userServices.GetAll(tenantId);
            if (list == null || users == null)
            {
                _logger.LogError($"Failed to execute GetAllOrders for {tenantId}");
                return BadRequest($"Failed to get order or user list.".ToJson());
            }

            List<OrderResponseDto> response = new List<OrderResponseDto>();
            foreach (var order in list)
            {
                // find user by id
                var user = users.FirstOrDefault(u => u.Id == order.UserId);
                if (user == null)
                {
                    _logger.LogError($"Failed to execute GetAllOrders for {tenantId}");
                    return BadRequest($"Failed to get order.User was not found.".ToJson());
                }
                response.Add(new OrderResponseDto(order, user));
            }

            _logger.LogInformation($"GetAllOrders operation finished successfully");
            return Ok(response);
        }


        [HttpGet("getById/{orderId}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<OrderDeliveryDto>> GetOrderByIdAsync([FromRoute] string tenantId, [FromRoute] string orderId)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var order = await _orderService.GetOrderById(tenantId, orderId);
            if (order == null)
            {
                _logger.LogError($"Failed to execute GetOrderById for {tenantId}.Order was not found.");
                return BadRequest($"Failed to get order.Order was not found.".ToJson());
            }

            var user = await _userServices.GetById(tenantId, order.UserId);
            if (user == null)
            {
                _logger.LogError($"Failed to execute GetOrderById for {tenantId}.User was not found.");
                return BadRequest($"Failed to get order.User was not found.".ToJson());
            }
            _logger.LogInformation($"GetOrderById operation finished successfully");
            return Ok(new OrderResponseDto(order, user));
        }


        [HttpGet("getByUser/{userId}/{status}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<OrderDeliveryDto>> GetOrderByUserAsync([FromRoute] string tenantId, [FromRoute] string userId, [FromRoute] string status)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            // validate status 
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Status {status} is not valid".ToJson());
            }

            var list = await _orderService.GetOrderByUser(tenantId, userId, status);
            var user = await _userServices.GetById(tenantId, userId);
            if (list == null || user == null)
            {
               _logger.LogError($"Failed to execute GetOrderByUser for {tenantId}");
                return BadRequest("Failed to get order.".ToJson());
            }

            List<OrderResponseDto> response = new List<OrderResponseDto>();
            foreach (var order in list)
            {
                if (order != null)
                {
                    response.Add(new OrderResponseDto(order, user));
                }
            }

            _logger.LogInformation($"GetOrderByUser operation finished successfully");
            return Ok(response);
        }


        [HttpPost]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<Order>> CreateOrderAsync([FromRoute] string tenantId, [FromBody] ShoppingCart cart)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var user = await _userServices.GetById(tenantId, cart.UserId);

            if (user == null)
            {
                _logger.LogError($"Failed to execute create order for {cart.UserId}. User was not found");
                return BadRequest("User was not found".ToJson());
            }

            Order order = cart.CreateOrderFromShoppingCart(user);

            var res = await _orderService.CreateOrder(tenantId, order);
            var tenant = await _tenantServices.Get(tenantId);
            if (res == null || user == null || tenant == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to create order {order.ToString()}".ToJson());
            }

            // Substract quantity from product stack
            var substract =  await _productServices.SubstractQuantity(tenantId, cart);
            if (substract == false)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to substract quantity from product stack".ToJson());
            }

            _logger.LogInformation($"Create order finished successfully. Sending confirmation emails");
            try
            {
                OrderResponseDto orderDeliveryDto = new OrderResponseDto(user.Id, user.Name, user.Email, user.PhoneNumber, cart.CartItems);
                OrderEmailReponseDto orderEmailReponseDto = new OrderEmailReponseDto(orderDeliveryDto, tenant);
                var sendEmailToUser = await _emailServices.SendUserOrderEmail(user.Email, orderEmailReponseDto);
                var sendEmailToTenant = await _emailServices.SendTenantOrderEmail(tenant.Email, orderEmailReponseDto);
                if (sendEmailToUser == true && sendEmailToTenant == true)
                {
                    return Ok(orderDeliveryDto);
                }
                // TODO: Decide what to do if confermation email failed.
            }
            catch (Exception ex) { _logger.LogError($"Failed to create order. Exception is : {ex}"); }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to send confirmation Email for {user.Email}. Order has been saved.".ToJson());
        }


        [HttpDelete("delete/{orderId}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Order>> DeleteOrderAsync([FromRoute] string tenantId, [FromRoute] string orderId)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var order = await _orderService.GetOrderById(tenantId, orderId);

            // Validate order
            if (order == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to execute delete for {orderId}. Order was not found".ToJson());
            }

            // Delete order
            var res = await _orderService.DeleteOrder(tenantId, order);
            if (res == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to execute delete for {orderId}".ToJson());
            }

            // Add quantity to product stack
            if (await _productServices.AddQuantity(tenantId, order) == false)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to update products stock for {orderId}. Order has been deleted".ToJson());
            }


            // TODO: Add quantity to product stack
            _logger.LogInformation($"Delete order finished successfully.");
            return Ok();
        }


        [HttpPut("update/orderStatus")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Order>?> ChangeOrderStatus([FromRoute] string tenantId, [FromBody] OrderDeliveryDto orderDeliveryDto)
        {
            // Validate access.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            Order? order = await _orderService.GetOrderById(tenantId, orderDeliveryDto.OrderId);
            if (order == null)
            {
                _logger.LogError($"Failed to execute update for {orderDeliveryDto.OrderId}. Order was not found");
                return BadRequest("Order was not found".ToJson());
            }

            // validate status 
            if (!Enum.IsDefined(typeof(OrderStatus), orderDeliveryDto.Status))
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Status {orderDeliveryDto.Status} is not valid".ToJson());
            }
            order.Status = orderDeliveryDto.Status;

            var res = await _orderService.UpdateOrder(tenantId, orderDeliveryDto.OrderId, order);
            if (res == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Failed to execute update for {orderDeliveryDto.OrderId}".ToJson());
            }
            _logger.LogInformation($"Update product finished successfully.");
            return Ok();
        }
    }
    
}
