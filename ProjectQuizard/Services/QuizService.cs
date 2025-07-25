using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public class QuizService : IQuizService
    {
        private readonly QuizardContext _context;

        public QuizService(QuizardContext context)
        {
            _context = context;
        }

        public async Task<List<Quiz>> GetPublicQuizzesAsync()
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.QuizLikes)
                .Where(q => q.IsPublic == true && q.IsActive == true)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Quiz>> GetQuizzesBySubjectAsync(int subjectId)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.QuizLikes)
                .Where(q => q.SubjectId == subjectId && q.IsPublic == true && q.IsActive == true)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizWithQuestionsAsync(int quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.Questions)
                    .ThenInclude(qu => qu.QuestionOptions)
                .Include(q => q.QuizLikes)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            quiz.CreatedAt = DateTime.UtcNow;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.IsActive = true;

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> UpdateQuizAsync(Quiz quiz)
        {
            try
            {
                var existingQuiz = await _context.Quizzes.FindAsync(quiz.QuizId);
                if (existingQuiz == null) return false;

                existingQuiz.Title = quiz.Title;
                existingQuiz.Description = quiz.Description;
                existingQuiz.SubjectId = quiz.SubjectId;
                existingQuiz.IsPublic = quiz.IsPublic;
                existingQuiz.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(quizId);
                if (quiz == null) return false;

                // Soft delete
                quiz.IsActive = false;
                quiz.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Quiz>> GetQuizzesByTeacherAsync(int teacherId)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.QuizLikes)
                .Include(q => q.StudentQuizzes)
                .Where(q => q.CreatedBy == teacherId && q.IsActive == true)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Quiz>> SearchQuizzesAsync(string keyword)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.QuizLikes)
                .Where(q => q.IsPublic == true && q.IsActive == true &&
                           (q.Title.Contains(keyword) || 
                            q.Description != null && q.Description.Contains(keyword) ||
                            q.Subject.SubjectName.Contains(keyword)))
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<bool> LikeQuizAsync(int userId, int quizId)
        {
            try
            {
                var existingLike = await _context.QuizLikes
                    .FirstOrDefaultAsync(ql => ql.UserId == userId && ql.QuizId == quizId);

                if (existingLike != null) return false; // Already liked

                var quizLike = new QuizLike
                {
                    UserId = userId,
                    QuizId = quizId,
                    LikedAt = DateTime.UtcNow
                };

                _context.QuizLikes.Add(quizLike);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnlikeQuizAsync(int userId, int quizId)
        {
            try
            {
                var quizLike = await _context.QuizLikes
                    .FirstOrDefaultAsync(ql => ql.UserId == userId && ql.QuizId == quizId);

                if (quizLike == null) return false;

                _context.QuizLikes.Remove(quizLike);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsQuizLikedAsync(int userId, int quizId)
        {
            return await _context.QuizLikes
                .AnyAsync(ql => ql.UserId == userId && ql.QuizId == quizId);
        }

        public async Task<int> GetQuizLikesCountAsync(int quizId)
        {
            return await _context.QuizLikes
                .CountAsync(ql => ql.QuizId == quizId);
        }

        public async Task<List<Quiz>> GetPopularQuizzesAsync(int count = 10)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.QuizLikes)
                .Include(q => q.StudentQuizzes)
                .Where(q => q.IsPublic == true && q.IsActive == true)
                .OrderByDescending(q => q.QuizLikes.Count)
                .ThenByDescending(q => q.StudentQuizzes.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Quiz>> GetRecentQuizzesAsync(int count = 10)
        {
            return await _context.Quizzes
                .Include(q => q.Subject)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.QuizLikes)
                .Where(q => q.IsPublic == true && q.IsActive == true)
                .OrderByDescending(q => q.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}