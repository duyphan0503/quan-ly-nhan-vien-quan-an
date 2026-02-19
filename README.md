# Quản Lý Nhân Viên Quán Ăn (Restaurant Employee Management)

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-red)
![Status](https://img.shields.io/badge/Status-Active-green)
![License](https://img.shields.io/badge/License-MIT-blue)

## 📖 Giới Thiệu

**Hệ thống quản lý nhân viên cho quán ăn** là một ứng dụng WinForms mạnh mẽ được xây dựng trên nền tảng .NET Framework 4.7.2, được thiết kế để tối ưu hóa quy trình quản lý nhân sự tại các nhà hàng, quán ăn.

Dự án này tập trung vào tính ổn định, giao diện hiện đại (với các thành phần UI tùy chỉnh như Glassmorphism, Rounded Buttons) và khả năng triển khai dễ dàng thông qua các công cụ tự động hóa.

## ✨ Tính Năng Chính

- **Quản Lý Nhân Viên:** Thêm, sửa, xóa, tra cứu thông tin nhân viên chi tiết.
- **Quản Lý Bộ Phận:** Tổ chức nhân sự theo bộ phận (Bếp, Phục vụ, Thu ngân...).
- **Tính Lương & Chấm Công:** Hệ thống bảng lương, ca làm việc và thưởng phạt.
- **Báo Cáo & Thống Kê:** Trực quan hóa dữ liệu qua Dashboard và các báo cáo thống kê.
- **Hệ Thống Đăng Nhập An Toàn:** Quy trình xác thực bảo mật.
- **Connection Wizard Thông Minh:** Tự động chẩn đoán và cấu hình kết nối cơ sở dữ liệu, giảm thiểu rủi ro triển khai.
- **Giao Diện Hiện Đại:** Sử dụng bộ màu Catppuccin Mocha và các control tùy biến (GlassPanel, RoundedButton) mang lại trải nghiệm người dùng tốt nhất.
- **Logging Hệ Thống:** Ghi lại lỗi và hoạt động quan trọng vào cả File và Database.

## 🛠️ Công Nghệ Sử Dụng

- **Ngôn ngữ:** C#
- **Framework:** .NET Framework 4.7.2
- **Giao diện:** Windows Forms (WinForms) Custom UI
- **Cơ sở dữ liệu:** Microsoft SQL Server 2019+
- **Kiến trúc:** Layered Architecture (Presentation layer, Service Layer, Data Access Layer, Infrastructure).

## 🚀 Cài Đặt & Triển Khai

Để triển khai hệ thống, vui lòng tham khảo tài liệu chi tiết:

👉 **[HƯỚNG DẪN TRIỂN KHAI CHI TIẾT](HUONG_DAN_TRIEN_KHAI.md)**

### Tóm tắt nhanh:

1.  **Yêu cầu hệ thống:**
    - Windows 10/11 hoặc Windows Server 2019+.
    - .NET Framework 4.7.2 Runtime.
    - SQL Server 2019 Express trở lên.

2.  **Cài đặt Cơ sở dữ liệu:**
    - Sử dụng script PowerShell trong thư mục `Deploy/` hoặc chạy script SQL thủ công trong thư mục `SQL/`.
    - Chạy theo thứ tự: `CreateDatabase.sql` -> `002_ExpandSchema.sql` -> `003_ErrorLog.sql`.

3.  **Cấu hình kết nối:**
    - Mở ứng dụng lần đầu, **Connection Wizard** sẽ tự động xuất hiện nếu không kết nối được DB.
    - Nhập thông tin Server, User, Password để ứng dụng tự động cấu hình.

## 📂 Cấu Trúc Dự Án

```
QuanLyNhanVien/
├── Forms/                 # Giao diện người dùng (Main, Login, Dashboard...)
├── Services/              # Xử lý nghiệp vụ logic
├── DataAccess/            # Tương tác trực tiếp với Database
├── Models/                # Các thực thể dữ liệu (Entity)
├── Infrastructure/        # Các thành phần dùng chung (Logger, ExceptionHandler...)
├── Controls/              # Các Custom User Controls (Button, Panel...)
└── SQL/                   # Script khởi tạo cơ sở dữ liệu
```

## 📸 Hình Ảnh Demo

_(Thêm hình ảnh chụp màn hình ứng dụng tại đây)_

## 🤝 Đóng Góp

Mọi sự đóng góp đều được hoan nghênh. Vui lòng tạo Pull Request hoặc gửi Issue nếu bạn tìm thấy lỗi.

## 📝 Giấy Phép

Dự án này được phân phối dưới giấy phép **MIT**. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

© 2026 - Dự án Quản Lý Nhân Viên Quán Ăn.
