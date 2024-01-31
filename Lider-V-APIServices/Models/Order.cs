using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lider_V_APIServices.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingPhone { get; set; }
        public string? ShippingEmail { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public enum OrderStatus
    {
        Accepted,
        Processing,
        Shipped,
        Delivered
    }
}
