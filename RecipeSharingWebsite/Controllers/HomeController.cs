using System;
using System.Collections.Generic;
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
            // chi lay bai da duyet
            var approved = db.Recipes.Where(r => r.Status == Recipe.DaDuyet);

            // lay 6 mon moi nhat
            var latest = approved.OrderByDescending(r => r.CreatedAt).Take(6).ToList();
            ViewBag.Categories = db.Categories.ToList();

            // mon ngon cua tuan: moi tuan doi 1 mon theo so tuan trong nam
            var all = approved.OrderBy(r => r.RecipeID).ToList();
            if (all.Count > 0)
            {
                int tuan = DateTime.Now.DayOfYear / 7;
                ViewBag.Featured = all[tuan % all.Count];
            }

            return View(latest);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thông tin liên hệ.";
            return View();
        }
    }
}
