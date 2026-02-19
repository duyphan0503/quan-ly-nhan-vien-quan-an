using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhanVien.Infrastructure
{
    public static class GridHelper
    {
        /// <summary>
        /// Fixes alignment issues in DataGridView for Mono/Linux (2026).
        /// Replaces buggy renderer with manual painting for Headers and specified Alignments.
        /// </summary>
        public static void FixAlignment(DataGridView dgv)
        {
            dgv.CellPainting += (sender, e) =>
            {
                if (e.ColumnIndex < 0) return;

                // 1. FIX HEADERS (Always center)
                if (e.RowIndex == -1)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                    using (var sf = new StringFormat 
                    { 
                        Alignment = StringAlignment.Center, 
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap,
                        Trimming = StringTrimming.EllipsisCharacter
                    })
                    {
                        using (var brush = new SolidBrush(e.CellStyle.ForeColor))
                        {
                            // Apply 5px horizontal padding (total 10px) - Safe for Mono
                            var paddedBounds = new Rectangle(e.CellBounds.X + 5, e.CellBounds.Y, e.CellBounds.Width - 10, e.CellBounds.Height);
                            e.Graphics.DrawString(e.Value?.ToString(), e.CellStyle.Font, brush, paddedBounds, sf);
                        }
                    }
                    e.Handled = true;
                }
                // 2. FIX CELLS (Based on Style Alignment)
                else if (e.RowIndex >= 0)
                {
                    // Manually paint MiddleRight, MiddleCenter, and MiddleLeft to ensure padding on Mono/Linux
                    if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight || 
                        e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter ||
                        e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleLeft)
                    {
                        e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                        
                        var alignment = StringAlignment.Near;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight) alignment = StringAlignment.Far;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter) alignment = StringAlignment.Center;
                        if (e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleLeft) alignment = StringAlignment.Near;

                        using (var sf = new StringFormat 
                        { 
                            Alignment = alignment, 
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.NoWrap,
                            Trimming = StringTrimming.EllipsisCharacter
                        })
                        {
                            using (var brush = new SolidBrush(e.CellStyle.ForeColor))
                            {
                                // Apply 5px horizontal padding (total 10px)
                                var paddedBounds = new Rectangle(e.CellBounds.X + 5, e.CellBounds.Y, e.CellBounds.Width - 10, e.CellBounds.Height);
                                e.Graphics.DrawString(e.FormattedValue?.ToString(), e.CellStyle.Font, brush, paddedBounds, sf);
                            }
                        }
                        e.Handled = true;
                    }
                }
            };
        }
    }
}
