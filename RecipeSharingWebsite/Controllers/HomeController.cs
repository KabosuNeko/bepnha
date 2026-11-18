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
            // chỉ lấy bài đã duyệt
            var approved = db.Recipes.Where(r => r.Status == Recipe.DaDuyet);

            // lấy 6 món mới nhất
            var latest = approved.OrderByDescending(r => r.CreatedAt).Take(6).ToList();
            ViewBag.Categories = db.Categories.ToList();

            // món ngon của tuần: mỗi tuần đổi 1 món theo số tuần trong năm
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
