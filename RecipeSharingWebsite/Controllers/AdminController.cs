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

        // chay truoc moi action: khong phai admin thi chuyen ve trang dang nhap
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                filterContext.Result = RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
        }

        // GET: Admin?status=Chờ duyệt - danh sach bai theo trang thai
        public ActionResult Index(string status)
        {
            if (String.IsNullOrEmpty(status))
                status = Recipe.ChoDuyet;

            ViewBag.Status = status;
            ViewBag.SoChoDuyet = db.Recipes.Count(r => r.Status == Recipe.ChoDuyet);
            var recipes = db.Recipes.Where(r => r.Status == status)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToList();
            return View(recipes);
        }

        // POST: Admin/Approve/5 - duyet bai
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

        // POST: Admin/Reject/5 - xoa bai kem ly do, nguoi dang xem ly do o trang Bai cua toi
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
