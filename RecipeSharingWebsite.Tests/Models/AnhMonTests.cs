using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Tests.Models
{
    // Kiểm tra quy định ảnh món ăn khi upload
    [TestClass]
    public class AnhMonTests
    {
        const int MB = 1024 * 1024;

        [TestMethod]
        public void AnhJpgNhoDuocNhan()
        {
            Assert.IsNull(AnhMon.KiemTra("pho-bo.jpg", 1 * MB));
            Assert.IsNull(AnhMon.KiemTra("ANH.PNG", 5 * MB)); // đuôi viết hoa vẫn nhận
            Assert.IsNull(AnhMon.KiemTra("anh.webp", 8 * MB)); // đúng 8 MB vẫn nhận
        }

        [TestMethod]
        public void SaiDuoiBiTuChoi()
        {
            Assert.IsNotNull(AnhMon.KiemTra("ghi-chu.txt", 1 * MB));
            Assert.IsNotNull(AnhMon.KiemTra("trang.html", 1 * MB));
            Assert.IsNotNull(AnhMon.KiemTra("khongcoduoi", 1 * MB));
        }

        [TestMethod]
        public void QuaTamMBBiTuChoi()
        {
            Assert.IsNotNull(AnhMon.KiemTra("anh-dien-thoai.jpg", 8 * MB + 1));
        }
    }
}
