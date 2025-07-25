using Microsoft.EntityFrameworkCore;
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
    public class StudentViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly QuizService _quizService;
        private readonly DatabaseService _databaseService;

        // Quiz List Properties
        private ObservableCollection<Quiz> _availableQuizzes = new();
        private Quiz? _selectedQuiz;

        // Quiz Taking Properties
        private Quiz? _currentQuiz;
        private StudentQuiz? _currentStudentQuiz;
        private int _currentQuestionIndex = 0;
        private Question? _currentQuestion;
        private string _selectedAnswer = string.Empty;
        private Dictionary<int, string> _answers = new();

        // Results Properties
        private StudentQuiz? _lastResult;
        private ObservableCollection<StudentAnswer> _lastAnswers = new();

        // History Properties
        private ObservableCollection<StudentQuiz> _quizHistory = new();

        // UI Properties
        private int _selectedTabIndex = 0;
        private bool _isLoading = false;

        public StudentViewModel()
        {
            _authService = new AuthService();
            _quizService = new QuizService();
            _databaseService = new DatabaseService();

            // Commands
            StartQuizCommand = new RelayCommand(async () => await StartQuiz(), () => SelectedQuiz != null);
            NextQuestionCommand = new RelayCommand(NextQuestion, CanGoNext);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, CanGoPrevious);
            SubmitQuizCommand = new RelayCommand(async () => await SubmitQuiz(), CanSubmitQuiz);
            RetakeQuizCommand = new RelayCommand(RetakeQuiz);
            LogoutCommand = new RelayCommand(Logout);

            LoadData();
        }

        #region Properties

        public ObservableCollection<Quiz> AvailableQuizzes
        {
            get => _availableQuizzes;
            set => SetProperty(ref _availableQuizzes, value);
        }

        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set => SetProperty(ref _selectedQuiz, value);
        }

        public Quiz? CurrentQuiz
        {
            get => _currentQuiz;
            set => SetProperty(ref _currentQuiz, value);
        }

        public Question? CurrentQuestion
        {
            get => _currentQuestion;
            set => SetProperty(ref _currentQuestion, value);
        }

        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set => SetProperty(ref _selectedAnswer, value);
        }

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set => SetProperty(ref _currentQuestionIndex, value);
        }

        public string QuestionProgress => CurrentQuiz != null ? 
            $"Câu {CurrentQuestionIndex + 1}/{CurrentQuiz.Questions.Count}" : "";

        public StudentQuiz? LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public ObservableCollection<StudentAnswer> LastAnswers
        {
            get => _lastAnswers;
            set => SetProperty(ref _lastAnswers, value);
        }

        public ObservableCollection<StudentQuiz> QuizHistory
        {
            get => _quizHistory;
            set => SetProperty(ref _quizHistory, value);
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

        #endregion

        #region Commands

        public ICommand StartQuizCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand SubmitQuizCommand { get; }
        public ICommand RetakeQuizCommand { get; }
        public ICommand LogoutCommand { get; }

        #endregion

        #region Methods

        private async void LoadData()
        {
            IsLoading = true;
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync();
                AvailableQuizzes = new ObservableCollection<Quiz>(quizzes);

                if (CurrentUser != null)
                {
                    var history = await _databaseService.GetStudentHistoryAsync(CurrentUser.UserId);
                    QuizHistory = new ObservableCollection<StudentQuiz>(history);
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

        private async System.Threading.Tasks.Task StartQuiz()
        {
            if (SelectedQuiz == null || CurrentUser == null) return;

            IsLoading = true;
            try
            {
                // Get full quiz with questions
                var quiz = await _quizService.GetQuizByIdAsync(SelectedQuiz.QuizId);
                if (quiz == null || !quiz.Questions.Any())
                {
                    MessageBox.Show("Quiz không có câu hỏi nào.", "Thông báo", 
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Start quiz session
                var studentQuiz = await _databaseService.StartQuizAsync(CurrentUser.UserId, quiz.QuizId);
                if (studentQuiz == null)
                {
                    MessageBox.Show("Không thể bắt đầu quiz.", "Lỗi", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentQuiz = quiz;
                _currentStudentQuiz = studentQuiz;
                CurrentQuestionIndex = 0;
                _answers.Clear();
                SelectedAnswer = string.Empty;

                LoadCurrentQuestion();
                SelectedTabIndex = 1; // Switch to quiz taking tab
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi bắt đầu quiz: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadCurrentQuestion()
        {
            if (CurrentQuiz?.Questions != null && CurrentQuestionIndex < CurrentQuiz.Questions.Count)
            {
                CurrentQuestion = CurrentQuiz.Questions.ElementAt(CurrentQuestionIndex);
                
                // Load existing answer if any
                if (_answers.ContainsKey(CurrentQuestion.QuestionId))
                {
                    SelectedAnswer = _answers[CurrentQuestion.QuestionId];
                }
                else
                {
                    SelectedAnswer = string.Empty;
                }
                
                OnPropertyChanged(nameof(QuestionProgress));
            }
        }

        private void NextQuestion()
        {
            SaveCurrentAnswer();
            if (CurrentQuestionIndex < (CurrentQuiz?.Questions.Count ?? 0) - 1)
            {
                CurrentQuestionIndex++;
                LoadCurrentQuestion();
            }
        }

        private void PreviousQuestion()
        {
            SaveCurrentAnswer();
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                LoadCurrentQuestion();
            }
        }

        private void SaveCurrentAnswer()
        {
            if (CurrentQuestion != null && !string.IsNullOrWhiteSpace(SelectedAnswer))
            {
                _answers[CurrentQuestion.QuestionId] = SelectedAnswer;
            }
        }

        private async System.Threading.Tasks.Task SubmitQuiz()
        {
            if (_currentStudentQuiz == null || CurrentQuiz == null) return;

            SaveCurrentAnswer(); // Save current answer before submitting

            var result = MessageBox.Show("Bạn có chắc muốn nộp bài?", "Xác nhận", 
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                // Save all answers
                foreach (var answer in _answers)
                {
                    await _databaseService.SaveAnswerAsync(_currentStudentQuiz.StudentQuizId, 
                                                         answer.Key, answer.Value);
                }

                // Finish quiz and calculate score
                var finishedQuiz = await _databaseService.FinishQuizAsync(_currentStudentQuiz.StudentQuizId);
                if (finishedQuiz != null)
                {
                    LastResult = finishedQuiz;
                    
                    // Load detailed results
                    var detailedResult = await _databaseService.GetStudentQuizWithAnswersAsync(finishedQuiz.StudentQuizId);
                    if (detailedResult?.StudentAnswers != null)
                    {
                        LastAnswers = new ObservableCollection<StudentAnswer>(detailedResult.StudentAnswers);
                    }

                    SelectedTabIndex = 2; // Switch to results tab
                    
                    // Refresh history
                    if (CurrentUser != null)
                    {
                        var history = await _databaseService.GetStudentHistoryAsync(CurrentUser.UserId);
                        QuizHistory = new ObservableCollection<StudentQuiz>(history);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nộp bài: {ex.Message}", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RetakeQuiz()
        {
            if (LastResult?.Quiz != null)
            {
                SelectedQuiz = LastResult.Quiz;
                SelectedTabIndex = 0; // Go back to quiz list
            }
        }

        private void Logout()
        {
            _authService.Logout();
            
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
            
            Application.Current.MainWindow?.Close();
        }

        private bool CanGoNext()
        {
            return CurrentQuiz != null && CurrentQuestionIndex < CurrentQuiz.Questions.Count - 1;
        }

        private bool CanGoPrevious()
        {
            return CurrentQuestionIndex > 0;
        }

        private bool CanSubmitQuiz()
        {
            return CurrentQuiz != null && _currentStudentQuiz != null;
        }

        #endregion
    }
}