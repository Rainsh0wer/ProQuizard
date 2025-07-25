using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectQuizard.Services
{
    public class DatabaseService
    {
        private readonly QuizardContext _context;

        public DatabaseService()
        {
            _context = new QuizardContext();
        }

        public async Task<StudentQuiz?> StartQuizAsync(int studentId, int quizId)
        {
            try
            {
                var studentQuiz = new StudentQuiz
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    StartedAt = DateTime.Now,
                    Score = null // Will be calculated when finished
                };

                _context.StudentQuizzes.Add(studentQuiz);
                await _context.SaveChangesAsync();

                return studentQuiz;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> SaveAnswerAsync(int studentQuizId, int questionId, string selectedOption)
        {
            try
            {
                // Check if answer already exists for this question
                var existingAnswer = await _context.StudentAnswers
                    .FirstOrDefaultAsync(a => a.StudentQuizId == studentQuizId && a.QuestionId == questionId);

                // Get the correct answer for this question
                var question = await _context.Questions.FindAsync(questionId);
                if (question == null) return false;

                bool isCorrect = question.CorrectOption == selectedOption;

                if (existingAnswer != null)
                {
                    // Update existing answer
                    existingAnswer.SelectedOption = selectedOption;
                    existingAnswer.IsCorrect = isCorrect;
                    existingAnswer.AnsweredAt = DateTime.Now;
                }
                else
                {
                    // Create new answer
                    var answer = new StudentAnswer
                    {
                        StudentQuizId = studentQuizId,
                        QuestionId = questionId,
                        SelectedOption = selectedOption,
                        IsCorrect = isCorrect,
                        AnsweredAt = DateTime.Now
                    };
                    _context.StudentAnswers.Add(answer);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<StudentQuiz?> FinishQuizAsync(int studentQuizId)
        {
            try
            {
                var studentQuiz = await _context.StudentQuizzes
                    .Include(sq => sq.StudentAnswers)
                    .Include(sq => sq.Quiz)
                        .ThenInclude(q => q.Questions)
                    .FirstOrDefaultAsync(sq => sq.StudentQuizId == studentQuizId);

                if (studentQuiz == null) return null;

                // Calculate score
                var totalQuestions = studentQuiz.Quiz.Questions.Count;
                var correctAnswers = studentQuiz.StudentAnswers.Count(a => a.IsCorrect == true);
                
                double score = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

                studentQuiz.Score = Math.Round(score, 2);
                studentQuiz.FinishedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return studentQuiz;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<StudentQuiz>> GetStudentHistoryAsync(int studentId)
        {
            try
            {
                return await _context.StudentQuizzes
                    .Include(sq => sq.Quiz)
                        .ThenInclude(q => q.Subject)
                    .Where(sq => sq.StudentId == studentId && sq.FinishedAt != null)
                    .OrderByDescending(sq => sq.FinishedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<StudentQuiz>();
            }
        }

        public async Task<StudentQuiz?> GetStudentQuizWithAnswersAsync(int studentQuizId)
        {
            try
            {
                return await _context.StudentQuizzes
                    .Include(sq => sq.Quiz)
                        .ThenInclude(q => q.Questions)
                            .ThenInclude(q => q.QuestionOptions)
                    .Include(sq => sq.StudentAnswers)
                        .ThenInclude(sa => sa.Question)
                    .FirstOrDefaultAsync(sq => sq.StudentQuizId == studentQuizId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<StudentQuiz>> GetQuizStatisticsAsync(int quizId)
        {
            try
            {
                return await _context.StudentQuizzes
                    .Include(sq => sq.Student)
                    .Where(sq => sq.QuizId == quizId && sq.FinishedAt != null)
                    .OrderByDescending(sq => sq.Score)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<StudentQuiz>();
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}