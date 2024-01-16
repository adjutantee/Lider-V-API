using Newtonsoft.Json;

namespace Lider_V_APIServices.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string? ProductCategoryName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public byte[]? ProductImage { get; set; }
        public double? ProductWeight { get; set; }
        public int? ProductQuantity { get; set; }
        public bool IsFavorite { get; set; }
        [JsonIgnore]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
