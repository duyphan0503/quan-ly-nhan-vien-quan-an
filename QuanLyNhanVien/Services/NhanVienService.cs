using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Business logic for employee (Nhân Viên) management.
    /// Encapsulates validation, cascade-delete checks, and CRUD orchestration.
    /// </summary>
    public class NhanVienService
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();

        /// <summary>Returns all employees.</summary>
        public List<NhanVien> LayTatCa()
        {
            return _dal.LayTatCa();
        }

        /// <summary>Search employees by keyword.</summary>
        public List<NhanVien> TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return _dal.LayTatCa();

            return _dal.TimKiem(tuKhoa.Trim());
        }

        /// <summary>
        /// Validate and add a new employee.
        /// </summary>
        public ServiceResult ThemNhanVien(NhanVien nv)
        {
            var validation = ValidateNhanVien(nv);
            if (!validation.Success)
                return validation;

            bool ok = _dal.Them(nv);
            return ok
                ? ServiceResult.Ok("Thêm nhân viên thành công.")
                : ServiceResult.Fail("Không thể thêm nhân viên. Vui lòng thử lại.");
        }

        /// <summary>
        /// Validate and update an existing employee.
        /// </summary>
        public ServiceResult CapNhatNhanVien(NhanVien nv)
        {
            if (nv.MaNV <= 0)
                return ServiceResult.Fail("Vui lòng chọn nhân viên cần sửa.");

            var validation = ValidateNhanVien(nv);
            if (!validation.Success)
                return validation;

            bool ok = _dal.CapNhat(nv);
            return ok
                ? ServiceResult.Ok("Cập nhật nhân viên thành công.")
                : ServiceResult.Fail("Không thể cập nhật. Nhân viên không tồn tại.");
        }

        /// <summary>
        /// Delete an employee with cascade-safety check.
        /// Employees with existing payroll records cannot be deleted — 
        /// their status should be changed to "Nghỉ việc" instead.
        /// </summary>
        public ServiceResult XoaNhanVien(int maNV)
        {
            if (maNV <= 0)
                return ServiceResult.Fail("Vui lòng chọn nhân viên cần xoá.");

            // Business rule: cannot delete employee who has payroll records
            if (_dal.CoLuong(maNV))
                return ServiceResult.Fail(
                    "Không thể xoá nhân viên đã có bảng lương.\n" +
                    "Hãy chuyển trạng thái sang 'Nghỉ việc'.");

            bool ok = _dal.Xoa(maNV);
            return ok
                ? ServiceResult.Ok("Đã xoá nhân viên.")
                : ServiceResult.Fail("Không thể xoá. Nhân viên không tồn tại.");
        }

        /// <summary>
        /// Centralized validation for employee data.
        /// </summary>
        private ServiceResult ValidateNhanVien(NhanVien nv)
        {
            if (nv == null)
                return ServiceResult.Fail("Dữ liệu nhân viên không hợp lệ.");

            if (string.IsNullOrWhiteSpace(nv.HoTen))
                return ServiceResult.Fail("Vui lòng nhập họ tên.");

            if (nv.MaBoPhan <= 0)
                return ServiceResult.Fail("Vui lòng chọn bộ phận.");

            if (nv.LuongCoBan < 0)
                return ServiceResult.Fail("Lương cơ bản không được âm.");

            if (string.IsNullOrWhiteSpace(nv.TrangThai))
                return ServiceResult.Fail("Vui lòng chọn trạng thái.");

            return ServiceResult.Ok();
        }
    }
}
