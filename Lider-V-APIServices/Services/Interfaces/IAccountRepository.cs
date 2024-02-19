using Lider_V_APIServices.Models;

namespace Lider_V_APIServices.Services.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> GenerateJwtTokenByUser(User user);
    }
}
