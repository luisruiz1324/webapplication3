using TimeClock.Domain;

namespace TimeClock.Services
{
    public interface IIdentityService 
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password, bool isAdmin);
        Task<AuthenticationResult> LoginAsync(string email, string password);
    }
}
