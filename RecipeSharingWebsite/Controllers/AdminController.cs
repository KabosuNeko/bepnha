using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class AdminController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        // chạy trước mọi action: không phải admin thì chuyển về trang đăng nhập
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                filterContext.Result = RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
        }

        // GET: Admin?status=Chờ duyệt - danh sách bài theo trạng thái
        public ActionResult Index(string status)
        {
            if (String.IsNullOrEmpty(status))
                status = Recipe.ChoDuyet;

            ViewBag.Status = status;
            ViewBag.SoChoDuyet = db.Recipes.Count(r => r.Status == Recipe.ChoDuyet);

            // thống kê nhanh, đếm thẳng từ database
            ViewBag.SoDaDuyet = db.Recipes.Count(r => r.Status == Recipe.DaDuyet);
            ViewBag.SoBiXoa = db.Recipes.Count(r => r.Status == Recipe.BiXoa);
            ViewBag.SoTaiKhoan = db.Users.Count();
            ViewBag.SoDanhGia = db.Ratings.Count();
            // món điểm cao nhất (ít nhất 2 lượt chấm), bằng điểm thì món nhiều lượt chấm hơn đứng trước
            ViewBag.MonDiemCao = db.Recipes.Where(r => r.Status == Recipe.DaDuyet && r.Ratings.Count >= 2)
                                           .OrderByDescending(r => r.Ratings.Average(x => (double)x.Stars))
                                           .ThenByDescending(r => r.Ratings.Count)
                                           .FirstOrDefault();
            var recipes = db.Recipes.Where(r => r.Status == status)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToList();
            return View(recipes);
        }

        // POST: Admin/Approve/5 - duyệt bài
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();

            recipe.Status = Recipe.DaDuyet;
            recipe.RejectReason = null;
            db.SaveChanges();
            TempData["Msg"] = "Đã duyệt bài \"" + recipe.Title + "\".";
            return RedirectToAction("Index");
        }

        // POST: Admin/Reject/5 - xóa bài kèm lý do, người đăng xem lý do ở trang Bài của tôi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id, string reason)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();

            if (String.IsNullOrWhiteSpace(reason))
            {
                TempData["Err"] = "Vui lòng nhập lý do khi xóa bài.";
                return RedirectToAction("Index", new { status = recipe.Status });
            }

            recipe.Status = Recipe.BiXoa;
            recipe.RejectReason = reason.Length > 500 ? reason.Substring(0, 500) : reason;
            db.SaveChanges();
            TempData["Msg"] = "Đã xóa bài \"" + recipe.Title + "\" và gửi lý do cho người đăng.";
            return RedirectToAction("Index", new { status = Recipe.BiXoa });
        }
    }
}
