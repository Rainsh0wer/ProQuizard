using ProjectQuizard.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProjectQuizard.Views
{
    public partial class StudentMainWindow : Window
    {
        public StudentMainWindow()
        {
            InitializeComponent();
            
            // Subscribe to DataContext changes to handle question loading
            DataContextChanged += (s, e) =>
            {
                if (DataContext is StudentViewModel viewModel)
                {
                    viewModel.PropertyChanged += ViewModel_PropertyChanged;
                }
            };
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StudentViewModel.CurrentQuestion))
            {
                LoadQuestionOptions();
            }
            else if (e.PropertyName == nameof(StudentViewModel.SelectedAnswer))
            {
                UpdateSelectedAnswer();
            }
        }

        private void LoadQuestionOptions()
        {
            if (DataContext is StudentViewModel viewModel && viewModel.CurrentQuestion != null)
            {
                var options = viewModel.CurrentQuestion.QuestionOptions.ToList();
                
                rbA.Content = options.Count > 0 ? $"A. {options.FirstOrDefault(o => o.OptionLabel == "A")?.Content ?? ""}" : "A. ";
                rbB.Content = options.Count > 1 ? $"B. {options.FirstOrDefault(o => o.OptionLabel == "B")?.Content ?? ""}" : "B. ";
                rbC.Content = options.Count > 2 ? $"C. {options.FirstOrDefault(o => o.OptionLabel == "C")?.Content ?? ""}" : "C. ";
                rbD.Content = options.Count > 3 ? $"D. {options.FirstOrDefault(o => o.OptionLabel == "D")?.Content ?? ""}" : "D. ";
                
                // Clear selection
                rbA.IsChecked = false;
                rbB.IsChecked = false;
                rbC.IsChecked = false;
                rbD.IsChecked = false;
            }
        }

        private void UpdateSelectedAnswer()
        {
            if (DataContext is StudentViewModel viewModel)
            {
                rbA.IsChecked = viewModel.SelectedAnswer == "A";
                rbB.IsChecked = viewModel.SelectedAnswer == "B";
                rbC.IsChecked = viewModel.SelectedAnswer == "C";
                rbD.IsChecked = viewModel.SelectedAnswer == "D";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && DataContext is StudentViewModel viewModel)
            {
                viewModel.SelectedAnswer = radioButton.Tag?.ToString() ?? "";
            }
        }
    }
}