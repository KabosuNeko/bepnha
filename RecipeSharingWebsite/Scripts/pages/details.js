// JS riêng của trang Chi tiết món (nạp qua @section scripts)
// Tick nguyên liệu đã chuẩn bị; lưu vào localStorage để mở lại trang vẫn còn
(function () {
    var khung = document.querySelector('.ingredient-list');
    if (!khung) return;

    var khoa = 'nguyenlieu-' + khung.dataset.recipe;   // mỗi món một khóa riêng
    var oTick = khung.querySelectorAll('input[type=checkbox]');
    var daTick = [];
    try {
        daTick = JSON.parse(localStorage.getItem(khoa) || '[]');
    } catch (e) {
        // trình duyệt chặn localStorage (chế độ ẩn danh): vẫn tick được, chỉ không nhớ
    }

    oTick.forEach(function (o, i) {
        o.checked = daTick.indexOf(i) >= 0;
        o.addEventListener('change', function () {
            var ds = [];
            oTick.forEach(function (x, j) { if (x.checked) ds.push(j); });
            try {
                localStorage.setItem(khoa, JSON.stringify(ds));
            } catch (e) { }
        });
    });
})();
