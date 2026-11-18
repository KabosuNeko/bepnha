using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class CategoriesController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        // quản lý danh mục chỉ dành cho admin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                filterContext.Result = RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
        }

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            var c2 = db.Categories.Find(category.CategoryID);
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!";
                return View(category);
            }
            if (c2 == null)
            {
                ViewBag.Err = "Không tìm thấy danh mục!";
                return View(category);
            }

            c2.CategoryName = category.CategoryName;
            c2.Description = category.Description;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            // danh mục còn công thức thì không cho xóa
            if (category.Recipes.Count > 0)
            {
                ViewBag.Err = "Danh mục này còn " + category.Recipes.Count + " công thức nên không xóa được!";
                return View(category);
            }
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
