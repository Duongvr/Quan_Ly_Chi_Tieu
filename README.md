# Ứng dụng Quản lý Chi tiêu và Thu nhập

Ứng dụng desktop quản lý tài chính cá nhân được xây dựng bằng **WPF** (.NET 8.0) và **Entity Framework Core**, cho phép người dùng theo dõi chi tiêu, thu nhập, phân loại theo danh mục và xem thống kê tài chính.

## 📋 Tính năng chính

### Xác thực & Tài khoản
- ✅ Đăng ký tài khoản mới
- ✅ Đăng nhập với email/mật khẩu
- ✅ Quên mật khẩu (hỗ trợ reset)
- ✅ Mã hóa mật khẩu an toàn (BCrypt)

### Quản lý Chi tiêu
- ✅ Thêm chi tiêu mới (số tiền, danh mục, ngày, ghi chú)
- ✅ Sửa chi tiêu hiện có
- ✅ Xóa chi tiêu
- ✅ Tìm kiếm theo danh mục hoặc ghi chú
- ✅ Hiển thị tổng chi tiêu
- ✅ DataGrid với sắp xếp theo ngày

### Quản lý Thu nhập
- ✅ Thêm thu nhập mới (số tiền, danh mục, ngày, ghi chú)
- ✅ Sửa thu nhập hiện có
- ✅ Xóa thu nhập
- ✅ Tìm kiếm theo danh mục hoặc ghi chú
- ✅ Hiển thị tổng thu nhập
- ✅ DataGrid với sắp xếp theo ngày

### Phân tích & Báo cáo
- ✅ Dashboard tổng quan (tổng chi tiêu, tổng thu nhập, số dư)
- ✅ Thống kê chi tiêu theo danh mục
- ✅ Thống kê thu nhập theo danh mục
- ✅ Xuất báo cáo

### Giao diện người dùng
- ✅ Giao diện hiện đại, thân thiện
- ✅ Hỗ trợ tiếng Việt đầy đủ
- ✅ Thông báo popup tự động (3 giây)
- ✅ Responsive design (tự động điều chỉnh theo kích thước cửa sổ)
- ✅ Placeholder text hỗ trợ tìm kiếm

## 🛠️ Yêu cầu hệ thống

| Yêu cầu | Phiên bản |
|---------|----------|
| .NET SDK | 8.0 trở lên |
| SQL Server | LocalDB, Express, hoặc Standard |
| Visual Studio | 2022 (hoặc VS Code + CLI) |
| Windows | 10 trở lên |

## 📦 Cài đặt & Chạy

### 1. Clone repository
```bash
git clone <repository-url>
cd ProjectWithWPF_byDuong
```

### 2. Cấu hình Connection String

Tạo file `appsettings.json` từ template:
```bash
cp appsettings.example.json appsettings.json
```

Chỉnh sửa `appsettings.json` với thông tin SQL Server của bạn:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER\\SQLEXPRESS;Initial Catalog=ExpenseManagementDB;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

**Ví dụ cho các trường hợp khác nhau:**

- **SQL Server LocalDB:**
  ```
  Data Source=(localdb)\mssqllocaldb;Initial Catalog=ExpenseManagementDB;Integrated Security=True;TrustServerCertificate=True
  ```

- **SQL Server Express:**
  ```
  Data Source=.\SQLEXPRESS;Initial Catalog=ExpenseManagementDB;Integrated Security=True;TrustServerCertificate=True
  ```

- **SQL Server trên máy khác:**
  ```
  Data Source=192.168.1.100;Initial Catalog=ExpenseManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True
  ```

### 3. Tạo Database

Chạy script SQL để tạo database và bảng:
```bash
# Mở SQL Server Management Studio hoặc Azure Data Studio
# Chạy file Project.sql
```

Hoặc sử dụng Entity Framework:
```bash
dotnet ef database update
```

### 4. Chạy ứng dụng

**Từ Visual Studio:**
- Mở `ProjectWithWPF_byDuong.sln`
- Nhấn F5 hoặc chọn Debug > Start Debugging

**Từ Command Line:**
```bash
dotnet run
```

## 📁 Cấu trúc Dự án

```
ProjectWithWPF_byDuong/
├── Models/                          # Entity models (EF Core)
│   ├── User.cs                      # Model người dùng
│   ├── Category.cs                  # Model danh mục
│   ├── Expense.cs                   # Model chi tiêu
│   ├── Income.cs                    # Model thu nhập
│   └── ExpenseManagementDbContext.cs # DbContext
│
├── Views/                           # XAML views
│   ├── LoginUI.xaml                 # Màn hình đăng nhập
│   ├── RegisterUI.xaml              # Màn hình đăng ký
│   ├── ForgotPasswordUI.xaml        # Màn hình quên mật khẩu
│   ├── DashboardUI.xaml             # Dashboard tổng quan
│   ├── ExpenseManagerUI.xaml        # Quản lý chi tiêu
│   ├── IncomeManagerUI.xaml         # Quản lý thu nhập
│   ├── AddExpenseUI.xaml            # Thêm chi tiêu
│   ├── AddIncomeUI.xaml             # Thêm thu nhập
│   ├── EditExpenseUI.xaml           # Sửa chi tiêu
│   ├── EditIncomeUI.xaml            # Sửa thu nhập
│   ├── StatisticsUI.xaml            # Thống kê
│   ├── ExportReportUI.xaml          # Xuất báo cáo
│   └── DeleteConfirmDialog.xaml     # Dialog xác nhận xóa
│
├── Helpers/                         # Helper classes
│   ├── NotificationHelper.cs        # Hiển thị thông báo popup
│   ├── PasswordHelper.cs            # Mã hóa/kiểm tra mật khẩu
│   ├── PlaceholderHelper.cs         # Quản lý placeholder text
│   └── ValidationHelper.cs          # Kiểm tra dữ liệu đầu vào
│
├── App.xaml                         # Application resources & styles
├── App.xaml.cs                      # Application code-behind
├── MainWindow.xaml                  # Main window
├── MainWindow.xaml.cs               # Main window code-behind
├── AppContext.cs                    # Global app context
├── appsettings.json                 # Configuration (⚠️ KHÔNG commit)
├── appsettings.example.json         # Configuration template
└── ProjectWithWPF_byDuong.csproj    # Project file
```

## 🗄️ Cấu trúc Database

### Bảng Users (Người dùng)
```sql
UserId (PK)      - Mã người dùng (tự động tăng)
Email            - Email (duy nhất, không trùng)
Password         - Mật khẩu (mã hóa)
CreatedAt        - Ngày tạo (tự động)
```

### Bảng Category (Danh mục)
```sql
CategoryId (PK)  - Mã danh mục
CategoryName     - Tên danh mục
Type             - Loại (Income hoặc Expense)
```

### Bảng Income (Thu nhập)
```sql
IncomeId (PK)    - Mã thu nhập
UserId (FK)      - Mã người dùng
CategoryId (FK)  - Mã danh mục
Amount           - Số tiền (decimal)
Date             - Ngày nhận tiền
Note             - Ghi chú
CreatedAt        - Ngày tạo (tự động)
```

### Bảng Expense (Chi tiêu)
```sql
ExpenseId (PK)   - Mã chi tiêu
UserId (FK)      - Mã người dùng
CategoryId (FK)  - Mã danh mục
Amount           - Số tiền (decimal)
Date             - Ngày chi tiêu
Note             - Ghi chú
CreatedAt        - Ngày tạo (tự động)
```

## 🔐 Bảo mật

### Thông tin nhạy cảm
⚠️ **QUAN TRỌNG:**
- `appsettings.json` chứa connection string - **KHÔNG commit lên Git**
- File này đã được thêm vào `.gitignore`
- Sử dụng `appsettings.example.json` làm template cho các developer khác

### Các biện pháp bảo mật
- ✅ Mật khẩu được mã hóa bằng BCrypt
- ✅ Connection string được lưu trong file cấu hình (không hardcode)
- ✅ Validation dữ liệu đầu vào
- ✅ Xác thực người dùng trước khi truy cập dữ liệu

## 👤 Tài khoản Demo

Sau khi tạo database, bạn có thể:
1. Tạo tài khoản mới qua giao diện **Đăng ký**
2. Đăng nhập với email và mật khẩu vừa tạo
3. Bắt đầu thêm chi tiêu/thu nhập

## 🛠️ Công nghệ sử dụng

| Công nghệ | Phiên bản | Mục đích |
|-----------|----------|---------|
| .NET | 8.0 | Framework chính |
| WPF | - | UI Framework |
| Entity Framework Core | 8.0 | ORM |
| SQL Server | - | Database |
| BCrypt.Net-Next | - | Mã hóa mật khẩu |
| Microsoft.Extensions.Configuration | - | Quản lý cấu hình |

## 📝 Hướng dẫn sử dụng

### Đăng ký tài khoản
1. Nhấn "Đăng ký" trên màn hình đăng nhập
2. Nhập email và mật khẩu
3. Nhấn "Đăng ký"
4. Thông báo thành công sẽ hiển thị trong 3 giây

### Thêm chi tiêu
1. Đăng nhập vào ứng dụng
2. Chọn "Quản lý Chi tiêu"
3. Nhấn "Thêm Chi tiêu"
4. Nhập số tiền, chọn danh mục, ngày, ghi chú
5. Nhấn "Lưu"

### Tìm kiếm chi tiêu
1. Vào "Quản lý Chi tiêu"
2. Nhập từ khóa vào ô tìm kiếm
3. Kết quả sẽ lọc theo danh mục hoặc ghi chú

### Sửa chi tiêu
1. Chọn chi tiêu cần sửa trong danh sách
2. Nhấn "Sửa"
3. Chỉnh sửa thông tin
4. Nhấn "Lưu"

### Xóa chi tiêu
1. Chọn chi tiêu cần xóa
2. Nhấn "Xóa"
3. Xác nhận xóa

## 🐛 Troubleshooting

### Lỗi kết nối database
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string trong `appsettings.json`
- Kiểm tra database đã được tạo chưa

### Lỗi "File not found: appsettings.json"
- Tạo file `appsettings.json` từ `appsettings.example.json`
- Cập nhật connection string

### Ứng dụng không hiển thị tiếng Việt
- Kiểm tra font Segoe UI đã được cài đặt
- Kiểm tra encoding của file XAML là UTF-8

## 📄 Giấy phép

MIT License - Tự do sử dụng, sửa đổi và phân phối

## 👨‍💻 Tác giả

Dự án được phát triển bởi Dương

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra phần Troubleshooting
2. Xem lại cấu hình connection string
3. Đảm bảo database đã được tạo đúng cách
