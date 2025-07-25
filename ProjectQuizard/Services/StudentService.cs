using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public class StudentService : IStudentService
    {
        private readonly QuizardContext _context;

        public StudentService(QuizardContext context)
        {
            _context = context;
        }

        public async Task<StudentQuiz> StartQuizAsync(int studentId, int quizId)
        {
            var studentQuiz = new StudentQuiz
            {
                StudentId = studentId,
                QuizId = quizId,
                StartTime = DateTime.UtcNow,
                IsCompleted = false
            };

            _context.StudentQuizzes.Add(studentQuiz);
            await _context.SaveChangesAsync();
            return studentQuiz;
        }

        public async Task<bool> SubmitAnswerAsync(int studentQuizId, int questionId, string selectedOption)
        {
            try
            {
                // Check if answer already exists
                var existingAnswer = await _context.StudentAnswers
                    .FirstOrDefaultAsync(sa => sa.StudentQuizId == studentQuizId && sa.QuestionId == questionId);

                if (existingAnswer != null)
                {
                    // Update existing answer
                    existingAnswer.SelectedOption = selectedOption;
                }
                else
                {
                    // Create new answer
                    var studentAnswer = new StudentAnswer
                    {
                        StudentQuizId = studentQuizId,
                        QuestionId = questionId,
                        SelectedOption = selectedOption
                    };
                    _context.StudentAnswers.Add(studentAnswer);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<StudentQuiz> FinishQuizAsync(int studentQuizId)
        {
            var studentQuiz = await _context.StudentQuizzes
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qu => qu.QuestionOptions)
                .Include(sq => sq.StudentAnswers)
                .FirstOrDefaultAsync(sq => sq.StudentQuizId == studentQuizId);

            if (studentQuiz == null)
                throw new ArgumentException("Student quiz not found");

            // Calculate score
            int correctAnswers = 0;
            int totalQuestions = studentQuiz.Quiz.Questions.Count;

            foreach (var question in studentQuiz.Quiz.Questions)
            {
                var correctOption = question.QuestionOptions.FirstOrDefault(o => o.IsCorrect == true);
                var studentAnswer = studentQuiz.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == question.QuestionId);

                if (correctOption != null && studentAnswer != null && 
                    studentAnswer.SelectedOption == correctOption.OptionText)
                {
                    correctAnswers++;
                }
            }

            studentQuiz.Score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
            studentQuiz.EndTime = DateTime.UtcNow;
            studentQuiz.IsCompleted = true;

            await _context.SaveChangesAsync();
            return studentQuiz;
        }

        public async Task<List<StudentQuiz>> GetStudentHistoryAsync(int studentId)
        {
            return await _context.StudentQuizzes
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(sq => sq.Quiz.CreatedByNavigation)
                .Where(sq => sq.StudentId == studentId && sq.IsCompleted == true)
                .OrderByDescending(sq => sq.EndTime)
                .ToListAsync();
        }

        public async Task<bool> SaveQuizAsync(int studentId, int quizId)
        {
            try
            {
                var existingSaved = await _context.SavedQuizzes
                    .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

                if (existingSaved != null) return false; // Already saved

                var savedQuiz = new SavedQuiz
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    SavedAt = DateTime.UtcNow
                };

                _context.SavedQuizzes.Add(savedQuiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnsaveQuizAsync(int studentId, int quizId)
        {
            try
            {
                var savedQuiz = await _context.SavedQuizzes
                    .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

                if (savedQuiz == null) return false;

                _context.SavedQuizzes.Remove(savedQuiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Quiz>> GetSavedQuizzesAsync(int studentId)
        {
            return await _context.SavedQuizzes
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(sq => sq.Quiz.CreatedByNavigation)
                .Include(sq => sq.Quiz.QuizLikes)
                .Where(sq => sq.StudentId == studentId)
                .Select(sq => sq.Quiz)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsQuizSavedAsync(int studentId, int quizId)
        {
            return await _context.SavedQuizzes
                .AnyAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

        public async Task<bool> JoinClassroomAsync(int studentId, string classCode)
        {
            try
            {
                // Find classroom by class code (assuming class code is stored in classroom name for now)
                var classroom = await _context.Classrooms
                    .FirstOrDefaultAsync(c => c.ClassCode == classCode);

                if (classroom == null) return false;

                // Check if already enrolled
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == studentId && e.ClassroomId == classroom.ClassroomId);

                if (existingEnrollment != null) return false;

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    ClassroomId = classroom.ClassroomId,
                    EnrolledAt = DateTime.UtcNow
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Classroom>> GetStudentClassroomsAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Classroom)
                    .ThenInclude(c => c.Teacher)
                .Where(e => e.StudentId == studentId)
                .Select(e => e.Classroom)
                .ToListAsync();
        }

        public async Task<List<QuizAssignment>> GetStudentAssignmentsAsync(int studentId)
        {
            // Get assignments for student's enrolled classrooms
            var studentClassrooms = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.ClassroomId)
                .ToListAsync();

            var classAssignments = await _context.QuizAssignments
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(qa => qa.Classroom)
                .Where(qa => qa.ClassroomId.HasValue && studentClassrooms.Contains(qa.ClassroomId.Value))
                .ToListAsync();

            // Get direct assignments to student
            var directAssignments = await _context.QuizAssignments
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Subject)
                .Where(qa => qa.StudentId == studentId)
                .ToListAsync();

            return classAssignments.Concat(directAssignments).ToList();
        }

        public async Task<StudentQuiz?> GetStudentQuizAsync(int studentQuizId)
        {
            return await _context.StudentQuizzes
                .Include(sq => sq.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qu => qu.QuestionOptions)
                .Include(sq => sq.Quiz.Subject)
                .Include(sq => sq.StudentAnswers)
                .FirstOrDefaultAsync(sq => sq.StudentQuizId == studentQuizId);
        }

        public async Task<List<StudentAnswer>> GetStudentAnswersAsync(int studentQuizId)
        {
            return await _context.StudentAnswers
                .Include(sa => sa.Question)
                    .ThenInclude(q => q.QuestionOptions)
                .Where(sa => sa.StudentQuizId == studentQuizId)
                .OrderBy(sa => sa.Question.QuestionId)
                .ToListAsync();
        }

        public async Task<bool> HasCompletedQuizAsync(int studentId, int quizId)
        {
            return await _context.StudentQuizzes
                .AnyAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId && sq.IsCompleted == true);
        }
    }
}