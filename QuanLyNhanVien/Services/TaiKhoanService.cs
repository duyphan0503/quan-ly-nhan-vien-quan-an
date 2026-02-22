using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Infrastructure;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Xử lý xác thực người dùng và quản lý tài khoản.
    /// Mật khẩu luôn được hash (SHA-256) trước khi lưu hoặc so sánh với DB.
    /// </summary>
    public class TaiKhoanService
    {
        private readonly TaiKhoanDAL _dal = new TaiKhoanDAL();

        /// <summary>
        /// Xác thực một người dùng với tên đăng nhập và mật khẩu.
        /// Mật khẩu được hash SHA-256 trước khi so sánh với giá trị trong DB.
        /// </summary>
        public ServiceResult<TaiKhoan> DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return ServiceResult<TaiKhoan>.Fail("Vui lòng nhập tên đăng nhập.");

            if (string.IsNullOrEmpty(matKhau))
                return ServiceResult<TaiKhoan>.Fail("Vui lòng nhập mật khẩu.");

            // Hash mật khẩu trước khi so sánh với DB
            string matKhauHash = SecurityHelper.HashPassword(matKhau);

            var tk = _dal.DangNhap(tenDangNhap.Trim(), matKhauHash);
            if (tk == null)
                return ServiceResult<TaiKhoan>.Fail("Sai tên đăng nhập hoặc mật khẩu.");

            return ServiceResult<TaiKhoan>.Ok(tk);
        }

        /// <summary>
        /// Cập nhật mật khẩu cho một tài khoản khả dụng.
        /// Mật khẩu mới được hash SHA-256 trước khi lưu vào DB.
        /// </summary>
        public ServiceResult DoiMatKhau(int maTK, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(matKhauMoi))
                return ServiceResult.Fail("Mật khẩu mới không được để trống.");

            if (matKhauMoi.Length < 4)
                return ServiceResult.Fail("Mật khẩu phải có ít nhất 4 ký tự.");

            // Hash mật khẩu mới trước khi lưu vào DB
            string matKhauHash = SecurityHelper.HashPassword(matKhauMoi);

            bool ok = _dal.DoiMatKhau(maTK, matKhauHash);
            return ok
                ? ServiceResult.Ok("Đổi mật khẩu thành công.")
                : ServiceResult.Fail("Không thể đổi mật khẩu. Tài khoản không tồn tại.");
        }
    }
}
