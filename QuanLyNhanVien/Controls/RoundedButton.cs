using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyNhanVien.Controls
{
    /// <summary>
    /// A modern rounded button with smooth hover color animation.
    /// Uses custom GDI+ painting — works on both .NET Framework and Mono.
    /// </summary>
    public class RoundedButton : Control
    {
        // === DESIGN PROPERTIES ===
        private Color _idleColor = AppColors.Surface0;
        private Color _hoverColor = AppColors.Surface1;
        private Color _pressColor = AppColors.Surface2;
        private Color _accentColor = Color.Empty;   // optional left-accent stripe
        private int _cornerRadius = 10;
        private int _accentWidth = 4;
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;

        // === ANIMATION STATE ===
        private Timer _animTimer;
        private float _animProgress = 0f;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private const int ANIM_INTERVAL = 20;  // ms per frame
        private const float ANIM_STEP = 0.15f;  // progress per frame

        public RoundedButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            this.Font = AppFonts.Body;
            this.ForeColor = AppColors.Text;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(220, 48);
            this.Padding = new Padding(20, 0, 10, 0);

            _animTimer = new Timer { Interval = ANIM_INTERVAL };
            _animTimer.Tick += AnimTimer_Tick;
        }

        #region Public Properties

        public Color IdleColor
        {
            get => _idleColor;
            set { _idleColor = value; Invalidate(); }
        }

        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        public Color PressColor
        {
            get => _pressColor;
            set { _pressColor = value; Invalidate(); }
        }

        /// <summary>
        /// Optional colored stripe on the left edge of the button.
        /// Set to Color.Empty to disable.
        /// </summary>
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Invalidate(); }
        }

        #endregion

        #region Animation

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (_isHovered || _isPressed)
            {
                _animProgress = Math.Min(1f, _animProgress + ANIM_STEP);
            }
            else
            {
                _animProgress = Math.Max(0f, _animProgress - ANIM_STEP);
            }

            if (_animProgress <= 0f || _animProgress >= 1f)
                _animTimer.Stop();

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            _animTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _isPressed = false;
            _animTimer.Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isPressed = false;
            Invalidate();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Determine current background color
            Color bgColor;
            if (_isPressed)
            {
                bgColor = _pressColor;
            }
            else
            {
                bgColor = AppColors.Lerp(_idleColor, _hoverColor, _animProgress);
            }

            // Draw rounded rectangle body
            using (var path = CreateRoundedRect(rect, _cornerRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

            }

            // Draw accent stripe (left bar on the button)
            if (_accentColor != Color.Empty)
            {
                using (var accentPath = CreateRoundedRect(
                    new Rectangle(0, 0, _accentWidth + _cornerRadius, Height - 1), _cornerRadius))
                {
                    // Clip to the left part only
                    g.SetClip(new Rectangle(0, 0, _accentWidth, Height));
                    using (var ab = new SolidBrush(_accentColor))
                    {
                        g.FillPath(ab, accentPath);
                    }
                    g.ResetClip();
                }
            }

            // Draw text
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            bool isCenter = (_textAlign == ContentAlignment.MiddleCenter);
            
            if (_textAlign == ContentAlignment.MiddleLeft)
                flags |= TextFormatFlags.Left;
            else if (isCenter)
                flags |= TextFormatFlags.HorizontalCenter;
            else
                flags |= TextFormatFlags.Right;

            Rectangle textRect;
            if (isCenter)
            {
                // For perfectly centered icons (collapsed mode), ignore padding and accent shift
                textRect = new Rectangle(0, 0, Width, Height);
            }
            else
            {
                textRect = new Rectangle(
                    Padding.Left + (_accentColor != Color.Empty ? _accentWidth + 4 : 0),
                    0,
                    Width - Padding.Horizontal - (_accentColor != Color.Empty ? _accentWidth + 4 : 0),
                    Height);
            }

            TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, flags);
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

            // Top-left arc
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            // Top-right arc
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            // Bottom-right arc
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            // Bottom-left arc
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animTimer?.Stop();
                _animTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
