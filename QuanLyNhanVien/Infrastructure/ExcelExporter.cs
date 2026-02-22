using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using QuanLyNhanVien.Models;

namespace QuanLyNhanVien.Infrastructure
{
    /// <summary>
    /// Xuất phiếu lương tháng sang file Excel (.xlsx) sử dụng thư viện ClosedXML.
    ///
    /// ClosedXML là thư viện mã nguồn mở (MIT License) giúp tạo file Excel
    /// một cách trực quan mà không cần cài Microsoft Office.
    /// Cài đặt: NuGet → Install-Package ClosedXML
    ///
    /// Chức năng:
    ///   • ExportMotPhieu:   Xuất 1 phiếu lương ra file Excel
    ///   • ExportTatCaPhieu: Xuất tất cả phiếu lương tháng (mỗi NV = 1 sheet)
    ///
    /// Mẫu phiếu lương theo chuẩn nhà hàng / quán ăn Việt Nam:
    ///   1. Tên đơn vị + Địa chỉ + SĐT (căn giữa, in đậm)
    ///   2. Tiêu đề "PHIẾU LƯƠNG THÁNG ..."
    ///   3. Thông tin nhân viên (Họ tên, Bộ phận, Mã NV, Kỳ lương)
    ///   4. Bảng chi tiết lương (7 khoản + Tổng khấu trừ + Thực nhận)
    ///   5. Số tiền bằng chữ tiếng Việt
    ///   6. Phần ký xác nhận (Người lập, Kế toán, Người nhận)
    /// </summary>
    public static class ExcelExporter
    {
        // ── Thông tin đơn vị (thay đổi theo doanh nghiệp thực tế) ──
        private const string TEN_DOANH_NGHIEP = "NHÀ HÀNG QUÁN ĂN NGON";
        private const string DIA_CHI = "56 Đường 3/2, P. Xuân Khánh, Q. Ninh Kiều, TP. Cần Thơ";
        private const string DIEN_THOAI = "ĐT: 0292-3820-456  |  MST: 1801234567";

        // ── Font mặc định cho phiếu lương (chuẩn in ấn VN) ──
        private const string FONT_NAME = "Times New Roman";
        private const int FONT_SIZE = 12;

        /// <summary>
        /// Xuất 1 phiếu lương đơn lẻ ra file Excel.
        /// </summary>
        public static void ExportMotPhieu(BangLuong bl, string filePath)
        {
            ExportTatCaPhieu(new List<BangLuong> { bl }, bl.Thang, bl.Nam, filePath);
        }

        /// <summary>
        /// Xuất toàn bộ phiếu lương trong tháng.
        /// Mỗi nhân viên được tạo 1 sheet riêng trong cùng 1 file Excel.
        /// </summary>
        public static void ExportTatCaPhieu(
            List<BangLuong> danhSach,
            int thang,
            int nam,
            string filePath
        )
        {
            if (danhSach == null || danhSach.Count == 0)
                throw new InvalidOperationException("Danh sách phiếu lương trống.");

            // Tạo workbook mới
            using (var workbook = new XLWorkbook())
            {
                foreach (var bl in danhSach)
                {
                    // Tên sheet = Họ tên nhân viên (tối đa 31 ký tự theo giới hạn Excel)
                    string tenSheet = bl.HoTen ?? $"NV_{bl.MaNV}";
                    if (tenSheet.Length > 31)
                        tenSheet = tenSheet.Substring(0, 31);

                    var sheet = workbook.Worksheets.Add(tenSheet);

                    // Tạo nội dung phiếu lương trên sheet
                    TaoPhieuLuong(sheet, bl, thang, nam);
                }

                // Lưu file Excel
                workbook.SaveAs(filePath);
            }
        }

        // ═══════════════════════════════════════════════════
        //  TẠO NỘI DUNG PHIẾU LƯƠNG TRÊN 1 SHEET
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Tạo toàn bộ nội dung phiếu lương lên 1 worksheet.
        /// Bố cục: Header → Thông tin NV → Bảng lương → Bằng chữ → Ký tên
        /// </summary>
        private static void TaoPhieuLuong(IXLWorksheet ws, BangLuong bl, int thang, int nam)
        {
            // ── Thiết lập font mặc định cho toàn sheet ──
            ws.Style.Font.FontName = FONT_NAME;
            ws.Style.Font.FontSize = FONT_SIZE;

            // ── Thiết lập độ rộng cột ──
            ws.Column("A").Width = 7; // STT
            ws.Column("B").Width = 34; // Khoản mục (rộng hơn để đủ chữ "TỔNG CÁC KHOẢN KHẤU TRỪ")
            ws.Column("C").Width = 16; // Đơn vị tính
            ws.Column("D").Width = 18; // Giá trị
            ws.Column("E").Width = 26; // Ghi chú

            int dong = 1; // Biến đếm dòng hiện tại

            // ══════════════ PHẦN 1: HEADER DOANH NGHIỆP ══════════════
            dong = TaoHeaderDoanhNghiep(ws, dong, thang, nam);

            // ══════════════ PHẦN 2: THÔNG TIN NHÂN VIÊN ══════════════
            dong = TaoThongTinNhanVien(ws, dong, bl, thang, nam);

            // ══════════════ PHẦN 3: BẢNG CHI TIẾT LƯƠNG ══════════════
            dong = TaoBangChiTietLuong(ws, dong, bl);

            // ══════════════ PHẦN 4: SỐ TIỀN BẰNG CHỮ ══════════════
            dong++; // Dòng trống
            var cellBangChu = ws.Cell(dong, 1);
            cellBangChu.Value = "Bằng chữ:  " + DocSoTien(bl.TongThucNhan);
            cellBangChu.Style.Font.Bold = true;
            cellBangChu.Style.Font.Italic = true;
            ws.Range(dong, 1, dong, 5).Merge();
            dong++;

            // ══════════════ PHẦN 5: KÝ XÁC NHẬN ══════════════
            TaoPhienKyTen(ws, dong, thang, nam);

            // ── Thiết lập in ấn: A4, căn giữa trang ──
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.CenterHorizontally = true;
            ws.PageSetup.Margins.Left = 0.6;
            ws.PageSetup.Margins.Right = 0.4;
            ws.PageSetup.Margins.Top = 0.6;
            ws.PageSetup.Margins.Bottom = 0.4;
        }

        // ═══════════════════════════════════════════════════
        //  CÁC PHẦN NHỎ CỦA PHIẾU LƯƠNG
        // ═══════════════════════════════════════════════════

        /// <summary>Phần header: Tên đơn vị, địa chỉ, SĐT</summary>
        private static int TaoHeaderDoanhNghiep(IXLWorksheet ws, int dong, int thang, int nam)
        {
            // Dòng 1: Tên đơn vị — Bold, 16pt, căn giữa
            ws.Cell(dong, 1).Value = TEN_DOANH_NGHIEP;
            var rngTen = ws.Range(dong, 1, dong, 5).Merge();
            rngTen.Style.Font.Bold = true;
            rngTen.Style.Font.FontSize = 16;
            rngTen.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dong++;

            // Dòng 2: Địa chỉ — căn giữa
            ws.Cell(dong, 1).Value = DIA_CHI;
            var rngDC = ws.Range(dong, 1, dong, 5).Merge();
            rngDC.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dong++;

            // Dòng 3: SĐT + MST — căn giữa
            ws.Cell(dong, 1).Value = DIEN_THOAI;
            var rngDT = ws.Range(dong, 1, dong, 5).Merge();
            rngDT.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dong++;

            // Dòng trống
            dong++;

            // Dòng 5: Tiêu đề phiếu lương — Bold, 14pt, căn giữa
            ws.Cell(dong, 1).Value = $"PHIẾU LƯƠNG THÁNG {thang:D2}/{nam}";
            var rngTitle = ws.Range(dong, 1, dong, 5).Merge();
            rngTitle.Style.Font.Bold = true;
            rngTitle.Style.Font.FontSize = 14;
            rngTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dong++;

            // Dòng trống
            dong++;

            return dong;
        }

        /// <summary>
        /// Phần thông tin nhân viên: Gộp label + value vào cùng 1 ô merge
        /// để tránh bị cắt chữ khi cột hẹp.
        /// </summary>
        private static int TaoThongTinNhanVien(
            IXLWorksheet ws,
            int dong,
            BangLuong bl,
            int thang,
            int nam
        )
        {
            // Dòng: "Họ và tên:  [Tên NV]" merge A:C  |  "Mã NV:  [Số]" merge D:E
            ws.Cell(dong, 1).Value = "Họ và tên:   " + (bl.HoTen ?? "");
            ws.Cell(dong, 1).Style.Font.Bold = true;
            ws.Range(dong, 1, dong, 3).Merge();

            ws.Cell(dong, 4).Value = "Mã NV:   " + bl.MaNV;
            ws.Cell(dong, 4).Style.Font.Bold = true;
            ws.Range(dong, 4, dong, 5).Merge();
            dong++;

            // Dòng: "Bộ phận:  [Tên BP]" merge A:C  |  "Kỳ lương:  Tháng X/Năm" merge D:E
            ws.Cell(dong, 1).Value = "Bộ phận:   " + (bl.TenBoPhan ?? "");
            ws.Cell(dong, 1).Style.Font.Bold = true;
            ws.Range(dong, 1, dong, 3).Merge();

            ws.Cell(dong, 4).Value = "Kỳ lương:   Tháng " + thang + "/" + nam;
            ws.Cell(dong, 4).Style.Font.Bold = true;
            ws.Range(dong, 4, dong, 5).Merge();
            dong++;

            // Dòng trống
            dong++;

            return dong;
        }

        /// <summary>Phần bảng chi tiết: 7 khoản mục + Tổng khấu trừ + Thực nhận</summary>
        private static int TaoBangChiTietLuong(IXLWorksheet ws, int dong, BangLuong bl)
        {
            int dongDau = dong; // Ghi nhớ dòng đầu bảng để kẻ viền sau

            // ── Header bảng (Bold, căn giữa, viền) ──
            string[] headers = { "STT", "KHOẢN MỤC", "ĐƠN VỊ TÍNH", "GIÁ TRỊ", "GHI CHÚ" };
            for (int c = 0; c < headers.Length; c++)
            {
                var cell = ws.Cell(dong, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            ws.Row(dong).Height = 24;
            dong++;

            int stt = 1;

            // ── Các khoản mục lương ──
            dong = ThemDongLuong(
                ws,
                dong,
                stt++,
                "Lương cơ bản",
                "VNĐ/tháng",
                bl.LuongCoBan,
                false,
                ""
            );
            dong = ThemDongLuong(ws, dong, stt++, "Ngày công chuẩn", "Ngày", 26, false, "Quy định");
            dong = ThemDongLuong(
                ws,
                dong,
                stt++,
                "Ngày công thực tế",
                "Ngày",
                bl.NgayCongThucTe,
                false,
                ""
            );
            dong = ThemDongLuong(
                ws,
                dong,
                stt++,
                "Lương theo ngày công",
                "VNĐ",
                bl.LuongTheoCong,
                true,
                "= LCB / 26 x Ngày công"
            );
            dong = ThemDongLuong(
                ws,
                dong,
                stt++,
                "Trừ BHXH (10.5%)",
                "VNĐ",
                bl.BHXH,
                false,
                "= LCB x 10.5%"
            );
            dong = ThemDongLuong(
                ws,
                dong,
                stt++,
                "Trừ Thuế TNCN",
                "VNĐ",
                bl.Thue,
                false,
                "Chưa áp dụng"
            );
            dong = ThemDongLuong(ws, dong, stt++, "Trừ Tiền tạm ứng", "VNĐ", bl.TienUng, false, "");

            // ── Dòng TỔNG KHẤU TRỪ (nền xám nhạt) ──
            decimal tongKhauTru = bl.BHXH + bl.Thue + bl.TienUng;
            ws.Cell(dong, 2).Value = "TỔNG CÁC KHOẢN KHẤU TRỪ";
            ws.Cell(dong, 2).Style.Font.Bold = true;
            ws.Cell(dong, 3).Value = "VNĐ";
            ws.Cell(dong, 4).Value = tongKhauTru;
            ws.Cell(dong, 4).Style.NumberFormat.Format = "#,##0";
            ws.Cell(dong, 4).Style.Font.Bold = true;
            // Nền xám nhạt cho toàn dòng
            var rngTong = ws.Range(dong, 1, dong, 5);
            rngTong.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E2E2");
            rngTong.Style.Font.Bold = true;
            rngTong.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dong, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Row(dong).Height = 22;
            dong++;

            // ── Dòng THỰC NHẬN (nền xanh lá, nổi bật) ──
            ws.Cell(dong, 2).Value = "THỰC NHẬN";
            ws.Cell(dong, 3).Value = "VNĐ";
            ws.Cell(dong, 4).Value = bl.TongThucNhan;
            ws.Cell(dong, 4).Style.NumberFormat.Format = "#,##0";
            // Nền xanh lá cho toàn dòng
            var rngThucNhan = ws.Range(dong, 1, dong, 5);
            rngThucNhan.Style.Fill.BackgroundColor = XLColor.FromHtml("#C8F0C8");
            rngThucNhan.Style.Font.Bold = true;
            rngThucNhan.Style.Font.FontSize = 13;
            rngThucNhan.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dong, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Row(dong).Height = 26;
            dong++;

            // ── Kẻ viền cho toàn bộ bảng ──
            var bangLuong = ws.Range(dongDau, 1, dong - 1, 5);
            bangLuong.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            bangLuong.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            return dong;
        }

        /// <summary>Thêm 1 dòng khoản mục vào bảng lương</summary>
        private static int ThemDongLuong(
            IXLWorksheet ws,
            int dong,
            int stt,
            string khoanMuc,
            string dvt,
            decimal giaTri,
            bool inDam,
            string ghiChu
        )
        {
            ws.Cell(dong, 1).Value = stt;
            ws.Cell(dong, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(dong, 2).Value = khoanMuc;

            ws.Cell(dong, 3).Value = dvt;
            ws.Cell(dong, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(dong, 4).Value = giaTri;
            ws.Cell(dong, 4).Style.NumberFormat.Format = "#,##0";
            ws.Cell(dong, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            if (inDam)
                ws.Cell(dong, 4).Style.Font.Bold = true;

            ws.Cell(dong, 5).Value = ghiChu;

            ws.Row(dong).Height = 20;
            return dong + 1;
        }

        /// <summary>
        /// Phần ký xác nhận cuối phiếu lương.
        /// Layout: A:B = Người lập | C:D = Kế toán | E = Người nhận
        /// </summary>
        private static void TaoPhienKyTen(IXLWorksheet ws, int dong, int thang, int nam)
        {
            // 2 dòng trống
            dong += 2;

            // Ngày tháng năm (căn giữa, merge C:E)
            ws.Cell(dong, 3).Value = $"Ngày ..... tháng {thang} năm {nam}";
            ws.Range(dong, 3, dong, 5).Merge();
            ws.Cell(dong, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(dong, 3).Style.Font.Italic = true;
            dong += 2;

            // Chức danh ký — merge cells để không bị cắt chữ
            // Cột A:B (2 cột = ~41) cho "NGƯỜI LẬP PHIẾ U"
            ws.Cell(dong, 1).Value = "NGƯỜI LẬP PHIẾU";
            ws.Cell(dong, 1).Style.Font.Bold = true;
            ws.Cell(dong, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(dong, 1, dong, 2).Merge();

            // Cột C:D (2 cột = ~34) cho "KẾ TOÁN TRƯỞNG"
            ws.Cell(dong, 3).Value = "KẾ TOÁN TRƯỞNG";
            ws.Cell(dong, 3).Style.Font.Bold = true;
            ws.Cell(dong, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(dong, 3, dong, 4).Merge();

            // Cột E (1 cột = 26) cho "NGƯỜI NHẬN LƯƠNG"
            ws.Cell(dong, 5).Value = "NGƯỜI NHẬN LƯƠNG";
            ws.Cell(dong, 5).Style.Font.Bold = true;
            ws.Cell(dong, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dong++;

            // Chữ ký — italic, căn giữa
            ws.Cell(dong, 1).Value = "(Ký, họ tên)";
            ws.Cell(dong, 1).Style.Font.Italic = true;
            ws.Cell(dong, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(dong, 1, dong, 2).Merge();

            ws.Cell(dong, 3).Value = "(Ký, họ tên)";
            ws.Cell(dong, 3).Style.Font.Italic = true;
            ws.Cell(dong, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(dong, 3, dong, 4).Merge();

            ws.Cell(dong, 5).Value = "(Ký, họ tên)";
            ws.Cell(dong, 5).Style.Font.Italic = true;
            ws.Cell(dong, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // ═══════════════════════════════════════════════════
        //  ĐỌC SỐ TIỀN BẰNG CHỮ (TIẾNG VIỆT)
        // ═══════════════════════════════════════════════════

        private static readonly string[] ChuSo =
        {
            "không",
            "một",
            "hai",
            "ba",
            "bốn",
            "năm",
            "sáu",
            "bảy",
            "tám",
            "chín",
        };

        /// <summary>
        /// Chuyển đổi số tiền sang dạng chữ tiếng Việt.
        /// Ví dụ: 5000000 → "Năm triệu đồng chẵn"
        /// </summary>
        public static string DocSoTien(decimal soTien)
        {
            long so = (long)Math.Abs(Math.Truncate(soTien));
            if (so == 0)
                return "Không đồng";

            string chuoi = DocSoNguyen(so);
            chuoi = char.ToUpper(chuoi[0]) + chuoi.Substring(1) + " đồng";

            if (soTien == Math.Truncate(soTien))
                chuoi += " chẵn";

            return chuoi;
        }

        private static string DocSoNguyen(long so)
        {
            if (so == 0)
                return "không";
            if (so < 0)
                return "âm " + DocSoNguyen(-so);

            string kq = "";

            if (so >= 1_000_000_000)
            {
                kq += DocSoNguyen(so / 1_000_000_000) + " tỷ ";
                so %= 1_000_000_000;
                if (so > 0 && so < 100_000_000)
                    kq += "không trăm ";
            }
            if (so >= 1_000_000)
            {
                kq += DocBaChuSo((int)(so / 1_000_000)) + " triệu ";
                so %= 1_000_000;
                if (so > 0 && so < 100_000)
                    kq += "không trăm ";
            }
            if (so >= 1_000)
            {
                kq += DocBaChuSo((int)(so / 1_000)) + " nghìn ";
                so %= 1_000;
                if (so > 0 && so < 100)
                    kq += "không trăm ";
            }
            if (so > 0)
                kq += DocBaChuSo((int)so);

            return kq.Trim();
        }

        private static string DocBaChuSo(int so)
        {
            if (so == 0)
                return "";
            if (so < 10)
                return ChuSo[so];

            string kq = "";
            int tram = so / 100;
            int chuc = (so % 100) / 10;
            int donVi = so % 10;

            if (tram > 0)
                kq += ChuSo[tram] + " trăm ";

            if (chuc > 1)
            {
                kq += ChuSo[chuc] + " mươi ";
                if (donVi == 1)
                    kq += "mốt";
                else if (donVi == 5)
                    kq += "lăm";
                else if (donVi > 0)
                    kq += ChuSo[donVi];
            }
            else if (chuc == 1)
            {
                kq += "mười ";
                if (donVi == 5)
                    kq += "lăm";
                else if (donVi > 0)
                    kq += ChuSo[donVi];
            }
            else
            {
                if (tram > 0 && donVi > 0)
                    kq += "lẻ " + ChuSo[donVi];
                else if (donVi > 0)
                    kq += ChuSo[donVi];
            }

            return kq.Trim();
        }
    }
}
