using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhanVien.Infrastructure
{
    public static class GridHelper
    {
        /// <summary>
        /// Khắc phục lỗi căn lề trong DataGridView trên môi trường Mono/Linux (2026).
        /// Thay thế trình vẽ gốc bị lỗi bằng cách vẽ thủ công cho các Tiêu đề và Căn lề chỉ định.
        /// </summary>
        public static void FixAlignment(DataGridView dgv)
        {
            dgv.CellPainting += (sender, e) =>
            {
                if (e.ColumnIndex < 0)
                    return;

                // 1. SỬA TIÊU ĐỀ (Luôn căn giữa)
                if (e.RowIndex == -1)
                {
                    e.Paint(
                        e.CellBounds,
                        DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground
                    );
                    using (
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.NoWrap,
                            Trimming = StringTrimming.EllipsisCharacter,
                        }
                    )
                    {
                        using (var brush = new SolidBrush(e.CellStyle.ForeColor))
                        {
                            // Thêm khoảng cách ngang đệm 5px (tổng cộng 10px) - An toàn cho Mono
                            var paddedBounds = new Rectangle(
                                e.CellBounds.X + 5,
                                e.CellBounds.Y,
                                e.CellBounds.Width - 10,
                                e.CellBounds.Height
                            );
                            e.Graphics.DrawString(
                                e.Value?.ToString(),
                                e.CellStyle.Font,
                                brush,
                                paddedBounds,
                                sf
                            );
                        }
                    }
                    e.Handled = true;
                }
                // 2. SỬA Ô DỮ LIỆU (Dựa trên cấu hình Căn lề)
                else if (e.RowIndex >= 0)
                {
                    // Vẽ thủ công các lề Phải, Giữa, Trái để đảm bảo khoảng cách hiển thị chuẩn trên Mono/Linux
                    if (
                        e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight
                        || e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter
                        || e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleLeft
                    )
                    {
                        e.Paint(
                            e.CellBounds,
                            DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground
                        );

                        var alignment = StringAlignment.Near;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                            alignment = StringAlignment.Far;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
                            alignment = StringAlignment.Center;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                            alignment = StringAlignment.Near;

                        using (
                            var sf = new StringFormat
                            {
                                Alignment = alignment,
                                LineAlignment = StringAlignment.Center,
                                FormatFlags = StringFormatFlags.NoWrap,
                                Trimming = StringTrimming.EllipsisCharacter,
                            }
                        )
                        {
                            using (var brush = new SolidBrush(e.CellStyle.ForeColor))
                            {
                                // Thêm khoảng cách dọc đệm 5px (tổng cộng 10px)
                                var paddedBounds = new Rectangle(
                                    e.CellBounds.X + 5,
                                    e.CellBounds.Y,
                                    e.CellBounds.Width - 10,
                                    e.CellBounds.Height
                                );
                                e.Graphics.DrawString(
                                    e.FormattedValue?.ToString(),
                                    e.CellStyle.Font,
                                    brush,
                                    paddedBounds,
                                    sf
                                );
                            }
                        }
                        e.Handled = true;
                    }
                }
            };
        }
    }
}
