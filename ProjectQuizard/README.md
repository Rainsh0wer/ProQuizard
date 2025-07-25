# Quiz App - Ứng dụng Quiz đơn giản

Ứng dụng Quiz được xây dựng bằng C# WPF Framework 8.0 với Entity Framework Core và SQL Server.

## Tính năng chính

### Cho Học sinh:
- Đăng nhập/đăng ký tài khoản
- Xem danh sách quiz có sẵn
- Làm bài quiz với 4 đáp án trắc nghiệm
- Xem kết quả và điểm số ngay sau khi hoàn thành
- Xem lịch sử các bài đã làm

### Cho Giáo viên:
- Đăng nhập/đăng ký tài khoản
- Tạo và quản lý quiz
- Thêm/sửa/xóa câu hỏi
- Xem thống kê học sinh đã làm bài

## Yêu cầu hệ thống

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 (khuyên dùng)

## Cài đặt và chạy

### 1. Clone dự án
```bash
git clone <repository-url>
cd ProjectQuizard
```

### 2. Cấu hình database
Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DB": "server=localhost;database=AP;uid=sa;pwd=123;TrustServerCertificate=True"
  }
}
```

### 3. Tạo database
Mở Package Manager Console trong Visual Studio và chạy:
```
Update-Database
```

### 4. Chạy ứng dụng
```bash
dotnet run
```

## Tài khoản mẫu

Ứng dụng sẽ tự động tạo dữ liệu mẫu khi chạy lần đầu:

### Giáo viên:
- Username: `teacher1`
- Password: `123456`

### Học sinh:
- Username: `student1` / Password: `123456`
- Username: `student2` / Password: `123456`

## Cấu trúc dự án

```
ProjectQuizard/
├── Models/              # Entity models và DbContext
├── Views/               # XAML windows
├── ViewModels/          # ViewModels cho MVVM pattern
├── Services/            # Business logic services
├── Helpers/             # Helper classes và converters
└── SampleDataCreator.cs # Tạo dữ liệu mẫu
```

## Cách sử dụng

### Đăng nhập
1. Chạy ứng dụng
2. Nhập username và password
3. Click "Đăng nhập"

### Đăng ký tài khoản mới
1. Click "Đăng ký tài khoản mới" từ màn hình đăng nhập
2. Điền đầy đủ thông tin
3. Chọn vai trò (Student/Teacher)
4. Click "Đăng ký"

### Học sinh làm bài
1. Đăng nhập với tài khoản học sinh
2. Tab "Danh sách Quiz": Chọn quiz và click "Bắt đầu làm bài"
3. Tab "Làm bài": Chọn đáp án và điều hướng giữa các câu
4. Click "Nộp bài" khi hoàn thành
5. Tab "Kết quả": Xem điểm số và chi tiết
6. Tab "Lịch sử": Xem các bài đã làm

### Giáo viên tạo quiz
1. Đăng nhập với tài khoản giáo viên
2. Tab "Quiz của tôi": Click "Tạo quiz mới"
3. Tab "Tạo/Sửa Quiz": Nhập thông tin quiz và lưu
4. Click "Thêm câu hỏi" để thêm câu hỏi
5. Tab "Tạo/Sửa Câu hỏi": Nhập câu hỏi và 4 đáp án
6. Chọn đáp án đúng và lưu

## Đặc điểm kỹ thuật

- **Framework**: .NET 8.0 WPF
- **Database**: Entity Framework Core với SQL Server
- **Pattern**: MVVM (Model-View-ViewModel)
- **Authentication**: Đơn giản (password không mã hóa - chỉ cho demo)
- **UI**: WPF controls cơ bản với styling đơn giản

## Lưu ý quan trọng

⚠️ **Chỉ dành cho mục đích học tập và demo**
- Password được lưu trực tiếp trong database (không mã hóa)
- Không có validation phức tạp
- UI đơn giản, tập trung vào chức năng core

## Troubleshooting

### Lỗi kết nối database
- Kiểm tra SQL Server đã được cài đặt và chạy
- Cập nhật connection string phù hợp với môi trường của bạn
- Chạy `Update-Database` trong Package Manager Console

### Lỗi build
- Đảm bảo đã cài .NET 8.0 SDK
- Restore NuGet packages: `dotnet restore`

## Phát triển thêm

Có thể mở rộng ứng dụng với:
- Mã hóa password
- Validation phức tạp hơn
- UI/UX đẹp hơn
- Export kết quả
- Timer cho quiz
- Nhiều loại câu hỏi hơn
- Role-based permissions chi tiết hơn