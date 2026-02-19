using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Business logic for department (Bộ Phận) management.
    /// Encapsulates validation, cascade-delete checks, and CRUD orchestration.
    /// </summary>
    public class BoPhanService
    {
        private readonly BoPhanDAL _dal = new BoPhanDAL();

        /// <summary>Returns all departments, ordered by name.</summary>
        public List<BoPhan> LayTatCa()
        {
            return _dal.LayTatCa();
        }

        /// <summary>
        /// Validate and add a new department.
        /// </summary>
        public ServiceResult ThemBoPhan(string tenBoPhan)
        {
            if (string.IsNullOrWhiteSpace(tenBoPhan))
                return ServiceResult.Fail("Vui lòng nhập tên bộ phận.");

            var bp = new BoPhan { TenBoPhan = tenBoPhan.Trim() };
            bool ok = _dal.Them(bp);
            return ok
                ? ServiceResult.Ok("Thêm bộ phận thành công.")
                : ServiceResult.Fail("Không thể thêm bộ phận. Vui lòng thử lại.");
        }

        /// <summary>
        /// Validate and update an existing department.
        /// </summary>
        public ServiceResult CapNhatBoPhan(int maBoPhan, string tenBoPhan)
        {
            if (maBoPhan <= 0)
                return ServiceResult.Fail("Vui lòng chọn bộ phận cần sửa.");

            if (string.IsNullOrWhiteSpace(tenBoPhan))
                return ServiceResult.Fail("Vui lòng nhập tên bộ phận.");

            var bp = new BoPhan
            {
                MaBoPhan = maBoPhan,
                TenBoPhan = tenBoPhan.Trim()
            };

            bool ok = _dal.CapNhat(bp);
            return ok
                ? ServiceResult.Ok("Cập nhật bộ phận thành công.")
                : ServiceResult.Fail("Không thể cập nhật. Bộ phận không tồn tại.");
        }

        /// <summary>
        /// Delete a department with cascade-safety check.
        /// Departments with assigned employees cannot be deleted.
        /// </summary>
        public ServiceResult XoaBoPhan(int maBoPhan)
        {
            if (maBoPhan <= 0)
                return ServiceResult.Fail("Vui lòng chọn bộ phận cần xoá.");

            // Business rule: cannot delete department that has employees
            if (_dal.DangDuocSuDung(maBoPhan))
                return ServiceResult.Fail(
                    "Không thể xoá bộ phận đang có nhân viên.");

            bool ok = _dal.Xoa(maBoPhan);
            return ok
                ? ServiceResult.Ok("Đã xoá bộ phận.")
                : ServiceResult.Fail("Không thể xoá. Bộ phận không tồn tại.");
        }
    }
}
