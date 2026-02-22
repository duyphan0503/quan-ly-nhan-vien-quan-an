using System;
using QuanLyNhanVien.DataAccess;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// Tổng hợp dữ liệu thống kê cho trang chủ tổng quan (dashboard).
    /// </summary>
    public class DashboardService
    {
        private readonly NhanVienDAL _nvDAL = new NhanVienDAL();
        private readonly BoPhanDAL _bpDAL = new BoPhanDAL();
        private readonly BangLuongDAL _blDAL = new BangLuongDAL();

        /// <summary>
        /// Dữ liệu tổng hợp có thể hiển thị.
        /// </summary>
        public class DashboardData
        {
            public int TongNhanVien { get; set; }
            public int TongBoPhan { get; set; }
            public int BangLuongThangNay { get; set; }
        }

        /// <summary>
        /// Tải thống kê trang chủ. Trả về cấu trúc mặc định nếu gặp lỗi
        /// nhằm giúp giao diện tổng quan không bị lỗi trắng màn hình.
        /// </summary>
        public DashboardData LayThongKe()
        {
            var data = new DashboardData();

            try
            {
                data.TongNhanVien = _nvDAL.LayTatCa().Count;
            }
            catch
            { /* thẻ hiển thị sẽ hiển thị số 0 */
            }

            try
            {
                data.TongBoPhan = _bpDAL.LayTatCa().Count;
            }
            catch
            { /* thẻ hiển thị sẽ hiển thị số 0 */
            }

            try
            {
                int thang = DateTime.Now.Month;
                int nam = DateTime.Now.Year;
                data.BangLuongThangNay = _blDAL.LayTheoThangNam(thang, nam).Count;
            }
            catch
            { /* thẻ hiển thị sẽ hiển thị số 0 */
            }

            return data;
        }
    }
}
