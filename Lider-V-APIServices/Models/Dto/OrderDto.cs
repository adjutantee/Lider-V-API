namespace Lider_V_APIServices.Models.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        //public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public string? ShippingEmail { get; set; }
        public OrderStatusDto StatusDto { get; set; }
        public ICollection<OrderItemDto> OrderItemsDto { get; set; }
    }

    public enum OrderStatusDto
    {
        Accepted,
        Processing,
        Shipped,
        Delivered
    }
}
