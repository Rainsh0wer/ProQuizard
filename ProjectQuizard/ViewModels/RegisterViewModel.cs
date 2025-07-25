using ProjectQuizard.Helpers;
using ProjectQuizard.Models;
using ProjectQuizard.Services;
using ProjectQuizard.Views.Student;
using ProjectQuizard.Views.Teacher;
using ProjectQuizard.Views.LoginRegister;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ProjectQuizard.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;
        
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _selectedRole = "Student";
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public RegisterViewModel(IAuthenticationService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            
            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
            ShowLoginCommand = new RelayCommand(ShowLogin);
            
            Roles = new List<string> { "Student", "Teacher" };
        }

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        [Required(ErrorMessage = "Please confirm your password")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public List<string> Roles { get; }

        public ICommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Password) && 
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !IsLoading;
        }

        private async Task RegisterAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate inputs
                if (!IsValidEmail(Email))
                {
                    ErrorMessage = "Please enter a valid email address.";
                    return;
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters long.";
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return;
                }

                var user = new User
                {
                    Username = Username.Trim(),
                    Email = Email.Trim().ToLower(),
                    FullName = FullName.Trim(),
                    Role = SelectedRole
                };

                var success = await _authService.RegisterAsync(user, Password);
                
                if (success)
                {
                    // Navigate based on user role
                    if (SelectedRole == "Student")
                    {
                        _navigationService.NavigateToWindow<StudentDashboard>();
                    }
                    else if (SelectedRole == "Teacher")
                    {
                        _navigationService.NavigateToWindow<TeacherDashboard>();
                    }
                }
                else
                {
                    ErrorMessage = "Registration failed. Username or email may already exist.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowLogin()
        {
            _navigationService.NavigateToWindow<LoginWindow>();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}