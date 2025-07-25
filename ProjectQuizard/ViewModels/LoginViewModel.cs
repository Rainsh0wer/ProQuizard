using ProjectQuizard.Helpers;
using ProjectQuizard.Services;
using ProjectQuizard.Views.Student;
using ProjectQuizard.Views.Teacher;
using ProjectQuizard.Views.LoginRegister;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

namespace ProjectQuizard.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;
        
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public LoginViewModel(IAuthenticationService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            ShowRegisterCommand = new RelayCommand(ShowRegister);
            ShowForgotPasswordCommand = new RelayCommand(ShowForgotPassword);
        }

        [Required(ErrorMessage = "Username is required")]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        [Required(ErrorMessage = "Password is required")]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ShowForgotPasswordCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password) && 
                   !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var user = await _authService.LoginAsync(Username, Password);
                
                if (user != null)
                {
                    // Navigate based on user role
                    if (user.Role == "Student")
                    {
                        _navigationService.NavigateToWindow<StudentDashboard>();
                    }
                    else if (user.Role == "Teacher")
                    {
                        _navigationService.NavigateToWindow<TeacherDashboard>();
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowRegister()
        {
            _navigationService.NavigateToWindow<RegisterWindow>();
        }

        private void ShowForgotPassword()
        {
            // TODO: Implement forgot password functionality
            MessageBox.Show("Forgot password functionality will be implemented soon.", "Info", 
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}