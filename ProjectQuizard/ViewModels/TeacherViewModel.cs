using ProjectQuizard.Helpers;
using ProjectQuizard.Models;
using ProjectQuizard.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProjectQuizard.ViewModels
{
    public class TeacherViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly QuizService _quizService;
        private readonly DatabaseService _databaseService;

        // Quiz Management Properties
        private ObservableCollection<Quiz> _myQuizzes = new();
        private Quiz? _selectedQuiz;
        private Quiz? _currentEditingQuiz;

        // Question Management Properties
        private ObservableCollection<Question> _currentQuestions = new();
        private Question? _selectedQuestion;
        private Question? _currentEditingQuestion;

        // Form Properties
        private string _quizTitle = string.Empty;
        private string _quizDescription = string.Empty;
        private Subject? _selectedSubject;
        private ObservableCollection<Subject> _subjects = new();

        // Question Form Properties
        private string _questionContent = string.Empty;
        private string _optionA = string.Empty;
        private string _optionB = string.Empty;
        private string _optionC = string.Empty;
        private string _optionD = string.Empty;
        private string _correctOption = "A";
        private string _explanation = string.Empty;

        // Statistics Properties
        private ObservableCollection<StudentQuiz> _quizStatistics = new();

        // UI Properties
        private int _selectedTabIndex = 0;
        private bool _isLoading = false;

        public TeacherViewModel()
        {
            _authService = new AuthService();
            _quizService = new QuizService();
            _databaseService = new DatabaseService();

            // Commands
            CreateQuizCommand = new RelayCommand(CreateNewQuiz);
            EditQuizCommand = new RelayCommand(EditQuiz, () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(async () => await DeleteQuiz(), () => SelectedQuiz != null);
            SaveQuizCommand = new RelayCommand(async () => await SaveQuiz(), CanSaveQuiz);
            
            AddQuestionCommand = new RelayCommand(AddNewQuestion, () => CurrentEditingQuiz != null);
            EditQuestionCommand = new RelayCommand(EditQuestion, () => SelectedQuestion != null);
            DeleteQuestionCommand = new RelayCommand(async () => await DeleteQuestion(), () => SelectedQuestion != null);
            SaveQuestionCommand = new RelayCommand(async () => await SaveQuestion(), CanSaveQuestion);
            
            ViewStatisticsCommand = new RelayCommand(async () => await ViewStatistics(), () => SelectedQuiz != null);
            LogoutCommand = new RelayCommand(Logout);

            LoadData();
        }

        #region Properties

        public ObservableCollection<Quiz> MyQuizzes
        {
            get => _myQuizzes;
            set => SetProperty(ref _myQuizzes, value);
        }

        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set => SetProperty(ref _selectedQuiz, value);
        }

        public Quiz? CurrentEditingQuiz
        {
            get => _currentEditingQuiz;
            set => SetProperty(ref _currentEditingQuiz, value);
        }

        public ObservableCollection<Question> CurrentQuestions
        {
            get => _currentQuestions;
            set => SetProperty(ref _currentQuestions, value);
        }

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        public Question? CurrentEditingQuestion
        {
            get => _currentEditingQuestion;
            set => SetProperty(ref _currentEditingQuestion, value);
        }

        public string QuizTitle
        {
            get => _quizTitle;
            set => SetProperty(ref _quizTitle, value);
        }

        public string QuizDescription
        {
            get => _quizDescription;
            set => SetProperty(ref _quizDescription, value);
        }

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public string QuestionContent
        {
            get => _questionContent;
            set => SetProperty(ref _questionContent, value);
        }

        public string OptionA
        {
            get => _optionA;
            set => SetProperty(ref _optionA, value);
        }

        public string OptionB
        {
            get => _optionB;
            set => SetProperty(ref _optionB, value);
        }

        public string OptionC
        {
            get => _optionC;
            set => SetProperty(ref _optionC, value);
        }

        public string OptionD
        {
            get => _optionD;
            set => SetProperty(ref _optionD, value);
        }

        public string CorrectOption
        {
            get => _correctOption;
            set => SetProperty(ref _correctOption, value);
        }

        public string Explanation
        {
            get => _explanation;
            set => SetProperty(ref _explanation, value);
        }

        public ObservableCollection<StudentQuiz> QuizStatistics
        {
            get => _quizStatistics;
            set => SetProperty(ref _quizStatistics, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public User? CurrentUser => _authService.CurrentUser;

        public List<string> CorrectOptions => new() { "A", "B", "C", "D" };

        #endregion

        #region Commands

        public ICommand CreateQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand SaveQuizCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand SaveQuestionCommand { get; }
        public ICommand ViewStatisticsCommand { get; }
        public ICommand LogoutCommand { get; }

        #endregion

        #region Methods

        private async void LoadData()
        {
            IsLoading = true;
            try
            {
                var subjects = await _quizService.GetAllSubjectsAsync();
                Subjects = new ObservableCollection<Subject>(subjects);

                if (CurrentUser != null)
                {
                    var quizzes = await _quizService.GetQuizzesByTeacherAsync(CurrentUser.UserId);
                    MyQuizzes = new ObservableCollection<Quiz>(quizzes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CreateNewQuiz()
        {
            CurrentEditingQuiz = new Quiz();
            QuizTitle = string.Empty;
            QuizDescription = string.Empty;
            SelectedSubject = Subjects.FirstOrDefault();
            CurrentQuestions = new ObservableCollection<Question>();
            SelectedTabIndex = 1; // Switch to create/edit tab
        }

        private void EditQuiz()
        {
            if (SelectedQuiz == null) return;

            CurrentEditingQuiz = SelectedQuiz;
            QuizTitle = SelectedQuiz.Title;
            QuizDescription = SelectedQuiz.Description ?? string.Empty;
            SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectId == SelectedQuiz.SubjectId);
            CurrentQuestions = new ObservableCollection<Question>(SelectedQuiz.Questions);
            SelectedTabIndex = 1; // Switch to create/edit tab
        }

        private async System.Threading.Tasks.Task DeleteQuiz()
        {
            if (SelectedQuiz == null) return;

            var result = MessageBox.Show($"Bạn có chắc muốn xóa quiz '{SelectedQuiz.Title}'?", 
                                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                var success = await _quizService.DeleteQuizAsync(SelectedQuiz.QuizId);
                if (success)
                {
                    MyQuizzes.Remove(SelectedQuiz);
                    SelectedQuiz = null;
                    MessageBox.Show("Xóa quiz thành công!", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa quiz.", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa quiz: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async System.Threading.Tasks.Task SaveQuiz()
        {
            if (CurrentEditingQuiz == null || SelectedSubject == null || CurrentUser == null) return;

            IsLoading = true;
            try
            {
                CurrentEditingQuiz.Title = QuizTitle;
                CurrentEditingQuiz.Description = QuizDescription;
                CurrentEditingQuiz.SubjectId = SelectedSubject.SubjectId;
                CurrentEditingQuiz.CreatedBy = CurrentUser.UserId;

                Quiz? savedQuiz;
                if (CurrentEditingQuiz.QuizId == 0)
                {
                    // Create new quiz
                    savedQuiz = await _quizService.CreateQuizAsync(CurrentEditingQuiz);
                    if (savedQuiz != null)
                    {
                        MyQuizzes.Add(savedQuiz);
                        CurrentEditingQuiz = savedQuiz;
                    }
                }
                else
                {
                    // Update existing quiz
                    var success = await _quizService.UpdateQuizAsync(CurrentEditingQuiz);
                    savedQuiz = success ? CurrentEditingQuiz : null;
                    
                    if (success)
                    {
                        // Update in list
                        var index = MyQuizzes.ToList().FindIndex(q => q.QuizId == CurrentEditingQuiz.QuizId);
                        if (index >= 0)
                        {
                            MyQuizzes[index] = CurrentEditingQuiz;
                        }
                    }
                }

                if (savedQuiz != null)
                {
                    MessageBox.Show("Lưu quiz thành công!", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể lưu quiz.", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu quiz: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddNewQuestion()
        {
            CurrentEditingQuestion = new Question();
            ClearQuestionForm();
            SelectedTabIndex = 2; // Switch to question edit tab
        }

        private void EditQuestion()
        {
            if (SelectedQuestion == null) return;

            CurrentEditingQuestion = SelectedQuestion;
            QuestionContent = SelectedQuestion.Content;
            Explanation = SelectedQuestion.Explanation ?? string.Empty;
            CorrectOption = SelectedQuestion.CorrectOption;

            var options = SelectedQuestion.QuestionOptions.ToList();
            OptionA = options.FirstOrDefault(o => o.OptionLabel == "A")?.Content ?? string.Empty;
            OptionB = options.FirstOrDefault(o => o.OptionLabel == "B")?.Content ?? string.Empty;
            OptionC = options.FirstOrDefault(o => o.OptionLabel == "C")?.Content ?? string.Empty;
            OptionD = options.FirstOrDefault(o => o.OptionLabel == "D")?.Content ?? string.Empty;

            SelectedTabIndex = 2; // Switch to question edit tab
        }

        private async System.Threading.Tasks.Task DeleteQuestion()
        {
            if (SelectedQuestion == null) return;

            var result = MessageBox.Show("Bạn có chắc muốn xóa câu hỏi này?", 
                                        "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                var success = await _quizService.DeleteQuestionAsync(SelectedQuestion.QuestionId);
                if (success)
                {
                    CurrentQuestions.Remove(SelectedQuestion);
                    SelectedQuestion = null;
                    MessageBox.Show("Xóa câu hỏi thành công!", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa câu hỏi.", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa câu hỏi: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async System.Threading.Tasks.Task SaveQuestion()
        {
            if (CurrentEditingQuestion == null || CurrentEditingQuiz == null) return;

            IsLoading = true;
            try
            {
                CurrentEditingQuestion.Content = QuestionContent;
                CurrentEditingQuestion.CorrectOption = CorrectOption;
                CurrentEditingQuestion.Explanation = Explanation;
                CurrentEditingQuestion.QuizId = CurrentEditingQuiz.QuizId;

                Question? savedQuestion;
                if (CurrentEditingQuestion.QuestionId == 0)
                {
                    // Create new question
                    savedQuestion = await _quizService.CreateQuestionAsync(CurrentEditingQuestion);
                    if (savedQuestion != null)
                    {
                        CurrentQuestions.Add(savedQuestion);
                    }
                }
                else
                {
                    // Update existing question
                    var success = await _quizService.UpdateQuestionAsync(CurrentEditingQuestion);
                    savedQuestion = success ? CurrentEditingQuestion : null;
                    
                    if (success)
                    {
                        // Update in list
                        var index = CurrentQuestions.ToList().FindIndex(q => q.QuestionId == CurrentEditingQuestion.QuestionId);
                        if (index >= 0)
                        {
                            CurrentQuestions[index] = CurrentEditingQuestion;
                        }
                    }
                }

                if (savedQuestion != null)
                {
                    // Save question options
                    await SaveQuestionOptions(savedQuestion);
                    
                    MessageBox.Show("Lưu câu hỏi thành công!", "Thành công", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearQuestionForm();
                    SelectedTabIndex = 1; // Go back to quiz edit tab
                }
                else
                {
                    MessageBox.Show("Không thể lưu câu hỏi.", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu câu hỏi: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private System.Threading.Tasks.Task SaveQuestionOptions(Question question)
        {
            // This is a simplified version - in a real app, you'd want to properly manage QuestionOptions
            // For now, we'll assume the options are managed elsewhere or simplified
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private async System.Threading.Tasks.Task ViewStatistics()
        {
            if (SelectedQuiz == null) return;

            IsLoading = true;
            try
            {
                var statistics = await _databaseService.GetQuizStatisticsAsync(SelectedQuiz.QuizId);
                QuizStatistics = new ObservableCollection<StudentQuiz>(statistics);
                SelectedTabIndex = 3; // Switch to statistics tab
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thống kê: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearQuestionForm()
        {
            QuestionContent = string.Empty;
            OptionA = string.Empty;
            OptionB = string.Empty;
            OptionC = string.Empty;
            OptionD = string.Empty;
            CorrectOption = "A";
            Explanation = string.Empty;
        }

        private void Logout()
        {
            _authService.Logout();
            
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            
            Application.Current.MainWindow?.Close();
        }

        private bool CanSaveQuiz()
        {
            return !string.IsNullOrWhiteSpace(QuizTitle) && SelectedSubject != null;
        }

        private bool CanSaveQuestion()
        {
            return !string.IsNullOrWhiteSpace(QuestionContent) &&
                   !string.IsNullOrWhiteSpace(OptionA) &&
                   !string.IsNullOrWhiteSpace(OptionB) &&
                   !string.IsNullOrWhiteSpace(OptionC) &&
                   !string.IsNullOrWhiteSpace(OptionD);
        }

        #endregion
    }
}