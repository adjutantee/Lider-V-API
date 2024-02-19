using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateCategory(CategoryDto categoryDto); 
        Task<bool> RemoveCategoryAsync(int categoryId);

    }
}
