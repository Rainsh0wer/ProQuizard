using ProjectQuizard.ViewModels;
using System.Windows;

namespace ProjectQuizard.Views.LoginRegister
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}