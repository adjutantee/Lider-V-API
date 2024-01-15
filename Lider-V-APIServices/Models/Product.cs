using System.ComponentModel.DataAnnotations;

namespace Lider_V_APIServices.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public byte[] ProductImage { get; set; }
        public double ProductWeight { get; set; }
        public int ProductQuantity { get; set; }
        public bool IsFavorite { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
