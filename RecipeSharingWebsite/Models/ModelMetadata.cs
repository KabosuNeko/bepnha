using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeSharingWebsite.Models
{

    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {
    }

    public class CategoryMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục tối đa 50 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [StringLength(200, ErrorMessage = "Mô tả tối đa 200 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(RecipeMetadata))]
    public partial class Recipe
    {
        // 3 trang thai cua bai dang (cot Status)
        public const string ChoDuyet = "Chờ duyệt";
        public const string DaDuyet = "Đã duyệt";
        public const string BiXoa = "Bị xóa";

        [Display(Name = "Tổng thời gian")]
        public int TotalTimeMinutes
        {
            get { return PrepTimeMinutes + CookTimeMinutes; }
        }
    }

    public class RecipeMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên món")]
        [StringLength(120, ErrorMessage = "Tên món tối đa 120 ký tự")]
        [Display(Name = "Tên món")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn")]
        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        [Display(Name = "Mô tả ngắn")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nguyên liệu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Nguyên liệu (mỗi dòng một nguyên liệu)")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập các bước làm")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Các bước làm (mỗi dòng một bước)")]
        public string Instructions { get; set; }

        [Range(1, 600, ErrorMessage = "Thời gian chuẩn bị từ 1 đến 600 phút")]
        [Display(Name = "Thời gian chuẩn bị (phút)")]
        public int PrepTimeMinutes { get; set; }

        [Range(1, 600, ErrorMessage = "Thời gian nấu từ 1 đến 600 phút")]
        [Display(Name = "Thời gian nấu (phút)")]
        public int CookTimeMinutes { get; set; }

        [Range(1, 50, ErrorMessage = "Khẩu phần từ 1 đến 50")]
        [Display(Name = "Khẩu phần (người)")]
        public int Servings { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn độ khó")]
        [Display(Name = "Độ khó")]
        public string Difficulty { get; set; }

        // khong bat nhap: lay ho ten cua tai khoan dang dang nhap
        [Display(Name = "Người đăng")]
        public string Author { get; set; }

        [Display(Name = "Ảnh món ăn")]
        public string ImagePath { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Lý do bị xóa")]
        public string RejectReason { get; set; }
    }

    [MetadataType(typeof(RatingMetadata))]
    public partial class Rating
    {
    }

    public class RatingMetadata
    {
        [Range(1, 5, ErrorMessage = "Vui lòng chọn từ 1 đến 5 sao")]
        [Display(Name = "Số sao")]
        public int Stars { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận tối đa 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Bình luận")]
        public string Comment { get; set; }
    }
}
