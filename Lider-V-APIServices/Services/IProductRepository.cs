using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateUptateProductAsync(ProductDto productDto);
        Task<IEnumerable<ProductDto>> GetFavoriteProductsAsync(string userId);
        Task ToggleFavoriteStatusAsync(int productId, string userId);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryId(int categoryId);
        Task AddProductToCategoryAsync(int productId, int categoryId);
        Task<bool> RemoveProductFromCategoryAsync(int productId, int categoryId);
        Task<bool> RemoveFromFavoritesAsync (int productId);
        Task<bool> DeleteProduct(int id);
    }
}
