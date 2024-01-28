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
    public class CategoryAPIController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        protected ResponseDto _response;
        private readonly UserManager<User> _userManager;

        public CategoryAPIController(ICategoryRepository categoryRepository, UserManager<User> userManger)
        {
            _categoryRepository = categoryRepository;
            this._response = new ResponseDto();
            _userManager = userManger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
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
                    IEnumerable<CategoryDto> categories = await _categoryRepository.GetCategoriesAsync();
                    _response.Result = categories;
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
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
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
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
                    CategoryDto addedCategory = await _categoryRepository.AddCategoryAsync(categoryDto);
                    _response.Result = addedCategory;
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
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
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
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
