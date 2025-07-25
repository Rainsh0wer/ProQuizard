using ProjectQuizard.ViewModels;
using System.Windows;

namespace ProjectQuizard.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            
            // Handle password binding since PasswordBox doesn't support direct binding
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (DataContext is RegisterViewModel viewModel)
                {
                    viewModel.Password = PasswordBox.Password;
                }
            };
        }
    }
}