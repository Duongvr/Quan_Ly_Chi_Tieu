-- 1. Tạo Database
CREATE DATABASE ExpenseManagementDB;
GO

USE ExpenseManagementDB;
GO

-- =============================================
-- 2. Tạo Bảng Users (Người dùng)
-- =============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY, -- Tự động tăng từ 1
    Email NVARCHAR(100) NOT NULL UNIQUE,  -- Không được để trống và không được trùng
    Password NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()  -- Tự động lấy giờ hệ thống
);
GO

-- =============================================
-- 3. Tạo Bảng Category (Danh mục)
-- =============================================
CREATE TABLE Category (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL CHECK (Type IN ('Income', 'Expense')) -- Chỉ nhận 2 chữ này
);
GO

-- =============================================
-- 4. Tạo Bảng Income (Thu nhập)
-- =============================================
CREATE TABLE Income (
    IncomeId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,        -- Kiểu tiền tệ, hỗ trợ số thập phân
    Date DATE NOT NULL,                   -- Ngày nhận tiền
    Note NVARCHAR(255),                   -- Có thể để trống (NULL)
    CreatedAt DATETIME DEFAULT GETDATE(),
    
    -- Tạo khóa ngoại liên kết bảng
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);
GO

-- =============================================
-- 5. Tạo Bảng Expense (Chi tiêu)
-- =============================================
CREATE TABLE Expense (
    ExpenseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Date DATE NOT NULL,
    Note NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    
    -- Tạo khóa ngoại liên kết bảng
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);
GO

-- =============================================
-- 6. THÊM DỮ LIỆU MẪU CHO BẢNG CATEGORY (Để test UI)
-- =============================================
INSERT INTO Category (CategoryName, Type) VALUES 
(N'Tiền lương', 'Income'),
(N'Làm thêm', 'Income'),
(N'Đầu tư', 'Income'),
(N'Ăn uống', 'Expense'),
(N'Di chuyển', 'Expense'),
(N'Tiền nhà', 'Expense'),
(N'Y tế', 'Expense'),
(N'Giải trí', 'Expense');
GO