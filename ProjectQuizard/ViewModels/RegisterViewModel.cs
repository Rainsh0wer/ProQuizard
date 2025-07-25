using ProjectQuizard.Helpers;
using ProjectQuizard.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ProjectQuizard.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _fullName = string.Empty;
        private string _selectedRole = "Student";
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new RelayCommand(async () => await Register(), () => !IsLoading);
            CancelCommand = new RelayCommand(Cancel);
            
            Roles = new List<string> { "Student", "Teacher" };
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
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
        public ICommand CancelCommand { get; }

        private async System.Threading.Tasks.Task Register()
        {
            if (string.IsNullOrWhiteSpace(Username) || 
                string.IsNullOrWhiteSpace(Email) || 
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var success = await _authService.RegisterAsync(Username, Email, Password, SelectedRole, FullName);
                
                if (success)
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Close register window
                    Application.Current.Windows[^1]?.Close();
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc email đã tồn tại.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi đăng ký: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Cancel()
        {
            Application.Current.Windows[^1]?.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}