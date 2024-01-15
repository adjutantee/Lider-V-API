using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lider_V_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductAPIController : Controller
    {
        protected ResponseDto _response;
        private IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            this._response = new ResponseDto();
            _productRepository = productRepository;
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
        public async Task<object> GetFavoriteProducts()
        {
            try
            {
                IEnumerable<ProductDto> favoriteProducts = await _productRepository.GetFavoriteProductsAsync();
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
        public async Task<object> ToggleFavoriteStatus(int id)
        {
            try
            {
                await _productRepository.ToggleFavoriteStatusAsync(id);
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
        public async Task<object> Post([FromForm] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                _response.Result = model;
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
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.CreateUptateProductAsync(productDto);
                _response.Result = model;
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
        public async Task<object> AddProductToCategory(int productId, int categoryId)
        {
            try
            {
                await _productRepository.AddProductToCategoryAsync(productId, categoryId);
                _response.Result = "Продукт успешно добавлен в категорию";
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
        public async Task<object> RemoveProductFromCategory(int productId, int categoryId)
        {
            try
            {
                await _productRepository.RemoveProductFromCategoryAsync(productId, categoryId);
                _response.Result = "Продукт успешно удален из категории";
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
        public async Task<object> Delete(int id)
        {
            try
            {
                bool isSuccess = await _productRepository.DeleteProduct(id);
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
        [Route("delete-favorite/{id}")]
        public async Task<object> DeleteFavoriteProduct(int id)
        {
            try
            {
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
    }
}
