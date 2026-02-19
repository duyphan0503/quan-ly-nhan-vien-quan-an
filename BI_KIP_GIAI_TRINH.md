# 📔 BÍ KÍP GIẢI TRÌNH ĐỒ ÁN: QUẢN LÝ NHÂN VIÊN QUÁN ĂN

> **Đọc kỹ trước khi bảo vệ. Học thuộc lòng Luồng Dữ Liệu ở Phần 3.**

---

## PHẦN 1: BẢN ĐỒ DỰ ÁN (Thư mục nào dùng để làm gì?)

```
QuanLyNhanVien/
├── 📁 Models/          → "Bản vẽ" các bảng trong Database
├── 📁 DataAccess/      → Nơi DUY NHẤT chứa code SQL
├── 📁 Services/        → "Bộ não" - kiểm tra logic, tính toán
├── 📁 Infrastructure/  → Công cụ hỗ trợ: Logging, Bảo mật, Lỗi
├── 📁 Forms/           → Màn hình giao diện người dùng
├── 📁 Controls/        → Các nút bấm, panel tùy chỉnh đẹp hơn
├── 📁 SQL/             → Script tạo Database từ đầu
├── AppColors.cs        → Định nghĩa bảng màu toàn ứng dụng
└── Program.cs          → Điểm khởi động của ứng dụng
```

**Câu nói "thần chú":** _"Em chia theo mô hình 3 lớp: Giao diện (Forms) — Nghiệp vụ (Services) — Dữ liệu (DataAccess) để tách biệt trách nhiệm, dễ bảo trì và mở rộng."_

---

## PHẦN 2: CÁC BẢNG TRONG DATABASE

> File: `QuanLyNhanVien/SQL/CreateDatabase.sql`

| Bảng        | Các cột chính                                                                                        | Ý nghĩa                         |
| ----------- | ---------------------------------------------------------------------------------------------------- | ------------------------------- |
| `TaiKhoan`  | `MaTK`, `TenDangNhap`, `MatKhau`, `VaiTro`                                                           | Lưu tài khoản đăng nhập (admin) |
| `BoPhan`    | `MaBoPhan`, `TenBoPhan`                                                                              | Bếp, Phục vụ, Thu ngân...       |
| `NhanVien`  | `MaNV`, `HoTen`, `ChucVu`, `MaBoPhan`, `LuongCoBan`, `TrangThai`                                     | Thông tin nhân viên             |
| `BangLuong` | `MaNV`, `Thang`, `Nam`, `NgayCongThucTe`, `LuongTheoCong`, `TienUng`, `BHXH`, `Thue`, `TongThucNhan` | Bảng lương theo tháng           |
| `ErrorLog`  | `MucDo`, `NguonLoi`, `ThongBao`, `NguoiDung`, `TenMay`                                               | Ghi lại lỗi hệ thống            |

**Quan hệ giữa các bảng:**

```
TaiKhoan (đăng nhập)
NhanVien → BoPhan  (một nhân viên thuộc một bộ phận)
BangLuong → NhanVien  (một nhân viên có nhiều bảng lương theo tháng)
AppLogger → ErrorLog  (ghi log lỗi vào DB)
```

**Tài khoản mặc định để đăng nhập:** `admin / admin123`

---

## PHẦN 3: LUỒNG DỮ LIỆU (QUAN TRỌNG NHẤT - ĐẶT CÂU HỎI Ở ĐÂY)

### 3.1. Khi ứng dụng khởi động (File: `Program.cs`)

```
App bật lên
    ↓
GlobalExceptionHandler.Install()  → Cài bộ lọc lỗi toàn cục
    ↓
DatabaseHelper.TestConnection()   → Thử kết nối SQL Server
    ↓ Thất bại
FormConnectionWizard.ShowDialog() → Cho người dùng nhập thông tin kết nối
    ↓ Thành công
DatabaseHelper.RefreshConnectionString() → Nạp lại cấu hình
    ↓
FormLogin.Show()                  → Hiển thị màn hình đăng nhập
```

### 3.2. Luồng ĐĂNG NHẬP

**Người dùng nhập tên, mật khẩu → Bấm nút Đăng Nhập**

```
FormLogin.cs (btnDangNhap_Click)
    ↓ gọi
TaiKhoanService.DangNhap(tenDangNhap, matKhau)
    ↓ kiểm tra: tên có rỗng không? mật khẩu có rỗng không?
    ↓ nếu OK, gọi
TaiKhoanDAL.DangNhap(tenDangNhap, matKhau)
    ↓ chạy SQL: SELECT * FROM TaiKhoan WHERE TenDangNhap=@td AND MatKhau=@mk
    ↓ trả về object TaiKhoan (hoặc null nếu sai)
    ↓ về đến Service
ServiceResult<TaiKhoan>.Ok(tk)  hoặc  ServiceResult.Fail("Sai tên/mật khẩu")
    ↓ về đến Form
→ Nếu thành công: đóng FormLogin, mở FormMain(currentUser)
→ Nếu thất bại:  MessageBox "Sai tên đăng nhập hoặc mật khẩu"
```

> **Tính năng bonus:** Nếu người dùng tích "Ghi nhớ đăng nhập", mật khẩu sẽ được mã hóa bằng `SecurityHelper.Encrypt()` (dùng Windows DPAPI) và lưu vào file `login.cfg`.

### 3.3. Luồng THÊM NHÂN VIÊN

**Người dùng điền form → Bấm nút Lưu**

```
FormNhanVien.cs (btnLuu_Click)
    ↓ đọc các TextBox, tạo object:
    NhanVien nv = { HoTen="...", ChucVu="...", MaBoPhan=3, LuongCoBan=5000000, TrangThai="Đang làm" }
    ↓ gọi
NhanVienService.ThemNhanVien(nv)
    ↓ kiểm tra validation:
        - HoTen có rỗng không? → "Vui lòng nhập họ tên"
        - MaBoPhan có hợp lệ không? → "Vui lòng chọn bộ phận"
        - LuongCoBan có âm không? → "Lương cơ bản không được âm"
    ↓ nếu hợp lệ, gọi
NhanVienDAL.Them(nv)
    ↓ chạy SQL:
    INSERT INTO NhanVien (HoTen, ChucVu, MaBoPhan, LuongCoBan, TrangThai)
    VALUES (@ten, @cv, @bp, @luong, @tt)
    ↓ trả về true/false
    ↓ về đến Service → ServiceResult.Ok("Thêm nhân viên thành công.")
    ↓ về đến Form
→ MessageBox "Thêm nhân viên thành công." + tải lại danh sách
```

### 3.4. Luồng XÓA NHÂN VIÊN (Có Business Rule đặc biệt)

```
FormNhanVien.cs (btnXoa_Click)
    ↓
NhanVienService.XoaNhanVien(maNV)
    ↓ KIỂM TRA ĐẶC BIỆT:
    NhanVienDAL.CoLuong(maNV)
    → SQL: SELECT COUNT(*) FROM BangLuong WHERE MaNV = @id
    ↓ Nếu COUNT > 0 (đã có bảng lương)
→ ServiceResult.Fail("Không thể xóa nhân viên đã có bảng lương. Hãy chuyển trạng thái sang 'Nghỉ việc'.")
    ↓ Nếu OK (chưa có lương)
NhanVienDAL.Xoa(maNV)
    → SQL: DELETE FROM NhanVien WHERE MaNV = @id
```

> **Lý do:** Đây là quy tắc nghiệp vụ (business rule). Xóa nhân viên có lịch sử lương sẽ vi phạm ràng buộc `FOREIGN KEY` trong Database và gây lỗi. Thay vì cho phép xóa, hệ thống bắt người dùng đổi trạng thái sang "Nghỉ việc" để giữ lịch sử.

### 3.5. Luồng TÍNH LƯƠNG

```
FormBangLuong.cs → Chọn nhân viên, nhập ngày công thực tế, tiền ứng → Bấm "Tính"
    ↓ gọi
BangLuongService.TinhLuong(luongCoBan, ngayCong, tienUng)
    ↓ CÔNG THỨC TÍNH (không truy cập DB):
        LuongTheoCong = LuongCoBan / 26 * NgayCong       (26 = ngày công chuẩn)
        BHXH          = LuongCoBan * 0.105               (10.5% Bảo hiểm xã hội)
        Thue          = 0                                 (Thuế TNCN - chưa triển khai)
        TongThucNhan  = LuongTheoCong - TienUng - BHXH - Thue
    ↓ → Hiển thị kết quả trên Form
    ↓ Người dùng bấm "Lưu"
BangLuongService.LuuBangLuong(maNV, thang, nam, ...)
    ↓ gọi
BangLuongDAL.LuuBangLuong(bl)
    ↓ THÔNG MINH: Kiểm tra đã có bảng lương tháng này chưa?
        → Nếu chưa có: INSERT INTO BangLuong ...
        → Nếu đã có:   UPDATE BangLuong SET ... WHERE MaNV=@id AND Thang=@th AND Nam=@nam
```

---

## PHẦN 4: CÁC "VŨ KHÍ BÍ MẬT" GHI ĐIỂM TUYỆT ĐỐI

### 4.1. Connection Wizard (File: `FormConnectionWizard.cs`)

**Câu hỏi:** "Phần mềm của em chạy ra sao khi cài lên máy mới không có database?"
**Trả lời:** _"Khi khởi động, `DatabaseHelper.TestConnection()` sẽ thử kết nối. Nếu thất bại, hệ thống tự động bật `FormConnectionWizard` để người dùng cấu hình địa chỉ máy chủ SQL. Sau khi lưu, `DatabaseHelper.RefreshConnectionString()` nạp lại cấu hình mà không cần khởi động lại."_

### 4.2. Hệ thống Logging (File: `Infrastructure/AppLogger.cs`)

**Câu hỏi:** "Làm sao em biết phần mềm bị lỗi ở đâu sau khi giao cho khách hàng?"
**Trả lời:** _"Em triển khai `AppLogger` - ghi log theo 2 nơi song song: vào file `.log` trong thư mục cài đặt (xoay vòng theo ngày, ví dụ `2026-02-19.log`) VÀ vào bảng `ErrorLog` trong Database. Mỗi entry chứa: Thời gian, Mức độ (Info/Warning/Error/Critical), Nguồn lỗi, Tên người dùng, và Tên máy tính."_

**Ví dụ có thể đọc từ log:**

```
[2026-02-19 10:30:15.123] [ERROR] [NhanVienService.ThemNhanVien] [User:admin]
  Message: Không thể thêm nhân viên. Vui lòng thử lại.
```

### 4.3. Xử lý lỗi toàn cục (File: `Infrastructure/GlobalExceptionHandler.cs`)

**Câu hỏi:** "Nếu có lỗi bất ngờ thì ứng dụng sẽ thế nào?"
**Trả lời:** _"Em dùng `GlobalExceptionHandler.Install()` ngay khi ứng dụng bật để bắt TẤT CẢ các lỗi chưa được xử lý. Hệ thống phân loại lỗi theo kiểu: lỗi kết nối SQL Server, lỗi quyền truy cập, lỗi bộ nhớ... và hiển thị thông báo bằng tiếng Việt thân thiện thay vì để chương trình văng ngay."_

**Ví dụ phân loại lỗi SQL (mã lỗi):**

- Mã `18456` → Sai tên đăng nhập SQL Server
- Mã `4060` → Database không tồn tại
- Mã `547` → Vi phạm ràng buộc khóa ngoại (Foreign Key)
- Mã `2627` → Dữ liệu bị trùng lặp (Unique)

### 4.4. ServiceResult - Kết quả trả về tiêu chuẩn (File: `Services/ServiceResult.cs`)

**Câu hỏi:** "Sao em không chỉ dùng true/false để kiểm tra thành công hay thất bại?"
**Trả lời:** _"Chỉ dùng true/false thì Form không biết tại sao lại thất bại. Em dùng `ServiceResult` để đóng gói cả kết quả (thành công/thất bại) lẫn thông điệp giải thích chi tiết. Ví dụ: thay vì trả về `false`, Service trả về `ServiceResult.Fail('Không thể xóa nhân viên đã có bảng lương.')` để Form hiển thị đúng nội dung lỗi."_

### 4.5. Bảo mật SQL Injection (Trong mọi file DAL)

**Câu hỏi:** "Ứng dụng có bị dính lỗ hổng SQL Injection không?"
**Trả lời:** _"Không. Em dùng `SqlParameter` (tham số hóa) cho mọi câu lệnh SQL. Ví dụ trong `NhanVienDAL.Them()`:_

```csharp
cmd.Parameters.AddWithValue("@ten", nv.HoTen);
// → SQL Server tự xử lý ký tự đặc biệt, không thể tấn công
```

_Nếu ghép chuỗi thẳng như `"... WHERE HoTen='" + txtTen.Text + "'"` thì mới bị SQL Injection."_

### 4.6. Bảo mật mật khẩu "Ghi nhớ đăng nhập" (File: `Infrastructure/SecurityHelper.cs`)

**Câu hỏi:** "Chức năng 'Ghi nhớ đăng nhập' có an toàn không?"
**Trả lời:** _"Mật khẩu không lưu dạng thô (plain text). Em dùng `Windows DPAPI (Data Protection API)` qua `ProtectedData.Protect()` để mã hóa. File `login.cfg` chỉ đọc được trên đúng tài khoản Windows đó, tức là dù lấy được file cũng không giải mã được trên máy khác."_

### 4.7. Giao diện UI cao cấp (File: `AppColors.cs`)

**Câu hỏi:** "Màu sắc ứng dụng của em lấy từ đâu?"
**Trả lời:** _"Em sử dụng bảng màu **Catppuccin Mocha** - một design system mã nguồn mở nổi tiếng trong cộng đồng lập trình viên, được thiết kế để giảm mỏi mắt khi làm việc lâu. Toàn bộ màu sắc được định nghĩa tập trung trong `AppColors.cs`, đảm bảo giao diện đồng nhất:_

- Nền tối: `#1E1E2E` (Base), `#181825` (Mantle)
- Màu nhấn: `#A6E3A1` (Xanh lá), `#89B4FA` (Xanh dương), `#F38BA8` (Đỏ)
- Hiệu ứng kính mờ: `GlassBg` với độ trong suốt (Alpha)\*

---

## PHẦN 5: BỘ CÂU HỎI "PHẢN XẠ NHANH" (Luyện đến nhớ như thuộc lòng)

### ❓ Kiến trúc

| Câu hỏi                        | Câu trả lời nhanh                                                                                                  |
| ------------------------------ | ------------------------------------------------------------------------------------------------------------------ |
| "Tại sao chia 3 lớp?"          | "Tách biệt trách nhiệm: Form lo hiển thị, Service lo logic, DAL lo database. Đổi DB chỉ sửa DAL, không đụng Form." |
| "Lớp Service để làm gì?"       | "Chứa rules nghiệp vụ: kiểm tra dữ liệu _trước_ khi lưu, tính toán lương, ngăn xóa nhân viên có lương."            |
| "Tại sao có `DatabaseHelper`?" | "Quản lý connection string tập trung. Connection Wizard chỉ cần gọi `RefreshConnectionString()` là cập nhật ngay." |

### ❓ Tính năng

| Câu hỏi                             | Câu trả lời nhanh                                                                                                                         |
| ----------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| "Công thức tính lương thế nào?"     | "Lương Theo Công = Lương Cơ Bản ÷ 26 × Ngày Công. BHXH = Lương Cơ Bản × 10.5%. Thực nhận = Lương Theo Công − Tiền Ứng − BHXH."            |
| "Tại sao không xóa được nhân viên?" | "Business rule: nhân viên đã có bảng lương thì không xóa vật lý. Phải chuyển TrangThai = 'Nghỉ việc'. Điều này bảo toàn lịch sử kế toán." |
| "Log ghi ở đâu?"                    | "Ghi 2 nơi song song: file `.log` trong thư mục Logs/ (theo ngày) VÀ bảng `ErrorLog` trong SQL Server."                                   |

### ❓ Bảo mật

| Câu hỏi                 | Câu trả lời nhanh                                                                             |
| ----------------------- | --------------------------------------------------------------------------------------------- |
| "SQL Injection?"        | "Dùng SqlParameter, tham số hóa hoàn toàn trong mọi file DAL."                                |
| "Mật khẩu lưu thế nào?" | "Plain text trong DB (bảo vệ cơ bản). Mật khẩu 'ghi nhớ đăng nhập' mã hóa DPAPI với Windows." |

---

## PHẦN 6: KHI BỊ YÊU CẦU "DEMO TRỰC TIẾP"

Nếu thầy cô yêu cầu: _"Em hãy chỉ cho tôi xem đoạn code tính lương"_

👉 **Mở file:** `QuanLyNhanVien/Services/BangLuongService.cs`
👉 **Chỉ vào hàm:** `TinhLuong()` ở dòng ~53
👉 **Giải thích:** "Đây là hàm thuần túy (pure function), không truy cập database, chỉ nhận input và trả về kết quả. Em muốn tách biệt logic tính toán ra khỏi thao tác lưu dữ liệu để dễ kiểm tra độc lập."

Nếu thầy cô hỏi: _"Khi lưu bảng lương, nếu đã tồn tại thì sao?"_

👉 **Mở file:** `QuanLyNhanVien/DataAccess/BangLuongDAL.cs`
👉 **Chỉ vào hàm:** `LuuBangLuong()` ở dòng ~101
👉 **Giải thích:** "Em kiểm tra trước bằng `SELECT COUNT(*)`. Nếu đã tồn tại thì `UPDATE`, nếu chưa thì `INSERT`. Gọi là `Upsert` pattern. Giúp người dùng không cần phân biệt là đang thêm mới hay cập nhật bảng lương của cùng 1 tháng."

---

## PHẦN 7: MẸO XỬ LÝ TÌNH HUỐNG KHÓ

**Khi bị hỏi câu không biết:**

> _"Phần này em đã đóng gói vào lớp `Infrastructure` / `Helper` để tập trung vào phần logic nghiệp vụ chính. Nó đảm bảo [tính năng X] hoạt động ổn định mà không ảnh hưởng đến các module khác."_

**Khi bị hỏi "Tại sao không dùng Entity Framework?":**

> _"Em chọn ADO.NET thuần để kiểm soát hoàn toàn các câu lệnh SQL, tối ưu hiệu năng cho hệ thống quản lý. Entity Framework phù hợp hơn cho các hệ thống lớn, còn với quy mô đồ án này, ADO.NET minh bạch và dễ debug hơn."_

**Khi bị hỏi "Hạn chế của đồ án là gì?":**

> _"Em nhận thấy hệ thống chưa có phân quyền theo vai trò (Role-based Access), ví dụ nhân viên bình thường chỉ xem được lương của mình còn admin mới xem được tất cả. Đây là hướng phát triển tiếp theo nếu có thêm thời gian."_

---

_© Chúc bảo vệ xuất sắc. Đọc đi đọc lại càng nhiều là tự tin bấy nhiêu._
