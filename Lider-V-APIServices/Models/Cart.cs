using System.ComponentModel.DataAnnotations;

namespace Lider_V_APIServices.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

        //public Cart()
        //{
        //    CartItems = new List<CartItem>();
        //}
    }
}
