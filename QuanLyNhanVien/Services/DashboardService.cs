using System;
using QuanLyNhanVien.DataAccess;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Aggregates statistics for the main dashboard.
    /// </summary>
    public class DashboardService
    {
        private readonly NhanVienDAL _nvDAL = new NhanVienDAL();
        private readonly BoPhanDAL _bpDAL = new BoPhanDAL();
        private readonly BangLuongDAL _blDAL = new BangLuongDAL();

        /// <summary>
        /// Display-ready dashboard data.
        /// </summary>
        public class DashboardData
        {
            public int TongNhanVien { get; set; }
            public int TongBoPhan { get; set; }
            public int BangLuongThangNay { get; set; }
        }

        /// <summary>
        /// Load dashboard statistics. Returns defaults on failure
        /// to prevent the dashboard from being blank.
        /// </summary>
        public DashboardData LayThongKe()
        {
            var data = new DashboardData();

            try
            {
                data.TongNhanVien = _nvDAL.LayTatCa().Count;
            }
            catch { /* card will show 0 */ }

            try
            {
                data.TongBoPhan = _bpDAL.LayTatCa().Count;
            }
            catch { /* card will show 0 */ }

            try
            {
                int thang = DateTime.Now.Month;
                int nam = DateTime.Now.Year;
                data.BangLuongThangNay = _blDAL.LayTheoThangNam(thang, nam).Count;
            }
            catch { /* card will show 0 */ }

            return data;
        }
    }
}
