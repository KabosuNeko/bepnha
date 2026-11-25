/*
  CapNhat_YeuThich.sql - thêm bảng Favorites (món yêu thích) cho database BepNhaDB ĐÃ CÓ SẴN.
  (Tạo database mới từ đầu thì chạy BepNhaDB.sql, file đó đã có bảng này.)
  Chạy xong: mở BepNhaModel.edmx > chuột phải > Update Model from Database.
*/

USE BepNhaDB;
GO

CREATE TABLE Favorites (
    FavoriteID INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT      NOT NULL CONSTRAINT FK_Favorites_Users FOREIGN KEY REFERENCES Users(UserID),
    RecipeID   INT      NOT NULL CONSTRAINT FK_Favorites_Recipes FOREIGN KEY REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    CreatedAt  DATETIME NOT NULL,
    CONSTRAINT UQ_Favorites UNIQUE (UserID, RecipeID)   -- mỗi người chỉ lưu một món một lần
);
GO

INSERT INTO Favorites (UserID, RecipeID, CreatedAt) VALUES
    (2, 1, N'2025-10-20T00:00:00'),
    (2, 3, N'2025-10-21T00:00:00'),
    (2, 17, N'2025-10-22T00:00:00');
GO
