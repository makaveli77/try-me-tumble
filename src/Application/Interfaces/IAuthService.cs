using TryMeTumble.Domain.Entities;

namespace TryMeTumble.Application.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}