using System;
using System.Data;
using QuanLyNhanVien.DataAccess;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Statistics and reporting service.
    /// Encapsulates all report-generation logic — 
    /// Forms only receive display-ready data.
    /// </summary>
    public class ThongKeService
    {
        private readonly BangLuongDAL _dal = new BangLuongDAL();

        /// <summary>
        /// Result container for annual payroll statistics.
        /// </summary>
        public class ThongKeNam
        {
            public DataTable ChiTietTheoThang { get; set; }
            public decimal TongChiNam { get; set; }
            public int Nam { get; set; }
        }

        /// <summary>
        /// Generate annual payroll statistics including monthly breakdown
        /// and the total annual expenditure.
        /// </summary>
        public ServiceResult<ThongKeNam> LayThongKeNam(int nam)
        {
            if (nam < 2000 || nam > 2100)
                return ServiceResult<ThongKeNam>.Fail("Năm không hợp lệ.");

            var dt = _dal.ThongKeLuong(nam);

            // Calculate annual total from the dataset
            decimal tongNam = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["TongThucNhan"] != DBNull.Value)
                    tongNam += Convert.ToDecimal(row["TongThucNhan"]);
            }

            return ServiceResult<ThongKeNam>.Ok(new ThongKeNam
            {
                ChiTietTheoThang = dt,
                TongChiNam = tongNam,
                Nam = nam
            });
        }
    }
}
