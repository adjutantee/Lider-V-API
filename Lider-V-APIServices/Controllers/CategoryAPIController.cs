using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lider_V_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryAPIController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        protected ResponseDto _response;

        public CategoryAPIController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            this._response = new ResponseDto();
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
                CategoryDto addedCategory = await _categoryRepository.AddCategoryAsync(categoryDto);
                _response.Result = addedCategory;
                return StatusCode(200, _response);
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
                bool isRemoved = await _categoryRepository.RemoveCategoryAsync(id);

                if (!isRemoved)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string> { "Category not found" };
                    return StatusCode(404, _response);
                }

                _response.Result = isRemoved;
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
