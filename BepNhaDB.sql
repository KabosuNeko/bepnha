/*
  BepNhaDB.sql - Database cho website chia sẻ công thức "Bếp Nhà"
  Cách dùng: mở file này trong SSMS (kết nối .\SQLEXPRESS) rồi bấm Execute (F5).
  Sau đó vào Visual Studio: Add > New Item > ADO.NET Entity Data Model > EF Designer from database.
  Tài khoản mẫu (quản trị):   admin / admin123   -   admin2 / admin456
  Tài khoản mẫu (người dùng): user1 / user123   -   user2 / user456   -   user3 / user789
*/

CREATE DATABASE BepNhaDB;
GO
USE BepNhaDB;
GO

/* ===== Bảng Categories: danh mục món ăn ===== */
CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50)  NOT NULL,
    Description  NVARCHAR(200) NULL
);
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

/* ===== Bảng Recipes: công thức ===== */
CREATE TABLE Recipes (
    RecipeID        INT IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(120) NOT NULL,
    CategoryID      INT           NOT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    Description     NVARCHAR(500) NOT NULL,
    Ingredients     NVARCHAR(MAX) NOT NULL,
    Instructions    NVARCHAR(MAX) NOT NULL,
    PrepTimeMinutes INT           NOT NULL,
    CookTimeMinutes INT           NOT NULL,
    Servings        INT           NOT NULL,
    Difficulty      NVARCHAR(20)  NOT NULL,
    Author          NVARCHAR(50)  NOT NULL,
    ImagePath       NVARCHAR(300) NULL,
    CreatedAt       DATETIME      NOT NULL,
    UserID          INT           NULL CONSTRAINT FK_Recipes_Users FOREIGN KEY REFERENCES Users(UserID),
    Status          NVARCHAR(20)  NOT NULL CONSTRAINT DF_Recipes_Status DEFAULT N'Đã duyệt',  -- Chờ duyệt / Đã duyệt / Bị xóa
    RejectReason    NVARCHAR(500) NULL
);
GO

/* ===== Bảng Ratings: đánh giá ===== */
CREATE TABLE Ratings (
    RatingID     INT IDENTITY(1,1) PRIMARY KEY,
    RecipeID     INT           NOT NULL FOREIGN KEY REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    ReviewerName NVARCHAR(50)  NOT NULL,
    Stars        INT           NOT NULL,
    Comment      NVARCHAR(500) NULL,
    CreatedAt    DATETIME      NOT NULL
);
GO

/* ===== Bảng Favorites: món yêu thích (người dùng lưu món) ===== */
CREATE TABLE Favorites (
    FavoriteID INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT      NOT NULL CONSTRAINT FK_Favorites_Users FOREIGN KEY REFERENCES Users(UserID),
    RecipeID   INT      NOT NULL CONSTRAINT FK_Favorites_Recipes FOREIGN KEY REFERENCES Recipes(RecipeID) ON DELETE CASCADE,
    CreatedAt  DATETIME NOT NULL,
    CONSTRAINT UQ_Favorites UNIQUE (UserID, RecipeID)   -- mỗi người chỉ lưu một món một lần
);
GO

/* ===== Dữ liệu mẫu: tài khoản ===== */
INSERT INTO Users (Username, PasswordHash, FullName, IsAdmin) VALUES
    (N'admin', N'AP2tsxuyYfCfVVX5xSDm6vugBdH8yVpmvP7Z2JYzch6p0CqWUqVxVor4YF2ez8askg==', N'Quản trị viên', 1),
    (N'user1', N'ADtuSx6yrRnJDCtScK2F7DhpUZakG/Gn18i9VWVUQtbkR4JKDXK8XWpjQWxi2LXwqQ==', N'Minh Anh', 0),
    (N'admin2', N'AMYH30UnddrpxFNrZzm/nw9fSDcXeBfHOWNhS9X0YuAszciTt3HpyFnr7KAZi0IN4A==', N'Quản trị viên 2', 1),
    (N'user2', N'AETvGhu0sIJIFsL+0QNNRyNNLHHXK0VLGKQSi6l6IZpSsiwjS91Qjv5uVf/0dcbpKw==', N'Thu Trang', 0),
    (N'user3', N'AGd8m7hS6b2lUJd6lhyFl5qpuIQFSgCITBgZMR1svVyf2rqRZXmj1snV5Li+Pa00Gg==', N'Hoàng Nam', 0);
GO

/* ===== Dữ liệu mẫu: danh mục ===== */
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (1, N'Món nước', N'Phở, bún, mì và các món có nước dùng');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (2, N'Cơm - Món khô', N'Cơm, bánh mì và các món ăn khô');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (3, N'Cuốn - Khai vị', N'Gỏi cuốn, chả giò và món khai vị');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (4, N'Canh - Súp', N'Canh, súp dùng kèm bữa cơm');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (5, N'Kho - Rim', N'Các món kho, rim đậm vị');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (6, N'Món chay', N'Món chay thanh đạm');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (7, N'Tráng miệng', N'Chè, bánh ngọt và món tráng miệng');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (8, N'Đồ uống', N'Trà, cà phê và đồ uống giải khát');
SET IDENTITY_INSERT Categories OFF;
GO

/* ===== Dữ liệu mẫu: công thức ===== */
SET IDENTITY_INSERT Recipes ON;
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (1, N'Phở bò', 1, N'Nước dùng xương bò ninh cùng gừng, hành tím, hoa hồi và quế, ăn kèm bánh phở và thịt bò tái.', N'1 kg xương ống bò
500 g thịt bò nạm
300 g bánh phở tươi
1 củ gừng, 2 củ hành tím
3 hoa hồi, 1 thanh quế, 2 hạt mùi
Nước mắm, muối, đường phèn
Hành lá, ngò gai, giá đỗ, chanh, ớt', N'1. Rửa xương bò, chần qua nước sôi rồi rửa lại cho sạch.
2. Ninh xương với 4 lít nước trong 2 giờ, hớt bọt thường xuyên.
3. Nướng gừng và hành tím cho thơm, đập dập rồi cho vào nồi cùng hoa hồi, quế, hạt mùi.
4. Nêm nước mắm, muối, đường phèn cho vừa ăn rồi ninh thêm 30 phút.
5. Thái mỏng thịt bò, trụng bánh phở rồi xếp vào tô cùng thịt bò.
6. Chan nước dùng đang sôi lên tô, thêm hành lá, ngò gai và ăn kèm giá đỗ, chanh, ớt.', 30, 180, 4, N'Trung bình', N'Minh Anh', N'~/Content/Images/pho-bo.jpg', N'2025-09-05T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (2, N'Bún chả', 1, N'Thịt ba chỉ và chả viên nướng than hoa, ăn kèm bún, rau sống và nước chấm chua ngọt.', N'500 g thịt ba chỉ
300 g thịt nạc xay
200 g bún tươi
Nước mắm, đường, tỏi, ớt
Hành khô, tiêu, dầu ăn
Rau sống: xà lách, húng quế, tía tô
Đu đủ xanh hoặc cà rốt làm đồ chua', N'1. Thái thịt ba chỉ thành miếng mỏng vừa ăn.
2. Ướp thịt với nước mắm, đường, hành khô băm và tiêu trong 30 phút.
3. Viên thịt nạc xay thành từng viên nhỏ rồi ướp cùng gia vị.
4. Nướng thịt và chả trên than hoa hoặc lò nướng đến khi vàng đều hai mặt.
5. Pha nước chấm: nước mắm, đường, nước ấm, tỏi ớt băm và đồ chua.
6. Xếp bún, rau sống và thịt nướng ra đĩa rồi dùng cùng nước chấm.', 40, 20, 4, N'Trung bình', N'Quốc Huy', N'~/Content/Images/bun-cha.jpg', N'2025-09-08T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (3, N'Cơm tấm sườn nướng', 2, N'Cơm tấm dẻo thơm ăn kèm sườn nướng sả, trứng ốp la và nước mắm pha.', N'400 g gạo tấm
4 miếng sườn non
2 quả trứng gà
Sả, tỏi, mật ong
Nước mắm, đường, dầu điều
Dưa leo, cà chua, đồ chua', N'1. Ngâm gạo tấm 30 phút rồi nấu chín như cơm thường.
2. Ướp sườn với sả băm, tỏi, mật ong, nước mắm và dầu điều trong 1 giờ.
3. Nướng sườn trên bếp than hoặc lò nướng, trở đều hai mặt.
4. Chiên trứng ốp la.
5. Pha nước mắm chua ngọt với đường, chanh, tỏi ớt.
6. Xới cơm ra đĩa, xếp sườn, trứng, dưa leo, cà chua rồi rưới nước mắm.', 20, 30, 4, N'Dễ', N'Thu Trang', N'~/Content/Images/com-tam.jpg', N'2025-09-10T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (4, N'Bánh mì thịt nướng', 2, N'Bánh mì giòn với thịt heo nướng, pate, đồ chua và rau thơm, rất tiện cho bữa sáng.', N'4 ổ bánh mì
400 g thịt heo nạc vai
Pate gan, bơ, sốt mayonnaise
Cà rốt, củ cải trắng làm đồ chua
Dưa leo, ngò rí, hành tím
Nước tương, đường, tiêu, tỏi', N'1. Thái thịt heo thành lát mỏng, ướp với tỏi, nước tương, đường và tiêu trong 30 phút.
2. Nướng thịt trên chảo hoặc lò đến khi chín vàng.
3. Bào cà rốt, củ cải rồi ngâm giấm đường 20 phút để làm đồ chua.
4. Nướng sơ bánh mì cho giòn rồi rạch dọc ổ bánh.
5. Phết pate và bơ vào trong bánh, xếp thịt, đồ chua, dưa leo và ngò rí.
6. Rưới chút nước tương cùng sốt mayonnaise rồi dùng ngay.', 25, 15, 4, N'Dễ', N'Hoàng Nam', N'~/Content/Images/banh-mi.jpg', N'2025-09-12T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (5, N'Gỏi cuốn tôm thịt', 3, N'Cuốn bánh tráng với tôm, thịt, bún và rau sống, chấm cùng tương đen hoặc nước mắm.', N'12 bánh tráng
200 g tôm tươi
200 g thịt ba chỉ
100 g bún tươi
Xà lách, húng quế, hẹ
Tương đen, đậu phộng rang', N'1. Luộc tôm chín, bóc vỏ và chẻ đôi theo chiều dọc.
2. Luộc thịt ba chỉ rồi thái mỏng.
3. Làm ướt bánh tráng bằng nước rồi xếp xà lách, bún và rau thơm.
4. Đặt tôm và thịt lên trên, cuộn chặt tay.
5. Cắt cuốn thành từng khúc vừa ăn.
6. Pha tương đen với đậu phộng rang giã nhỏ để chấm.', 30, 15, 4, N'Dễ', N'Lan Phương', N'~/Content/Images/goi-cuon.jpg', N'2025-09-15T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (6, N'Bún bò Huế', 1, N'Bún bò Huế với nước dùng sả ớt đậm đà, thịt bắp bò và chả lụa.', N'1 kg xương ống bò
400 g bắp bò
200 g chả lụa
500 g bún sợi to
Sả, ớt bột, mắm ruốc
Hành tím, gừng, hạt nêm
Bắp chuối, giá, rau thơm', N'1. Chần xương bò rồi ninh với 4 lít nước trong 2 giờ.
2. Đập dập sả, nướng hành tím và gừng rồi cho vào nồi.
3. Phi ớt bột với dầu ăn cho lên màu rồi đổ vào nước dùng.
4. Nêm mắm ruốc, hạt nêm và muối cho vừa khẩu vị.
5. Luộc bắp bò rồi thái mỏng, cắt chả lụa thành lát.
6. Trụng bún, xếp thịt và chả vào tô rồi chan nước dùng nóng.', 30, 150, 4, N'Khó', N'Bích Ngọc', N'~/Content/Images/bun-bo-hue.jpg', N'2025-09-18T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (7, N'Thịt kho trứng', 5, N'Thịt ba chỉ kho nước dừa cùng trứng vịt, vị mặn ngọt đưa cơm.', N'600 g thịt ba chỉ
4 quả trứng vịt
1 quả dừa tươi lấy nước
Nước mắm, đường, muối
Hành tím, tỏi, tiêu
Nước màu dừa', N'1. Cắt thịt ba chỉ thành miếng vuông vừa ăn, ướp với nước mắm, đường, hành tỏi băm trong 30 phút.
2. Luộc trứng rồi bóc vỏ.
3. Thắng nước màu dừa rồi cho thịt vào xào săn.
4. Đổ nước dừa tươi vào ngập thịt, kho lửa nhỏ 45 phút.
5. Cho trứng vào kho cùng thêm 15 phút cho thấm.
6. Nêm nếm lại và rắc tiêu trước khi dùng.', 20, 60, 4, N'Trung bình', N'Minh Anh', N'~/Content/Images/thit-kho-trung.jpg', N'2025-09-20T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (8, N'Canh chua cá lóc', 4, N'Canh chua miền Nam với cá lóc, me, dứa và cà chua, ăn kèm cơm trắng rất ngon.', N'1 con cá lóc khoảng 700 g
1 quả dứa (thơm)
2 quả cà chua
Giá đỗ, bạc hà, ngò om
Me chín hoặc nước me
Nước mắm, đường, muối', N'1. Làm sạch cá lóc, cắt khúc vừa ăn.
2. Dầm me với nước nóng rồi lọc lấy nước cốt.
3. Đun nước, cho cá vào nấu đến khi chín tới.
4. Thêm dứa, cà chua và nêm đường, muối, nước mắm cho vừa ăn.
5. Cho giá đỗ, bạc hà, ngò om vào rồi tắt bếp.
6. Múc canh ra tô và rắc rau ngò lên trên.', 20, 25, 4, N'Trung bình', N'Thu Trang', N'~/Content/Images/canh-chua.jpg', N'2025-09-23T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (9, N'Đậu hũ sốt cà chua', 6, N'Đậu hũ non chiên vàng rồi sốt cà chua mềm, món chay dễ làm cho bữa cơm gia đình.', N'4 miếng đậu hũ non
3 quả cà chua
Hành lá, hành tím
Nước tương, đường, tiêu
Dầu ăn', N'1. Cắt đậu hũ thành miếng vuông rồi để ráo nước.
2. Chiên đậu trên chảo dầu nóng đến khi vàng đều, vớt ra.
3. Phi hành tím cho thơm rồi cho cà chua cắt múi vào xào mềm.
4. Nêm nước tương và đường cho vừa ăn.
5. Cho đậu hũ vào sốt khoảng 5 phút cho thấm.
6. Rắc hành lá và tiêu rồi tắt bếp.', 10, 20, 3, N'Dễ', N'Thanh Hà', N'~/Content/Images/dau-hu-sot-ca.jpg', N'2025-09-25T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (10, N'Chè đậu xanh', 7, N'Chè đậu xanh nấu nước dừa, ngọt thanh, ăn nóng hay để lạnh đều ngon.', N'300 g đậu xanh đãi vỏ
200 ml nước cốt dừa
150 g đường cát
1 củ gừng nhỏ
1/2 thìa muối', N'1. Vo sạch đậu xanh rồi ngâm nước 2 giờ.
2. Hấp hoặc nấu đậu cho chín mềm.
3. Dầm đậu nhẹ tay cho tơi rồi thêm đường và muối.
4. Đổ nước vào nấu sôi, khuấy đều cho đường tan.
5. Nêm nước cốt dừa và gừng thái sợi, nấu thêm 5 phút.
6. Múc chè ra chén và dùng nóng hoặc để lạnh.', 15, 30, 4, N'Dễ', N'Lan Phương', N'~/Content/Images/che-dau-xanh.jpg', N'2025-09-28T00:00:00');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (11, N'Cà phê sữa đá', 8, N'Cà phê phin pha đậm, hòa cùng sữa đặc và đá, thức uống quen thuộc mỗi sáng.', N'25 g cà phê bột rang xay
2 thìa sữa đặc
100 ml nước sôi
1 ly đá viên', N'1. Tráng phin và ly bằng nước sôi cho nóng.
2. Cho cà phê vào phin, nén nhẹ rồi rót 20 ml nước sôi để ủ 30 giây.
3. Rót tiếp phần nước sôi còn lại, đậy nắp và chờ cà phê nhỏ giọt hết.
4. Cho sữa đặc vào ly, đổ cà phê vào khuấy đều.
5. Thêm đá viên rồi dùng ngay.', 5, 5, 1, N'Dễ', N'Hoàng Nam', N'~/Content/Images/ca-phe-sua-da.jpg', '2025-10-01');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (12, N'Sinh tố bơ', 8, N'Sinh tố bơ sánh mịn, béo ngậy, xay cùng sữa đặc và đá.', N'2 quả bơ chín
3 thìa sữa đặc
100 ml sữa tươi
1 chén đá viên', N'1. Bổ đôi quả bơ, bỏ hạt và dùng thìa nạo lấy thịt bơ.
2. Cho bơ, sữa đặc, sữa tươi và đá vào máy xay.
3. Xay khoảng 1 phút đến khi sinh tố mịn.
4. Nếm lại, thêm sữa đặc nếu thích ngọt hơn.
5. Rót ra ly và dùng ngay.', 10, 5, 2, N'Dễ', N'Thanh Hà', N'~/Content/Images/sinh-to-bo.jpg', '2025-10-03');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (13, N'Rau muống xào tỏi', 6, N'Rau muống xào lửa lớn với tỏi phi thơm, giòn xanh, món chay đơn giản cho bữa cơm.', N'1 bó rau muống
1 củ tỏi
2 thìa dầu ăn
Nước tương, muối, hạt nêm chay', N'1. Nhặt rau muống, rửa sạch rồi để ráo.
2. Bóc tỏi, đập dập.
3. Phi tỏi với dầu ăn đến khi vàng thơm, vớt bớt một nửa ra.
4. Cho rau muống vào xào lửa lớn, đảo nhanh tay.
5. Nêm nước tương, muối và hạt nêm chay cho vừa ăn.
6. Xào đến khi rau vừa chín tới, rắc phần tỏi phi lên rồi tắt bếp.', 10, 5, 3, N'Dễ', N'Bích Ngọc', N'~/Content/Images/rau-muong-xao-toi.jpg', '2025-10-05');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (14, N'Canh bí đao nấu tôm', 4, N'Canh bí đao thanh mát nấu cùng tôm tươi, rất hợp những ngày nóng.', N'1 quả bí đao khoảng 500 g
150 g tôm tươi
2 củ hành tím
Hành lá, ngò
Nước mắm, muối, hạt nêm', N'1. Gọt vỏ bí đao, bỏ ruột rồi cắt miếng vừa ăn.
2. Bóc vỏ tôm, băm nhỏ và ướp với chút nước mắm.
3. Phi hành tím cho thơm rồi cho tôm vào xào săn.
4. Đổ 1 lít nước vào nồi, đun sôi.
5. Cho bí đao vào nấu khoảng 10 phút đến khi bí mềm.
6. Nêm muối, hạt nêm cho vừa ăn, thêm hành lá và ngò rồi tắt bếp.', 15, 20, 4, N'Dễ', N'Quốc Huy', N'~/Content/Images/canh-bi-dao.jpg', '2025-10-07');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (15, N'Cá kho tộ', 5, N'Cá basa kho trong niêu đất với nước màu và tiêu, đậm đà, ăn kèm cơm trắng.', N'500 g cá basa cắt khúc
3 thìa nước mắm
2 thìa đường
1 thìa nước màu
Hành tím, tỏi, ớt, tiêu
Hành lá', N'1. Rửa cá với nước muối loãng cho hết nhớt.
2. Ướp cá với nước mắm, đường, nước màu, hành tỏi băm trong 30 phút.
3. Xếp cá vào niêu đất, đổ phần nước ướp và thêm ít nước lọc.
4. Kho lửa vừa đến khi sôi rồi hạ lửa nhỏ.
5. Kho khoảng 30 phút đến khi nước sánh lại.
6. Rắc tiêu, ớt và hành lá rồi tắt bếp.', 35, 40, 4, N'Trung bình', N'Thu Trang', N'~/Content/Images/ca-kho-to.jpg', '2025-10-09');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (16, N'Chả giò', 3, N'Chả giò nhân thịt, khoai môn và miến, chiên vàng giòn, chấm nước mắm chua ngọt.', N'300 g thịt heo xay
1 củ khoai môn nhỏ
1 củ cà rốt
30 g miến, 10 g nấm mèo
20 lá bánh tráng
Hành tím, tiêu, nước mắm
Dầu ăn để chiên', N'1. Ngâm miến và nấm mèo cho mềm rồi cắt nhỏ.
2. Bào sợi khoai môn và cà rốt.
3. Trộn thịt xay với khoai môn, cà rốt, miến, nấm mèo, hành tím và gia vị.
4. Đặt nhân lên bánh tráng rồi cuộn chặt tay.
5. Chiên chả giò trong dầu nóng vừa đến khi vàng giòn.
6. Vớt ra giấy thấm dầu và dùng cùng rau sống, nước mắm chua ngọt.', 40, 20, 4, N'Trung bình', N'Lan Phương', N'~/Content/Images/cha-gio.jpg', '2025-10-11');
INSERT INTO Recipes (RecipeID, Title, CategoryID, Description, Ingredients, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, Author, ImagePath, CreatedAt) VALUES (17, N'Bánh flan', 7, N'Bánh flan mềm mịn, thơm mùi trứng sữa với lớp caramel đắng nhẹ.', N'4 quả trứng gà
400 ml sữa tươi
100 g đường
1/2 thìa vani', N'1. Nấu 50 g đường với chút nước đến khi thành caramel vàng nâu, đổ vào đáy khuôn.
2. Đun ấm sữa với phần đường còn lại cho tan.
3. Đánh tan trứng rồi từ từ đổ sữa ấm vào, khuấy nhẹ và thêm vani.
4. Lọc hỗn hợp qua rây rồi rót vào khuôn.
5. Hấp lửa nhỏ khoảng 25 phút đến khi bánh đông.
6. Để nguội rồi cho vào tủ lạnh, khi ăn úp bánh ra đĩa.', 20, 25, 6, N'Trung bình', N'Minh Anh', N'~/Content/Images/banh-flan.jpg', '2025-10-13');
SET IDENTITY_INSERT Recipes OFF;
GO

/* ===== Dữ liệu mẫu: đánh giá ===== */
SET IDENTITY_INSERT Ratings ON;
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (1, 1, N'Ngọc Mai', 5, N'Nước dùng trong và thơm, mình ninh đúng 2 tiếng là vừa.', N'2025-09-06T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (2, 1, N'Đức Long', 4, N'Ngon, nhưng mình bớt đường phèn một chút cho hợp khẩu vị.', N'2025-09-07T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (3, 1, N'Hải Yến', 5, N'Cả nhà ăn hết nồi trong bữa tối, sẽ nấu lại.', N'2025-09-09T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (4, 2, N'Tuấn Kiệt', 4, N'Chả viên nướng than thơm, nước chấm nên pha nhạt hơn chút.', N'2025-09-09T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (5, 2, N'Mỹ Duyên', 5, N'Ướp thịt 30 phút là thấm vừa đủ, rất vừa miệng.', N'2025-09-11T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (6, 3, N'Văn Sơn', 5, N'Sườn nướng mật ong lên màu đẹp, ăn kèm dưa leo rất hợp.', N'2025-09-11T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (7, 3, N'Kim Chi', 4, N'Dễ làm, mình thêm chút dầu điều cho sườn đỏ đẹp hơn.', N'2025-09-13T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (8, 4, N'Bảo Trân', 5, N'Bữa sáng nhanh gọn, đồ chua ngâm giấm đường rất vừa vị.', N'2025-09-14T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (9, 5, N'Phúc An', 5, N'Cuốn chặt tay nên không bị bung, tương đen thêm đậu phộng rất thơm.', N'2025-09-16T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (10, 5, N'Lệ Quyên', 4, N'Mình thay tôm bằng thịt luộc vì nhà không ăn hải sản, vẫn ngon.', N'2025-09-17T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (11, 6, N'Trung Hiếu', 5, N'Mắm ruốc và sả đúng vị Huế, nước dùng cay nhẹ rất cuốn.', N'2025-09-19T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (12, 6, N'Diệu Linh', 5, N'Ninh xương lâu nhưng xứng đáng, nước ngọt tự nhiên.', N'2025-09-21T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (13, 6, N'Quang Dũng', 3, N'Hơi tốn thời gian, mình rút ngắn còn 1 tiếng nhưng vẫn ăn được.', N'2025-09-22T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (14, 7, N'Hồng Nhung', 5, N'Kho nước dừa nên thịt mềm và bóng, trứng thấm đều.', N'2025-09-22T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (15, 7, N'Anh Tuấn', 4, N'Mình kho lửa nhỏ 1 tiếng cho thịt mềm hơn, rất hao cơm.', N'2025-09-24T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (16, 8, N'Thùy Dương', 4, N'Canh chua vừa miệng, mình thêm chút nước me cho chua hơn.', N'2025-09-24T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (17, 8, N'Minh Nhật', 5, N'Cá lóc nấu chín tới nên thịt ngọt, không bị bở.', N'2025-09-26T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (18, 9, N'Trà My', 4, N'Món chay dễ làm, mình bớt dầu bằng cách áp chảo đậu hũ.', N'2025-09-27T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (19, 9, N'Đình Khoa', 5, N'Đậu hũ non non mềm, sốt cà chua đậm vị rất hợp cơm trắng.', N'2025-09-28T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (20, 10, N'Ngọc Hân', 5, N'Chè ngọt thanh, mình để lạnh ăn kèm đá rất mát.', N'2025-09-29T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (21, 10, N'Thảo Vy', 4, NULL, N'2025-09-30T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (22, 11, N'Quang Dũng', 5, N'Pha phin đúng kiểu, đậm và thơm, sáng nào cũng uống.', N'2025-10-02T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (23, 11, N'Mỹ Duyên', 4, N'Mình bớt sữa đặc một chút cho đỡ ngọt.', N'2025-10-03T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (24, 12, N'Ngọc Mai', 5, N'Bơ chín xay với sữa đặc béo ngậy, cả nhà đều thích.', N'2025-10-04T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (25, 13, N'Đức Long', 4, N'Xào lửa lớn nên rau vẫn giòn xanh.', N'2025-10-06T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (26, 13, N'Trà My', 5, N'Món chay nhanh gọn, tỏi phi rất thơm.', N'2025-10-07T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (27, 14, N'Hải Yến', 4, N'Canh thanh mát, hợp ngày nóng.', N'2025-10-08T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (28, 15, N'Văn Sơn', 5, N'Kho niêu đất nước sánh lại rất đưa cơm.', N'2025-10-10T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (29, 15, N'Hồng Nhung', 4, N'Mình thêm chút ớt cho cay hơn.', N'2025-10-11T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (30, 16, N'Bảo Trân', 5, N'Cuốn chặt tay, chiên lửa vừa nên vỏ giòn lâu.', N'2025-10-12T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (31, 17, N'Diệu Linh', 5, N'Bánh mịn, không bị rỗ, caramel đắng nhẹ vừa miệng.', N'2025-10-14T00:00:00');
INSERT INTO Ratings (RatingID, RecipeID, ReviewerName, Stars, Comment, CreatedAt) VALUES (32, 17, N'Minh Nhật', 4, NULL, N'2025-10-15T00:00:00');
SET IDENTITY_INSERT Ratings OFF;
GO

/* ===== Dữ liệu mẫu: món yêu thích của user1 (Minh Anh) ===== */
INSERT INTO Favorites (UserID, RecipeID, CreatedAt) VALUES
    (2, 1, N'2025-10-20T00:00:00'),
    (2, 3, N'2025-10-21T00:00:00'),
    (2, 17, N'2025-10-22T00:00:00');
GO
