using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
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

        public OrderAPIController(IOrderRepository orderRepository, UserManager<User> userManager)
        {
            this._response = new ResponseDto();
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        [Route("user-orders")]
        public async Task<object> GetUserOrders()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                IEnumerable<OrderDto> userOrders = (IEnumerable<OrderDto>)await _orderRepository.GetUserOrdersAsync(userId);
                _response.Result = userOrders;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.AdminRoleName)]
        [Route("all-orders")]
        public async Task<object> GetAllOrders()
        {
            try
            {
                IEnumerable<OrderDto> allOrders = (IEnumerable<OrderDto>)await _orderRepository.GetAllOrdersAsync();
                _response.Result = allOrders;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{orderId}")]
        public async Task<object> GetOrderDetails(int orderId)
        {
            try
            {
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
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("place-order")]
        public async Task<object> PlaceOrder([FromBody] OrderDto orderDto)
        {
            try
            {
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
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        [Route("update-order-status/{orderId}/{newStatus}")]
        public async Task<object> UpdateOrderStatus(int orderId, OrderStatusDto newStatusDto)
        {
            try
            {
                bool success = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatusDto);

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
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }
    }
}
