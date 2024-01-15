using System.ComponentModel.DataAnnotations;

namespace Lider_V_APIServices.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public double ProductWeight { get; set; }
        public int ProductQuantity { get; set; }
    }
}
