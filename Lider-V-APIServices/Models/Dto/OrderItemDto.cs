namespace Lider_V_APIServices.Models.Dto
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        //public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderId { get; set; }
        //public Order Order { get; set; }
    }
}
