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
    public class ProductAPIController : Controller
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;

        public ProductAPIController(IProductRepository productRepository, UserManager<User> userManager)
        {
            this._response = new ResponseDto();
            _productRepository = productRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<object> GetProducts()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProductsAsync();
                _response.Result = productDtos;
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
        [Route("favorites")]
        [Authorize]
        public async Task<object> GetFavoriteProducts()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                IEnumerable<ProductDto> favoriteProducts = await _productRepository.GetFavoriteProductsAsync(userId);
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

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<object> Get(int id)
        {
            try
            {
                ProductDto productDto = await _productRepository.GetProductByIdAsync(id);
                _response.Result = productDto;
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
                await _productRepository.ToggleFavoriteStatusAsync(id, userId);
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

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromForm] ProductDto productDto)
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
                    ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                    _response.Result = model;
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

        [HttpPut]
        [Authorize]
        public async Task<object> Put([FromBody] ProductDto productDto)
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
                    ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                    _response.Result = model;
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

        [HttpGet]
        [Route("by-category/{categoryId}")]
        public async Task<object> GetProductsByCategory(int categoryId)
        {
            try
            {
                IEnumerable<ProductDto> productsByCategory = await _productRepository.GetProductsByCategoryId(categoryId);
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
                    await _productRepository.AddProductToCategoryAsync(productId, categoryId);
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
                    await _productRepository.RemoveProductFromCategoryAsync(productId, categoryId);
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

        [HttpDelete]
        [Authorize]
        public async Task<object> Delete(int id)
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
                    bool isSuccess = await _productRepository.DeleteProduct(id);
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
                    bool isSuccess = await _productRepository.RemoveFromFavoritesAsync(id);
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
