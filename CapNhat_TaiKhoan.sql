/*
  CapNhat_TaiKhoan.sql - thêm tài khoản, duyệt bài cho database BepNhaDB ĐÃ CÓ SẴN.
  (Tạo database mới từ đầu thì chạy BepNhaDB.sql, file đó đã có sẵn các phần này.)
  Chạy xong: mở BepNhaModel.edmx > chuột phải > Update Model from Database.

  Tài khoản mẫu:  admin / admin123  (quản trị)   -   user1 / user123  (người dùng)
*/

USE BepNhaDB;
GO

/* ===== Bảng Users: tài khoản ===== */
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,          -- mật khẩu đã mã hóa (Crypto.HashPassword)
    FullName     NVARCHAR(50)  NOT NULL,
    IsAdmin      BIT           NOT NULL DEFAULT 0
);
GO

/* ===== Recipes: thêm người đăng, trạng thái duyệt, lý do xóa ===== */
ALTER TABLE Recipes ADD
    UserID       INT           NULL CONSTRAINT FK_Recipes_Users FOREIGN KEY REFERENCES Users(UserID),
    Status       NVARCHAR(20)  NOT NULL CONSTRAINT DF_Recipes_Status DEFAULT N'Đã duyệt',  -- Chờ duyệt / Đã duyệt / Bị xóa
    RejectReason NVARCHAR(500) NULL;
GO

INSERT INTO Users (Username, PasswordHash, FullName, IsAdmin) VALUES
    (N'admin', N'AP2tsxuyYfCfVVX5xSDm6vugBdH8yVpmvP7Z2JYzch6p0CqWUqVxVor4YF2ez8askg==', N'Quản trị viên', 1),
    (N'user1', N'ADtuSx6yrRnJDCtScK2F7DhpUZakG/Gn18i9VWVUQtbkR4JKDXK8XWpjQWxi2LXwqQ==', N'Minh Anh', 0);
GO
