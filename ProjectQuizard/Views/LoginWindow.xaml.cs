using ProjectQuizard.ViewModels;
using System.Windows;

namespace ProjectQuizard.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            
            // Handle password binding since PasswordBox doesn't support direct binding
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.Password = PasswordBox.Password;
                }
            };
        }
    }
}