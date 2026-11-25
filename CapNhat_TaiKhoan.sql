/*
  CapNhat_TaiKhoan.sql - thêm tài khoản, duyệt bài cho database BepNhaDB ĐÃ CÓ SẴN.
  (Tạo database mới từ đầu thì chạy BepNhaDB.sql, file đó đã có sẵn các phần này.)
  Chạy xong: mở BepNhaModel.edmx > chuột phải > Update Model from Database.

  Tài khoản mẫu (quản trị):   admin / admin123   -   admin2 / admin456
  Tài khoản mẫu (người dùng): user1 / user123   -   user2 / user456   -   user3 / user789
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
    (N'user1', N'ADtuSx6yrRnJDCtScK2F7DhpUZakG/Gn18i9VWVUQtbkR4JKDXK8XWpjQWxi2LXwqQ==', N'Minh Anh', 0),
    (N'admin2', N'AMYH30UnddrpxFNrZzm/nw9fSDcXeBfHOWNhS9X0YuAszciTt3HpyFnr7KAZi0IN4A==', N'Quản trị viên 2', 1),
    (N'user2', N'AETvGhu0sIJIFsL+0QNNRyNNLHHXK0VLGKQSi6l6IZpSsiwjS91Qjv5uVf/0dcbpKw==', N'Thu Trang', 0),
    (N'user3', N'AGd8m7hS6b2lUJd6lhyFl5qpuIQFSgCITBgZMR1svVyf2rqRZXmj1snV5Li+Pa00Gg==', N'Hoàng Nam', 0);
GO
