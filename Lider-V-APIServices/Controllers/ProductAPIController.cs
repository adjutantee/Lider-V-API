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
    public class ProductAPIController : Controller
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ProductAPIController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductAPIController(IProductRepository productRepository, UserManager<User> userManager, ILogger<ProductAPIController> logger, IWebHostEnvironment webHostEnvironment)
        {
            this._response = new ResponseDto();
            _productRepository = productRepository;
            _userManager = userManager;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Product section

        [HttpGet]
        public async Task<object> GetProducts()
        {
            try
            {
                _logger.LogInformation("Запрос списка продуктов");
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProductsAsync();
                _response.Result = productDtos;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка продуктов");
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
                _logger.LogInformation("Запрос продукта по Id");
                ProductDto productDto = await _productRepository.GetProductByIdAsync(id);
                _response.Result = productDto;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении продукта по Id");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        public async Task<object> Post([FromForm] ProductDto productDto, IFormFile productImage)
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


                if (await _userManager.IsInRoleAsync(user, Constants.AdminRoleName))
                {
                    _logger.LogInformation("Добавление модели продукта");
                    var productImageFileName = await SaveProductImage(productImage);
                    productDto.ProductImage = productImageFileName;
                    ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                    _response.Result = model;

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
                _logger.LogError(ex, "Ошибка при попытке создания модели продукта");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPut]
        public async Task<object> Put([FromForm] ProductDto productDto, IFormFile productImage)
        {
            try
            {
                _logger.LogInformation("Начало обновления модели продукта");
                _logger.LogInformation("Проверка пользователя");
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
                    var productImageFileName = await SaveProductImage(productImage);

                    // Обновляем DTO с учетом ссылки на изображение
                    productDto.ProductImage = productImageFileName;

                    _logger.LogInformation("Обновление модели продукта");
                    ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                    _response.Result = model;

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
                _logger.LogError(ex, "Ошибка при попытке обновить модель продукта");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Начало удаления модели продукта");
                _logger.LogInformation("Проверка пользователя");
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
                    _logger.LogInformation("Удаление модели продукта");
                    bool isSuccess = await _productRepository.DeleteProduct(id);
                    _response.Result = isSuccess;

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
                _logger.LogError(ex, "Ошибка при попытке удалить модель продукта");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        private async Task<string> SaveProductImage(IFormFile productImage)
        {
            if (productImage == null || productImage.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImageFiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + productImage.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await productImage.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        #endregion

        #region ProductCategory section

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
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
                    return StatusCode(403, _response);
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
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
                    return StatusCode(403, _response);
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

        #endregion

        #region Favorite section

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

                bool isSuccess = await _productRepository.RemoveFromFavoritesAsync(id);
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

        #endregion

    }
}
