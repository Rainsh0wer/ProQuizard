using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public interface IQuizService
    {
        Task<List<Quiz>> GetPublicQuizzesAsync();
        Task<List<Quiz>> GetQuizzesBySubjectAsync(int subjectId);
        Task<Quiz?> GetQuizWithQuestionsAsync(int quizId);
        Task<Quiz> CreateQuizAsync(Quiz quiz);
        Task<bool> UpdateQuizAsync(Quiz quiz);
        Task<bool> DeleteQuizAsync(int quizId);
        Task<List<Quiz>> GetQuizzesByTeacherAsync(int teacherId);
        Task<List<Quiz>> SearchQuizzesAsync(string keyword);
        Task<List<Subject>> GetSubjectsAsync();
        Task<bool> LikeQuizAsync(int userId, int quizId);
        Task<bool> UnlikeQuizAsync(int userId, int quizId);
        Task<bool> IsQuizLikedAsync(int userId, int quizId);
        Task<int> GetQuizLikesCountAsync(int quizId);
        Task<List<Quiz>> GetPopularQuizzesAsync(int count = 10);
        Task<List<Quiz>> GetRecentQuizzesAsync(int count = 10);
    }
}