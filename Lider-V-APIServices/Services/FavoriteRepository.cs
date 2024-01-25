using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Lider_V_APIServices.Services
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public FavoriteRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetFavoriteProductsAsync(string userId)
        {
            var favoriteProducts = await _context.UserFavoriteProducts
                .Where(ufp => ufp.UserId == userId)
                .Include(ufp => ufp.Product)
                .Select(ufp => _mapper.Map<ProductDto>(ufp.Product))
                .ToListAsync();

            return favoriteProducts;
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

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ToggleFavoriteStatusAsync(int productId, string userId)
        {
            var userFavoriteProduct = await _context.UserFavoriteProducts
                .FirstOrDefaultAsync(ufp => ufp.ProductId == productId && ufp.UserId == userId);

            if (userFavoriteProduct != null)
            {
                _context.UserFavoriteProducts.Remove(userFavoriteProduct);
            }
            else
            {
                _context.UserFavoriteProducts.Add(new UserFavoriteProduct
                {
                    UserId = userId,
                    ProductId = productId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
