namespace QuanLyNhanVien.Models
{
    public class NhanVien
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string ChucVu { get; set; }
        public int MaBoPhan { get; set; }
        public decimal LuongCoBan { get; set; }
        public string TrangThai { get; set; }

        // Điều hướng kết nối (chỉ dùng để hiển thị)
        public string TenBoPhan { get; set; }
    }
}
