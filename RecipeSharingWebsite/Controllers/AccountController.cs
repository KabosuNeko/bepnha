using System;
using System.Collections.Generic;
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
                PasswordHash = Crypto.HashPassword(model.Password), // ma hoa mat khau, khong luu mat khau that
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

            // chi quay ve link trong web minh, tranh bi chuyen sang trang la
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

        // GET: Account/MyRecipes - bai cua toi: xem trang thai duyet va ly do bi xoa
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

        // luu nguoi dung vao Session
        private void DangNhap(User user)
        {
            Session["UserID"] = user.UserID;
            Session["FullName"] = user.FullName;
            Session["IsAdmin"] = user.IsAdmin;
        }
    }
}
