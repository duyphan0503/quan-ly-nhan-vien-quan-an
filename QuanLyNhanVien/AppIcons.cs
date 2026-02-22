using System.Collections.Generic;
using System.Drawing;
using FontAwesome.Sharp;

namespace QuanLyNhanVien
{
    /// <summary>
    /// Centralized icon provider using FontAwesome.Sharp NuGet package.
    /// All icons are rendered as bitmaps from Font Awesome's vector font,
    /// ensuring crisp, scalable, theme-consistent icons throughout the app.
    /// </summary>
    public static class AppIcons
    {
        private static readonly Dictionary<string, Image> _cache = new Dictionary<string, Image>();

        private static readonly int DefaultSize = 20;
        private static readonly Color DefaultColor = Color.White;

        // ── Navigation Icons ──
        public static Image Home => Get(IconChar.House, DefaultColor);
        public static Image Users => Get(IconChar.Users, DefaultColor);
        public static Image Building => Get(IconChar.Building, DefaultColor);
        public static Image Money => Get(IconChar.MoneyBillWave, DefaultColor);
        public static Image Chart => Get(IconChar.ChartBar, DefaultColor);
        public static Image Logout => Get(IconChar.RightFromBracket, DefaultColor);

        // ── Action Icons ──
        public static Image Calculator => Get(IconChar.Calculator, DefaultColor);
        public static Image Add => Get(IconChar.Plus, DefaultColor);
        public static Image Edit => Get(IconChar.PenToSquare, DefaultColor);
        public static Image Delete => Get(IconChar.TrashCan, DefaultColor);
        public static Image Refresh => Get(IconChar.ArrowsRotate, DefaultColor);
        public static Image Search => Get(IconChar.MagnifyingGlass, DefaultColor);
        public static Image Save => Get(IconChar.FloppyDisk, DefaultColor);
        public static Image List => Get(IconChar.ListUl, DefaultColor);
        public static Image User => Get(IconChar.User, DefaultColor);
        public static Image Plug => Get(IconChar.Plug, DefaultColor);
        public static Image Eye => Get(IconChar.Eye, DefaultColor);

        // ── Dashboard Icons ──
        public static Image UserGroup => Get(IconChar.UserGroup, AppColors.Green, 32);
        public static Image BuildingLg => Get(IconChar.Building, AppColors.Blue, 32);
        public static Image MoneyLg => Get(IconChar.MoneyBillTrendUp, AppColors.Yellow, 32);

        /// <summary>
        /// Gets an icon image by FontAwesome char, rendering and caching it.
        /// </summary>
        public static Image Get(IconChar icon, Color color, int size = 0)
        {
            if (size <= 0)
                size = DefaultSize;
            string key = $"{icon}_{color.ToArgb()}_{size}";

            if (_cache.TryGetValue(key, out Image img))
                return img;

            img = icon.ToBitmap(color, size);
            _cache[key] = img;
            return img;
        }
    }
}
