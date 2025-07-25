using ProjectQuizard.ViewModels;
using System.Windows;

namespace ProjectQuizard.Views.LoginRegister
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}