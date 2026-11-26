using System.IO;
using System.Linq;

namespace RecipeSharingWebsite.Models
{
    // Quy định ảnh món ăn khi upload. Tách riêng (không dùng gì của web) để viết test được.
    public static class AnhMon
    {
        public const int DungLuongToiDa = 8 * 1024 * 1024; // 8 MB, đủ cho ảnh chụp bằng điện thoại
        static readonly string[] DuoiDuocPhep = { ".jpg", ".jpeg", ".png", ".webp" };

        // Trả về câu báo lỗi, hoặc null nếu ảnh hợp lệ
        public static string KiemTra(string tenFile, int dungLuong)
        {
            string duoi = (Path.GetExtension(tenFile) ?? "").ToLower();
            if (!DuoiDuocPhep.Contains(duoi))
                return "Chỉ nhận ảnh .jpg, .jpeg, .png hoặc .webp";
            if (dungLuong > DungLuongToiDa)
                return "Ảnh tối đa 8 MB";
            return null;
        }
    }
}
