using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;
using System;
using System.Threading.Tasks;

namespace ProjectQuizard.Services
{
    public class AuthService
    {
        private readonly QuizardContext _context;
        
        public User? CurrentUser { get; set; }

        public AuthService()
        {
            _context = new QuizardContext();
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && 
                                            u.PasswordHash == password && 
                                            u.IsActive == true);
                
                if (user != null)
                {
                    CurrentUser = user;
                }
                
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string role, string? fullName = null)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);
                
                if (existingUser != null)
                {
                    return false; // User already exists
                }

                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = password, // Store password directly (as requested - no encryption)
                    Role = role,
                    FullName = fullName,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}