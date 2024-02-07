using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lider_V_APIServices.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductSize{ get; set; }
        public int? ProductQuantity { get; set; }
        public int? CategoryId { get; set; }
        public ICollection<UserFavoriteProduct> UserFavorites { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
