using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectQuizard.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<(Type ViewType, object? Parameter)> _navigationHistory = new();
        private Frame? _mainFrame;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanGoBack => _navigationHistory.Count > 1;

        public event EventHandler<NavigationEventArgs>? Navigated;

        public void SetMainFrame(Frame frame)
        {
            _mainFrame = frame;
        }

        public void NavigateTo<T>() where T : UserControl, new()
        {
            NavigateTo<T>(null);
        }

        public void NavigateTo<T>(object parameter) where T : UserControl, new()
        {
            if (_mainFrame == null)
                throw new InvalidOperationException("Main frame not set. Call SetMainFrame first.");

            var view = new T();
            
            // Set DataContext if view model exists
            var viewModelType = GetViewModelType(typeof(T));
            if (viewModelType != null)
            {
                var viewModel = _serviceProvider.GetService(viewModelType);
                if (viewModel != null)
                {
                    view.DataContext = viewModel;
                    
                    // If parameter is provided and view model has SetParameter method
                    if (parameter != null)
                    {
                        var setParameterMethod = viewModelType.GetMethod("SetParameter");
                        setParameterMethod?.Invoke(viewModel, new[] { parameter });
                    }
                }
            }

            _mainFrame.Navigate(view);
            _navigationHistory.Push((typeof(T), parameter));
            
            Navigated?.Invoke(this, new NavigationEventArgs 
            { 
                ViewType = typeof(T), 
                Parameter = parameter 
            });
        }

        public void NavigateToWindow<T>() where T : Window, new()
        {
            NavigateToWindow<T>(null);
        }

        public void NavigateToWindow<T>(object parameter) where T : Window, new()
        {
            var window = new T();
            
            // Set DataContext if view model exists
            var viewModelType = GetViewModelType(typeof(T));
            if (viewModelType != null)
            {
                var viewModel = _serviceProvider.GetService(viewModelType);
                if (viewModel != null)
                {
                    window.DataContext = viewModel;
                    
                    // If parameter is provided and view model has SetParameter method
                    if (parameter != null)
                    {
                        var setParameterMethod = viewModelType.GetMethod("SetParameter");
                        setParameterMethod?.Invoke(viewModel, new[] { parameter });
                    }
                }
            }

            // Close current main window and show new one
            Application.Current.MainWindow?.Close();
            Application.Current.MainWindow = window;
            window.Show();
        }

        public void ShowDialog<T>() where T : Window, new()
        {
            ShowDialog<T>(null);
        }

        public void ShowDialog<T>(object parameter) where T : Window, new()
        {
            var window = new T();
            
            // Set DataContext if view model exists
            var viewModelType = GetViewModelType(typeof(T));
            if (viewModelType != null)
            {
                var viewModel = _serviceProvider.GetService(viewModelType);
                if (viewModel != null)
                {
                    window.DataContext = viewModel;
                    
                    // If parameter is provided and view model has SetParameter method
                    if (parameter != null)
                    {
                        var setParameterMethod = viewModelType.GetMethod("SetParameter");
                        setParameterMethod?.Invoke(viewModel, new[] { parameter });
                    }
                }
            }

            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            _navigationHistory.Pop(); // Remove current
            var (viewType, parameter) = _navigationHistory.Peek();
            
            // Use reflection to call NavigateTo with the correct type
            var method = GetType().GetMethod(nameof(NavigateTo), new[] { typeof(object) });
            var genericMethod = method?.MakeGenericMethod(viewType);
            genericMethod?.Invoke(this, new[] { parameter });
        }

        private Type? GetViewModelType(Type viewType)
        {
            var viewName = viewType.Name;
            var viewModelName = viewName.Replace("View", "ViewModel").Replace("Window", "ViewModel");
            var viewModelFullName = $"{viewType.Namespace?.Replace("Views", "ViewModels")}.{viewModelName}";
            
            return Type.GetType(viewModelFullName);
        }
    }
}