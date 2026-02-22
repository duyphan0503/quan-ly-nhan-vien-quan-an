using System.Collections.Generic;
using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// File xử lý nghiệp vụ cho đối tượng Nhân Viên.
    /// Bao bọc lại quy trình xác thực, kiểm tra xóa đồng loạt theo chuỗi, và tích hợp CRUD.
    /// </summary>
    public class NhanVienService
    {
        private readonly NhanVienDAL _dal = new NhanVienDAL();

        /// <summary>Lấy thông tin tất cả nhân viên.</summary>
        public List<NhanVien> LayTatCa()
        {
            return _dal.LayTatCa();
        }

        /// <summary>Tìm kiếm nhân viên qua từ khóa tham chiếu.</summary>
        public List<NhanVien> TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return _dal.LayTatCa();

            return _dal.TimKiem(tuKhoa.Trim());
        }

        /// <summary>
        /// Xác thực dữ liệu và tạo mới nhân viên.
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
        /// Xác thực dữ liệu và cập nhật thông tin nhân viên đã lưu.
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
        /// Xóa nhân viên theo thủ tục kiểm tra an toàn chuỗi.
        /// Cấm xóa nhân viên đã được gán bảng lương chi tiết —
        /// Thay vào đó hãy thay đổi trạng thái của nhân viên thành "Nghỉ việc".
        /// </summary>
        public ServiceResult XoaNhanVien(int maNV)
        {
            if (maNV <= 0)
                return ServiceResult.Fail("Vui lòng chọn nhân viên cần xoá.");

            // Quy tắc logic: bỏ qua thao tác xóa với nhân viên đã thuộc danh sách một hoặc nhiều bảng lương
            if (_dal.CoLuong(maNV))
                return ServiceResult.Fail(
                    "Không thể xoá nhân viên đã có bảng lương.\n"
                        + "Hãy chuyển trạng thái sang 'Nghỉ việc'."
                );

            bool ok = _dal.Xoa(maNV);
            return ok
                ? ServiceResult.Ok("Đã xoá nhân viên.")
                : ServiceResult.Fail("Không thể xoá. Nhân viên không tồn tại.");
        }

        /// <summary>
        /// Nguồn xác minh thông tin tập trung cho dữ liệu thẻ nhân viên.
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
