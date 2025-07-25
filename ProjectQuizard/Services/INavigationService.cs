using System.Windows.Controls;
using System.Windows;

namespace ProjectQuizard.Services
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : UserControl, new();
        void NavigateTo<T>(object parameter) where T : UserControl, new();
        void NavigateToWindow<T>() where T : Window, new();
        void NavigateToWindow<T>(object parameter) where T : Window, new();
        void ShowDialog<T>() where T : Window, new();
        void ShowDialog<T>(object parameter) where T : Window, new();
        void GoBack();
        bool CanGoBack { get; }
        
        event EventHandler<NavigationEventArgs>? Navigated;
    }

    public class NavigationEventArgs : EventArgs
    {
        public Type? ViewType { get; set; }
        public object? Parameter { get; set; }
    }
}