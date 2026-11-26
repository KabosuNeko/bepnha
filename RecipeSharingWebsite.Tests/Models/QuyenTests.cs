using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeSharingWebsite.Models;

namespace RecipeSharingWebsite.Tests.Models
{
    // Kiểm tra quy tắc ai được sửa / xóa bài
    [TestClass]
    public class QuyenTests
    {
        [TestMethod]
        public void ChuBaiDuocSua()
        {
            Assert.IsTrue(Quyen.LaChuBai(5, 5));
            Assert.IsTrue(Quyen.DuocSua(5, false, 5));
        }

        [TestMethod]
        public void NguoiKhacBiChan()
        {
            Assert.IsFalse(Quyen.LaChuBai(6, 5));
            Assert.IsFalse(Quyen.DuocSua(6, false, 5));
        }

        [TestMethod]
        public void ChuaDangNhapBiChan()
        {
            Assert.IsFalse(Quyen.DuocSua(null, false, 5));
            // món mẫu chưa gắn tài khoản: người chưa đăng nhập không được coi là chủ bài
            Assert.IsFalse(Quyen.LaChuBai(null, null));
        }

        [TestMethod]
        public void AdminDuocSuaMoiBai()
        {
            Assert.IsTrue(Quyen.DuocSua(1, true, 5));
            Assert.IsTrue(Quyen.DuocSua(1, true, null)); // cả món mẫu
            Assert.IsFalse(Quyen.LaChuBai(1, 5));        // nhưng không phải chủ bài
        }

        [TestMethod]
        public void LaAdminChiKhiSessionLaTrue()
        {
            Assert.IsTrue(Quyen.LaAdmin(true));
            Assert.IsFalse(Quyen.LaAdmin(false));
            Assert.IsFalse(Quyen.LaAdmin(null)); // chưa đăng nhập
        }
    }
}
