namespace Lider_V_APIServices.Models.Dto
{
    public class UserInfoDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
