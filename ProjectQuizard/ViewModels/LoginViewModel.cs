using ProjectQuizard.Helpers;
using ProjectQuizard.Models;
using ProjectQuizard.Services;
using ProjectQuizard.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace ProjectQuizard.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async () => await Login(), () => !IsLoading);
            RegisterCommand = new RelayCommand(ShowRegisterWindow);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

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
        public ICommand RegisterCommand { get; }

        private async System.Threading.Tasks.Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.LoginAsync(Username, Password);
                
                if (user != null)
                {
                    // Navigate to appropriate window based on role
                    Window currentWindow = Application.Current.MainWindow;
                    
                    if (user.Role.ToLower() == "student")
                    {
                        var studentWindow = new StudentMainWindow();
                        studentWindow.Show();
                    }
                    else if (user.Role.ToLower() == "teacher")
                    {
                        var teacherWindow = new TeacherMainWindow();
                        teacherWindow.Show();
                    }
                    
                    currentWindow?.Close();
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowRegisterWindow()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}