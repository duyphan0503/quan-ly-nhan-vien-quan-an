using System;
using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// File xử lý cốt lõi tính toán lương bổng và quản trị thu nhập nhân sự.
    /// Toàn bộ các quy tắc bất biến và công thức toán học chuyên ngành đều được đặt ở đây —
    /// Các Form giao diện (UI) tuyệt đối không tự ý thực hiện các phép tài chính.
    /// </summary>
    public class BangLuongService
    {
        private readonly BangLuongDAL _blDAL = new BangLuongDAL();
        private readonly NhanVienDAL _nvDAL = new NhanVienDAL();

        // ══════════════════════════════════════════════
        //  CÁC QUY TẮC BẤT BIẾN NGHIỆP VỤ — hãy chỉ thay đổi ở đây
        //  nếu muốn tác động lên toàn bộ ứng dụng tính lương
        // ══════════════════════════════════════════════

        /// <summary>Số ngày công quy chuẩn trong một tháng hóa đơn.</summary>
        public const decimal NGAY_CONG_CHUAN = 26m;

        /// <summary>Tỷ lệ đóng bảo hiểm xã hội chuẩn (10.5% lương cơ sở hiện tại).</summary>
        public const decimal TY_LE_BHXH = 0.105m;

        // ══════════════════════════════════════════════
        //  TÍNH TOÁN LƯƠNG
        // ══════════════════════════════════════════════

        /// <summary>
        /// Kết quả của hàm tính lương tổng — đây chỉ là một vỏ bọc thuần dữ liệu
        /// không tác động (side-effects) thay đổi trạng thái gốc CSDL.
        /// </summary>
        public class KetQuaTinhLuong
        {
            public decimal LuongTheoCong { get; set; }
            public decimal BHXH { get; set; }
            public decimal Thue { get; set; }
            public decimal TongThucNhan { get; set; }
        }

        /// <summary>
        /// Hàm chức năng tính toán bảng lương tài chính cơ bản.
        /// Không dùng tới CSDL, không gây ảnh hưởng hàm ngoài — thiết kế dễ dàng cho Unit Test.
        /// </summary>
        /// <param name="luongCoBan">Mức lương cơ sở.</param>
        /// <param name="ngayCong">Số lượng ngày công đi làm thực tế.</param>
        /// <param name="tienUng">Tiền tạm ứng tháng của nhân viên.</param>
        /// <returns>Bảng thống kê kết quả nếu thành công, xuất lỗi nếu thất bại đầu vào.</returns>
        public ServiceResult<KetQuaTinhLuong> TinhLuong(
            decimal luongCoBan,
            decimal ngayCong,
            decimal tienUng
        )
        {
            if (ngayCong < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail("Ngày công không được âm.");

            if (ngayCong > 31)
                return ServiceResult<KetQuaTinhLuong>.Fail("Ngày công không được vượt quá 31.");

            if (tienUng < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail("Tiền ứng không được âm.");

            if (luongCoBan < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail("Lương cơ bản không hợp lệ.");

            // Công thức tính lương cốt lõi
            decimal luongTheoCong = Math.Round(luongCoBan / NGAY_CONG_CHUAN * ngayCong);
            decimal bhxh = Math.Round(luongCoBan * TY_LE_BHXH);
            decimal thue = 0m; // Thuế TNCN tạm giản lược — có thể mở rộng logic về sau

            decimal tongThucNhan = luongTheoCong - tienUng - bhxh - thue;
            if (tongThucNhan < 0)
                tongThucNhan = 0;

            return ServiceResult<KetQuaTinhLuong>.Ok(
                new KetQuaTinhLuong
                {
                    LuongTheoCong = luongTheoCong,
                    BHXH = bhxh,
                    Thue = thue,
                    TongThucNhan = tongThucNhan,
                }
            );
        }

        // ══════════════════════════════════════════════
        //  XỬ LÝ NGHIỆP VỤ BẢNG LƯƠNG TẠI CSDL
        // ══════════════════════════════════════════════

        /// <summary>
        /// Tiến hành tính toán và lưu trực tiếp hồ sơ tính lương của nhân viên vào CSDL.
        /// Tổng hợp quy trình: Xác thực → Tính Toán Tiền → Khởi tạo CSDL.
        /// </summary>
        public ServiceResult LuuBangLuong(
            int maNV,
            int thang,
            int nam,
            decimal luongCoBan,
            decimal ngayCong,
            decimal tienUng
        )
        {
            if (maNV <= 0)
                return ServiceResult.Fail("Vui lòng chọn nhân viên.");

            if (thang < 1 || thang > 12)
                return ServiceResult.Fail("Tháng không hợp lệ.");

            if (nam < 2000 || nam > 2100)
                return ServiceResult.Fail("Năm không hợp lệ.");

            // Tính các đầu mục lương qua hàm tĩnh thuần túy
            var calcResult = TinhLuong(luongCoBan, ngayCong, tienUng);
            if (!calcResult.Success)
                return ServiceResult.Fail(calcResult.Message);

            var kq = calcResult.Data;
            var bl = new BangLuong
            {
                MaNV = maNV,
                Thang = thang,
                Nam = nam,
                NgayCongThucTe = ngayCong,
                LuongTheoCong = kq.LuongTheoCong,
                TienUng = tienUng,
                BHXH = kq.BHXH,
                Thue = kq.Thue,
                TongThucNhan = kq.TongThucNhan,
            };

            bool ok = _blDAL.LuuBangLuong(bl);
            return ok
                ? ServiceResult.Ok("Lưu bảng lương thành công.")
                : ServiceResult.Fail("Không thể lưu bảng lương. Vui lòng thử lại.");
        }

        /// <summary>Tải toàn bộ hồ sơ truy xuất bảng lương theo tháng.</summary>
        public List<BangLuong> LayTheoThangNam(int thang, int nam)
        {
            return _blDAL.LayTheoThangNam(thang, nam);
        }

        /// <summary>Tải toàn bộ danh sách nhân viên cho Dropdown cập nhật.</summary>
        public List<NhanVien> LayDanhSachNhanVien()
        {
            return _nvDAL.LayTatCa();
        }

        /// <summary>Xoá phiếu thông tin theo mã quản lý (ID).</summary>
        public ServiceResult Xoa(int maBangLuong)
        {
            if (maBangLuong <= 0)
                return ServiceResult.Fail("Mã bảng lương không hợp lệ.");

            bool ok = _blDAL.Xoa(maBangLuong);
            return ok
                ? ServiceResult.Ok("Đã xoá bảng lương.")
                : ServiceResult.Fail("Không thể xoá. Bảng lương không tồn tại.");
        }
    }
}
