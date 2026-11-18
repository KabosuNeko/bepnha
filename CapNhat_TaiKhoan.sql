/*
  CapNhat_TaiKhoan.sql - them tai khoan, duyet bai cho database BepNhaDB DA CO SAN.
  (Tao database moi tu dau thi chay BepNhaDB.sql, file do da co san cac phan nay.)
  Chay xong: mo BepNhaModel.edmx > chuot phai > Update Model from Database.

  Tai khoan mau:  admin / admin123  (quan tri)   -   user1 / user123  (nguoi dung)
*/

USE BepNhaDB;
GO

/* ===== Bang Users: tai khoan ===== */
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,          -- mat khau da ma hoa (Crypto.HashPassword)
    FullName     NVARCHAR(50)  NOT NULL,
    IsAdmin      BIT           NOT NULL DEFAULT 0
);
GO

/* ===== Recipes: them nguoi dang, trang thai duyet, ly do xoa ===== */
ALTER TABLE Recipes ADD
    UserID       INT           NULL CONSTRAINT FK_Recipes_Users FOREIGN KEY REFERENCES Users(UserID),
    Status       NVARCHAR(20)  NOT NULL CONSTRAINT DF_Recipes_Status DEFAULT N'Đã duyệt',  -- Chờ duyệt / Đã duyệt / Bị xóa
    RejectReason NVARCHAR(500) NULL;
GO

INSERT INTO Users (Username, PasswordHash, FullName, IsAdmin) VALUES
    (N'admin', N'AP2tsxuyYfCfVVX5xSDm6vugBdH8yVpmvP7Z2JYzch6p0CqWUqVxVor4YF2ez8askg==', N'Quản trị viên', 1),
    (N'user1', N'ADtuSx6yrRnJDCtScK2F7DhpUZakG/Gn18i9VWVUQtbkR4JKDXK8XWpjQWxi2LXwqQ==', N'Minh Anh', 0);
GO
