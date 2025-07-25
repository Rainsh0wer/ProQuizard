using ProjectQuizard.Models;

namespace ProjectQuizard.Services
{
    public interface ITeacherService
    {
        Task<Classroom> CreateClassroomAsync(string className, int teacherId);
        Task<List<Classroom>> GetTeacherClassroomsAsync(int teacherId);
        Task<bool> AssignQuizToClassAsync(int quizId, int classId, DateTime? dueDate = null);
        Task<bool> AssignQuizToStudentAsync(int quizId, int studentId, DateTime? dueDate = null);
        Task<List<StudentQuiz>> GetClassProgressAsync(int classId, int quizId);
        Task<List<Enrollment>> GetClassStudentsAsync(int classId);
        string GenerateClassCode(int classId);
        Task<bool> RemoveStudentFromClassAsync(int classId, int studentId);
        Task<bool> DeleteClassroomAsync(int classId);
        Task<bool> UpdateClassroomAsync(Classroom classroom);
        Task<List<QuizAssignment>> GetTeacherAssignmentsAsync(int teacherId);
        Task<bool> UpdateAssignmentDueDateAsync(int assignmentId, DateTime? newDueDate);
        Task<bool> DeleteAssignmentAsync(int assignmentId);
        Task<List<StudentQuiz>> GetQuizResultsAsync(int quizId);
        Task<Dictionary<string, object>> GetQuizStatisticsAsync(int quizId);
        Task<List<User>> SearchStudentsAsync(string keyword);
    }
}