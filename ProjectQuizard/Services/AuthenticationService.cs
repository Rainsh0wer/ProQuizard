using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;
using BCrypt.Net;

namespace ProjectQuizard.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly QuizardContext _context;
        private User? _currentUser;

        public AuthenticationService(QuizardContext context)
        {
            _context = context;
        }

        public User? CurrentUser 
        { 
            get => _currentUser;
            private set
            {
                _currentUser = value;
                UserChanged?.Invoke(this, value);
            }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public event EventHandler<User?>? UserChanged;

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    CurrentUser = user;
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log error
                throw new InvalidOperationException("Login failed", ex);
            }
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Username == user.Username || u.Email == user.Email);

                if (existingUser)
                    return false;

                // Hash password and set user properties
                user.PasswordHash = HashPassword(password);
                user.IsActive = true;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Auto login after registration
                CurrentUser = user;
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                throw new InvalidOperationException("Registration failed", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || !VerifyPassword(oldPassword, user.PasswordHash))
                    return false;

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                throw new InvalidOperationException("Password change failed", ex);
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}