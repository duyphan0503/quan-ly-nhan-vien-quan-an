using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyNhanVien.Controls
{
    /// <summary>
    /// A panel that simulates a Glassmorphism effect using gradient overlays.
    /// Works on both .NET Framework (Windows) and Mono (Linux) —
    /// no P/Invoke or platform-specific APIs used.
    /// </summary>
    public class GlassPanel : Panel
    {
        private Color _gradientTop = Color.FromArgb(200, 28, 28, 42);
        private Color _gradientBottom = Color.FromArgb(220, 20, 20, 32);
        private Color _borderColor = Color.FromArgb(40, 166, 227, 161);
        private int _borderRadius = 0;
        private bool _drawBorder = true;
        private GlassBorderSide _borderSide = GlassBorderSide.Right;

        public GlassPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
        }

        public enum GlassBorderSide
        {
            None, Left, Right, Top, Bottom, All
        }

        #region Properties

        public Color GradientTop
        {
            get => _gradientTop;
            set { _gradientTop = value; Invalidate(); }
        }

        public Color GradientBottom
        {
            get => _gradientBottom;
            set { _gradientBottom = value; Invalidate(); }
        }

        public Color GlassBorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public bool DrawGlassBorder
        {
            get => _drawBorder;
            set { _drawBorder = value; Invalidate(); }
        }

        public GlassBorderSide BorderSide
        {
            get => _borderSide;
            set { _borderSide = value; Invalidate(); }
        }

        #endregion

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Don't call base — we paint everything ourselves
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;

            // Main gradient background
            using (var brush = new LinearGradientBrush(
                rect, _gradientTop, _gradientBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect);
            }

            // Subtle frosted-glass horizontal band at the top
            var frostRect = new Rectangle(0, 0, Width, Math.Max(1, Height / 4));
            using (var frostBrush = new LinearGradientBrush(
                frostRect,
                Color.FromArgb(15, 255, 255, 255),
                Color.FromArgb(0, 255, 255, 255),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(frostBrush, frostRect);
            }

            // Edge glow border
            if (_drawBorder)
            {
                using (var pen = new Pen(_borderColor, 1.5f))
                {
                    switch (_borderSide)
                    {
                        case GlassBorderSide.Right:
                            g.DrawLine(pen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                            break;
                        case GlassBorderSide.Left:
                            g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                            break;
                        case GlassBorderSide.Top:
                            g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                            break;
                        case GlassBorderSide.Bottom:
                            g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                            break;
                        case GlassBorderSide.All:
                            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                            break;
                    }
                }
            }
        }
    }
}
