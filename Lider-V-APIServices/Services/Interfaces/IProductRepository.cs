using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface IProductRepository
    {
        // Product model
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateUptateProductAsync(ProductDto productDto);
        Task<bool> DecreaseProductStock(int productId, int quantity);
        Task<bool> DeleteProduct(int id);

        // ProductCategory model
        Task<IEnumerable<ProductDto>> GetProductsByCategoryId(int categoryId);
        Task AddProductToCategoryAsync(int productId, int categoryId);
        Task<bool> RemoveProductFromCategoryAsync(int productId, int categoryId);

        // Favorite model
        Task<IEnumerable<ProductDto>> GetFavoriteProductsAsync(string userId);
        Task ToggleFavoriteStatusAsync(int productId, string userId);
        Task<bool> RemoveFromFavoritesAsync(int productId);
    }
}
