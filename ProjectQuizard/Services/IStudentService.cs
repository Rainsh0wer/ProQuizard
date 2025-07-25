using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public interface IStudentService
    {
        Task<StudentQuiz> StartQuizAsync(int studentId, int quizId);
        Task<bool> SubmitAnswerAsync(int studentQuizId, int questionId, string selectedOption);
        Task<StudentQuiz> FinishQuizAsync(int studentQuizId);
        Task<List<StudentQuiz>> GetStudentHistoryAsync(int studentId);
        Task<bool> SaveQuizAsync(int studentId, int quizId);
        Task<bool> UnsaveQuizAsync(int studentId, int quizId);
        Task<List<Quiz>> GetSavedQuizzesAsync(int studentId);
        Task<bool> IsQuizSavedAsync(int studentId, int quizId);
        Task<bool> JoinClassroomAsync(int studentId, string classCode);
        Task<List<Classroom>> GetStudentClassroomsAsync(int studentId);
        Task<List<QuizAssignment>> GetStudentAssignmentsAsync(int studentId);
        Task<StudentQuiz?> GetStudentQuizAsync(int studentQuizId);
        Task<List<StudentAnswer>> GetStudentAnswersAsync(int studentQuizId);
        Task<bool> HasCompletedQuizAsync(int studentId, int quizId);
    }
}