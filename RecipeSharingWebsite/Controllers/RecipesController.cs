using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        const int SoMonMoiTrang = 8;

        // GET: Recipes - chỉ hiện bài đã duyệt
        public ActionResult Index(string searchString, string sortStyle, int? categoryID, string difficulty, int? maxTime, int page = 1)
        {
            ViewBag.NameSort = sortStyle == "name" ? "name_desc" : "name";
            ViewBag.TimeSort = sortStyle == "time" ? "time_desc" : "time";

            // Include: lấy luôn danh mục trong cùng câu truy vấn, không phải hỏi database lại cho từng món
            var recipes = from r in db.Recipes.Include(r => r.Category)
                          where r.Status == Recipe.DaDuyet
                          select r;

            // tìm kiếm theo tên món hoặc nguyên liệu
            if (!String.IsNullOrEmpty(searchString))
                recipes = recipes.Where(r => r.Title.Contains(searchString) || r.Ingredients.Contains(searchString));

            // lọc theo danh mục
            if (categoryID != null)
                recipes = recipes.Where(r => r.CategoryID == categoryID);

            // lọc theo độ khó và tổng thời gian nấu
            if (!String.IsNullOrEmpty(difficulty))
                recipes = recipes.Where(r => r.Difficulty == difficulty);
            if (maxTime != null)
                recipes = recipes.Where(r => r.PrepTimeMinutes + r.CookTimeMinutes <= maxTime);

            switch (sortStyle)
            {
                case "name": recipes = recipes.OrderBy(r => r.Title); break;
                case "name_desc": recipes = recipes.OrderByDescending(r => r.Title); break;
                case "time": recipes = recipes.OrderBy(r => r.PrepTimeMinutes + r.CookTimeMinutes); break;
                case "time_desc": recipes = recipes.OrderByDescending(r => r.PrepTimeMinutes + r.CookTimeMinutes); break;
                default: recipes = recipes.OrderByDescending(r => r.CreatedAt); break;
            }

            // phân trang: mỗi trang 8 món
            int tongSo = recipes.Count();
            int soTrang = Math.Max(1, (int)Math.Ceiling(tongSo / (double)SoMonMoiTrang));
            page = Math.Min(Math.Max(page, 1), soTrang);

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.SearchString = searchString;
            ViewBag.Total = tongSo;
            ViewBag.Page = page;
            ViewBag.TotalPages = soTrang;
            return View(recipes.Skip((page - 1) * SoMonMoiTrang).Take(SoMonMoiTrang).ToList());
        }

        // GET: Recipes/ByAuthor?name=Minh Anh - các món của một người đăng
        public ActionResult ByAuthor(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return RedirectToAction("Index");

            // Lưu ý: tìm theo tên người đăng vì món mẫu chưa gắn tài khoản; hai người trùng tên sẽ gộp chung
            var recipes = db.Recipes.Include(r => r.Category).Include(r => r.Ratings)
                                    .Where(r => r.Status == Recipe.DaDuyet && r.Author == name)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToList();
            ViewBag.Name = name;
            return View(recipes);
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int id)
        {
            var recipe = db.Recipes.Find(id);
            // bài chưa duyệt / bị xóa: chỉ chủ bài và admin xem được
            if (recipe == null || (recipe.Status != Recipe.DaDuyet && !DuocSua(recipe)))
                return HttpNotFound();

            ViewBag.DuocSua = DuocSua(recipe);
            ViewBag.LaChuBai = LaChuBai(recipe);

            // món liên quan: 3 món mới nhất cùng danh mục
            ViewBag.Related = db.Recipes.Include(r => r.Category).Include(r => r.Ratings)
                                        .Where(r => r.CategoryID == recipe.CategoryID && r.RecipeID != recipe.RecipeID && r.Status == Recipe.DaDuyet)
                                        .OrderByDescending(r => r.CreatedAt)
                                        .Take(3)
                                        .ToList();

            // người đang xem đã lưu món này chưa, đã đánh giá món này chưa
            int? userId = (int?)Session["UserID"];
            ViewBag.DaLuu = userId != null && db.Favorites.Any(f => f.UserID == userId && f.RecipeID == id);
            ViewBag.DaDanhGia = DaDanhGia(id);
            return View(recipe);
        }

        // POST: Recipes/ToggleFavorite/5 - luu = true thì lưu món, false thì bỏ lưu.
        // Nút gửi sẵn việc cần làm nên bấm đúp vẫn ra đúng kết quả, không bị lưu rồi bỏ lưu ngay.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleFavorite(int id, bool luu)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { id }) });

            var recipe = db.Recipes.Find(id);
            if (recipe == null || recipe.Status != Recipe.DaDuyet)
                return HttpNotFound();

            int userId = (int)Session["UserID"];
            var fav = db.Favorites.FirstOrDefault(f => f.UserID == userId && f.RecipeID == id);
            if (luu && fav == null)
            {
                db.Favorites.Add(new Favorite { UserID = userId, RecipeID = id, CreatedAt = DateTime.Now });
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    // hai lần bấm đến cùng lúc: lần kia đã lưu rồi (bảng có ràng buộc không cho trùng), coi như đã lưu
                }
            }
            else if (!luu && fav != null)
            {
                db.Favorites.Remove(fav);
                db.SaveChanges();
            }
            TempData["Msg"] = luu ? "Đã lưu món vào Món đã lưu." : "Đã bỏ lưu món.";
            return RedirectToAction("Details", new { id });
        }

        // POST: Recipes/Rate - đánh giá món ăn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate(Rating rating)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { id = rating.RecipeID }) });

            var recipe = db.Recipes.Find(rating.RecipeID);
            if (recipe == null || recipe.Status != Recipe.DaDuyet)
                return HttpNotFound();

            // mỗi người chỉ chấm một món một lần, chủ bài không tự chấm món mình
            if (LaChuBai(recipe))
            {
                TempData["Err"] = "Bạn không thể tự đánh giá món của mình.";
                return RedirectToAction("Details", new { id = rating.RecipeID });
            }
            if (DaDanhGia(rating.RecipeID))
            {
                TempData["Err"] = "Bạn đã đánh giá món này rồi.";
                return RedirectToAction("Details", new { id = rating.RecipeID });
            }

            if (ModelState.IsValid)
            {
                rating.ReviewerName = (string)Session["FullName"];
                rating.CreatedAt = DateTime.Now;
                db.Ratings.Add(rating);
                try
                {
                    db.SaveChanges();
                    TempData["Msg"] = "Cảm ơn bạn đã đánh giá món này!";
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    // bấm đúp "Gửi": lần kia đã lưu rồi, bảng có ràng buộc UQ_Ratings nên lần này bị chặn
                    TempData["Err"] = "Bạn đã đánh giá món này rồi.";
                }
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
        public ActionResult Create(Recipe recipe, HttpPostedFileBase anhMon)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create") });

            ViewBag.CategoryList = new SelectList(db.Categories, "CategoryID", "CategoryName");
            string duongDanAnh = LuuAnh(anhMon);
            if (ModelState.IsValid)
            {
                // người đăng lấy theo tài khoản, không cho nhập tay
                recipe.UserID = (int)Session["UserID"];
                recipe.Author = (string)Session["FullName"];
                recipe.CreatedAt = DateTime.Now;
                recipe.ImagePath = duongDanAnh ?? "~/Content/Images/placeholder.svg"; // không chọn ảnh thì dùng ảnh mặc định
                // admin đăng thì hiện luôn, người dùng thường phải chờ duyệt
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
        public ActionResult Edit(Recipe recipe, HttpPostedFileBase anhMon)
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
            string duongDanAnh = LuuAnh(anhMon);
            if (!ModelState.IsValid)
            {
                ViewBag.Err = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại!";
                recipe.ImagePath = r2.ImagePath; // để form vẫn hiện ảnh hiện tại
                return View(recipe);
            }

            // có chọn ảnh mới thì thay, không chọn thì giữ ảnh cũ
            string anhCu = r2.ImagePath;
            if (duongDanAnh != null)
                r2.ImagePath = duongDanAnh;

            r2.Title = recipe.Title;
            r2.CategoryID = recipe.CategoryID;
            r2.Description = recipe.Description;
            r2.Ingredients = recipe.Ingredients;
            r2.Instructions = recipe.Instructions;
            r2.PrepTimeMinutes = recipe.PrepTimeMinutes;
            r2.CookTimeMinutes = recipe.CookTimeMinutes;
            r2.Servings = recipe.Servings;
            r2.Difficulty = recipe.Difficulty;
            // người dùng sửa bài thì phải duyệt lại, admin sửa thì giữ nguyên
            if (!LaAdmin())
            {
                r2.Status = Recipe.ChoDuyet;
                r2.RejectReason = null;
            }
            db.SaveChanges();
            if (duongDanAnh != null)
                XoaAnhUpload(anhCu); // lưu xong mới xóa ảnh cũ, lỡ lưu lỗi thì vẫn còn ảnh
            return RedirectToAction("Details", new { id = recipe.RecipeID });
        }

        // GET: Recipes/Delete/5 - chủ bài tự xóa bài của mình
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

            string anh = recipe.ImagePath;
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            XoaAnhUpload(anh);
            return RedirectToAction("MyRecipes", "Account");
        }

        // ===== upload ảnh món ăn =====
        // Lưu ảnh vào Content/Images/Uploads, trả về đường dẫn "~/..."; không chọn ảnh thì trả null.
        // Ảnh sai định dạng hoặc quá lớn thì thêm lỗi vào ModelState để form báo lại.
        private string LuuAnh(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            string loi = AnhMon.KiemTra(file.FileName, file.ContentLength);
            if (loi != null)
            {
                ModelState.AddModelError("ImagePath", loi);
                return null;
            }
            if (!ModelState.IsValid)
                return null; // form còn lỗi khác thì chưa lưu ảnh, tránh để file thừa

            // đặt tên file mới bằng Guid để không trùng và không dùng tên file người dùng gửi lên
            string tenFile = Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(file.FileName).ToLower();
            string thuMuc = Server.MapPath("~/Content/Images/Uploads");
            System.IO.Directory.CreateDirectory(thuMuc);
            file.SaveAs(System.IO.Path.Combine(thuMuc, tenFile));
            return "~/Content/Images/Uploads/" + tenFile;
        }

        // Xóa file ảnh người dùng đã upload. Ảnh mẫu (không nằm trong Uploads) thì giữ nguyên.
        private void XoaAnhUpload(string duongDan)
        {
            if (duongDan == null || !duongDan.StartsWith("~/Content/Images/Uploads/"))
                return;
            string file = Server.MapPath(duongDan);
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);
        }

        // người đang đăng nhập đã đánh giá món này chưa
        // Lưu ý: so theo họ tên vì bảng Ratings chưa có cột UserID; hai người trùng họ tên sẽ chặn nhau, thêm UserID nếu cần chính xác
        private bool DaDanhGia(int recipeId)
        {
            string ten = (string)Session["FullName"];
            return ten != null && db.Ratings.Any(x => x.RecipeID == recipeId && x.ReviewerName == ten);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose(); // giải phóng kết nối database khi xong mỗi yêu cầu
            base.Dispose(disposing);
        }

        // ===== kiểm tra quyền =====
        // quy tắc nằm ở Models/Quyen.cs (có test), ở đây chỉ lấy thông tin từ Session
        private bool LaAdmin()
        {
            return Quyen.LaAdmin(Session["IsAdmin"]);
        }

        private bool LaChuBai(Recipe recipe)
        {
            return Quyen.LaChuBai((int?)Session["UserID"], recipe.UserID);
        }

        private bool DuocSua(Recipe recipe)
        {
            return Quyen.DuocSua((int?)Session["UserID"], LaAdmin(), recipe.UserID);
        }

        // chưa đăng nhập thì chuyển sang đăng nhập, đã đăng nhập mà không có quyền thì báo lỗi 403
        private ActionResult KhongCoQuyen()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}
