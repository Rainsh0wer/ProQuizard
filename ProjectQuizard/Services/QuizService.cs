using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectQuizard.Services
{
    public class QuizService
    {
        private readonly QuizardContext _context;

        public QuizService()
        {
            _context = new QuizardContext();
        }

        public async Task<List<Quiz>> GetAllQuizzesAsync()
        {
            try
            {
                return await _context.Quizzes
                    .Include(q => q.Subject)
                    .Include(q => q.Questions)
                    .Where(q => q.IsPublic == true)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Quiz>();
            }
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
            try
            {
                return await _context.Quizzes
                    .Include(q => q.Subject)
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync(q => q.QuizId == quizId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Quiz?> CreateQuizAsync(Quiz quiz)
        {
            try
            {
                quiz.CreatedAt = DateTime.Now;
                quiz.IsPublic = true;
                
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                
                return quiz;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateQuizAsync(Quiz quiz)
        {
            try
            {
                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(quizId);
                if (quiz != null)
                {
                    _context.Quizzes.Remove(quiz);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Quiz>> GetQuizzesByTeacherAsync(int teacherId)
        {
            try
            {
                return await _context.Quizzes
                    .Include(q => q.Subject)
                    .Include(q => q.Questions)
                    .Where(q => q.CreatedBy == teacherId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Quiz>();
            }
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            try
            {
                return await _context.Subjects.ToListAsync();
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }

        public async Task<Question?> CreateQuestionAsync(Question question)
        {
            try
            {
                question.CreatedAt = DateTime.Now;
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                return question;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            try
            {
                _context.Questions.Update(question);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            try
            {
                var question = await _context.Questions.FindAsync(questionId);
                if (question != null)
                {
                    _context.Questions.Remove(question);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}