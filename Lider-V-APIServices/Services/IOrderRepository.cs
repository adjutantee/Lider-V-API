using Lider_V_APIServices.Models.Dto;

namespace Lider_V_APIServices.Services
{
    public interface IOrderRepository
    {
        Task<object> GetUserOrdersAsync(string userId);
        Task<object> GetAllOrdersAsync();
        Task<object> GetOrderDetailsAsync(int orderId, string userId);
        Task<bool> PlaceOrderAsync(string userId, OrderDto orderDto);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatusDto newStatusDto);
    }
}
