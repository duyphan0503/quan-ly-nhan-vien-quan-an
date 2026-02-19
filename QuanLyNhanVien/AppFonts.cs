using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace QuanLyNhanVien
{
    /// <summary>
    /// Centralized font helper — ensures cross-platform font compatibility.
    /// On Windows: uses "Segoe UI". On Linux/Mono: falls back through
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

        // Pre-built font instances for common sizes (cached for performance)
        public static Font Title     => Create(16, FontStyle.Bold);
        public static Font Heading   => Create(13, FontStyle.Bold);
        public static Font SubHead   => Create(12, FontStyle.Bold);
        public static Font Body      => Create(11);
        public static Font BodyBold  => Create(11, FontStyle.Bold);
        public static Font Small     => Create(10);
        public static Font SmallBold => Create(10, FontStyle.Bold);
        public static Font Tiny      => Create(9);
        public static Font TinyBold  => Create(9, FontStyle.Bold);
        public static Font XLarge    => Create(14);
        public static Font XLargeBold => Create(14, FontStyle.Bold);
        public static Font Huge      => Create(72);

        public static Font Create(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FamilyName, size, style);
        }

        private static string DetectBestFont()
        {
            // Priority order: modern → widely available → fallback
            string[] candidates = new[]
            {
                "Segoe UI",         // Windows native
                "Liberation Sans",  // Metrics-compatible with Arial, available on most Linux
                "Ubuntu Sans",      // Ubuntu systems
                "DejaVu Sans",      // Almost universal Linux fallback
                "Noto Sans",        // Google's universal font
                "FreeSans",         // GNU fallback
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

            // Ultimate fallback: system default sans-serif
            return FontFamily.GenericSansSerif.Name;
        }
    }
}
