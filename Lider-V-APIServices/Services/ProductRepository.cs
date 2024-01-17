using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
namespace Lider_V_APIServices.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public ProductRepository(ApplicationDbContext context, IMapper mapper)
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

        public async Task<ProductDto> CreateUptateProductAsync(ProductDto productDto)
        {
            Product product = _mapper.Map<ProductDto, Product>(productDto);

            if (productDto.ProductImage != null && productDto.ProductImage.Length > 0)
            {
                product.ProductImage = productDto.ProductImage;
            }

            if (product.Id > 0)
            {
                _context.Products.Update(product);
            }
            else
            {
                _context.Products.Add(product);
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetFavoriteProductsAsync()
        {
            List<Product> favoriteProducts = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(favoriteProducts);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            Product product = await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            List<Product> productList = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(productList);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryId(int categoryId)
        {
            List<Product> products = await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<bool> RemoveFromFavoritesAsync(int productId)
        {
            try
            {
                Product product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return false;
                }

                //if (product != null && product.IsFavorite)
                //{
                //    product.IsFavorite = false;
                //    await _context.SaveChangesAsync();
                //}

                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task ToggleFavoriteStatusAsync(int productId)
        {
            Product product = await _context.Products.FindAsync(productId);

            //if (product != null)
            //{
            //    product.IsFavorite = !product.IsFavorite;
            //    await _context.SaveChangesAsync();
            //}

            return;
        }
    }
}
