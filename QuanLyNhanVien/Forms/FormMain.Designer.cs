namespace QuanLyNhanVien.Forms
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblMainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSidebar = new QuanLyNhanVien.Controls.GlassPanel();
            this.btnToggle = new QuanLyNhanVien.Controls.RoundedButton();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblMenu = new System.Windows.Forms.Label();
            this.btnDashboard = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnNhanVien = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnBoPhan = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnBangLuong = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnThongKe = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnDangXuat = new QuanLyNhanVien.Controls.RoundedButton();
            this._pnlRightSide = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.tblMainLayout.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this._pnlRightSide.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMainLayout
            // 
            this.tblMainLayout.BackColor = System.Drawing.Color.Transparent;
            this.tblMainLayout.ColumnCount = 2;
            this.tblMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tblMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMainLayout.Controls.Add(this.pnlSidebar, 0, 0);
            this.tblMainLayout.Controls.Add(this._pnlRightSide, 1, 0);
            this.tblMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMainLayout.Location = new System.Drawing.Point(0, 0);
            this.tblMainLayout.Margin = new System.Windows.Forms.Padding(0);
            this.tblMainLayout.Name = "tblMainLayout";
            this.tblMainLayout.RowCount = 1;
            this.tblMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMainLayout.Size = new System.Drawing.Size(1200, 720);
            this.tblMainLayout.TabIndex = 0;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BorderRadius = 0;
            this.pnlSidebar.BorderSide = QuanLyNhanVien.Controls.GlassPanel.GlassBorderSide.None;
            this.pnlSidebar.Controls.Add(this.btnToggle);
            this.pnlSidebar.Controls.Add(this.pbLogo);
            this.pnlSidebar.Controls.Add(this.lblAppName);
            this.pnlSidebar.Controls.Add(this.lblWelcome);
            this.pnlSidebar.Controls.Add(this.lblMenu);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.btnNhanVien);
            this.pnlSidebar.Controls.Add(this.btnBoPhan);
            this.pnlSidebar.Controls.Add(this.btnBangLuong);
            this.pnlSidebar.Controls.Add(this.btnThongKe);
            this.pnlSidebar.Controls.Add(this.btnDangXuat);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSidebar.DrawGlassBorder = true;
            this.pnlSidebar.GlassBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.pnlSidebar.GradientBottom = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(27)))));
            this.pnlSidebar.GradientTop = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(35)))));
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(260, 720);
            this.pnlSidebar.TabIndex = 0;
            // 
            // btnToggle
            // 
            this.btnToggle.AccentColor = System.Drawing.Color.Empty;
            this.btnToggle.CornerRadius = 10;
            this.btnToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.btnToggle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnToggle.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnToggle.IdleColor = System.Drawing.Color.Transparent;
            this.btnToggle.Image = null;
            this.btnToggle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToggle.Location = new System.Drawing.Point(210, 10);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Padding = new System.Windows.Forms.Padding(20, 0, 10, 0);
            this.btnToggle.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(91)))), ((int)(((byte)(112)))));
            this.btnToggle.Size = new System.Drawing.Size(40, 40);
            this.btnToggle.TabIndex = 0;
            this.btnToggle.Text = "≡";
            this.btnToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnToggle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // pbLogo
            // 
            this.pbLogo.BackColor = System.Drawing.Color.Transparent;
            this.pbLogo.Location = new System.Drawing.Point(0, 30);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(260, 60);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 1;
            this.pbLogo.TabStop = false;
            // 
            // lblAppName
            // 
            this.lblAppName.BackColor = System.Drawing.Color.Transparent;
            this.lblAppName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.lblAppName.Location = new System.Drawing.Point(0, 90);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(260, 30);
            this.lblAppName.TabIndex = 2;
            this.lblAppName.Text = "QUÁN ĂN";
            this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWelcome
            // 
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.lblWelcome.Location = new System.Drawing.Point(0, 120);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(260, 20);
            this.lblWelcome.TabIndex = 3;
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMenu
            // 
            this.lblMenu.AutoSize = true;
            this.lblMenu.BackColor = System.Drawing.Color.Transparent;
            this.lblMenu.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblMenu.Location = new System.Drawing.Point(22, 160);
            this.lblMenu.Name = "lblMenu";
            this.lblMenu.Size = new System.Drawing.Size(41, 13);
            this.lblMenu.TabIndex = 4;
            this.lblMenu.Text = "MENU";
            // 
            // btnDashboard
            // 
            this.btnDashboard.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(166)))), ((int)(((byte)(247)))));
            this.btnDashboard.CornerRadius = 12;
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnDashboard.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(203)))), ((int)(((byte)(166)))), ((int)(((byte)(247)))));
            this.btnDashboard.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(203)))), ((int)(((byte)(166)))), ((int)(((byte)(247)))));
            this.btnDashboard.Image = null;
            this.btnDashboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.Location = new System.Drawing.Point(18, 190);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnDashboard.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(203)))), ((int)(((byte)(166)))), ((int)(((byte)(247)))));
            this.btnDashboard.Size = new System.Drawing.Size(224, 46);
            this.btnDashboard.TabIndex = 5;
            this.btnDashboard.Text = "   Trang Chủ";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // btnNhanVien
            // 
            this.btnNhanVien.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnNhanVien.CornerRadius = 12;
            this.btnNhanVien.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNhanVien.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNhanVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnNhanVien.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnNhanVien.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnNhanVien.Image = null;
            this.btnNhanVien.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhanVien.Location = new System.Drawing.Point(18, 248);
            this.btnNhanVien.Name = "btnNhanVien";
            this.btnNhanVien.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnNhanVien.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnNhanVien.Size = new System.Drawing.Size(224, 46);
            this.btnNhanVien.TabIndex = 6;
            this.btnNhanVien.Text = "   Nhân Viên";
            this.btnNhanVien.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNhanVien.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // btnBoPhan
            // 
            this.btnBoPhan.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnBoPhan.CornerRadius = 12;
            this.btnBoPhan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBoPhan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBoPhan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnBoPhan.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnBoPhan.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnBoPhan.Image = null;
            this.btnBoPhan.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBoPhan.Location = new System.Drawing.Point(18, 306);
            this.btnBoPhan.Name = "btnBoPhan";
            this.btnBoPhan.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnBoPhan.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnBoPhan.Size = new System.Drawing.Size(224, 46);
            this.btnBoPhan.TabIndex = 7;
            this.btnBoPhan.Text = "   Bộ Phận";
            this.btnBoPhan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBoPhan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // btnBangLuong
            // 
            this.btnBangLuong.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.btnBangLuong.CornerRadius = 12;
            this.btnBangLuong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBangLuong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnBangLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnBangLuong.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.btnBangLuong.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.btnBangLuong.Image = null;
            this.btnBangLuong.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBangLuong.Location = new System.Drawing.Point(18, 364);
            this.btnBangLuong.Name = "btnBangLuong";
            this.btnBangLuong.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnBangLuong.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.btnBangLuong.Size = new System.Drawing.Size(224, 46);
            this.btnBangLuong.TabIndex = 8;
            this.btnBangLuong.Text = "   Tính Lương";
            this.btnBangLuong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBangLuong.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // btnThongKe
            // 
            this.btnThongKe.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.btnThongKe.CornerRadius = 12;
            this.btnThongKe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThongKe.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnThongKe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnThongKe.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.btnThongKe.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.btnThongKe.Image = null;
            this.btnThongKe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThongKe.Location = new System.Drawing.Point(18, 422);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnThongKe.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(180)))), ((int)(((byte)(190)))), ((int)(((byte)(254)))));
            this.btnThongKe.Size = new System.Drawing.Size(224, 46);
            this.btnThongKe.TabIndex = 9;
            this.btnThongKe.Text = "   Thống Kê";
            this.btnThongKe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThongKe.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(139)))), ((int)(((byte)(168)))));
            this.btnDangXuat.CornerRadius = 12;
            this.btnDangXuat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDangXuat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDangXuat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnDangXuat.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(243)))), ((int)(((byte)(139)))), ((int)(((byte)(168)))));
            this.btnDangXuat.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(243)))), ((int)(((byte)(139)))), ((int)(((byte)(168)))));
            this.btnDangXuat.Image = null;
            this.btnDangXuat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDangXuat.Location = new System.Drawing.Point(18, 500);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnDangXuat.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(243)))), ((int)(((byte)(139)))), ((int)(((byte)(168)))));
            this.btnDangXuat.Size = new System.Drawing.Size(224, 46);
            this.btnDangXuat.TabIndex = 10;
            this.btnDangXuat.Text = "   Đăng Xuất";
            this.btnDangXuat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDangXuat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _pnlRightSide
            // 
            this._pnlRightSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this._pnlRightSide.Controls.Add(this.pnlContent);
            this._pnlRightSide.Controls.Add(this.pnlHeader);
            this._pnlRightSide.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pnlRightSide.Location = new System.Drawing.Point(260, 0);
            this._pnlRightSide.Margin = new System.Windows.Forms.Padding(0);
            this._pnlRightSide.Name = "_pnlRightSide";
            this._pnlRightSide.Size = new System.Drawing.Size(940, 720);
            this._pnlRightSide.TabIndex = 1;
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 60);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(940, 660);
            this.pnlContent.TabIndex = 1;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(37)))));
            this.pnlHeader.Controls.Add(this.lblPageTitle);
            this.pnlHeader.Controls.Add(this.lblDateTime);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.pnlHeader.Size = new System.Drawing.Size(940, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.lblPageTitle.Location = new System.Drawing.Point(24, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(300, 60);
            this.lblPageTitle.TabIndex = 0;
            this.lblPageTitle.Text = "Dashboard";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDateTime
            // 
            this.lblDateTime.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDateTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblDateTime.Location = new System.Drawing.Point(690, 0);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.lblDateTime.Size = new System.Drawing.Size(250, 60);
            this.lblDateTime.TabIndex = 1;
            this.lblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.tblMainLayout);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(950, 600);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Nhân Viên Quán Ăn";
            this.tblMainLayout.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this._pnlRightSide.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // === CÁC THÀNH PHẦN GIAO DIỆN ===
        private QuanLyNhanVien.Controls.GlassPanel pnlSidebar;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TableLayoutPanel tblMainLayout;
        private System.Windows.Forms.Panel _pnlRightSide;

        // Các thành phần của Thanh Tiêu Đề (Header items)
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblDateTime;
        private QuanLyNhanVien.Controls.RoundedButton btnToggle;

        // Các thành phần của Thanh Bên (Sidebar items)
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblMenu;

        // Các phím Điều Hướng (Navigation Buttons)
        private QuanLyNhanVien.Controls.RoundedButton btnDashboard;
        private QuanLyNhanVien.Controls.RoundedButton btnNhanVien;
        private QuanLyNhanVien.Controls.RoundedButton btnBoPhan;
        private QuanLyNhanVien.Controls.RoundedButton btnBangLuong;
        private QuanLyNhanVien.Controls.RoundedButton btnThongKe;
        private QuanLyNhanVien.Controls.RoundedButton btnDangXuat;
    }
}
