using System.Data;
using System.Data.SqlClient;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.DataAccess
{
    public class TaiKhoanDAL
    {
        public TaiKhoan DangNhap(string tenDangNhap, string matKhau)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT MaTK, TenDangNhap, MatKhau, VaiTro FROM TaiKhoan " +
                             "WHERE TenDangNhap = @user AND MatKhau = @pass";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", tenDangNhap);
                    cmd.Parameters.AddWithValue("@pass", matKhau);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TaiKhoan
                            {
                                MaTK = reader.GetInt32(0),
                                TenDangNhap = reader.GetString(1),
                                MatKhau = reader.GetString(2),
                                VaiTro = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool DoiMatKhau(int maTK, string matKhauMoi)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE TaiKhoan SET MatKhau = @pass WHERE MaTK = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pass", matKhauMoi);
                    cmd.Parameters.AddWithValue("@id", maTK);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
