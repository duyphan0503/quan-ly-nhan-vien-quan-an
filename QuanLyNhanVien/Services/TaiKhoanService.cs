using QuanLyNhanVien.DataAccess;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Handles authentication and account management.
    /// </summary>
    public class TaiKhoanService
    {
        private readonly TaiKhoanDAL _dal = new TaiKhoanDAL();

        /// <summary>
        /// Authenticate a user with username and password.
        /// Returns the account on success, or a failure message.
        /// </summary>
        public ServiceResult<TaiKhoan> DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return ServiceResult<TaiKhoan>.Fail("Vui lòng nhập tên đăng nhập.");

            if (string.IsNullOrEmpty(matKhau))
                return ServiceResult<TaiKhoan>.Fail("Vui lòng nhập mật khẩu.");

            var tk = _dal.DangNhap(tenDangNhap.Trim(), matKhau);
            if (tk == null)
                return ServiceResult<TaiKhoan>.Fail("Sai tên đăng nhập hoặc mật khẩu.");

            return ServiceResult<TaiKhoan>.Ok(tk);
        }

        /// <summary>
        /// Change the password for an account.
        /// </summary>
        public ServiceResult DoiMatKhau(int maTK, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(matKhauMoi))
                return ServiceResult.Fail("Mật khẩu mới không được để trống.");

            if (matKhauMoi.Length < 4)
                return ServiceResult.Fail("Mật khẩu phải có ít nhất 4 ký tự.");

            bool ok = _dal.DoiMatKhau(maTK, matKhauMoi);
            return ok
                ? ServiceResult.Ok("Đổi mật khẩu thành công.")
                : ServiceResult.Fail("Không thể đổi mật khẩu. Tài khoản không tồn tại.");
        }
    }
}
