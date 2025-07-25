using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;
using ProjectQuizard.Services;

namespace ProjectQuizard.Helpers
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(QuizardContext context, IAuthenticationService authService)
        {
            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Check if data already exists
                if (await context.Users.AnyAsync())
                    return; // Data already seeded

                // Create Subjects
                var subjects = new List<Subject>
                {
                    new Subject { SubjectName = "Mathematics" },
                    new Subject { SubjectName = "Science" },
                    new Subject { SubjectName = "History" },
                    new Subject { SubjectName = "English" },
                    new Subject { SubjectName = "Geography" },
                    new Subject { SubjectName = "Computer Science" }
                };

                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();

                // Create sample users
                var teacher1 = new User
                {
                    Username = "teacher1",
                    Email = "teacher1@example.com",
                    FullName = "John Teacher",
                    Role = "Teacher"
                };

                var teacher2 = new User
                {
                    Username = "teacher2", 
                    Email = "teacher2@example.com",
                    FullName = "Jane Educator",
                    Role = "Teacher"
                };

                var student1 = new User
                {
                    Username = "student1",
                    Email = "student1@example.com",
                    FullName = "Alice Student",
                    Role = "Student"
                };

                var student2 = new User
                {
                    Username = "student2",
                    Email = "student2@example.com", 
                    FullName = "Bob Learner",
                    Role = "Student"
                };

                // Register users with default password "123456"
                await authService.RegisterAsync(teacher1, "123456");
                await authService.RegisterAsync(teacher2, "123456");
                await authService.RegisterAsync(student1, "123456");
                await authService.RegisterAsync(student2, "123456");

                // Get the registered users with their IDs
                var registeredTeacher1 = await context.Users.FirstAsync(u => u.Username == "teacher1");
                var registeredTeacher2 = await context.Users.FirstAsync(u => u.Username == "teacher2");
                var mathSubject = subjects.First(s => s.SubjectName == "Mathematics");
                var scienceSubject = subjects.First(s => s.SubjectName == "Science");

                // Create sample quizzes
                var quiz1 = new Quiz
                {
                    Title = "Basic Algebra",
                    Description = "Test your knowledge of basic algebraic concepts",
                    CreatedBy = registeredTeacher1.UserId,
                    SubjectId = mathSubject.SubjectId,
                    IsPublic = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var quiz2 = new Quiz
                {
                    Title = "Introduction to Physics",
                    Description = "Basic physics concepts and principles",
                    CreatedBy = registeredTeacher2.UserId,
                    SubjectId = scienceSubject.SubjectId,
                    IsPublic = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Quizzes.AddRange(quiz1, quiz2);
                await context.SaveChangesAsync();

                // Create sample questions for quiz1
                var question1 = new Question
                {
                    QuizId = quiz1.QuizId,
                    QuestionText = "What is 2 + 2?",
                    QuestionOrder = 1
                };

                var question2 = new Question
                {
                    QuizId = quiz1.QuizId,
                    QuestionText = "Solve for x: 2x + 4 = 10",
                    QuestionOrder = 2
                };

                context.Questions.AddRange(question1, question2);
                await context.SaveChangesAsync();

                // Create options for question1
                context.QuestionOptions.AddRange(
                    new QuestionOption { QuestionId = question1.QuestionId, OptionText = "3", IsCorrect = false },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionText = "4", IsCorrect = true },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionText = "5", IsCorrect = false },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionText = "6", IsCorrect = false }
                );

                // Create options for question2
                context.QuestionOptions.AddRange(
                    new QuestionOption { QuestionId = question2.QuestionId, OptionText = "x = 2", IsCorrect = false },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionText = "x = 3", IsCorrect = true },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionText = "x = 4", IsCorrect = false },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionText = "x = 5", IsCorrect = false }
                );

                await context.SaveChangesAsync();

                Console.WriteLine("Database seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                throw;
            }
        }
    }
}