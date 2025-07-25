using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ProjectQuizard.Models;
using ProjectQuizard.Services;
using ProjectQuizard.Views.LoginRegister;
using ProjectQuizard.Helpers;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ProjectQuizard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            _serviceProvider = services.BuildServiceProvider();
            ServiceProvider = _serviceProvider;

            // Seed database
            Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<QuizardContext>();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                    await DataSeeder.SeedAsync(context, authService);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error initializing database: {ex.Message}", "Database Error", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            // Show login window
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Database Context
            services.AddDbContext<QuizardContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DB")));

            // Services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();

            // Windows
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
