using ProjectQuizard.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectQuizard
{
    public class SampleDataCreator
    {
        public static async Task CreateSampleDataAsync()
        {
            using var context = new QuizardContext();

            // Check if data already exists
            if (context.Users.Any())
            {
                Console.WriteLine("Sample data already exists.");
                return;
            }

            try
            {
                // Create subjects
                var mathSubject = new Subject
                {
                    Name = "Toán học",
                    Description = "Môn toán học cơ bản",
                    CreatedAt = DateTime.Now
                };

                var englishSubject = new Subject
                {
                    Name = "Tiếng Anh",
                    Description = "Môn tiếng Anh cơ bản",
                    CreatedAt = DateTime.Now
                };

                context.Subjects.AddRange(mathSubject, englishSubject);
                await context.SaveChangesAsync();

                // Create users
                var teacher = new User
                {
                    Username = "teacher1",
                    Email = "teacher@example.com",
                    PasswordHash = "123456", // Simple password for demo
                    FullName = "Nguyễn Văn Giáo",
                    Role = "Teacher",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var student1 = new User
                {
                    Username = "student1",
                    Email = "student1@example.com",
                    PasswordHash = "123456",
                    FullName = "Trần Thị Học",
                    Role = "Student",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var student2 = new User
                {
                    Username = "student2",
                    Email = "student2@example.com",
                    PasswordHash = "123456",
                    FullName = "Lê Văn Sinh",
                    Role = "Student",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Users.AddRange(teacher, student1, student2);
                await context.SaveChangesAsync();

                // Create a sample quiz
                var mathQuiz = new Quiz
                {
                    Title = "Toán học lớp 10 - Chương 1",
                    Description = "Bài kiểm tra về hàm số và đồ thị",
                    SubjectId = mathSubject.SubjectId,
                    CreatedBy = teacher.UserId,
                    IsPublic = true,
                    CreatedAt = DateTime.Now
                };

                context.Quizzes.Add(mathQuiz);
                await context.SaveChangesAsync();

                // Create sample questions
                var question1 = new Question
                {
                    QuizId = mathQuiz.QuizId,
                    Content = "Hàm số y = 2x + 3 có đồ thị là đường thẳng có hệ số góc bằng bao nhiêu?",
                    CorrectOption = "A",
                    Explanation = "Hệ số góc của đường thẳng y = ax + b là a, trong trường hợp này a = 2",
                    CreatedAt = DateTime.Now
                };

                var question2 = new Question
                {
                    QuizId = mathQuiz.QuizId,
                    Content = "Giá trị của hàm số f(x) = x² - 4x + 3 tại x = 2 là?",
                    CorrectOption = "C",
                    Explanation = "f(2) = 2² - 4(2) + 3 = 4 - 8 + 3 = -1",
                    CreatedAt = DateTime.Now
                };

                context.Questions.AddRange(question1, question2);
                await context.SaveChangesAsync();

                // Create question options
                var q1Options = new[]
                {
                    new QuestionOption { QuestionId = question1.QuestionId, OptionLabel = "A", Content = "2" },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionLabel = "B", Content = "3" },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionLabel = "C", Content = "5" },
                    new QuestionOption { QuestionId = question1.QuestionId, OptionLabel = "D", Content = "1" }
                };

                var q2Options = new[]
                {
                    new QuestionOption { QuestionId = question2.QuestionId, OptionLabel = "A", Content = "3" },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionLabel = "B", Content = "1" },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionLabel = "C", Content = "-1" },
                    new QuestionOption { QuestionId = question2.QuestionId, OptionLabel = "D", Content = "0" }
                };

                context.QuestionOptions.AddRange(q1Options);
                context.QuestionOptions.AddRange(q2Options);
                await context.SaveChangesAsync();

                Console.WriteLine("Sample data created successfully!");
                Console.WriteLine("Teacher account: teacher1 / 123456");
                Console.WriteLine("Student accounts: student1 / 123456, student2 / 123456");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sample data: {ex.Message}");
            }
        }
    }
}