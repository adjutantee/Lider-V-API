namespace Lider_V_APIServices.Models.Dto
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
