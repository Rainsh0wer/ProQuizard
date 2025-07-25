# Quizard - Quiz Learning Application

**Ứng dụng C# WPF Framework 8.0 quản lý và tham gia quiz tương tự Quizlet**

## 🚀 Tổng quan

Quizard là một ứng dụng học tập thông minh được phát triển bằng C# WPF Framework 8.0, sử dụng Entity Framework Core và SQL Server. Ứng dụng hỗ trợ hai loại người dùng chính: **Học sinh** và **Giáo viên** với các tính năng phong phú và giao diện hiện đại.

## 📋 Tính năng chính

### 🔐 Authentication System
- **Đăng nhập/Đăng ký** với mã hóa mật khẩu BCrypt
- **Phân quyền role-based**: Student/Teacher
- **Validation** input với regex và DataAnnotations
- **Quản lý session** và bảo mật

### 👨‍🎓 Tính năng dành cho Học sinh
- **Dashboard cá nhân** với thống kê học tập
- **Tìm kiếm và duyệt quiz** theo môn học, độ khó
- **Tham gia quiz** với timer và auto-save
- **Xem kết quả chi tiết** với giải thích đáp án
- **Lưu quiz yêu thích** và lịch sử làm bài
- **Tham gia lớp học** bằng class code
- **Nhận quiz assignment** từ giáo viên

### 👨‍🏫 Tính năng dành cho Giáo viên
- **Tạo và quản lý quiz** với multiple choice questions
- **Tạo lớp học** và generate class code
- **Assign quiz** cho lớp hoặc học sinh cụ thể
- **Theo dõi tiến độ học sinh** với analytics chi tiết
- **Quản lý học sinh** trong lớp
- **Xem thống kê quiz** (điểm trung bình, phân phối điểm)

## 🏗️ Kiến trúc ứng dụng

### 📁 Cấu trúc project
```
ProjectQuizard/
├── Models/                    # Entity Framework Models (13 bảng)
│   ├── User.cs               # Người dùng
│   ├── Quiz.cs               # Bài quiz
│   ├── Question.cs           # Câu hỏi
│   ├── QuestionOption.cs     # Đáp án
│   ├── StudentQuiz.cs        # Phiên làm bài
│   ├── Classroom.cs          # Lớp học
│   └── QuizardContext.cs     # Database Context
├── Views/                    # XAML Views
│   ├── LoginRegister/        # Màn hình đăng nhập/đăng ký
│   ├── Student/              # Dashboard học sinh
│   └── Teacher/              # Dashboard giáo viên
├── ViewModels/               # MVVM ViewModels
│   ├── BaseViewModel.cs      # Base class với INotifyPropertyChanged
│   ├── LoginViewModel.cs     # Logic đăng nhập
│   └── RegisterViewModel.cs  # Logic đăng ký
├── Services/                 # Business Logic Services
│   ├── AuthenticationService.cs  # Xác thực
│   ├── QuizService.cs            # Quản lý quiz
│   ├── StudentService.cs         # Tính năng học sinh
│   ├── TeacherService.cs         # Tính năng giáo viên
│   └── NavigationService.cs     # Navigation
└── Helpers/                  # Utilities
    ├── RelayCommand.cs       # MVVM Commands
    ├── Converters.cs         # XAML Converters
    └── DataSeeder.cs         # Seed dữ liệu mẫu
```

### 🏛️ Architecture Pattern
- **MVVM (Model-View-ViewModel)** với WPF
- **Dependency Injection** với Microsoft.Extensions.DependencyInjection
- **Repository Pattern** thông qua Entity Framework Core
- **Service Layer** cho business logic
- **Navigation Service** cho quản lý điều hướng

### 🛡️ Security & Best Practices
- **Password hashing** với BCrypt (salt rounds: 12)
- **SQL Injection prevention** với Entity Framework parameterized queries
- **Input validation** với Data Annotations
- **Role-based authorization**
- **Async/await pattern** throughout

## 💾 Database Schema

### 13 bảng chính:
1. **Users** - Quản lý tài khoản (Teacher/Student)
2. **Subjects** - Môn học
3. **Quizzes** - Bài kiểm tra
4. **Questions** - Câu hỏi với 4 đáp án A,B,C,D
5. **QuestionOptions** - Các lựa chọn đáp án
6. **StudentQuizzes** - Phiên làm bài của học sinh
7. **StudentAnswers** - Câu trả lời cụ thể
8. **Classrooms** - Lớp học do giáo viên tạo
9. **Enrollments** - Đăng ký lớp học
10. **QuizAssignments** - Giao bài quiz
11. **Tags** - Nhãn phân loại câu hỏi
12. **QuizLikes** - Like quiz
13. **SavedQuizzes** - Lưu quiz yêu thích

### 🔗 Relationships
- **User 1:N Quiz** (Teacher tạo nhiều quiz)
- **Quiz 1:N Question** (Quiz có nhiều câu hỏi)
- **Question 1:N QuestionOption** (Câu hỏi có 4 đáp án)
- **User N:M Quiz** (Student làm nhiều quiz)
- **Classroom N:M User** (Lớp có nhiều học sinh)

## 🛠️ Technologies Stack

### Core Framework
- **.NET 8.0** - Latest LTS version
- **WPF (Windows Presentation Foundation)** - Desktop UI framework
- **C# 12** - Programming language

### Database & ORM
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database engine
- **LINQ** - Data querying

### UI/UX
- **Material Design themes** - Modern UI components
- **XAML** - Declarative UI markup
- **Data Binding** - Two-way binding với ViewModels

### Security
- **BCrypt.Net** - Password hashing
- **Data Annotations** - Input validation
- **Role-based access control**

### Architecture & DI
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **Microsoft.Extensions.Configuration** - Configuration management

## 📊 Key Features Implementation

### 🔄 Real-time Quiz Taking
```csharp
// Auto-save answers
await _studentService.SubmitAnswerAsync(studentQuizId, questionId, selectedOption);

// Calculate score real-time
var score = (correctAnswers / totalQuestions) * 100;
```

### 📈 Advanced Analytics
```csharp
// Quiz statistics for teachers
var stats = await _teacherService.GetQuizStatisticsAsync(quizId);
// Returns: Average score, completion rate, time distribution
```

### 🏫 Classroom Management
```csharp
// Generate unique class codes
string classCode = _teacherService.GenerateClassCode(classId);
// Format: CLASS001AB

// Bulk quiz assignment
await _teacherService.AssignQuizToClassAsync(quizId, classId, dueDate);
```

### 🔍 Smart Search & Filtering
```csharp
// Multi-criteria search
var quizzes = await _quizService.SearchQuizzesAsync(keyword);
var filtered = await _quizService.GetQuizzesBySubjectAsync(subjectId);
```

## 🚀 Getting Started

### Prerequisites
- **.NET 8.0 SDK**
- **SQL Server** (LocalDB/Express/Full)
- **Visual Studio 2022** hoặc **VS Code**

### Installation
1. **Clone repository**
```bash
git clone <repository-url>
cd ProjectQuizard
```

2. **Update connection string** trong `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DB": "server=localhost;database=AP;uid=sa;pwd=123;TrustServerCertificate=True"
  }
}
```

3. **Install dependencies**
```bash
dotnet restore
```

4. **Build application**
```bash
dotnet build
```

5. **Run application**
```bash
dotnet run
```

### 🎯 Demo Data
Ứng dụng tự động seed dữ liệu mẫu khi chạy lần đầu:

**Sample Accounts:**
- **Teacher**: `teacher1` / `123456`
- **Student**: `student1` / `123456`

**Sample Content:**
- 6 môn học (Math, Science, History, English, Geography, Computer Science)
- 2 quiz mẫu với câu hỏi multiple choice
- Class codes và assignments

## 🎨 UI/UX Design

### 🌈 Design System
- **Material Design** principles
- **Consistent color palette**: Deep Purple primary, Lime accent
- **Typography**: Roboto font family
- **Icons**: Material Design icons
- **Responsive layouts**

### 📱 User Experience
- **Intuitive navigation** với breadcrumbs
- **Real-time feedback** và loading states
- **Error handling** với user-friendly messages
- **Accessibility** support
- **Dark/Light theme** toggle (planned)

## 🚧 Development Roadmap

### Phase 1: ✅ Core Infrastructure (Completed)
- Authentication system
- Basic MVVM setup
- Database models và services
- Login/Register UI

### Phase 2: 🚧 Student Features (In Progress)
- Complete quiz browsing
- Quiz taking experience
- Results visualization
- Saved quizzes management

### Phase 3: 📅 Teacher Features (Planned)
- Advanced quiz creator
- Classroom analytics
- Student progress tracking
- Bulk operations

### Phase 4: 🔮 Advanced Features (Future)
- Real-time collaboration
- Mobile app companion
- AI-powered recommendations
- Video/audio questions
- Gamification elements

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📝 Code Quality Standards

- **SOLID principles** adherence
- **Unit testing** với xUnit
- **Code coverage** > 80%
- **XML documentation** cho public APIs
- **Consistent naming conventions**
- **Error handling** và logging

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Team

- **Project Lead**: Your Name
- **Backend Developer**: Your Name
- **UI/UX Designer**: Your Name
- **QA Engineer**: Your Name

---

**🎓 Quizard - Empowering education through interactive learning!**