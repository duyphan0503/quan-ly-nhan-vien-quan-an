namespace QuanLyNhanVien.Models
{
    public class BangLuong
    {
        public int MaBangLuong { get; set; }
        public int MaNV { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal NgayCongThucTe { get; set; }
        public decimal LuongTheoCong { get; set; }
        public decimal TienUng { get; set; }
        public decimal BHXH { get; set; }
        public decimal Thue { get; set; }
        public decimal TongThucNhan { get; set; }

        // Navigation (display only)
        public string HoTen { get; set; }
        public string TenBoPhan { get; set; }
        public decimal LuongCoBan { get; set; }
    }
}
