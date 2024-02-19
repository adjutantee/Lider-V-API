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
    public class CartAPIController : Controller
    {
        private ICartRepository _cartRepository;
        protected ResponseDto _response;
        private readonly UserManager<User> _userManager;

        public CartAPIController(ICartRepository cartRepository, UserManager<User> userManager)
        {
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                CartDto cartDto = await _cartRepository.GetCartAsync(user.Id);
                _response.Result = cartDto;

                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetCartById")]
        public async Task<object> Get(int cartId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                var cartDto = await _cartRepository.GetCartByIdAsync(cartId);

                if (cartDto != null && cartDto.UserId == user.Id)
                {
                    _response.Result = cartDto;
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Корзина не найдена или не принадлежит текущему пользователю";
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
        public async Task<object> AddToCart([FromBody] CartItemDto cartItemDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                bool isSuccess = await _cartRepository.AddToCartAsync(cartItemDto.ProductId, cartItemDto.Quantity, user.Id);
                _response.Result = isSuccess;

                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpDelete]
        public async Task<object> RemoveFromCart(int cartItemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                bool isSuccess = await _cartRepository.RemoveFromCartAsync(cartItemId);
                _response.Result = isSuccess;

                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("ClearCart")]
        public async Task<object> ClearCart(string userId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                bool isSuccess = await _cartRepository.ClearCartAsync(userId);
                _response.Result = isSuccess;
                return StatusCode(200, _response);
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
