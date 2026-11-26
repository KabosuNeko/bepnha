// JS riêng của trang Đăng món và Sửa món (nạp qua @section scripts)
// Khi chọn ảnh: ảnh quá lớn thì báo lỗi và bỏ chọn, ảnh hợp lệ thì hiện xem trước.
(function () {
    var oChon = document.getElementById('anhMon');
    var khungXem = document.getElementById('xemTruoc');
    var loi = document.getElementById('loiAnh');
    if (!oChon || !khungXem) return;

    var toiDa = parseInt(oChon.dataset.toiDa, 10);    // dung lượng tối đa (byte), lấy từ server
    var anhCu = khungXem.getAttribute('src');          // ảnh ban đầu, để trả lại khi bỏ chọn
    var anCu = khungXem.classList.contains('d-none');  // lúc đầu khung có đang ẩn không

    function traLaiAnhCu() {
        khungXem.src = anhCu;
        khungXem.classList.toggle('d-none', anCu);
    }

    oChon.addEventListener('change', function () {
        var file = this.files[0];
        loi.textContent = '';

        // bỏ chọn file thì hiện lại ảnh cũ
        if (!file) {
            traLaiAnhCu();
            return;
        }

        // ảnh quá lớn: báo ngay để form không gửi lên server rồi ra trang lỗi
        if (file.size > toiDa) {
            loi.textContent = 'Ảnh này nặng ' + (file.size / 1048576).toFixed(1) + ' MB, chỉ nhận ảnh tối đa '
                + (toiDa / 1048576) + ' MB. Bạn chọn ảnh khác nhé.';
            this.value = '';
            traLaiAnhCu();
            return;
        }

        // ảnh hợp lệ: hiện xem trước ngay, chưa cần gửi lên server
        khungXem.src = URL.createObjectURL(file);
        khungXem.alt = 'Ảnh xem trước: ' + file.name;
        khungXem.classList.remove('d-none');
    });
})();
