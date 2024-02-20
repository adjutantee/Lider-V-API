using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lider_V_APIServices.Services
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public ProductCategoryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddProductToCategoryAsync(int productId, int categoryId)
        {
            Product product = await _context.Products.FindAsync(productId);
            Category category = await _context.Categories.FindAsync(categoryId);

            if (product != null && category != null)
            {
                product.CategoryId = categoryId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryId(int categoryId)
        {
            List<Product> products = await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<bool> RemoveProductFromCategoryAsync(int productId, int categoryId)
        {
            try
            {
                Product product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return false;
                }

                if (product != null && product.CategoryId == categoryId)
                {
                    product.CategoryId = null;
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
