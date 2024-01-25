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
    public class FavoriteAPIController : Controller
    {
        private readonly UserManager<User> _userManager;
        protected ResponseDto _response;
        private readonly IFavoriteRepository _favoriteRepository;
        public FavoriteAPIController(UserManager<User> userManager, IFavoriteRepository favoriteRepository)
        {
            _userManager = userManager;
            this._response = new ResponseDto();
            _favoriteRepository = favoriteRepository;
        }

        [HttpGet]
        [Route("favorites")]
        [Authorize]
        public async Task<object> GetFavoriteProducts()
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (userId == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                IEnumerable<ProductDto> favoriteProducts = await _favoriteRepository.GetFavoriteProductsAsync(userId);
                _response.Result = favoriteProducts;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        [Route("toggle-favorite/{id}")]
        [Authorize]
        public async Task<object> ToggleFavoriteStatus(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (userId == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                await _favoriteRepository.ToggleFavoriteStatusAsync(id, userId);
                _response.Result = "Cтатус избранного установлен успешно";
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
        [Route("delete-favorite/{id}")]
        [Authorize]
        public async Task<object> DeleteFavoriteProduct(int id)
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

                if (await _userManager.IsInRoleAsync(user, Constants.AdminRoleName))
                {
                    bool isSuccess = await _favoriteRepository.RemoveFromFavoritesAsync(id);
                    _response.Result = isSuccess;
                }

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
