using System;
using System.Data;
using QuanLyNhanVien.DataAccess;

namespace QuanLyNhanVien.Services
{
    /// <summary>
    /// File xử lý dịch vụ báo cáo và thống kê.
    /// Bao bọc lại toàn bộ quy trình tạo báo cáo thống kê —
    /// Các Form chỉ có nhiệm vụ nhận về thông tin đã cấu trúc và hiển thị.
    /// </summary>
    public class ThongKeService
    {
        private readonly BangLuongDAL _dal = new BangLuongDAL();

        /// <summary>
        /// Bộ chứa dữ liệu lưu trữ kết quả thống kê hàng năm.
        /// </summary>
        public class ThongKeNam
        {
            public DataTable ChiTietTheoThang { get; set; }
            public decimal TongChiNam { get; set; }
            public int Nam { get; set; }
        }

        /// <summary>
        /// Trích xuất bảng báo cáo thống kê năm bao gồm chi tiết lương từng tháng
        /// và tiền quỹ cả năm.
        /// </summary>
        public ServiceResult<ThongKeNam> LayThongKeNam(int nam)
        {
            if (nam < 2000 || nam > 2100)
                return ServiceResult<ThongKeNam>.Fail("Năm không hợp lệ.");

            var dt = _dal.ThongKeLuong(nam);

            // Tính toán quỹ lương hàng năm từ một tệp dữ liệu
            decimal tongNam = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["TongThucNhan"] != DBNull.Value)
                    tongNam += Convert.ToDecimal(row["TongThucNhan"]);
            }

            return ServiceResult<ThongKeNam>.Ok(
                new ThongKeNam
                {
                    ChiTietTheoThang = dt,
                    TongChiNam = tongNam,
                    Nam = nam,
                }
            );
        }
    }
}
