using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.DataAccess
{
    public class BoPhanDAL
    {
        public List<BoPhan> LayTatCa()
        {
            var list = new List<BoPhan>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT MaBoPhan, TenBoPhan FROM BoPhan ORDER BY TenBoPhan";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BoPhan
                        {
                            MaBoPhan = reader.GetInt32(0),
                            TenBoPhan = reader.GetString(1)
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(BoPhan bp)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO BoPhan (TenBoPhan) VALUES (@ten)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ten", bp.TenBoPhan);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool CapNhat(BoPhan bp)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE BoPhan SET TenBoPhan = @ten WHERE MaBoPhan = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ten", bp.TenBoPhan);
                    cmd.Parameters.AddWithValue("@id", bp.MaBoPhan);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Xoa(int maBoPhan)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM BoPhan WHERE MaBoPhan = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maBoPhan);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DangDuocSuDung(int maBoPhan)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM NhanVien WHERE MaBoPhan = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", maBoPhan);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
    }
}
