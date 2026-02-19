using System.Drawing;

namespace QuanLyNhanVien
{
    /// <summary>
    /// Centralized design tokens — Catppuccin Mocha-inspired palette.
    /// All UI colors are defined here for consistency across the app.
    /// </summary>
    public static class AppColors
    {
        // === BASE LAYERS ===
        public static readonly Color Base      = Color.FromArgb(30, 30, 46);     // #1E1E2E
        public static readonly Color Mantle    = Color.FromArgb(24, 24, 37);     // #181825
        public static readonly Color Crust     = Color.FromArgb(17, 17, 27);     // #11111B

        // === SURFACES ===
        public static readonly Color Surface0  = Color.FromArgb(49, 50, 68);     // #313244
        public static readonly Color Surface1  = Color.FromArgb(69, 71, 90);     // #45475A
        public static readonly Color Surface2  = Color.FromArgb(88, 91, 112);    // #585B70

        // === INPUT FIELDS ===
        public static readonly Color InputBg   = Color.FromArgb(55, 55, 78);
        public static readonly Color InputBorder = Color.FromArgb(80, 82, 104);

        // === TEXT ===
        public static readonly Color Text      = Color.FromArgb(205, 214, 244);  // #CDD6F4
        public static readonly Color SubText   = Color.FromArgb(166, 173, 200);  // #A6ADC8
        public static readonly Color Overlay   = Color.FromArgb(108, 112, 134);  // #6C7086

        // === ACCENTS ===
        public static readonly Color Green     = Color.FromArgb(166, 227, 161);  // #A6E3A1
        public static readonly Color Blue      = Color.FromArgb(137, 180, 250);  // #89B4FA
        public static readonly Color Red       = Color.FromArgb(243, 139, 168);  // #F38BA8
        public static readonly Color Yellow    = Color.FromArgb(249, 226, 175);  // #F9E2AF
        public static readonly Color Lavender  = Color.FromArgb(180, 190, 254);  // #B4BEFE
        public static readonly Color Peach     = Color.FromArgb(250, 179, 135);  // #FAB387
        public static readonly Color Teal      = Color.FromArgb(148, 226, 213);  // #94E2D5
        public static readonly Color Mauve     = Color.FromArgb(203, 166, 247);  // #CBA6F7

        // === GLASS EFFECT ===
        public static readonly Color GlassBg   = Color.FromArgb(180, 24, 24, 37);
        public static readonly Color GlassEdge = Color.FromArgb(60, 166, 227, 161);

        // === HELPERS ===
        /// <summary>
        /// Linearly interpolate between two colors.
        /// t = 0.0 → colorA, t = 1.0 → colorB.
        /// </summary>
        public static Color Lerp(Color a, Color b, float t)
        {
            if (t <= 0f) return a;
            if (t >= 1f) return b;
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }

        /// <summary>
        /// Returns a lighter version of a color for hover/highlight.
        /// </summary>
        public static Color Lighten(Color c, float amount = 0.15f)
        {
            return Color.FromArgb(c.A,
                (int)System.Math.Min(255, c.R + 255 * amount),
                (int)System.Math.Min(255, c.G + 255 * amount),
                (int)System.Math.Min(255, c.B + 255 * amount));
        }

        /// <summary>
        /// Returns a darker version of a color for pressed state.
        /// </summary>
        public static Color Darken(Color c, float amount = 0.1f)
        {
            return Color.FromArgb(c.A,
                (int)System.Math.Max(0, c.R - 255 * amount),
                (int)System.Math.Max(0, c.G - 255 * amount),
                (int)System.Math.Max(0, c.B - 255 * amount));
        }
    }
}
