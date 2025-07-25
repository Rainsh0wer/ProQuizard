using Microsoft.EntityFrameworkCore;
using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly QuizardContext _context;

        public TeacherService(QuizardContext context)
        {
            _context = context;
        }

        public async Task<Classroom> CreateClassroomAsync(string className, int teacherId)
        {
            var classroom = new Classroom
            {
                ClassName = className,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow
            };

            // Generate unique class code
            classroom.ClassCode = GenerateClassCode(0); // Will be updated after getting ID

            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();

            // Update class code with actual ID
            classroom.ClassCode = GenerateClassCode(classroom.ClassroomId);
            await _context.SaveChangesAsync();

            return classroom;
        }

        public async Task<List<Classroom>> GetTeacherClassroomsAsync(int teacherId)
        {
            return await _context.Classrooms
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AssignQuizToClassAsync(int quizId, int classId, DateTime? dueDate = null)
        {
            try
            {
                var assignment = new QuizAssignment
                {
                    QuizId = quizId,
                    ClassroomId = classId,
                    AssignedAt = DateTime.UtcNow,
                    DueDate = dueDate
                };

                _context.QuizAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AssignQuizToStudentAsync(int quizId, int studentId, DateTime? dueDate = null)
        {
            try
            {
                var assignment = new QuizAssignment
                {
                    QuizId = quizId,
                    StudentId = studentId,
                    AssignedAt = DateTime.UtcNow,
                    DueDate = dueDate
                };

                _context.QuizAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<StudentQuiz>> GetClassProgressAsync(int classId, int quizId)
        {
            // Get all students in the class
            var classStudents = await _context.Enrollments
                .Where(e => e.ClassroomId == classId)
                .Select(e => e.StudentId)
                .ToListAsync();

            return await _context.StudentQuizzes
                .Include(sq => sq.Student)
                .Include(sq => sq.Quiz)
                .Where(sq => sq.QuizId == quizId && classStudents.Contains(sq.StudentId))
                .OrderByDescending(sq => sq.Score)
                .ThenBy(sq => sq.Student.FullName)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetClassStudentsAsync(int classId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.ClassroomId == classId)
                .OrderBy(e => e.Student.FullName)
                .ToListAsync();
        }

        public string GenerateClassCode(int classId)
        {
            // Generate a simple class code format: CLASS + ID + Random letters
            var random = new Random();
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var randomSuffix = new string(Enumerable.Repeat(letters, 2)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            return $"CLASS{classId:D3}{randomSuffix}";
        }

        public async Task<bool> RemoveStudentFromClassAsync(int classId, int studentId)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.ClassroomId == classId && e.StudentId == studentId);

                if (enrollment == null) return false;

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteClassroomAsync(int classId)
        {
            try
            {
                var classroom = await _context.Classrooms.FindAsync(classId);
                if (classroom == null) return false;

                // Remove all enrollments first
                var enrollments = await _context.Enrollments
                    .Where(e => e.ClassroomId == classId)
                    .ToListAsync();
                _context.Enrollments.RemoveRange(enrollments);

                // Remove all quiz assignments for this class
                var assignments = await _context.QuizAssignments
                    .Where(qa => qa.ClassroomId == classId)
                    .ToListAsync();
                _context.QuizAssignments.RemoveRange(assignments);

                // Remove classroom
                _context.Classrooms.Remove(classroom);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            try
            {
                var existingClassroom = await _context.Classrooms.FindAsync(classroom.ClassroomId);
                if (existingClassroom == null) return false;

                existingClassroom.ClassName = classroom.ClassName;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<QuizAssignment>> GetTeacherAssignmentsAsync(int teacherId)
        {
            // Get assignments for teacher's classrooms
            var teacherClassrooms = await _context.Classrooms
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ClassroomId)
                .ToListAsync();

            var classAssignments = await _context.QuizAssignments
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(qa => qa.Classroom)
                .Where(qa => qa.ClassroomId.HasValue && teacherClassrooms.Contains(qa.ClassroomId.Value))
                .ToListAsync();

            // Get direct assignments from teacher's quizzes
            var teacherQuizzes = await _context.Quizzes
                .Where(q => q.CreatedBy == teacherId)
                .Select(q => q.QuizId)
                .ToListAsync();

            var directAssignments = await _context.QuizAssignments
                .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Subject)
                .Include(qa => qa.Student)
                .Where(qa => qa.StudentId.HasValue && teacherQuizzes.Contains(qa.QuizId))
                .ToListAsync();

            return classAssignments.Concat(directAssignments)
                .OrderByDescending(qa => qa.AssignedAt)
                .ToList();
        }

        public async Task<bool> UpdateAssignmentDueDateAsync(int assignmentId, DateTime? newDueDate)
        {
            try
            {
                var assignment = await _context.QuizAssignments.FindAsync(assignmentId);
                if (assignment == null) return false;

                assignment.DueDate = newDueDate;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int assignmentId)
        {
            try
            {
                var assignment = await _context.QuizAssignments.FindAsync(assignmentId);
                if (assignment == null) return false;

                _context.QuizAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<StudentQuiz>> GetQuizResultsAsync(int quizId)
        {
            return await _context.StudentQuizzes
                .Include(sq => sq.Student)
                .Include(sq => sq.Quiz)
                .Where(sq => sq.QuizId == quizId && sq.IsCompleted == true)
                .OrderByDescending(sq => sq.Score)
                .ThenBy(sq => sq.EndTime)
                .ToListAsync();
        }

        public async Task<Dictionary<string, object>> GetQuizStatisticsAsync(int quizId)
        {
            var completedQuizzes = await _context.StudentQuizzes
                .Where(sq => sq.QuizId == quizId && sq.IsCompleted == true)
                .ToListAsync();

            var statistics = new Dictionary<string, object>();

            if (completedQuizzes.Any())
            {
                statistics["TotalAttempts"] = completedQuizzes.Count;
                statistics["AverageScore"] = completedQuizzes.Average(sq => sq.Score ?? 0);
                statistics["HighestScore"] = completedQuizzes.Max(sq => sq.Score ?? 0);
                statistics["LowestScore"] = completedQuizzes.Min(sq => sq.Score ?? 0);
                
                // Score distribution
                var scoreRanges = new Dictionary<string, int>
                {
                    ["90-100"] = completedQuizzes.Count(sq => (sq.Score ?? 0) >= 90),
                    ["80-89"] = completedQuizzes.Count(sq => (sq.Score ?? 0) >= 80 && (sq.Score ?? 0) < 90),
                    ["70-79"] = completedQuizzes.Count(sq => (sq.Score ?? 0) >= 70 && (sq.Score ?? 0) < 80),
                    ["60-69"] = completedQuizzes.Count(sq => (sq.Score ?? 0) >= 60 && (sq.Score ?? 0) < 70),
                    ["Below 60"] = completedQuizzes.Count(sq => (sq.Score ?? 0) < 60)
                };
                statistics["ScoreDistribution"] = scoreRanges;

                // Average completion time
                var completionTimes = completedQuizzes
                    .Where(sq => sq.StartTime.HasValue && sq.EndTime.HasValue)
                    .Select(sq => (sq.EndTime!.Value - sq.StartTime!.Value).TotalMinutes)
                    .ToList();

                if (completionTimes.Any())
                {
                    statistics["AverageCompletionTimeMinutes"] = completionTimes.Average();
                }
            }
            else
            {
                statistics["TotalAttempts"] = 0;
                statistics["AverageScore"] = 0;
                statistics["HighestScore"] = 0;
                statistics["LowestScore"] = 0;
                statistics["ScoreDistribution"] = new Dictionary<string, int>();
            }

            return statistics;
        }

        public async Task<List<User>> SearchStudentsAsync(string keyword)
        {
            return await _context.Users
                .Where(u => u.Role == "Student" && u.IsActive == true &&
                           (u.Username.Contains(keyword) || 
                            u.FullName != null && u.FullName.Contains(keyword) ||
                            u.Email.Contains(keyword)))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}