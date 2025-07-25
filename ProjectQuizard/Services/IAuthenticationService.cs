using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public interface IAuthenticationService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        void Logout();
        User? CurrentUser { get; }
        bool IsLoggedIn { get; }
        event EventHandler<User?>? UserChanged;
    }
}