# KẾ HOẠCH TRIỂN KHAI ĐỒ ÁN MÔN HỌC

**Tên đề tài:** Hệ thống quản lý nhân viên cho quán ăn

---

## 1. TỔNG QUAN DỰ ÁN (Project Overview)

| Hạng mục               | Chi tiết                                   |
| :--------------------- | :----------------------------------------- |
| **Loại ứng dụng**      | Desktop Application (Windows Forms)        |
| **Công nghệ**          | WinForms — C#                              |
| **Ngôn ngữ**           | C# (.NET Framework)                        |
| **Cơ sở dữ liệu**      | Microsoft SQL Server                       |
| **Công cụ phát triển** | Visual Studio                              |
| **Mục đích**           | Quản lý nhân viên & tính lương cho quán ăn |

> [!TIP]
> **Câu nói tóm tắt khi thầy hỏi:**
> _"Đồ án của em là ứng dụng WinForms viết bằng C#, sử dụng SQL Server để quản lý nhân viên và tính lương cho quán ăn."_

---

## 2. CÁC FORM TRONG HỆ THỐNG (6 Form)

### 2.1. Form bắt buộc (4 form — đủ điểm)

| #   | Form                       | Mô tả                     | Chức năng chính                                                           |
| :-- | :------------------------- | :------------------------ | :------------------------------------------------------------------------ |
| 1   | **Form Login**             | Tài khoản quản lý (Admin) | Bảo mật quyền truy cập vào phần mềm                                       |
| 2   | **Form Quản lý Nhân viên** | Hồ sơ nhân sự             | Thêm / Sửa / Xoá / Xem nhân viên                                          |
| 3   | **Form Bộ phận – Chức vụ** | Quản lý bộ phận           | Thêm / sửa / xoá bộ phận (bếp, phục vụ, HHS...)                           |
| 4   | **Form Bảng lương tháng**  | Tính lương                | Chọn nhân viên → Chọn tháng/năm → Nhập công, ứng lương → Bấm _Tính lương_ |

> [!IMPORTANT]
> **Chỉ cần 4 form đầu là đủ điểm.**

### 2.2. Form nâng cao (không bắt buộc — đẹp bài)

| #   | Form                               | Mô tả                                        |
| :-- | :--------------------------------- | :------------------------------------------- |
| 5   | **Form Xem phiếu lương chi tiết**  | Hiển thị chi tiết phiếu lương từng nhân viên |
| 6   | **Form Thống kê lương theo tháng** | Thống kê tổng hợp lương đã chi trả           |

> [!NOTE]
> 6 form là **rất đẹp** cho đồ án 2, không thừa không thiếu.

### 2.3. Form Trang chủ (Main Menu)

- Giao diện tổng quan, menu điều hướng đến các chức năng quản lý.

---

## 3. THIẾT KẾ CƠ SỞ DỮ LIỆU — 4 Bảng SQL

> [!NOTE]
>
> - **KHÔNG** cần bảng chấm công chi tiết — dữ liệu công được nhập từ máy chấm công bên ngoài.
> - Các dòng chi tiết (100%, 150%, thưởng...) **KHÔNG** cần tách bảng, có thể để = 0 hoặc ghi chú nghiệp vụ trong báo cáo.

### 3.1. Bảng `TaiKhoan` — Tài khoản quản lý (Admin)

| Cột           | Mô tả             |
| :------------ | :---------------- |
| `MaTK`        | Mã tài khoản (PK) |
| `TenDangNhap` | Tên đăng nhập     |
| `MatKhau`     | Mật khẩu          |
| `VaiTro`      | Vai trò — Admin   |

### 3.2. Bảng `BoPhan` — Bộ phận / Chức vụ

| Cột         | Mô tả                                           |
| :---------- | :---------------------------------------------- |
| `MaBoPhan`  | Mã bộ phận (PK)                                 |
| `TenBoPhan` | Tên bộ phận (Bếp, Phục vụ, Thu ngân, Bảo vệ...) |

### 3.3. Bảng `NhanVien` — Thông tin nhân viên

| Cột          | Mô tả                                      |
| :----------- | :----------------------------------------- |
| `MaNV`       | Mã nhân viên (PK)                          |
| `HoTen`      | Họ và tên                                  |
| `ChucVu`     | Chức vụ                                    |
| `MaBoPhan`   | Mã bộ phận (FK → BoPhan)                   |
| `LuongCoBan` | Mức lương cơ bản                           |
| `TrangThai`  | Trạng thái làm việc (đang làm / nghỉ việc) |

### 3.4. Bảng `BangLuong` — Lương theo tháng (dữ liệu tổng hợp)

| Cột              | Mô tả                                     |
| :--------------- | :---------------------------------------- |
| `MaBangLuong`    | Mã bảng lương (PK)                        |
| `MaNV`           | Mã nhân viên (FK → NhanVien)              |
| `Thang`          | Tháng                                     |
| `Nam`            | Năm                                       |
| `NgayCongThucTe` | Số ngày công làm việc thực tế trong tháng |
| `LuongTheoCong`  | Lương tính theo ngày/giờ công             |
| `TienUng`        | Tiền tạm ứng                              |
| `BHXH`           | Bảo hiểm xã hội (10.5%)                   |
| `Thue`           | Thuế TNCN                                 |
| `TongThucNhan`   | Tổng thu nhập thực nhận                   |

---

## 4. MẪU PHIẾU LƯƠNG THAM KHẢO

Dựa trên phiếu lương mẫu thực tế đã cung cấp, cấu trúc phiếu lương bao gồm:

### 4.1. Ngày công – Giờ công

| #   | Hạng mục                                              | Ví dụ     |
| :-- | :---------------------------------------------------- | :-------- |
| 1   | Lương cơ bản                                          | 6.250.000 |
| 2   | Ngày/giờ công làm việc thực tế trong tháng            | 23,5      |
| 3   | Ngày/giờ công được tính lương                         | 23,5      |
| 4   | Tổng ngày nghỉ có hưởng lương (bao gồm 2 ngày thưởng) | 0         |
| 5   | Tổng ngày nghỉ không hưởng lương                      | —         |
| 6   | Giờ làm thêm 100%                                     | 0         |
| 7   | Giờ làm thêm 150%                                     | 0         |
| 8   | Giờ làm thêm 200%                                     | 0         |
| 9   | Giờ làm thêm 300%                                     | 0         |
| 10  | Giờ làm đêm %                                         | 0         |

### 4.2. Các khoản thu nhập trong tháng

| #   | Hạng mục                                     |
| :-- | :------------------------------------------- |
| 1   | Lương tính theo ngày/giờ công                |
| 2   | Lương ngoài giờ (giờ 100%, 150%, 200%, 300%) |
| 3   | Phụ cấp trách nhiệm                          |
| 4   | Phụ cấp cơm                                  |
| 5   | Phụ cấp xăng                                 |
| 6   | Phụ cấp điện thoại                           |
| 7   | Thưởng                                       |
| —   | **Tổng thu nhập trong tháng**                |

### 4.3. Các khoản trừ

| #   | Hạng mục                    |
| :-- | :-------------------------- |
| 1   | Tiền ứng                    |
| 2   | BHXH 10.5%                  |
| 3   | Thuế TNCN                   |
| 4   | Các khoản giảm trừ khác     |
| —   | **TỔNG THU NHẬP THỰC NHẬN** |

> [!NOTE]
> **Công thức tính:**
> `Tổng Thực Nhận = Tổng Thu Nhập Trong Tháng - Tiền Ứng - BHXH - Thuế TNCN - Khoản giảm trừ khác`

---

## 5. HẠNG MỤC BÁO CÁO (Word Documentation)

Soạn thảo tài liệu thuyết minh đồ án hoàn chỉnh (khoảng 20–30 trang), bao gồm:

1. **Chương 1: Mở đầu** — Lý do chọn đề tài "Hệ thống quản lý nhân viên cho quán ăn".
2. **Chương 2: Phân tích & Thiết kế**
   - Sơ đồ Use Case (Chức năng).
   - Sơ đồ ERD (Cơ sở dữ liệu).
   - Mô tả chi tiết các bảng dữ liệu.
3. **Chương 3: Xây dựng chương trình**
   - Hình ảnh giao diện các Form thực tế.
   - Mô tả thuật toán / code xử lý tính lương.
4. **Chương 4: Kết luận & Hướng phát triển.**

---

## 6. LỘ TRÌNH THỰC HIỆN (Timeline)

| Giai đoạn       | Công việc                                                                       | Kết quả               |
| :-------------- | :------------------------------------------------------------------------------ | :-------------------- |
| **Giai đoạn 1** | Khởi tạo Project & CSDL SQL Server. Thiết kế giao diện (GUI) 6 Form.            | Khung chương trình    |
| **Giai đoạn 2** | Lập trình chức năng thêm/sửa/xóa. Lập trình logic tính lương & kết nối dữ liệu. | Phần mềm chạy ổn định |
| **Giai đoạn 3** | Viết báo cáo Word chi tiết. Kiểm tra lỗi & Đóng gói sản phẩm.                   | Hoàn thiện bàn giao   |

---

## 7. SẢN PHẨM BÀN GIAO (Deliverables)

1. **Source Code:** Trọn bộ mã nguồn C# (WinForms).
2. **Database:** File Script SQL (kèm dữ liệu mẫu nhập sẵn để test).
3. **Báo cáo:** File Word thuyết minh đồ án chuẩn chỉnh.
4. **Hỗ trợ:** Hướng dẫn cài đặt và giải thích code (nếu cần).

---

## 8. BẢNG TRA CỨU TỪ KHÓA SKILL (Skill Keyword Reference)

Dưới đây là danh sách các Skill đã được cài đặt và các từ khóa trigger để AC gọi hỗ trợ trong quá trình phát triển dự án này:

| Skill Name                    | Keywords / Triggers                                          | Purpose / When to use                                        |
| :---------------------------- | :----------------------------------------------------------- | :----------------------------------------------------------- |
| **architecture-patterns**     | architecture patterns, Clean Architecture, Hexagonal, DDD    | Designing system structure, defining boundaries and modules. |
| **backend-dev-guidelines**    | backend development, API doctrines, layering, BaseController | Implementation guidelines for robust backend services.       |
| **clean-code**                | clean code, refactor, naming conventions, SOLID              | Ensuring code quality, readability, and maintainability.     |
| **csharp-pro**                | csharp pro, modern C#, async/await, LINQ optimization        | Advanced C# features, enterprise patterns, performance.      |
| **database-admin**            | database admin, HA/DR, security, monitoring                  | Database reliability, automation, and backup strategies.     |
| **database-architect**        | database architect, scalability, modeling, query design      | High-level data architecture and scalability planning.       |
| **database-design**           | database design, schema, indexing, ORM selection             | Designing schemas, indexing strategies, and ORM choices.     |
| **database-migration**        | database migration, schema transformation, rollback          | Safe schema changes, data migrations, and zero-downtime.     |
| **documentation-templates**   | documentation templates, README, API docs, ADR               | Standardized templates for project documentation.            |
| **dotnet-architect**          | dotnet architect, ASP.NET Core, EF Core, Dapper              | production-grade .NET APIs and architecture decisions.       |
| **dotnet-backend**            | dotnet backend, Web API, Minimal APIs, Identity              | Building .NET backend services and authentication.           |
| **dotnet-backend-patterns**   | dotnet backend patterns, DI, Redis, xUnit                    | Master patterns for robust .NET backend applications.        |
| **error-handling-patterns**   | error handling, Result types, circuit breaker, retry         | Implementation of resilient error handling strategies.       |
| **plan-writing**              | plan writing, task-slug, verification, multi-step work       | Structured planning for features, refactoring, or bug fixes. |
| **readme**                    | write readme, create readme, project documentation           | Creating absurdly thorough project documentation (README).   |
| **software-architecture**     | software architecture, library-first, NIH syndrome           | Quality-focused architecture using Clean/DDD principles.     |
| **sql-optimization-patterns** | sql optimization, EXPLAIN analysis, N+1 problem              | Improving query performance and indexing strategies.         |
| **sql-pro**                   | sql pro, complex window functions, CTEs, tuning              | Advanced SQL, analytics, and performance tuning.             |
| **systematic-debugging**      | systematic debugging, root cause, reproduction               | Methodical approach to fixing bugs and unexpected behavior.  |
| **ui-skills**                 | ui skills, opinionated constraints, interface guide          | Constraints and patterns for building user interfaces.       |
| **ui-ux-designer**            | ui ux designer, design tokens, Figma, accessibility          | Interface design, wireframes, and design systems.            |
| **writing-skills**            | writing skills, new skill, update skill, skill template      | Creating, updating, or improving agent skills.               |

---
