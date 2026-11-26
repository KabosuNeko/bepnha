namespace RecipeSharingWebsite.Models
{
    // Quy tắc phân quyền dùng chung cho controller và view.
    // Chỉ nhận giá trị đơn giản (không dùng Session trực tiếp) để viết test được.
    public static class Quyen
    {
        // giá trị lưu trong Session["IsAdmin"]: chưa đăng nhập thì là null
        public static bool LaAdmin(object isAdmin)
        {
            return isAdmin is bool b && b;
        }

        // người đang xem là người đăng bài (món mẫu chưa gắn tài khoản thì không ai là chủ)
        public static bool LaChuBai(int? nguoiXem, int? chuBai)
        {
            return nguoiXem != null && nguoiXem == chuBai;
        }

        // admin hoặc chủ bài mới được sửa bài
        public static bool DuocSua(int? nguoiXem, bool laAdmin, int? chuBai)
        {
            return laAdmin || LaChuBai(nguoiXem, chuBai);
        }
    }
}
