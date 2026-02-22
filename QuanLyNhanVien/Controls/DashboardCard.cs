using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyNhanVien.Controls
{
    /// <summary>
    /// A statistics card for the dashboard with an accent stripe,
    /// icon, value, and subtitle — all custom-painted.
    /// </summary>
    public class DashboardCard : Control
    {
        private Image _icon = null;
        private string _value = "0";
        private string _subtitle = "Label";
        private Color _accentColor = AppColors.Green;
        private int _cornerRadius = 12;

        public DashboardCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.UserPaint
                    | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.ResizeRedraw
                    | ControlStyles.SupportsTransparentBackColor,
                true
            );

            this.Size = new Size(200, 110);
            this.Cursor = Cursors.Default;
        }

        #region Properties

        public Image Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                Invalidate();
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                Invalidate();
            }
        }

        public string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                Invalidate();
            }
        }

        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                Invalidate();
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Card body (rounded rect)
            using (var path = CreateRoundedRect(rect, _cornerRadius))
            {
                using (
                    var bgBrush = new LinearGradientBrush(
                        rect,
                        AppColors.Surface0,
                        AppColors.Lerp(AppColors.Surface0, AppColors.Mantle, 0.5f),
                        LinearGradientMode.Vertical
                    )
                )
                {
                    g.FillPath(bgBrush, path);
                }

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(30, 255, 255, 255), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Accent stripe on the left
            g.SetClip(new Rectangle(0, 0, 5, Height));
            using (
                var accentPath = CreateRoundedRect(
                    new Rectangle(0, 0, _cornerRadius * 2 + 5, Height - 1),
                    _cornerRadius
                )
            )
            using (var ab = new SolidBrush(_accentColor))
            {
                g.FillPath(ab, accentPath);
            }
            g.ResetClip();

            // Icon
            int iconX = 25;
            int iconWidth = 0;
            if (_icon != null)
            {
                int iconY = (Height - _icon.Height) / 2;
                g.DrawImage(_icon, iconX, iconY);
                iconWidth = _icon.Width;
            }

            // Value text
            int textX = iconX + iconWidth + 15;
            var valueFont = AppFonts.Create(20, FontStyle.Bold);
            using (var vBrush = new SolidBrush(AppColors.Text))
            {
                g.DrawString(_value, valueFont, vBrush, textX, Height / 2 - 28);
            }
            valueFont.Dispose();

            // Subtitle text
            using (var sBrush = new SolidBrush(AppColors.SubText))
            {
                g.DrawString(_subtitle, AppFonts.Tiny, sBrush, textX, Height / 2 + 5);
            }
        }

        private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
