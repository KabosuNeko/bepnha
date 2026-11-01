using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Controllers
{
    public class RecipesController : Controller
    {
        BepNhaDBEntities db = new BepNhaDBEntities();

        // GET: Recipes
        public ActionResult Index(string searchString, string sortStyle, int? categoryID)
        {
            ViewBag.NameSort = sortStyle == "name" ? "name_desc" : "name";
            ViewBag.TimeSort = sortStyle == "time" ? "time_desc" : "time";

            var recipes = from r in db.Recipes select r;

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
            if (recipe == null)
                return HttpNotFound();
            return View(recipe);
        }

        // GET: Recipes/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe recipe)
        {
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            if (ModelState.IsValid)
            {
                recipe.CreatedAt = DateTime.Now;
                recipe.ImagePath = "~/Content/Images/placeholder.svg"; // TODO: lam chuc nang upload anh
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Recipe recipe)
        {
            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            var r2 = db.Recipes.Find(recipe.RecipeID);
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!";
                return View(recipe);
            }
            if (r2 == null)
            {
                ViewBag.Err = "Không tìm thấy công thức!";
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
            r2.Author = recipe.Author;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = recipe.RecipeID });
        }

        // GET: Recipes/Delete/5
        public ActionResult Delete(int id)
        {
            var recipe = db.Recipes.Find(id);
            if (recipe == null)
                return HttpNotFound();
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
