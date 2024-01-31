using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Lider_V_APIServices.Services
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;
        public CartRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity, string userId)
        {
            try
            {
                var getProductById = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

                if (getProductById == null)
                {
                    return false;
                }

                var userCart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCart == null)
                {
                    userCart = new Cart
                    {
                        UserId = userId,
                        CartItems = new List<CartItem>()
                    };
                }

                var existingCartItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (existingCartItem != null)
                {
                    // Если товар уже есть в корзине, увеличиваем количество
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // Если товара еще нет в корзине, добавляем новый элемент
                    userCart.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart != null)
                {
                    _context.CartItems.RemoveRange(cart.CartItems);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var userCart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart != null)
            {
                var cartDto = _mapper.Map<CartDto>(userCart);
                return cartDto;
            }

            return null;
        }

        public async Task<CartDto> GetCartByIdAsync(int cartId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartId);

                if (cart != null)
                {
                    var cartDto = _mapper.Map<CartDto>(cart);
                    return cartDto;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);

                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
