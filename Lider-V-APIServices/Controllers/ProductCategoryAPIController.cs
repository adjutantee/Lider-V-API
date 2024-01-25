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
    public class ProductCategoryAPIController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProductCategoryRepository _productCategoryRepository;
        protected ResponseDto _response;

        public ProductCategoryAPIController(UserManager<User> userManager, IProductCategoryRepository productCategoryRepository)
        {
            _userManager = userManager;
            _productCategoryRepository = productCategoryRepository;
            this._response = new ResponseDto();
        }

        [HttpGet]
        [Route("by-category/{categoryId}")]
        public async Task<object> GetProductsByCategory(int categoryId)
        {
            try
            {
                IEnumerable<ProductDto> productsByCategory = await _productCategoryRepository.GetProductsByCategoryId(categoryId);
                _response.Result = productsByCategory;
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
        [Route("add-to-category/{productId}/{categoryId}")]
        [Authorize]
        public async Task<object> AddProductToCategory(int productId, int categoryId)
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
                    await _productCategoryRepository.AddProductToCategoryAsync(productId, categoryId);
                    _response.Result = "Продукт успешно добавлен в категорию";
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

        [HttpPost]
        [Route("remove-from-category/{productId}/{categoryId}")]
        [Authorize]
        public async Task<object> RemoveProductFromCategory(int productId, int categoryId)
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
                    await _productCategoryRepository.RemoveProductFromCategoryAsync(productId, categoryId);
                    _response.Result = "Продукт успешно удален из категории";
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
