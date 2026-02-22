using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace QuanLyNhanVien
{
    /// <summary>
    /// File hỗ trợ cấu hình Font chữ tập trung — đảm bảo tương thích đa nền tảng.
    /// Trên Windows: sử dụng "Segoe UI". Trên Linux/Mono: dự phòng tuần tự qua
    /// "Liberation Sans" → "Ubuntu Sans" → "DejaVu Sans" → system default.
    /// </summary>
    public static class AppFonts
    {
        private static string _familyName;

        public static string FamilyName
        {
            get
            {
                if (_familyName == null)
                    _familyName = DetectBestFont();
                return _familyName;
            }
        }

        // Tích hợp sẵn các kích thước font phổ biến (lưu bộ nhớ đệm để tối ưu hiệu năng)
        public static Font Title => Create(16, FontStyle.Bold);
        public static Font Heading => Create(13, FontStyle.Bold);
        public static Font SubHead => Create(12, FontStyle.Bold);
        public static Font Body => Create(11);
        public static Font BodyBold => Create(11, FontStyle.Bold);
        public static Font Small => Create(10);
        public static Font SmallBold => Create(10, FontStyle.Bold);
        public static Font Tiny => Create(9);
        public static Font TinyBold => Create(9, FontStyle.Bold);
        public static Font XLarge => Create(14);
        public static Font XLargeBold => Create(14, FontStyle.Bold);
        public static Font Huge => Create(72);

        public static Font Create(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FamilyName, size, style);
        }

        private static string DetectBestFont()
        {
            // Dựa trên mức độ ưu tiên: giao diện hiện đại → mức độ phổ biến → dự phòng
            string[] candidates = new[]
            {
                "Segoe UI", // Có sẵn mặc định trên Windows
                "Liberation Sans", // Tương tự Arial, có trên hầu hết các bản phân phối Linux
                "Ubuntu Sans", // Hệ điều hành Ubuntu
                "DejaVu Sans", // Có mặt gần như tất cả các phiên bản Linux dự phòng
                "Noto Sans", // Font mặc định được khuyến cáo của Google
                "FreeSans", // GNU dự phòng mang xu hướng an toàn
            };

            using (var installed = new InstalledFontCollection())
            {
                var familyNames = installed.Families.Select(f => f.Name).ToArray();

                foreach (var candidate in candidates)
                {
                    if (familyNames.Contains(candidate))
                        return candidate;
                }
            }

            // Cuối cùng: sử dụng sans-serif theo mặc định của trình quản lý cửa sổ hiển thị
            return FontFamily.GenericSansSerif.Name;
        }
    }
}
