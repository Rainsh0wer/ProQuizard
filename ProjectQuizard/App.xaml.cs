using System.Configuration;
using System.Data;
using System.Windows;

namespace ProjectQuizard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Create sample data if needed
            await SampleDataCreator.CreateSampleDataAsync();
        }
    }

}
