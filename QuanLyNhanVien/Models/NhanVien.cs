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

        // Navigation (display only)
        public string TenBoPhan { get; set; }
    }
}
