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
    public class CategoryAPIController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        protected ResponseDto _response;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CategoryAPIController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryAPIController(ICategoryRepository categoryRepository, UserManager<User> userManger, ILogger<CategoryAPIController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _categoryRepository = categoryRepository;
            this._response = new ResponseDto();
            _userManager = userManger;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                IEnumerable<CategoryDto> categories = await _categoryRepository.GetCategoriesAsync();
                _response.Result = categories;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при попытке получения модели списка категорий");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };

                return StatusCode(500, _response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                CategoryDto category = await _categoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { "Категория не найдена" };
                    return StatusCode(404, _response);
                }

                _response.Result = category;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при попытке получения модели категории по id");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };

                return StatusCode(500, _response);
            }
        }

        [Authorize]
        [HttpPost("AddCategory")]
        public async Task<object> AddCategory([FromForm] CategoryDto categoryDto, IFormFile categoryImage)
        {
            try
            {
                _logger.LogInformation("Начало добавления модели категории");
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
                    _logger.LogInformation("Добавление модели категории");
                    var categoryImageFileName = await SaveCategoryImage(categoryImage);
                    categoryDto.CategoryImage = categoryImageFileName;
                    CategoryDto addedCategory = await _categoryRepository.AddCategoryAsync(categoryDto);
                    _response.Result = addedCategory;

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
                _logger.LogError(ex, "Ошибка при попытке создания модели категории");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [Authorize]
        [HttpPut("UpdateCategory")]
        public async Task<object> UpdateCategory([FromForm] CategoryDto categoryDto, IFormFile categoryImage)
        {
            try
            {
                _logger.LogInformation("Начало обновления модели категории");
                _logger.LogInformation("Проверка пользователя");
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                if (await _userManager.IsInRoleAsync(user, Constants.AdminRoleName))
                {
                    _logger.LogInformation("Обновление модели категории");
                    var categoryImageFileName = await SaveCategoryImage(categoryImage);
                    categoryDto.CategoryImage = categoryImageFileName;
                    CategoryDto updateCategory = await _categoryRepository.UpdateCategory(categoryDto);
                    _response.Result = updateCategory;

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
                _logger.LogError(ex, "Ошибка при попытке смены имени модели категории");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCategory(int id)
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
                    bool isRemoved = await _categoryRepository.RemoveCategoryAsync(id);

                    if (!isRemoved)
                    {
                        _response.IsSuccess = false;
                        _response.Result = "Категория не найдена";
                        return StatusCode(404, _response);
                    }

                    _response.Result = isRemoved;
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
                _logger.LogError(ex, "Ошибка при попытке удаления модели категории");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };

                return StatusCode(500, _response);
            }
        }

        private async Task<string> SaveCategoryImage(IFormFile productImage)
        {
            if (productImage == null || productImage.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "CategoryImageFiles");
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
    }
}
