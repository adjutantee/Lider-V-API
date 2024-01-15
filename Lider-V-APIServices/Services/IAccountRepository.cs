using Lider_V_APIServices.Models;

namespace Lider_V_APIServices.Services
{
    public interface IAccountRepository
    {
        Task<string> GenerateJwtTokenByUser(User user);
    }
}
