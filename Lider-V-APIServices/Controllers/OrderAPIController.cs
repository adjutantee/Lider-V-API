using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lider_V_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderAPIController : Controller
    {
        protected ResponseDto _response;
        private IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<OrderAPIController> _logger;

        public OrderAPIController(IOrderRepository orderRepository, UserManager<User> userManager, ILogger<OrderAPIController> logger)
        {
            this._response = new ResponseDto();
            _orderRepository = orderRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [Route("user-orders")]
        public async Task<object> GetUserOrders()
        {
            try
            {
                _logger.LogInformation("Начало работы метода GetUserOrders");
                var userId = _userManager.GetUserId(User);
                IEnumerable<OrderDto> userOrders = (IEnumerable<OrderDto>)await _orderRepository.GetUserOrdersAsync(userId);
                _response.Result = userOrders;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка заказов пользователя");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet]
        [Route("all-orders")]
        public async Task<object> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Начало работы метода GetAllOrders");
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError("Пользователь не авторизован или не найден в базе");
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }
                
                if (await _userManager.IsInRoleAsync(user, Constants.AdminRoleName))
                {
                    _logger.LogInformation("Подверждение статуста администратора пользователя. Продолжение работы метода");
                    IEnumerable<OrderDto> allOrders = (IEnumerable<OrderDto>)await _orderRepository.GetAllOrdersAsync();
                    _response.Result = allOrders;
                    return StatusCode(200, _response);
                }
                else
                {
                    _logger.LogWarning("Данный метод доступен только для администратора");
                    _response.IsSuccess = false;
                    _response.Result = "Запрашиваемый ресурс недоступен";
                    return StatusCode(403, _response);
                }             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка заказов");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<object> GetOrderDetails(int orderId)
        {
            try
            {
                _logger.LogInformation("Начало работы метода GetOrderDetails");
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError("Пользователь не авторизован или не найден в базе");
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                var userId = _userManager.GetUserId(User);
                OrderDto orderDetails = (OrderDto)await _orderRepository.GetOrderDetailsAsync(orderId, userId);

                if (orderDetails != null)
                {
                    _response.Result = orderDetails;
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Заказ не найден";
                    return StatusCode(404, _response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении деталей заказа");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Route("place-order")]
        public async Task<object> PlaceOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                _logger.LogInformation("Начало работы метода PlaceOrder");
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError("Пользователь не авторизован или не найден в базе");
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                var userId = _userManager.GetUserId(User);
                bool success = await _orderRepository.PlaceOrderAsync(userId, orderDto);

                if (success)
                {
                    _response.Result = "Заказ успешно оформлен";
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при оформлении заказа";
                    return StatusCode(400, _response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при попытке изменить статус заказа");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Route("update-order-status/{orderId}/{newStatus}")]
        public async Task<object> UpdateOrderStatus(int orderId, OrderStatusDto newStatusDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError("Пользователь не авторизован или не найден в базе");
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }


                bool success = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatusDto);

                if (await _userManager.IsInRoleAsync(user, Constants.UserRoleName))
                {
                    if (success)
                    {
                        _response.Result = "Статус заказа успешно обновлен";
                        return StatusCode(200, _response);
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Result = "Ошибка при обновлении статуса заказа";
                        return StatusCode(400, _response);
                    }
                }
                else
                {
                    _logger.LogWarning("Данный метод доступен только для администратора");
                    _response.IsSuccess = false;
                    _response.Result = "Запрашиваемый ресурс недоступен";
                    return StatusCode(403, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }
    }
}
