using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lider_V_APIServices.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public CategoryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryDto> AddCategoryAsync(CategoryDto categoryDto)
        {
            Category category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategory(CategoryDto categoryDto)
        {
            Category category = _mapper.Map<CategoryDto, Category>(categoryDto);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            Category category = await _context.Categories.FindAsync(categoryId);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> RemoveCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
