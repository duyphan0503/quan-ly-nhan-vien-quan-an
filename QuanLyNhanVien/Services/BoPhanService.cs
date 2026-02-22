using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// File xử lý nghiệp vụ cho quản lý phòng ban (Bộ Phận).
    /// Bao bọc lại quy trình xác thực, kiểm tra xóa đồng loạt theo chuỗi, và tích hợp CRUD.
    /// </summary>
    public class BoPhanService
    {
        private readonly BoPhanDAL _dal = new BoPhanDAL();

        /// <summary>Truy xuất toàn danh sách phòng ban, tự động sắp xếp tên bộ phận.</summary>
        public List<BoPhan> LayTatCa()
        {
            return _dal.LayTatCa();
        }

        /// <summary>
        /// Xác thực và tạo mới phòng ban hợp lệ.
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
        /// Xác thực và cập nhật nội dung phòng ban cũ.
        /// </summary>
        public ServiceResult CapNhatBoPhan(int maBoPhan, string tenBoPhan)
        {
            if (maBoPhan <= 0)
                return ServiceResult.Fail("Vui lòng chọn bộ phận cần sửa.");

            if (string.IsNullOrWhiteSpace(tenBoPhan))
                return ServiceResult.Fail("Vui lòng nhập tên bộ phận.");

            var bp = new BoPhan { MaBoPhan = maBoPhan, TenBoPhan = tenBoPhan.Trim() };

            bool ok = _dal.CapNhat(bp);
            return ok
                ? ServiceResult.Ok("Cập nhật bộ phận thành công.")
                : ServiceResult.Fail("Không thể cập nhật. Bộ phận không tồn tại.");
        }

        /// <summary>
        /// Xóa bộ phận qua quy trình kiểm tra điều kiện an toàn thông tin chuỗi (cascade-safety).
        /// Nghiêm cấm xóa các phòng ban nếu còn gắn với nhân sự.
        /// </summary>
        public ServiceResult XoaBoPhan(int maBoPhan)
        {
            if (maBoPhan <= 0)
                return ServiceResult.Fail("Vui lòng chọn bộ phận cần xoá.");

            // Cấu trúc phân luồng: không áp dụng xóa các bộ phận còn sở hữu lượng nhân sự
            if (_dal.DangDuocSuDung(maBoPhan))
                return ServiceResult.Fail("Không thể xoá bộ phận đang có nhân viên.");

            bool ok = _dal.Xoa(maBoPhan);
            return ok
                ? ServiceResult.Ok("Đã xoá bộ phận.")
                : ServiceResult.Fail("Không thể xoá. Bộ phận không tồn tại.");
        }
    }
}
