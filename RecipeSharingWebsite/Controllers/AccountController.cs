using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class AccountController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (db.Users.Any(u => u.Username == model.Username))
                ModelState.AddModelError("Username", "Tên đăng nhập đã có người dùng");

            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Username = model.Username,
                FullName = model.FullName,
                PasswordHash = Crypto.HashPassword(model.Password), // mã hóa mật khẩu, không lưu mật khẩu thật
                IsAdmin = false
            };
            db.Users.Add(user);
            db.SaveChanges();

            DangNhap(user);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var user = db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null || !Crypto.VerifyHashedPassword(user.PasswordHash, model.Password))
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
                return View(model);
            }

            DangNhap(user);

            // chỉ quay về link trong web mình, tránh bị chuyển sang trang lạ
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/MyRecipes - bài của tôi: xem trạng thái duyệt và lý do bị xóa
        public ActionResult MyRecipes()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyRecipes") });

            int userId = (int)Session["UserID"];
            var recipes = db.Recipes.Where(r => r.UserID == userId)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToList();
            return View(recipes);
        }

        // GET: Account/Favorites - món đã lưu
        public ActionResult Favorites()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", new { returnUrl = Url.Action("Favorites") });

            int userId = (int)Session["UserID"];
            // món đã lưu (đã duyệt), món lưu sau cùng lên đầu; Include để thẻ món không hỏi database lại
            var recipes = db.Recipes.Include(r => r.Category).Include(r => r.Ratings)
                                    .Where(r => r.Status == Recipe.DaDuyet && r.Favorites.Any(f => f.UserID == userId))
                                    .OrderByDescending(r => r.Favorites.Where(f => f.UserID == userId).Max(f => f.CreatedAt))
                                    .ToList();
            return View(recipes);
        }

        // GET: Account/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", new { returnUrl = Url.Action("ChangePassword") });
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", new { returnUrl = Url.Action("ChangePassword") });

            var user = db.Users.Find((int)Session["UserID"]);
            if (user == null)
                return HttpNotFound();

            // phải nhập đúng mật khẩu hiện tại mới được đổi
            if (!string.IsNullOrEmpty(model.OldPassword) && !Crypto.VerifyHashedPassword(user.PasswordHash, model.OldPassword))
                ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không đúng");

            if (!ModelState.IsValid)
                return View(model);

            user.PasswordHash = Crypto.HashPassword(model.NewPassword);
            db.SaveChanges();
            TempData["Msg"] = "Đã đổi mật khẩu.";
            return RedirectToAction("Index", "Home");
        }

        // lưu người dùng vào Session
        private void DangNhap(User user)
        {
            Session["UserID"] = user.UserID;
            Session["FullName"] = user.FullName;
            Session["IsAdmin"] = user.IsAdmin;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose(); // giải phóng kết nối database khi xong mỗi yêu cầu
            base.Dispose(disposing);
        }
    }
}
