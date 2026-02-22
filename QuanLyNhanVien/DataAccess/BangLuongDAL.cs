using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.DataAccess
{
    public class BangLuongDAL
    {
        public List<BangLuong> LayTheoThangNam(int thang, int nam)
        {
            var list = new List<BangLuong>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql =
                    @"SELECT bl.MaBangLuong, bl.MaNV, bl.Thang, bl.Nam,
                                      bl.NgayCongThucTe, bl.LuongTheoCong,
                                      bl.TienUng, bl.BHXH, bl.Thue, bl.TongThucNhan,
                                      nv.HoTen, bp.TenBoPhan, nv.LuongCoBan
                               FROM BangLuong bl
                               INNER JOIN NhanVien nv ON bl.MaNV = nv.MaNV
                               INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
                               WHERE bl.Thang = @thang AND bl.Nam = @nam
                               ORDER BY nv.HoTen";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@thang", thang);
                    cmd.Parameters.AddWithValue("@nam", nam);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(
                                new BangLuong
                                {
                                    MaBangLuong = reader.GetInt32(0),
                                    MaNV = reader.GetInt32(1),
                                    Thang = reader.GetInt32(2),
                                    Nam = reader.GetInt32(3),
                                    NgayCongThucTe = reader.GetDecimal(4),
                                    LuongTheoCong = reader.GetDecimal(5),
                                    TienUng = reader.GetDecimal(6),
                                    BHXH = reader.GetDecimal(7),
                                    Thue = reader.GetDecimal(8),
                                    TongThucNhan = reader.GetDecimal(9),
                                    HoTen = reader.GetString(10),
                                    TenBoPhan = reader.GetString(11),
                                    LuongCoBan = reader.GetDecimal(12),
                                }
                            );
                        }
                    }
                }
            }
            return list;
        }

        public BangLuong LayTheoNV_ThangNam(int maNV, int thang, int nam)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql =
                    @"SELECT bl.MaBangLuong, bl.MaNV, bl.Thang, bl.Nam,
                                      bl.NgayCongThucTe, bl.LuongTheoCong,
                                      bl.TienUng, bl.BHXH, bl.Thue, bl.TongThucNhan,
                                      nv.HoTen, bp.TenBoPhan, nv.LuongCoBan
                               FROM BangLuong bl
                               INNER JOIN NhanVien nv ON bl.MaNV = nv.MaNV
                               INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
                               WHERE bl.MaNV = @maNV AND bl.Thang = @thang AND bl.Nam = @nam";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maNV", maNV);
                    cmd.Parameters.AddWithValue("@thang", thang);
                    cmd.Parameters.AddWithValue("@nam", nam);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BangLuong
                            {
                                MaBangLuong = reader.GetInt32(0),
                                MaNV = reader.GetInt32(1),
                                Thang = reader.GetInt32(2),
                                Nam = reader.GetInt32(3),
                                NgayCongThucTe = reader.GetDecimal(4),
                                LuongTheoCong = reader.GetDecimal(5),
                                TienUng = reader.GetDecimal(6),
                                BHXH = reader.GetDecimal(7),
                                Thue = reader.GetDecimal(8),
                                TongThucNhan = reader.GetDecimal(9),
                                HoTen = reader.GetString(10),
                                TenBoPhan = reader.GetString(11),
                                LuongCoBan = reader.GetDecimal(12),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool LuuBangLuong(BangLuong bl)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                // Kiểm tra tồn tại
                string checkSql =
                    "SELECT COUNT(*) FROM BangLuong WHERE MaNV = @maNV AND Thang = @thang AND Nam = @nam";
                bool exists;
                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@maNV", bl.MaNV);
                    checkCmd.Parameters.AddWithValue("@thang", bl.Thang);
                    checkCmd.Parameters.AddWithValue("@nam", bl.Nam);
                    exists = (int)checkCmd.ExecuteScalar() > 0;
                }

                string sql;
                if (exists)
                {
                    sql =
                        @"UPDATE BangLuong
                            SET NgayCongThucTe = @cong, LuongTheoCong = @luongCong,
                                TienUng = @ung, BHXH = @bhxh, Thue = @thue,
                                TongThucNhan = @tong
                            WHERE MaNV = @maNV AND Thang = @thang AND Nam = @nam";
                }
                else
                {
                    sql =
                        @"INSERT INTO BangLuong (MaNV, Thang, Nam, NgayCongThucTe,
                                LuongTheoCong, TienUng, BHXH, Thue, TongThucNhan)
                            VALUES (@maNV, @thang, @nam, @cong, @luongCong,
                                    @ung, @bhxh, @thue, @tong)";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maNV", bl.MaNV);
                    cmd.Parameters.AddWithValue("@thang", bl.Thang);
                    cmd.Parameters.AddWithValue("@nam", bl.Nam);
                    cmd.Parameters.AddWithValue("@cong", bl.NgayCongThucTe);
                    cmd.Parameters.AddWithValue("@luongCong", bl.LuongTheoCong);
                    cmd.Parameters.AddWithValue("@ung", bl.TienUng);
                    cmd.Parameters.AddWithValue("@bhxh", bl.BHXH);
                    cmd.Parameters.AddWithValue("@thue", bl.Thue);
                    cmd.Parameters.AddWithValue("@tong", bl.TongThucNhan);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Xoa(int maBangLuong)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM BangLuong WHERE MaBangLuong = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maBangLuong);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Thống kê tổng lương theo tháng/năm cho Form Thống kê
        /// </summary>
        public DataTable ThongKeLuong(int nam)
        {
            var dt = new DataTable();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql =
                    @"SELECT Thang,
                                      COUNT(MaNV) AS SoNhanVien,
                                      SUM(LuongTheoCong) AS TongLuong,
                                      SUM(TienUng) AS TongUng,
                                      SUM(BHXH) AS TongBHXH,
                                      SUM(Thue) AS TongThue,
                                      SUM(TongThucNhan) AS TongThucNhan
                               FROM BangLuong
                               WHERE Nam = @nam
                               GROUP BY Thang
                               ORDER BY Thang";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nam", nam);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}
