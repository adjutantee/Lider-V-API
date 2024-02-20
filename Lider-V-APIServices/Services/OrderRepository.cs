using AutoMapper;
using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lider_V_APIServices.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public OrderRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> GetAllOrdersAsync()
        {
            var allOrders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(allOrders);
        }

        public async Task<object> GetOrderDetailsAsync(int orderId, string userId)
        {
            var order = await _context.Orders
            .Where(o => o.Id == orderId && (o.UserId == userId || userId == Constants.AdminRoleName)) // Доступ только для пользователя или админа
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync();

            return _mapper.Map<OrderDto>(order); ;
        }

        public async Task<object> GetUserOrdersAsync(string userId)
        {
            var userOrders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(userOrders);
        }

        public async Task<bool> PlaceOrderAsync(string userId, OrderDto orderDto)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var order = _mapper.Map<OrderDto, Order>(orderDto);
                order.UserId = userId;
                order.OrderDate = DateTime.UtcNow;
                order.Status = Models.OrderStatus.Accepted;

                foreach (var orderItemDto in orderDto.OrderItemsDto)
                {
                    var product = await _context.Products.FindAsync(orderItemDto.ProductId);

                    if (product == null || product.ProductQuantity < orderItemDto.Quantity)
                    {
                        transaction.Rollback();
                        return false; // Недостаточное количество товара в наличии
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = orderItemDto.ProductId,
                        Quantity = orderItemDto.Quantity,
                        TotalPrice = product.ProductPrice * orderItemDto.Quantity
                    };

                    order.OrderItems.Add(orderItem);
                    product.ProductQuantity -= orderItemDto.Quantity;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatusDto newStatusDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    return false; // Заказ не найден
                }

                order.Status = (OrderStatus)newStatusDto;
                await _context.SaveChangesAsync();

                return true; // Статус успешно обновлен
            }
            catch (Exception)
            {
                return false; // Возникла ошибка при обновлении статуса
            }
        }
    }
}
