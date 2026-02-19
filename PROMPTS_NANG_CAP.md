# PROJECT UPGRADE PROMPTS (DEV: LINUX | DEPLOY: WINDOWS)

This document contains optimized prompts to upgrade the "Employee Management System for Restaurants" project. These prompts use **Skill Keywords** to trigger advanced AI capabilities while respecting the dual-environment requirement (Linux for Dev, Windows for Clients).

---

## 1. 🏗️ ARCHITECTURE & REFACTORING

**Objective:** Improve code maintainability and cross-platform reliability.

**Prompt:**

> "Apply **software architecture** principles to decouple the business logic from the UI forms by implementing a Service Layer. Use **csharp pro** to ensure all database operations are thread-safe and utilize modern C# features compatible with both Mono (Linux) and .NET Framework 4.7.2 (Windows). Ensure that the `AppFonts` utility is strictly used for all UI elements to maintain visual consistency across both environments."

---

## 2. 🎨 UI/UX ENHANCEMENT

**Objective:** Create a premium "wow" factor for the client while ensuring performance on Linux.

**Prompt:**

> "Use **ui-ux-designer** and **ui skills** to redesign the main dashboard. Implement a modern 'Glassmorphism' effect for the sidebar and use **custom control painting** to create sleek, rounded buttons with smooth hover transitions. Ensure the design feels native and premium on Windows 10/11 but remains fully functional and readable on Mono Linux."

---

## 3. 📊 DATABASE & BUSINESS LOGIC

**Objective:** Scale the system for real-world restaurant usage.

**Prompt:**

> "Utilize **database design** to expand the schema to include employee shift scheduling, attendance logs, and bonus/penalty management. Then, use **sql pro** to write optimized stored procedures for monthly payroll generation. These scripts must be tested on the Docker SQL Server environment but designed for standard SQL Server 2019+ deployment at the client's site."

---

## Nâng cấp giao diện SPA & Sidebar

**Mục tiêu:** Chuyển đổi sang cơ chế Form-in-Form và Menu thu gọn.

**Prompt (Vietnamese):**

```markdown
Sử dụng skill ui-ux-designer và csharp-pro để tái cấu trúc FormMain.

1. Thêm Panel Content trung tâm để chứa các Form con (Nested Forms).
2. Xây dựng hàm OpenChildForm(Form childForm) để nhúng Form con vào Panel (TopLevel=false).
3. Thiết kế Menu SideBar có khả năng thu gọn (Collapse) chỉ còn hiện Icon.
4. Sử dụng TableLayoutPanel để đảm bảo giao diện Responsive khi thay đổi kích thước cửa sổ.
```

**Prompt (English):**

```markdown
Using ui-ux-designer and csharp-pro skills, restructure FormMain:

1. Add a central Content Panel for Nested Forms.
2. Implement OpenChildForm(Form childForm) to embed sub-forms (TopLevel=false, Dock=Fill).
3. Design a Collapsible Sidebar that toggles between Icon+Text and Icon-only states.
4. Utilize TableLayoutPanel and Anchor/Dock properties to ensure a Responsive layout.
```

---

## Hệ thống Báo cáo & In ấn

---

## 4. 🛡️ RELIABILITY & ERROR HANDLING

**Objective:** Minimize support calls from clients by making the app resilient.

**Prompt:**

> "Implement **error-handling-patterns** throughout the application. Create a centralized exception handling mechanism that logs errors to both a local file and a 'Log' table in the database. Use **systematic debugging** logic to implement a 'Database Connection Wizard' that helps clients troubleshoot connection string issues (Server name, IP, Credentials) upon first run on Windows."

---

## 5. 📦 DEPLOYMENT & DOCUMENTATION

**Objective:** Smooth transition from Dev (Linux/Docker) to Production (Windows).

**Prompt:**

> "Use **readme** and **documentation-templates** to create a 'Deployment & Handover Guide'. Document the exact process of migrating from the development Docker environment to a native Windows Server. Include a PowerShell script for automatic SQL Server database initialization and a guide for bundling the application into a professional Windows Installer (.msi/.exe)."

---

## 💡 HOW TO USE

Copy and paste these prompts directly into the chat when you are ready to start a specific upgrade phase. The assistant will recognize the **bolded keywords** and apply specialized professional standards to the task.
