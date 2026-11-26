# Bếp Nhà - Website chia sẻ công thức nấu ăn

Đồ án môn Lập trình Web cơ bản (đề 9: đăng, tìm kiếm và đánh giá công thức nấu ăn).

- Sinh viên: Nguyễn Hoàng Long - MSSV 25DH110432
- Web chạy thật: https://bepnha.somee.com

## 1. Chức năng

| Người dùng | Chức năng |
|---|---|
| Khách | Xem món theo danh mục, tìm theo tên món hoặc nguyên liệu, lọc theo độ khó và thời gian nấu, sắp xếp, phân trang |
| Thành viên | Đăng ký, đăng nhập, đổi mật khẩu; đăng món kèm ảnh (chờ duyệt); sửa, xóa món của mình; chấm sao (mỗi món một lần); lưu món yêu thích |
| Admin | Duyệt hoặc xóa bài kèm lý do; thêm, sửa, xóa danh mục; xem thống kê nhanh |

## 2. Công nghệ

- ASP.NET MVC 5, .NET Framework 4.8
- SQL Server, Entity Framework 6 (Database First)
- Bootstrap 5, jQuery Validation
- Hosting: Somee.com (IIS + SQL Server 2022 Express)

## 3. Cơ sở dữ liệu

5 bảng, tạo bằng file `BepNhaDB.sql`:

| Bảng | Lưu gì | Liên kết |
|---|---|---|
| Categories | Danh mục món | |
| Users | Tài khoản, mật khẩu đã mã hóa, quyền admin | |
| Recipes | Công thức, ảnh, trạng thái duyệt | thuộc 1 Category, 1 User |
| Ratings | Số sao và nhận xét | thuộc 1 Recipe, mỗi người chấm 1 lần |
| Favorites | Món đã lưu | nối User với Recipe, không lưu trùng |

## 4. Cấu trúc project

```
RecipeSharingWebsite/
  Controllers/    Home, Recipes, Account, Admin, Categories
  Models/         Model EF (.edmx), ràng buộc validate, quy tắc upload ảnh và phân quyền
  Views/          mỗi controller một thư mục; Shared chứa _Layout và partial dùng chung
  Content/        Site.css dùng chung, pages/ là CSS riêng từng trang, Images/
  Scripts/pages/  JS riêng của trang đăng món và trang chi tiết
RecipeSharingWebsite.Tests/   unit test cho quy tắc upload ảnh và phân quyền
BepNhaDB.sql                  script tạo database và dữ liệu mẫu
```

## 5. Chạy trên máy

1. Mở `BepNhaDB.sql` trong SSMS (server `.\SQLEXPRESS`) và chạy để tạo database `BepNhaDB`.
2. Mở `RecipeSharingWebsite.sln` bằng Visual Studio 2022, bấm F5.
3. Nếu SQL Server có tên khác, sửa `data source` trong chuỗi kết nối `BepNhaDBEntities` ở `RecipeSharingWebsite/Web.config`.

Tài khoản mẫu (admin và người dùng) ghi ở đầu file `BepNhaDB.sql`.

## 6. Kiểm thử

Mở Test > Test Explorer trong Visual Studio, chạy tất cả (8 test).

## 7. Nguồn ảnh

Ảnh món ăn lấy từ Wikimedia Commons, giấy phép tự do. Tác giả và giấy phép từng ảnh ghi trong `RecipeSharingWebsite/Content/Images/NGUON-ANH.txt`.
