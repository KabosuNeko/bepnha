using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class RecipesController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        // GET: Recipes - chi hien bai da duyet
        public ActionResult Index(string searchString, string sortStyle, int? categoryID)
        {
            ViewBag.NameSort = sortStyle == "name" ? "name_desc" : "name";
            ViewBag.TimeSort = sortStyle == "time" ? "time_desc" : "time";

            var recipes = from r in db.Recipes
                          where r.Status == Recipe.DaDuyet
                          select r;

            // tim kiem theo ten mon
            if (!String.IsNullOrEmpty(searchString))
                recipes = recipes.Where(r => r.Title.Contains(searchString));

            // loc theo danh muc
            if (categoryID != null)
                recipes = recipes.Where(r => r.CategoryID == categoryID);

            switch (sortStyle)
            {
                case "name": recipes = recipes.OrderBy(r => r.Title); break;
                case "name_desc": recipes = recipes.OrderByDescending(r => r.Title); break;
                case "time": recipes = recipes.OrderBy(r => r.PrepTimeMinutes + r.CookTimeMinutes); break;
                case "time_desc": recipes = recipes.OrderByDescending(r => r.PrepTimeMinutes + r.CookTimeMinutes); break;
                default: recipes = recipes.OrderByDescending(r => r.CreatedAt); break;
            }

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.SearchString = searchString;
            return View(recipes.ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int id)
        {
            var recipe = db.Recipes.Find(id);
            // bai chua duyet / bi xoa: chi chu bai va admin xem duoc
            if (recipe == null || (recipe.Status != Recipe.DaDuyet && !DuocSua(recipe)))
                return HttpNotFound();

            ViewBag.DuocSua = DuocSua(recipe);
            ViewBag.LaChuBai = LaChuBai(recipe);
            return View(recipe);
        }

        // POST: Recipes/Rate - danh gia mon an
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(Rating rating)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { id = rating.RecipeID }) });

            var recipe = db.Recipes.Find(rating.RecipeID);
            if (recipe == null || recipe.Status != Recipe.DaDuyet)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                rating.ReviewerName = (string)Session["FullName"];
                rating.CreatedAt = DateTime.Now;
                db.Ratings.Add(rating);
                db.SaveChanges();
                TempData["Msg"] = "Cảm ơn bạn đã đánh giá món này!";
            }
            else
            {
                TempData["Err"] = "Vui lòng chọn số sao từ 1 đến 5.";
            }
            return RedirectToAction("Details", new { id = rating.RecipeID });
        }

        // GET: Recipes/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create") });

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe recipe)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create") });

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            if (ModelState.IsValid)
            {
                // nguoi dang lay theo tai khoan, khong cho nhap tay
                recipe.UserID = (int)Session["UserID"];
                recipe.Author = (string)Session["FullName"];
                recipe.CreatedAt = DateTime.Now;
                recipe.ImagePath = "~/Content/Images/placeholder.svg"; // TODO: lam chuc nang upload anh
                // admin dang thi hien luon, nguoi dung thuong phai cho duyet
                recipe.Status = LaAdmin() ? Recipe.DaDuyet : Recipe.ChoDuyet;
                recipe.RejectReason = null;
                db.Recipes.Add(recipe);
                db.SaveChanges();

                TempData["Msg"] = recipe.Status == Recipe.ChoDuyet
                    ? "Đã gửi công thức. Bài sẽ hiện sau khi admin duyệt."
                    : "Đã đăng công thức.";
                return RedirectToAction("MyRecipes", "Account");
            }
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();
            if (!DuocSua(recipe))
                return KhongCoQuyen();

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Recipe recipe)
        {
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            var r2 = db.Recipes.Find(recipe.RecipeID);
            if (r2 == null)
            {
                ViewBag.Err = "Không tìm thấy công thức!";
                return View(recipe);
            }
            if (!DuocSua(r2))
                return KhongCoQuyen();
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!";
                return View(recipe);
            }

            r2.Title = recipe.Title;
            r2.CategoryID = recipe.CategoryID;
            r2.Description = recipe.Description;
            r2.Ingredients = recipe.Ingredients;
            r2.Instructions = recipe.Instructions;
            r2.PrepTimeMinutes = recipe.PrepTimeMinutes;
            r2.CookTimeMinutes = recipe.CookTimeMinutes;
            r2.Servings = recipe.Servings;
            r2.Difficulty = recipe.Difficulty;
            // nguoi dung sua bai thi phai duyet lai, admin sua thi giu nguyen
            if (!LaAdmin())
            {
                r2.Status = Recipe.ChoDuyet;
                r2.RejectReason = null;
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { id = recipe.RecipeID });
        }

        // GET: Recipes/Delete/5 - chu bai tu xoa bai cua minh
        public ActionResult Delete(int id)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();
            if (!LaChuBai(recipe))
                return KhongCoQuyen();
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();
            if (!LaChuBai(recipe))
                return KhongCoQuyen();

            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("MyRecipes", "Account");
        }

        // ===== kiem tra quyen =====
        private bool LaAdmin()
        {
            return Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
        }

        private bool LaChuBai(Recipe recipe)
        {
            return Session["UserID"] != null && recipe.UserID == (int)Session["UserID"];
        }

        private bool DuocSua(Recipe recipe)
        {
            return LaAdmin() || LaChuBai(recipe);
        }

        // chua dang nhap thi chuyen sang dang nhap, da dang nhap ma khong co quyen thi bao loi 403
        private ActionResult KhongCoQuyen()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}
