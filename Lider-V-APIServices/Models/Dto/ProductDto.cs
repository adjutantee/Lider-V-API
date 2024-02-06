using Newtonsoft.Json;

namespace Lider_V_APIServices.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public byte[]? ProductImage { get; set; }
        public string? Base64Image { get; set; }
        public string? ProductSize { get; set; }
        public int? ProductQuantity { get; set; }
        public int? CategoryId { get; set; }
    }
}
