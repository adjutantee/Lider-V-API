namespace Lider_V_APIServices.Models
{
    public class UserInfo : User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
