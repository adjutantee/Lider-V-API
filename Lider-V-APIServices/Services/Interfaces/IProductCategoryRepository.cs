using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsByCategoryId(int categoryId);
        Task AddProductToCategoryAsync(int productId, int categoryId);
        Task<bool> RemoveProductFromCategoryAsync(int productId, int categoryId);
    }
}
