using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class HomeController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        public ActionResult Index()
        {
            // chỉ lấy bài đã duyệt
            var approved = db.Recipes.Where(r => r.Status == Recipe.DaDuyet);

            // lấy 6 món mới nhất; Include để lấy luôn danh mục và đánh giá, không hỏi database lại cho từng thẻ
            var latest = approved.Include(r => r.Category).Include(r => r.Ratings)
                                 .OrderByDescending(r => r.CreatedAt).Take(6).ToList();
            ViewBag.Categories = db.Categories.ToList();

            // ảnh đại diện mỗi danh mục: ảnh của món đầu tiên trong danh mục, lấy hết trong 1 câu truy vấn
            ViewBag.AnhDanhMuc = approved.Where(r => r.ImagePath != null)
                                         .GroupBy(r => r.CategoryID)
                                         .Select(g => new { g.Key, Anh = g.OrderBy(r => r.RecipeID).Select(r => r.ImagePath).FirstOrDefault() })
                                         .ToDictionary(x => x.Key, x => x.Anh);

            // món ngon của tuần: mỗi tuần đổi 1 món theo số tuần trong năm (đếm trước, chỉ tải đúng 1 món)
            int soMon = approved.Count();
            if (soMon > 0)
            {
                int tuan = DateTime.Now.DayOfYear / 7;
                ViewBag.Featured = approved.OrderBy(r => r.RecipeID).Skip(tuan % soMon).FirstOrDefault();
            }

            return View(latest);
        }

        // trang liên hệ: thông tin thành viên nhóm
        public ActionResult Contact()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose(); // giải phóng kết nối database khi xong mỗi yêu cầu
            base.Dispose(disposing);
        }
    }
}
