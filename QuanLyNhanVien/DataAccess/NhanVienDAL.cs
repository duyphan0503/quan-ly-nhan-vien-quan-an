using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.DataAccess
{
    public class NhanVienDAL
    {
        public List<NhanVien> LayTatCa()
        {
            var list = new List<NhanVien>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nv.MaNV, nv.HoTen, nv.ChucVu, nv.MaBoPhan,
                                      nv.LuongCoBan, nv.TrangThai, bp.TenBoPhan
                               FROM NhanVien nv
                               INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
                               ORDER BY nv.HoTen";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new NhanVien
                        {
                            MaNV = reader.GetInt32(0),
                            HoTen = reader.GetString(1),
                            ChucVu = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            MaBoPhan = reader.GetInt32(3),
                            LuongCoBan = reader.GetDecimal(4),
                            TrangThai = reader.GetString(5),
                            TenBoPhan = reader.GetString(6)
                        });
                    }
                }
            }
            return list;
        }

        public List<NhanVien> TimKiem(string tuKhoa)
        {
            var list = new List<NhanVien>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nv.MaNV, nv.HoTen, nv.ChucVu, nv.MaBoPhan,
                                      nv.LuongCoBan, nv.TrangThai, bp.TenBoPhan
                               FROM NhanVien nv
                               INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
                               WHERE nv.HoTen LIKE @kw OR nv.ChucVu LIKE @kw
                                  OR bp.TenBoPhan LIKE @kw
                               ORDER BY nv.HoTen";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kw", "%" + tuKhoa + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NhanVien
                            {
                                MaNV = reader.GetInt32(0),
                                HoTen = reader.GetString(1),
                                ChucVu = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                MaBoPhan = reader.GetInt32(3),
                                LuongCoBan = reader.GetDecimal(4),
                                TrangThai = reader.GetString(5),
                                TenBoPhan = reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool Them(NhanVien nv)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO NhanVien (HoTen, ChucVu, MaBoPhan, LuongCoBan, TrangThai)
                               VALUES (@ten, @cv, @bp, @luong, @tt)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ten", nv.HoTen);
                    cmd.Parameters.AddWithValue("@cv", (object)nv.ChucVu ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@bp", nv.MaBoPhan);
                    cmd.Parameters.AddWithValue("@luong", nv.LuongCoBan);
                    cmd.Parameters.AddWithValue("@tt", nv.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CapNhat(NhanVien nv)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE NhanVien
                               SET HoTen = @ten, ChucVu = @cv, MaBoPhan = @bp,
                                   LuongCoBan = @luong, TrangThai = @tt
                               WHERE MaNV = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ten", nv.HoTen);
                    cmd.Parameters.AddWithValue("@cv", (object)nv.ChucVu ?? System.DBNull.Value);
                    cmd.Parameters.AddWithValue("@bp", nv.MaBoPhan);
                    cmd.Parameters.AddWithValue("@luong", nv.LuongCoBan);
                    cmd.Parameters.AddWithValue("@tt", nv.TrangThai);
                    cmd.Parameters.AddWithValue("@id", nv.MaNV);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Xoa(int maNV)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM NhanVien WHERE MaNV = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maNV);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CoLuong(int maNV)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM BangLuong WHERE MaNV = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maNV);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
    }
}
