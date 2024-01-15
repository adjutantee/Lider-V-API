using Microsoft.AspNetCore.Identity;

namespace Lider_V_APIServices.Models
{
    public class User : IdentityUser
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
