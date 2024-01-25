using System.ComponentModel.DataAnnotations;

namespace Lider_V_APIServices.Models.Dto
{
    public class CartItemDto
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
