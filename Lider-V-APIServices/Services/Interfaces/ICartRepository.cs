using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface ICartRepository
    {
        Task<bool> AddToCartAsync(int productId, int quantity, string userId);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> GetCartByIdAsync(int cartId);
        Task<bool> ClearCartAsync(string userId);
    }
}
