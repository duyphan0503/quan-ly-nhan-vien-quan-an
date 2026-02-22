# 📔 BÍ KÍP GIẢI TRÌNH ĐỒ ÁN: QUẢN LÝ NHÂN VIÊN QUÁN ĂN

> ⭐ **Đọc kỹ ít nhất 3 lần trước ngày bảo vệ.**
> Mục tiêu: Sinh viên đọc xong file này có thể tự tin giải trình bất kỳ câu hỏi nào từ giảng viên.
> Mỗi mục đều có **mẫu câu trả lời** — có thể tham khảo và điều chỉnh cho phù hợp.

---

## 📑 MỤC LỤC

1. [Bản đồ dự án — File nào ở đâu, dùng làm gì?](#phần-1-bản-đồ-dự-án)
2. [Database — Bảng nào, cột nào, quan hệ thế nào?](#phần-2-database)
3. [Luồng dữ liệu — Từ nút bấm đến Database rồi quay lại](#phần-3-luồng-dữ-liệu)
4. [Giải thích từng file quan trọng](#phần-4-giải-thích-từng-file)
5. [Cách demo trực tiếp khi giảng viên yêu cầu](#phần-5-demo-trực-tiếp)
6. [Bộ 50+ câu hỏi & câu trả lời sẵn](#phần-6-bộ-câu-hỏi--trả-lời)
7. [Xử lý tình huống khó — Khi bị hỏi câu không biết](#phần-7-xử-lý-tình-huống-khó)

---

## PHẦN 1: BẢN ĐỒ DỰ ÁN

### Cấu trúc thư mục

```
QuanLyNhanVien/
├── 📁 Forms/           → Giao diện (nơi người dùng nhìn thấy & bấm nút)
├── 📁 Services/        → "Bộ não" — xử lý logic, validation, tính toán
├── 📁 DataAccess/      → Nơi DUY NHẤT viết code SQL (SELECT, INSERT, UPDATE...)
├── 📁 Models/          → "Bản vẽ" các bảng Database → thành class C#
├── 📁 Infrastructure/  → Công cụ hạ tầng (Log, Bảo mật, Excel, Xử lý lỗi)
├── 📁 Controls/        → Custom control tự thiết kế (nút đẹp, panel kính mờ)
├── 📁 SQL/             → Script tạo Database
├── AppColors.cs        → Bảng màu toàn ứng dụng
├── AppFonts.cs         → Font chữ
├── AppIcons.cs         → Icon
└── Program.cs          → Điểm khởi động
```

### Tóm tắt kiến trúc (Câu mở đầu khi giải trình)

> _"Em chia theo **mô hình 3 lớp** (3-Layer Architecture): Giao diện (Forms) — Nghiệp vụ (Services) — Dữ liệu (DataAccess). Ngoài ra có thêm tầng hỗ trợ: Infrastructure (hạ tầng: log, bảo mật, Excel), Models (thực thể), Controls (giao diện tùy chỉnh). Mục đích là **tách biệt trách nhiệm**: mỗi lớp chỉ làm một việc, dễ bảo trì, dễ mở rộng."_

### Giải thích từng lớp (dành cho người mới bắt đầu)

**3 lớp chính:**

| Lớp | Ví dụ thực tế | Trong code |
|---|---|---|
| **Forms** (Giao diện) | Cái màn hình bạn nhìn thấy, nút bấm, ô nhập liệu | `FormLogin.cs`, `FormNhanVien.cs`, ... |
| **Services** (Nghiệp vụ) | "Bộ lọc thông minh" — kiểm tra dữ liệu có đúng không trước khi lưu | `NhanVienService.cs`, `BangLuongService.cs` |
| **DataAccess** (Dữ liệu) | Người đi giao hàng — mang dữ liệu từ DB lên và từ code xuống DB | `NhanVienDAL.cs`, `BangLuongDAL.cs` |

**Các tầng hỗ trợ (không thuộc 3 lớp chính nhưng rất quan trọng):**

| Tầng hỗ trợ | Ví dụ thực tế | Trong code |
|---|---|---|
| **Models** (Thực thể) | Tờ giấy mẫu — mô tả 1 nhân viên gồm những thông tin gì | `NhanVien.cs`, `BangLuong.cs` |
| **Infrastructure** (Hạ tầng) | Đội hậu cần — lo log lỗi, bảo mật, xuất Excel | `AppLogger.cs`, `ExcelExporter.cs` |
| **Controls** (UI tùy chỉnh) | Nút bấm đẹp hơn mặc định, panel kính mờ | `RoundedButton.cs`, `GlassPanel.cs` |

---

## PHẦN 2: DATABASE

### Các bảng chính (file `SQL/CreateDatabase.sql`)

| # | Bảng | Cột quan trọng | Dùng để làm gì |
|---|---|---|---|
| 1 | `TaiKhoan` | `MaTK`, `TenDangNhap`, `MatKhau`, `VaiTro` | Lưu tài khoản đăng nhập. TK mặc định: **admin / admin123** |
| 2 | `BoPhan` | `MaBoPhan`, `TenBoPhan` | Bếp, Phục vụ, Thu ngân, Bảo vệ, Quản lý, Hải sản |
| 3 | `NhanVien` | `MaNV`, `HoTen`, `ChucVu`, `MaBoPhan` (FK), `LuongCoBan`, `TrangThai` | Thông tin chi tiết nhân viên |
| 4 | `BangLuong` | `MaNV` (FK), `Thang`, `Nam`, `NgayCongThucTe`, `LuongTheoCong`, `TienUng`, `BHXH`, `Thue`, `TongThucNhan` | Bảng lương tháng — mỗi NV có tối đa 1 bản ghi/tháng |
| 5 | `ErrorLog` | `MucDo`, `NguonLoi`, `ThongBao`, `NguoiDung`, `TenMay` | Ghi lại lỗi hệ thống tự động |

### Quan hệ giữa các bảng (QUAN TRỌNG — hay hỏi)

```
BoPhan (1) ──── (N) NhanVien       Một bộ phận có nhiều nhân viên
NhanVien (1) ── (N) BangLuong      Một nhân viên có nhiều bảng lương (theo tháng)
```

**Khi bị hỏi "Quan hệ giữa các bảng?":**
> _"Bảng `NhanVien` có khóa ngoại `MaBoPhan` tham chiếu đến `BoPhan` — quan hệ 1-N (1 bộ phận có nhiều NV). Bảng `BangLuong` có khóa ngoại `MaNV` tham chiếu đến `NhanVien` — quan hệ 1-N (1 NV có nhiều bảng lương theo tháng). Thêm ràng buộc `UNIQUE(MaNV, Thang, Nam)` đảm bảo mỗi NV chỉ có 1 bản ghi lương mỗi tháng."_

### Tài khoản đăng nhập mặc định

| Tên đăng nhập | Mật khẩu | Vai trò |
|---|---|---|
| `admin` | `admin123` | Admin |

---

## PHẦN 3: LUỒNG DỮ LIỆU

> ⭐ **ĐÂY LÀ PHẦN QUAN TRỌNG NHẤT.** Giảng viên thường hỏi "Khi em bấm nút X, code chạy qua đâu?"
> Học thuộc ít nhất 3 luồng đầu tiên.

### 3.1. Luồng KHỞI ĐỘNG ứng dụng

**File: `Program.cs`**

```
Bước 1: GlobalExceptionHandler.Install()
    → Cài đặt bộ bắt lỗi toàn cục (nếu có lỗi bất ngờ, app không crash mà hiện thông báo đẹp)

Bước 2: DatabaseHelper.TestConnection()
    → Thử kết nối SQL Server bằng connection string trong App.config
    
    NẾU KẾT NỐI THẤT BẠI:
        → Mở FormConnectionWizard (wizard 4 bước cấu hình kết nối)
        → Wizard kiểm tra: TCP → Auth → DB → Schema
        → Lưu connection string mới → RefreshConnectionString()
    
    NẾU THÀNH CÔNG:
        → Mở FormLogin (màn hình đăng nhập)

Bước 3: Application.Run(new FormLogin())
    → Chạy ứng dụng
```

**Khi bị hỏi:** _"Nếu cài lên máy mới chưa có DB thì sao?"_
> _"Khi khởi động, `DatabaseHelper.TestConnection()` thử kết nối. Nếu thất bại, hệ thống tự bật `FormConnectionWizard` — một wizard 4 bước hướng dẫn nhập Server, User, Password rồi kiểm tra tự động. Sau đó `RefreshConnectionString()` cập nhật App.config mà KHÔNG cần khởi động lại."_

### 3.2. Luồng ĐĂNG NHẬP

**Người dùng: Nhập tên + mật khẩu → Bấm "Đăng Nhập"**

```
FormLogin.cs  →  btnDangNhap_Click()
│
├── Gọi: TaiKhoanService.DangNhap("admin", "admin123")
│   │
│   ├── Kiểm tra tên rỗng?     → "Vui lòng nhập tên đăng nhập."
│   ├── Kiểm tra mật khẩu rỗng? → "Vui lòng nhập mật khẩu."
│   │
│   ├── Hash mật khẩu: SecurityHelper.HashPassword("admin123")
│   │   → "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
│   │
│   └── Gọi: TaiKhoanDAL.DangNhap("admin", "240be518...")
│       │
│       └── Chạy SQL: SELECT * FROM TaiKhoan 
│                      WHERE TenDangNhap = @td AND MatKhau = @mk
│           │
│           ├── Tìm thấy → Trả về object TaiKhoan
│           └── Không tìm → Trả về null
│
├── Service trả về: ServiceResult<TaiKhoan>.Ok(tk) hoặc .Fail("Sai tên/mật khẩu")
│
├── NẾU THÀNH CÔNG:
│   ├── AppLogger.SetCurrentUser("admin")  ← Ghi nhớ ai đang đăng nhập
│   ├── Đóng FormLogin
│   └── Mở FormMain(tk)  ← Truyền thông tin tài khoản vào
│
└── NẾU THẤT BẠI:
    └── MessageBox: "Sai tên đăng nhập hoặc mật khẩu"
```

**Bonus "Ghi nhớ đăng nhập":**
> _"Nếu tích checkbox, mật khẩu được mã hóa bằng `SecurityHelper.Encrypt()` sử dụng Windows **DPAPI** (`ProtectedData.Protect`) rồi lưu vào file `login.cfg`. Lần sau mở app sẽ tự điền. File `login.cfg` chỉ giải mã được trên đúng tài khoản Windows đó."_

### 3.3. Luồng THÊM NHÂN VIÊN

**Người dùng: Điền form → Bấm "Lưu"**

```
FormNhanVien.cs  →  btnLuu_Click()
│
├── Đọc TextBox/ComboBox → Tạo object NhanVien:
│   NhanVien nv = new NhanVien {
│       HoTen = "Nguyễn Văn A",
│       MaBoPhan = 2,           ← Phục vụ
│       LuongCoBan = 5000000,
│       TrangThai = "Đang làm"
│   };
│
├── Gọi: NhanVienService.ThemNhanVien(nv)
│   │
│   ├── ValidateNhanVien(nv):
│   │   ├── HoTen rỗng?         → ServiceResult.Fail("Vui lòng nhập họ tên.")
│   │   ├── MaBoPhan <= 0?      → ServiceResult.Fail("Vui lòng chọn bộ phận.")
│   │   ├── LuongCoBan < 0?     → ServiceResult.Fail("Lương cơ bản không được âm.")
│   │   └── TrangThai rỗng?     → ServiceResult.Fail("Vui lòng chọn trạng thái.")
│   │
│   └── NhanVienDAL.Them(nv)
│       └── SQL: INSERT INTO NhanVien (HoTen, ChucVu, MaBoPhan, LuongCoBan, TrangThai)
│                VALUES (@ten, @cv, @bp, @luong, @tt)
│
├── NẾU OK: MessageBox "Thêm nhân viên thành công." + Tải lại danh sách
└── NẾU LỖI: MessageBox hiển thị lý do lỗi từ Service
```

### 3.4. Luồng XÓA NHÂN VIÊN (có Business Rule đặc biệt ⭐)

```
FormNhanVien.cs  →  btnXoa_Click()
│
├── Lấy MaNV từ dòng đang chọn trên bảng
│
├── Gọi: NhanVienService.XoaNhanVien(maNV)
│   │
│   ├── BƯỚC ĐẶC BIỆT: NhanVienDAL.CoLuong(maNV)
│   │   └── SQL: SELECT COUNT(*) FROM BangLuong WHERE MaNV = @id
│   │
│   ├── NẾU COUNT > 0 (đã có bảng lương):
│   │   └── KHÔNG CHO XÓA → Fail("Không thể xoá NV đã có bảng lương.\n
│   │                             Hãy chuyển trạng thái sang 'Nghỉ việc'.")
│   │
│   └── NẾU COUNT = 0 (chưa có lương):
│       └── NhanVienDAL.Xoa(maNV)
│           └── SQL: DELETE FROM NhanVien WHERE MaNV = @id
```

**Khi bị hỏi "Tại sao không xóa được nhân viên?":**
> _"Đây là **business rule** (quy tắc nghiệp vụ): nhân viên đã có bảng lương thì không được xóa vật lý, vì sẽ vi phạm ràng buộc `FOREIGN KEY` — bảng `BangLuong` đang tham chiếu đến `NhanVien`. Thay vào đó, người dùng phải chuyển trạng thái sang 'Nghỉ việc' để bảo toàn lịch sử kế toán."_

### 3.5. Luồng TÍNH LƯƠNG

```
FormBangLuong.cs  →  Chọn NV, nhập ngày công, tiền ứng  →  Bấm "Tính Lương"
│
├── Gọi: BangLuongService.TinhLuong(luongCoBan, ngayCong, tienUng)
│   │
│   ├── Validation:
│   │   ├── ngayCong < 0 hoặc > 31?  → Fail
│   │   ├── tienUng < 0?             → Fail
│   │   └── luongCoBan < 0?          → Fail
│   │
│   ├── CÔNG THỨC (file BangLuongService.cs, dòng ~73):
│   │   ├── LuongTheoCong = round(LuongCoBan ÷ 26 × NgayCong)
│   │   ├── BHXH          = round(LuongCoBan × 10.5%)
│   │   ├── Thue          = 0  (chưa triển khai)
│   │   └── TongThucNhan  = LuongTheoCong − TienUng − BHXH − Thue
│   │       (nếu < 0 thì = 0)
│   │
│   └── Trả về: ServiceResult<KetQuaTinhLuong>.Ok(kq)
│
├── Hiển thị kết quả xem trước: "Lương theo công: 4,615,385đ | BHXH: 525,000đ | ..."
│
├── Bấm "Lưu":
│   ├── BangLuongService.LuuBangLuong(maNV, thang, nam, ...)
│   │   └── BangLuongDAL.LuuBangLuong(bl)
│   │       ├── SELECT COUNT(*) → Đã tồn tại tháng này chưa?
│   │       ├── Chưa → INSERT INTO BangLuong ...
│   │       └── Rồi  → UPDATE BangLuong SET ... WHERE MaNV=@id AND Thang=@th AND Nam=@nam
│   │
│   └── Đây gọi là "UPSERT pattern" (Insert or Update)
```

**Khi bị hỏi "Công thức tính lương?":**
> _"Lương theo ngày công = Lương cơ bản chia 26 ngày chuẩn, nhân ngày công thực tế. BHXH = 10.5% lương cơ bản. Thực nhận = Lương theo công trừ BHXH trừ tiền ứng. Hằng số 26 và 10.5% được khai báo ở đầu file `BangLuongService.cs` dòng ~24 để dễ thay đổi sau này."_

**Khi bị hỏi "Upsert là gì?":**
> _"Upsert là viết tắt của Update + Insert. Khi lưu bảng lương, hệ thống kiểm tra: nếu nhân viên chưa có bảng lương tháng đó thì INSERT, nếu đã có thì UPDATE. Người dùng không cần phân biệt thao tác nào."_

### 3.6. Luồng XUẤT PHIẾU LƯƠNG EXCEL ⭐

```
FormBangLuong.cs  →  Bấm "📄 Xuất 1 Phiếu" hoặc "📋 Xuất Tất Cả Tháng"
│
├── BtnXuatExcel_Click (xuất 1 phiếu):
│   ├── Lấy dòng đang chọn trên DataGridView
│   ├── Tìm object BangLuong tương ứng từ Service
│   ├── Mở SaveFileDialog → Người dùng chọn nơi lưu
│   └── Gọi: ExcelExporter.ExportMotPhieu(bl, filePath)
│
├── BtnXuatTatCa_Click (xuất tất cả):
│   ├── Lấy tháng/năm đang chọn
│   ├── BangLuongService.LayTheoThangNam(thang, nam) → List<BangLuong>
│   ├── Mở SaveFileDialog
│   └── Gọi: ExcelExporter.ExportTatCaPhieu(danhSach, thang, nam, filePath)
│
├── Bên trong ExcelExporter (file Infrastructure/ExcelExporter.cs):
│   │
│   ├── using (var workbook = new XLWorkbook())  ← Tạo file Excel mới
│   │
│   ├── Với MỖI nhân viên trong danh sách:
│   │   ├── workbook.Worksheets.Add(tenNV)  ← Mỗi NV = 1 sheet riêng
│   │   └── TaoPhieuLuong(sheet, bl, thang, nam):
│   │       │
│   │       ├── TaoHeaderDoanhNghiep():
│   │       │   ├── Dòng 1: "NHÀ HÀNG QUÁN ĂN NGON" (bold, 16pt, merge A:E)
│   │       │   ├── Dòng 2: Địa chỉ (merge A:E, căn giữa)
│   │       │   └── Dòng 3: SĐT + MST
│   │       │
│   │       ├── TaoThongTinNhanVien():
│   │       │   ├── "Họ và tên:  Nguyễn Văn A" (merge A:C)
│   │       │   └── "Mã NV:  9" (merge D:E)
│   │       │
│   │       ├── TaoBangChiTietLuong():
│   │       │   ├── Header: STT | KHOẢN MỤC | ĐƠN VỊ TÍNH | GIÁ TRỊ | GHI CHÚ
│   │       │   ├── 7 dòng khoản mục (LCB, ngày công, lương theo công, BHXH, thuế, tiền ứng)
│   │       │   ├── Dòng TỔNG KHẤU TRỪ (nền XÁM #E2E2E2)
│   │       │   ├── Dòng THỰC NHẬN (nền XANH LÁ #C8F0C8, bold 13pt)
│   │       │   └── Kẻ viền toàn bộ bảng
│   │       │
│   │       ├── DocSoTien(bl.TongThucNhan):
│   │       │   └── VD: 4,475,000 → "Bốn triệu bốn trăm bảy mươi lăm nghìn đồng chẵn"
│   │       │
│   │       └── TaoPhienKyTen():
│   │           ├── "NGƯỜI LẬP PHIẾU" (merge A:B)
│   │           ├── "KẾ TOÁN TRƯỞNG" (merge C:D)
│   │           └── "NGƯỜI NHẬN LƯƠNG" (cột E)
│   │
│   └── workbook.SaveAs(filePath)  ← Lưu file .xlsx
│
└── MessageBox "Xuất thành công!" → Process.Start(filePath) → Tự mở file Excel
```

**Khi bị hỏi "Giải thích chức năng xuất Excel?":**
> _"Em dùng thư viện **ClosedXML** — thư viện mã nguồn mở MIT License, cài qua NuGet, không cần cài Microsoft Office. Code được chia thành 5 phương thức riêng biệt: `TaoHeaderDoanhNghiep()` tạo phần tên quán, `TaoThongTinNhanVien()` tạo thông tin NV dùng merge cells, `TaoBangChiTietLuong()` tạo bảng 7 khoản mục có viền, `DocSoTien()` chuyển số thành chữ tiếng Việt, `TaoPhienKyTen()` tạo phần chữ ký. Mỗi nhân viên là 1 sheet riêng."_

---

## PHẦN 4: GIẢI THÍCH TỪNG FILE QUAN TRỌNG

### 4.1. `ServiceResult.cs` — Mẫu kết quả trả về

**File:** `Services/ServiceResult.cs` (63 dòng)

**Nó là gì?** Một class "bọc" kết quả, gồm 3 thứ: `Success` (bool), `Data` (dữ liệu), `Message` (thông báo).

**Cách dùng:**
```csharp
// Thành công — kèm dữ liệu
return ServiceResult<TaiKhoan>.Ok(tk);

// Thất bại — kèm lý do
return ServiceResult.Fail("Vui lòng nhập họ tên.");
```

**Khi bị hỏi "Sao không chỉ dùng true/false?":**
> _"Nếu chỉ trả true/false thì Form không biết **tại sao** thất bại. `ServiceResult` đóng gói cả kết quả lẫn thông điệp. VD: `ServiceResult.Fail("Không thể xóa NV đã có bảng lương.")` — Form nhận được và hiện đúng lý do cho người dùng. Đây là **Result Pattern**, một design pattern phổ biến."_

### 4.2. `AppLogger.cs` — Hệ thống ghi log kép

**File:** `Infrastructure/AppLogger.cs` (227 dòng)

**Nó làm gì?** Ghi lại mọi lỗi, cảnh báo, thông tin vào **2 nơi đồng thời**:

| Kênh | Cách ghi | Khi nào hoạt động |
|---|---|---|
| **File log** | File `.log` trong thư mục `Logs/`, đặt tên theo ngày (VD: `2026-02-22.log`) | **LUÔN LUÔN** — kể cả khi DB chết |
| **Database** | Bảng `ErrorLog` trên SQL Server qua Stored Procedure | **Best-effort** — nếu DB chết thì bỏ qua, không crash |

**4 mức độ log:**
```csharp
AppLogger.Info("Đăng nhập thành công");           // Thông tin bình thường
AppLogger.Warning("Ngày công vượt quá 26");       // Cảnh báo
AppLogger.Error("Không thể lưu bảng lương", ex);  // Lỗi (kèm Exception)
AppLogger.Critical("Database connection lost");    // Nghiêm trọng
```

**Mỗi dòng log chứa:** Thời gian, Mức độ, Nguồn lỗi (class + method), Tên user, Tên máy tính.

**Khi bị hỏi "Log ghi ở đâu? Làm sao biết lỗi?":**
> _"AppLogger ghi đồng thời 2 nơi: file `.log` xoay vòng theo ngày (luôn hoạt động) VÀ bảng `ErrorLog` trong DB (best-effort). Mỗi entry có đầy đủ: thời gian, mức độ (Info/Warning/Error/Critical), tên hàm gây lỗi, user đang đăng nhập, tên máy tính. Khi giao khách, admin chỉ cần mở file log hoặc query bảng ErrorLog là thấy ngay lỗi ở đâu."_

### 4.3. `GlobalExceptionHandler.cs` — Bắt lỗi toàn cục

**File:** `Infrastructure/GlobalExceptionHandler.cs` (217 dòng)

**Nó làm gì?** Bắt TẤT CẢ các lỗi chưa được xử lý — cả trên UI thread lẫn background thread — để ứng dụng không crash đột ngột.

**Cách hoạt động:**
```
Lỗi xảy ra (bất kỳ đâu trong app)
    ↓
GlobalExceptionHandler bắt được
    ↓
1. Ghi log bằng AppLogger.Critical(...)
2. Phân loại lỗi:
   - SqlException? → Dịch mã lỗi SQL sang tiếng Việt
   - UnauthorizedAccessException? → "Không có quyền truy cập"
   - OutOfMemoryException? → "Hết bộ nhớ"
   - Lỗi khác? → Thông báo chung
3. Hiện MessageBox tiếng Việt thân thiện (không crash!)
```

**Mã lỗi SQL thường gặp (hàm `ClassifySqlError`):**
| Mã | Ý nghĩa tiếng Việt |
|---|---|
| `18456` | Sai tên đăng nhập SQL Server |
| `4060` | Database không tồn tại |
| `547` | Vi phạm ràng buộc khóa ngoại (FK) |
| `2627` | Dữ liệu trùng lặp (UNIQUE) |
| `-2` | Timeout kết nối |

### 4.4. `SecurityHelper.cs` — Hash mật khẩu + Mã hóa DPAPI

**File:** `Infrastructure/SecurityHelper.cs` (67 dòng)

**Có 2 cơ chế bảo mật riêng biệt:**

| Cơ chế | Dùng cho | Hàm | Có thể giải mã? |
|---|---|---|---|
| **SHA-256 Hash** | Lưu mật khẩu vào DB | `HashPassword()` | ❌ Không (một chiều) |
| **DPAPI Encrypt** | "Ghi nhớ đăng nhập" (file `login.cfg`) | `Encrypt()` / `Decrypt()` | ✅ Có (trên cùng tài khoản Windows) |

**Cách dùng SHA-256:**
```csharp
// Hash mật khẩu — kết quả là chuỗi hex 64 ký tự, KHÔNG thể giải mã
string hash = SecurityHelper.HashPassword("admin123");
// → "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"

// Đăng nhập: hash mật khẩu rồi so sánh với DB
string matKhauHash = SecurityHelper.HashPassword(matKhauNhapVao);
var tk = _dal.DangNhap(tenDangNhap, matKhauHash);
```

**Cách dùng DPAPI:**
```csharp
// Mã hóa — dùng Windows DPAPI + entropy salt
string encrypted = SecurityHelper.Encrypt("admin123");
// → "AQAAANCMnd8BFdERjHoAwE..."  (chuỗi Base64 rất dài)

// Giải mã — CHỈ hoạt động trên cùng tài khoản Windows
string original = SecurityHelper.Decrypt(encrypted);
// → "admin123"
```

**Khi bị hỏi "Mật khẩu lưu thế nào? Có an toàn không?":**
> _"Mật khẩu trong database được hash bằng **SHA-256** — hàm một chiều, không thể giải mã ngược. Khi đăng nhập, hệ thống hash mật khẩu người dùng nhập vào rồi so sánh với hash trong DB. Riêng chức năng 'Ghi nhớ đăng nhập' dùng **Windows DPAPI** (`ProtectedData.Protect`) mã hóa hai chiều, lưu vào file `login.cfg` — chỉ giải mã được trên cùng tài khoản Windows."_

### 4.5. `ExcelExporter.cs` — Xuất phiếu lương Excel ⭐

**File:** `Infrastructure/ExcelExporter.cs` (531 dòng)

**5 phương thức chính:**

| Phương thức | Dòng | Chức năng |
|---|---|---|
| `ExportMotPhieu()` | ~38 | Xuất 1 phiếu lương đơn lẻ |
| `ExportTatCaPhieu()` | ~46 | Xuất tất cả phiếu tháng → mỗi NV 1 sheet |
| `TaoPhieuLuong()` | ~89 | Tạo layout phiếu lương trên 1 sheet |
| `DocSoTien()` | ~431 | Chuyển số tiền → chữ tiếng Việt |
| `DocBaChuSo()` | ~486 | Đọc từng nhóm 3 chữ số (trăm, chục, đơn vị) |

**Kỹ thuật đặc biệt:**
- **Merge cells** (`ws.Range(dong, 1, dong, 5).Merge()`) — gộp nhiều ô thành 1, tránh bị cắt chữ
- **Number format** (`#,##0`) — hiển thị dấu phẩy phân cách nghìn: `5,000,000` thay vì `5000000`
- **Nền màu** — Dòng tổng khấu trừ: xám `#E2E2E2`, dòng thực nhận: xanh lá `#C8F0C8`
- **Page Setup** — A4 Portrait, căn giữa trang, lề hẹp → in không cần chỉnh

**Khi bị hỏi "Giải thích hàm `DocSoTien()`?":**
> _"Hàm nhận vào số decimal, rồi gọi `DocSoNguyen()` để chia thành nhóm: tỷ, triệu, nghìn, đơn vị. Mỗi nhóm 3 chữ số gọi `DocBaChuSo()` xử lý riêng. Có xử lý đặc biệt tiếng Việt: số 1 ở hàng đơn vị đọc 'mốt' (21 = hai mươi mốt), số 5 đọc 'lăm' (25 = hai mươi lăm), hàng chục = 0 thì đọc 'lẻ' (105 = một trăm lẻ năm). Kết quả: `5,000,000` → 'Năm triệu đồng chẵn'."_

### 4.6. `AppColors.cs` — Bảng màu Catppuccin Mocha

**File:** `AppColors.cs` (90 dòng)

**Các nhóm màu:**
| Nhóm | Tên | Mã hex | Dùng cho |
|---|---|---|---|
| Nền | `Base` | `#1E1E2E` | Nền chính toàn app |
| Nền đậm | `Mantle` | `#181825` | Sidebar, panel |
| Chữ | `Text` | `#CDD6F4` | Chữ chính |
| Xanh lá | `Green` | `#A6E3A1` | Nút lưu, thành công |
| Xanh dương | `Blue` | `#89B4FA` | Nút tính lương |
| Đỏ | `Red` | `#F38BA8` | Nút xóa, lỗi |
| Vàng | `Yellow` | `#F9E2AF` | Cảnh báo |
| Kính mờ | `GlassBg` | `rgba(24,24,37,0.7)` | Hiệu ứng glassmorphism |

**Khi bị hỏi "Bảng màu lấy từ đâu?":**
> _"Em dùng **Catppuccin Mocha** — một design system mã nguồn mở nổi tiếng, được thiết kế đặc biệt để giảm mỏi mắt khi nhìn lâu. Toàn bộ màu sắc tập trung trong 1 file `AppColors.cs`, đảm bảo giao diện đồng nhất. Thêm hàm `Lerp()` cho nội suy màu và `Lighten()`/`Darken()` cho hiệu ứng hover/pressed."_

### 4.7. `BangLuongService.cs` — Tính lương

**File:** `Services/BangLuongService.cs` (167 dòng)

**Hằng số quan trọng (dòng ~24):**
```csharp
public const decimal NGAY_CONG_CHUAN = 26m;  // 26 ngày/tháng
public const decimal TY_LE_BHXH = 0.105m;    // 10.5% lương cơ bản
```

**Khi bị hỏi "Tại sao 26 ngày? Có thể thay đổi không?":**
> _"26 ngày là ngày công chuẩn theo quy định lao động phổ biến (30 ngày trừ 4 ngày chủ nhật). Nó được khai báo là hằng số `NGAY_CONG_CHUAN` ở đầu file, nếu muốn thay đổi chỉ cần sửa 1 chỗ duy nhất."_

**`KetQuaTinhLuong`** (dòng ~37) — class chứa kết quả:
```csharp
public class KetQuaTinhLuong
{
    public decimal LuongTheoCong { get; set; }
    public decimal BHXH { get; set; }
    public decimal Thue { get; set; }
    public decimal TongThucNhan { get; set; }
}
```

**Khi bị hỏi "Tại sao tách TinhLuong() ra riêng?":**
> _"Hàm `TinhLuong()` là **pure function** (hàm thuần túy) — không truy cập database, chỉ nhận input và trả output. Em tách riêng để: (1) dễ test độc lập, (2) tách biệt logic tính toán và thao tác lưu, (3) có thể tái sử dụng cho preview trước khi lưu."_

---

## PHẦN 5: DEMO TRỰC TIẾP

> ⭐ Khi giảng viên yêu cầu "Chỉ cho tôi xem code", làm theo hướng dẫn:

### Demo 1: "Chỉ code tính lương"
1. 👉 Mở `Services/BangLuongService.cs`
2. 👉 Cuộn đến dòng **~55** → Hàm `TinhLuong()`
3. 👉 Nói: _"Đây ạ. Hàm nhận 3 tham số: lương cơ bản, ngày công, tiền ứng. Dòng 73 là công thức cốt lõi: `luongCoBan / 26 * ngayCong`. Dòng 74: BHXH = lương cơ bản nhân 10.5%. Dòng 77: tổng thực nhận = lương theo công trừ các khoản khấu trừ. Hàm này là pure function, không đụng database."_

### Demo 2: "Chỉ code xóa nhân viên có bảng lương"
1. 👉 Mở `Services/NhanVienService.cs`
2. 👉 Cuộn đến dòng **~68** → Hàm `XoaNhanVien()`
3. 👉 Nói: _"Dòng 75: em gọi `_dal.CoLuong(maNV)` kiểm tra nhân viên có bảng lương không. Nếu có thì dòng 76 trả về Fail('Không thể xoá NV đã có bảng lương'), yêu cầu chuyển trạng thái 'Nghỉ việc' thay vì xóa."_

### Demo 3: "Chỉ code xuất Excel"
1. 👉 Mở `Infrastructure/ExcelExporter.cs`
2. 👉 Cuộn đến dòng **~89** → Hàm `TaoPhieuLuong()`
3. 👉 Nói: _"Mỗi phần phiếu lương là 1 phương thức riêng: dòng 105 gọi `TaoHeaderDoanhNghiep()`, dòng 108 gọi `TaoThongTinNhanVien()`, dòng 111 gọi `TaoBangChiTietLuong()`. Em dùng merge cells, number format, và background color để phiếu lương đẹp khi in."_

### Demo 4: "Chỉ code đọc số tiền bằng chữ"
1. 👉 Mở `Infrastructure/ExcelExporter.cs`
2. 👉 Cuộn đến dòng **~431** → Hàm `DocSoTien()`
3. 👉 Nói: _"Hàm chia số thành nhóm 3 chữ số: tỷ, triệu, nghìn, đơn vị. Xem dòng 486 — hàm `DocBaChuSo()` xử lý đặc biệt tiếng Việt: 'mốt' cho số 1, 'lăm' cho số 5, 'lẻ' cho hàng chục = 0."_

### Demo 5: "Chỉ code ServiceResult"
1. 👉 Mở `Services/ServiceResult.cs`
2. 👉 Nói: _"Có 2 class: `ServiceResult<T>` (có dữ liệu) dùng cho đăng nhập, trả về TaiKhoan. `ServiceResult` (không dữ liệu) dùng cho thêm/sửa/xóa, chỉ cần biết thành công hay thất bại. Cả 2 đều có `Success`, `Message`, và factory methods `Ok()` / `Fail()`."_

### Demo 6: "Chỉ code chống SQL Injection"
1. 👉 Mở bất kỳ file DAL nào (VD: `DataAccess/NhanVienDAL.cs`)
2. 👉 Tìm dòng có `Parameters.AddWithValue`
3. 👉 Nói: _"Tất cả câu SQL đều dùng `SqlParameter` — tham số hóa hoàn toàn. Dòng này `cmd.Parameters.AddWithValue("@ten", nv.HoTen)` — SQL Server tự động escape ký tự đặc biệt, không thể SQL injection."_

---

## PHẦN 6: BỘ CÂU HỎI & TRẢ LỜI

### ❓ Kiến trúc & Thiết kế

| # | Câu hỏi | Mẫu câu trả lời |
|---|---|---|
| 1 | "Tại sao chia 3 lớp?" | "Mô hình 3 lớp: Form lo hiển thị, Service lo logic nghiệp vụ, DAL lo truy xuất database. Thêm tầng hỗ trợ: Models (thực thể), Infrastructure (log, bảo mật, Excel), Controls (UI tùy chỉnh). Nếu đổi DB từ SQL Server sang MySQL, chỉ sửa DAL mà không đụng Form hay Service." |
| 2 | "Lớp Service để làm gì?" | "Chứa business rules: validate dữ liệu trước khi lưu, tính toán lương, ngăn xóa NV có lương. Không để Form tự validate hay tính toán." |
| 3 | "Tại sao có Infrastructure?" | "Infrastructure chứa các công cụ dùng chung cho toàn app: Logger, bảo mật, Excel, xử lý lỗi. Tách riêng để không trộn lẫn với logic nghiệp vụ." |
| 4 | "DatabaseHelper để làm gì?" | "Quản lý connection string tập trung. Chỉ cần gọi `GetConnection()` là có kết nối. Connection Wizard chỉ cần gọi `RefreshConnectionString()` là cập nhật mà không restart." |
| 5 | "Tại sao không dùng Entity Framework?" | "Em chọn ADO.NET thuần để kiểm soát hoàn toàn câu SQL, dễ debug, hiệu năng tốt cho quy mô nhỏ. Entity Framework phù hợp hệ thống lớn hơn." |
| 6 | "Design Pattern nào đang dùng?" | "Result Pattern (ServiceResult), Layered Architecture, Factory Method (Ok/Fail), Repository Pattern (DAL), Singleton (AppLogger), Upsert Pattern." |

### ❓ Tính năng

| # | Câu hỏi | Câu trả lời |
|---|---|---|
| 7 | "Công thức tính lương?" | "LuongTheoCong = LuongCoBan ÷ 26 × NgàyCông. BHXH = LuongCoBan × 10.5%. ThựcNhận = LuongTheoCong − TiềnỨng − BHXH − Thuế." |
| 8 | "Tại sao 26 ngày?" | "26 là ngày công chuẩn (30 trừ 4 chủ nhật). Khai báo hằng số `NGAY_CONG_CHUAN = 26m` ở đầu BangLuongService.cs, dễ thay đổi." |
| 9 | "Nếu đã có lương tháng đó thì sao?" | "Dùng Upsert pattern: SELECT COUNT trước — chưa có thì INSERT, có rồi thì UPDATE. User không cần biết đang thêm hay sửa." |
| 10 | "Tại sao không xóa NV có lương?" | "Business rule: xóa sẽ vi phạm Foreign Key + mất lịch sử kế toán. Thay vào đó chuyển trạng thái 'Nghỉ việc'." |
| 11 | "Export Excel dùng gì?" | "ClosedXML — thư viện mã nguồn mở MIT License, cài qua NuGet, không cần Office. Code rõ ràng: `ws.Cell(1,1).Value = '...'`." |
| 12 | "Tại sao không dùng EPPlus?" | "EPPlus từ v5 yêu cầu license thương mại. ClosedXML miễn phí hoàn toàn (MIT License), API tương tự, cộng đồng lớn." |
| 13 | "DocSoTien hoạt động sao?" | "Chia nhóm 3 chữ số → đọc từng nhóm (tỷ/triệu/nghìn/đơn vị) bằng DocBaChuSo(). Xử lý tiếng Việt: 1→'mốt', 5→'lăm', chục 0→'lẻ'." |
| 14 | "Log ghi ở đâu?" | "2 nơi song song: file `.log` (theo ngày, luôn hoạt động) VÀ bảng ErrorLog (SQL Server, best-effort)." |
| 15 | "Connection Wizard hoạt động sao?" | "4 bước: (1) nhập Server+Port, (2) nhập User+Pass+DB, (3) kiểm tra tự động TCP→Auth→DB→Schema, (4) lưu vào App.config." |

### ❓ Bảo mật

| # | Câu hỏi | Câu trả lời |
|---|---|---|
| 16 | "SQL Injection?" | "Dùng SqlParameter tham số hóa hoàn toàn. VD: `cmd.Parameters.AddWithValue('@ten', nv.HoTen)`. SQL Server tự escape." |
| 17 | "Mật khẩu lưu thế nào?" | "DB lưu SHA-256 hash (64 ký tự hex). Khi đăng nhập, hash mật khẩu nhập vào rồi so sánh với DB. 'Ghi nhớ đăng nhập' dùng DPAPI — chỉ giải mã trên cùng tài khoản Windows." |
| 18 | "DPAPI là gì?" | "Data Protection API — API bảo mật tích hợp sẵn trong Windows. Mỗi tài khoản Windows có key riêng, file mã hóa chỉ giải mã trên đúng tài khoản đó." |
| 19 | "Nếu app crash thì sao?" | "GlobalExceptionHandler bắt tất cả exception, ghi log, hiện thông báo tiếng Việt thân thiện. App không crash đột ngột." |

### ❓ Giao diện

| # | Câu hỏi | Câu trả lời |
|---|---|---|
| 20 | "Custom control nào?" | "3 control tự thiết kế: RoundedButton (nút bo tròn có hover effect), GlassPanel (panel hiệu ứng kính mờ), DashboardCard (thẻ thống kê)." |
| 21 | "Glassmorphism là gì?" | "Hiệu ứng kính mờ — panel có nền bán trong suốt (`GlassBg = rgba(24,24,37,0.7)`), viền sáng nhẹ. Rất phổ biến trong thiết kế UI hiện đại." |
| 22 | "Font chữ?" | "Segoe UI — font mặc định của Windows 10/11, hỗ trợ đầy đủ tiếng Việt. Có fallback cho hệ thống cũ." |
| 23 | "Icon lấy từ đâu?" | "FontAwesome.Sharp — thư viện NuGet cung cấp hàng nghìn icon vector. Dùng enum `IconChar.Home`, `IconChar.Users`,..." |

### ❓ Database nâng cao

| # | Câu hỏi | Câu trả lời |
|---|---|---|
| 24 | "Script SQL có an toàn khi chạy lại?" | "Tất cả script đều idempotent — dùng `IF NOT EXISTS` trước CREATE. Chạy bao nhiêu lần cũng không lỗi." |
| 25 | "Có stored procedure không?" | "Có 7 SP trong 002_ExpandSchema.sql: tính lương tháng, báo cáo chấm công, tổng hợp thưởng/phạt, xem lịch, thống kê, đọc/dọn log." |
| 26 | "Backup DB thế nào?" | "Có script PowerShell `Deploy/Backup-Database.ps1` — tạo backup nén `.bak`, dọn backup cũ, dọn ErrorLog. Có thể đặt Task Scheduler chạy tự động hàng ngày." |

### ❓ NuGet & Dependencies

| # | Câu hỏi | Câu trả lời |
|---|---|---|
| 27 | "NuGet package nào?" | "2 packages: **ClosedXML** v0.105.0 (xuất Excel) và **FontAwesome.Sharp** v6.6.0 (icon). Cả 2 đều MIT License, mã nguồn mở." |
| 28 | "Nếu máy khách không có NuGet?" | "Các DLL đã nằm trong thư mục bin/Release. Chỉ cần copy cả thư mục, không cần cài NuGet." |

---

## PHẦN 7: XỬ LÝ TÌNH HUỐNG KHÓ

### 😰 Khi bị hỏi câu ngoài phạm vi chuẩn bị — Dùng 1 trong 3 phương án sau:

**Phương án 1 — "Đóng gói":**
> _"Phần này em đã đóng gói vào lớp Infrastructure để tập trung phần logic nghiệp vụ chính. Nó hoạt động ổn định mà không ảnh hưởng các module khác."_

**Phương án 2 — "Tái sử dụng":**
> _"Em thiết kế phần này theo nguyên lý tái sử dụng — các phương thức được định nghĩa một lần và gọi lại nhiều nơi, giảm trùng lặp code."_

**Phương án 3 — "Mở rộng":**
> _"Phần này em để dạng extensible — cấu trúc sẵn rồi, nếu cần mở rộng chỉ cần thêm method mới mà không sửa code cũ."_

### 😰 "Tại sao không dùng Entity Framework?"
> _"Em chọn ADO.NET thuần để kiểm soát hoàn toàn câu SQL, tối ưu hiệu năng, dễ debug. Với quy mô đồ án này, ADO.NET minh bạch hơn — em thấy rõ từng câu SELECT, INSERT, UPDATE."_

### 😰 "SHA-256 có đủ an toàn không?"
> _"SHA-256 là hàm hash một chiều — không thể giải mã ngược. Tuy nhiên, trong production nên dùng BCrypt hoặc PBKDF2 vì có tính năng 'salt' tự động và điều chỉnh được độ chậm (cost factor) để chống brute-force. Đây là hướng cải thiện tiếp theo."_

### 😰 "Hạn chế của đồ án?"
> _"Em nhận thấy 2 hạn chế: (1) Chưa phân quyền theo vai trò — NV chỉ xem lương mình, admin xem tất cả. (2) Thuế TNCN chưa tính lũy tiến theo bậc. Đây là hướng phát triển tiếp theo."_

### 😰 "Nếu 2 người dùng cùng sửa 1 bản ghi thì sao?"
> _"Hiện tại ứng dụng chưa xử lý concurrency conflict (đồ án 1 user). Nếu mở rộng, em sẽ thêm cột `RowVersion` (timestamp) làm optimistic locking — khi UPDATE sẽ kiểm tra version, nếu đã bị thay đổi bởi người khác thì báo lỗi."_

### 😰 "Tại sao không dùng Web thay vì WinForms?"
> _"WinForms phù hợp cho ứng dụng nội bộ nhà hàng — chạy trên LAN, không cần internet, khởi động nhanh, giao diện native Windows. Web app phù hợp khi cần truy cập từ xa hoặc mobile."_

---

## 📋 CHECKLIST TRƯỚC NGÀY BẢO VỆ

- [ ] Đọc Phần 3 (Luồng dữ liệu) **ít nhất 3 lần**
- [ ] Thuộc 5 luồng chính: Khởi động → Đăng nhập → Thêm NV → Xóa NV → Tính lương
- [ ] Biết mở 6 file demo (Phần 5) khi được yêu cầu
- [ ] Thuộc công thức tính lương: `LCB ÷ 26 × NgàyCông`, BHXH = `LCB × 10.5%`
- [ ] Biết giải thích: ServiceResult, AppLogger, GlobalExceptionHandler
- [ ] Biết giải thích: ExcelExporter, DocSoTien, ClosedXML
- [ ] Biết trả lời: SQL Injection, DPAPI, Upsert, Business Rule xóa NV
- [ ] Chuẩn bị 3 phương án trả lời khi gặp câu ngoài dự kiến (Phần 7)
- [ ] Chạy thử app + xuất thử file Excel trước ngày bảo vệ

---

> 🎓 _**Nhớ: Tự tin là 50% điểm. Nói rõ ràng, nhìn thẳng vào thầy/cô, chỉ đúng dòng code. Chúc bảo vệ thành công!**_
