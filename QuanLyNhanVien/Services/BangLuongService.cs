using System;
using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Core salary calculation engine and payroll management.
    /// All business constants and formulas are centralized here — 
    /// the Forms layer never performs financial calculations directly.
    /// </summary>
    public class BangLuongService
    {
        private readonly BangLuongDAL _blDAL = new BangLuongDAL();
        private readonly NhanVienDAL _nvDAL = new NhanVienDAL();

        // ══════════════════════════════════════════════
        //  BUSINESS CONSTANTS — change here to affect
        //  the entire application
        // ══════════════════════════════════════════════

        /// <summary>Standard working days per month.</summary>
        public const decimal NGAY_CONG_CHUAN = 26m;

        /// <summary>Social insurance rate (10.5% of base salary).</summary>
        public const decimal TY_LE_BHXH = 0.105m;

        // ══════════════════════════════════════════════
        //  SALARY CALCULATION
        // ══════════════════════════════════════════════

        /// <summary>
        /// Result of a salary calculation — a pure data container
        /// with no side effects.
        /// </summary>
        public class KetQuaTinhLuong
        {
            public decimal LuongTheoCong { get; set; }
            public decimal BHXH { get; set; }
            public decimal Thue { get; set; }
            public decimal TongThucNhan { get; set; }
        }

        /// <summary>
        /// Pure salary calculation function. 
        /// No DB access, no side effects — can be unit tested independently.
        /// </summary>
        /// <param name="luongCoBan">Base salary.</param>
        /// <param name="ngayCong">Actual working days.</param>
        /// <param name="tienUng">Advance payment.</param>
        /// <returns>Calculation result on success, or a failure message.</returns>
        public ServiceResult<KetQuaTinhLuong> TinhLuong(
            decimal luongCoBan, decimal ngayCong, decimal tienUng)
        {
            if (ngayCong < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail(
                    "Ngày công không được âm.");

            if (ngayCong > 31)
                return ServiceResult<KetQuaTinhLuong>.Fail(
                    "Ngày công không được vượt quá 31.");

            if (tienUng < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail(
                    "Tiền ứng không được âm.");

            if (luongCoBan < 0)
                return ServiceResult<KetQuaTinhLuong>.Fail(
                    "Lương cơ bản không hợp lệ.");

            // Core formula
            decimal luongTheoCong = Math.Round(luongCoBan / NGAY_CONG_CHUAN * ngayCong);
            decimal bhxh = Math.Round(luongCoBan * TY_LE_BHXH);
            decimal thue = 0m; // Simplified — extend here for TNCN brackets

            decimal tongThucNhan = luongTheoCong - tienUng - bhxh - thue;
            if (tongThucNhan < 0) tongThucNhan = 0;

            return ServiceResult<KetQuaTinhLuong>.Ok(new KetQuaTinhLuong
            {
                LuongTheoCong = luongTheoCong,
                BHXH = bhxh,
                Thue = thue,
                TongThucNhan = tongThucNhan
            });
        }

        // ══════════════════════════════════════════════
        //  PAYROLL CRUD
        // ══════════════════════════════════════════════

        /// <summary>
        /// Calculate and save a payroll entry for an employee.
        /// Orchestrates validation → calculation → persistence.
        /// </summary>
        public ServiceResult LuuBangLuong(
            int maNV, int thang, int nam,
            decimal luongCoBan, decimal ngayCong, decimal tienUng)
        {
            if (maNV <= 0)
                return ServiceResult.Fail("Vui lòng chọn nhân viên.");

            if (thang < 1 || thang > 12)
                return ServiceResult.Fail("Tháng không hợp lệ.");

            if (nam < 2000 || nam > 2100)
                return ServiceResult.Fail("Năm không hợp lệ.");

            // Calculate salary via the pure function
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
                TongThucNhan = kq.TongThucNhan
            };

            bool ok = _blDAL.LuuBangLuong(bl);
            return ok
                ? ServiceResult.Ok("Lưu bảng lương thành công.")
                : ServiceResult.Fail("Không thể lưu bảng lương. Vui lòng thử lại.");
        }

        /// <summary>Get all payroll entries for a given month/year.</summary>
        public List<BangLuong> LayTheoThangNam(int thang, int nam)
        {
            return _blDAL.LayTheoThangNam(thang, nam);
        }

        /// <summary>Get all employees for the payroll dropdown.</summary>
        public List<NhanVien> LayDanhSachNhanVien()
        {
            return _nvDAL.LayTatCa();
        }

        /// <summary>Delete a payroll entry by ID.</summary>
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
