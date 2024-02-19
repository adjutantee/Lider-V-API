using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<ProductDto>> GetFavoriteProductsAsync(string userId);
        Task ToggleFavoriteStatusAsync(int productId, string userId);
        Task<bool> RemoveFromFavoritesAsync(int productId);
    }
}
