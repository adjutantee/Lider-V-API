using System.ComponentModel.DataAnnotations;

namespace Lider_V_APIServices.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
