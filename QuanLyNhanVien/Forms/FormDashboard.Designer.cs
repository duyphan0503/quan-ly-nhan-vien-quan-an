namespace QuanLyNhanVien.Forms
{
    partial class FormDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.cardNhanVien = new QuanLyNhanVien.Controls.DashboardCard();
            this.cardBoPhan = new QuanLyNhanVien.Controls.DashboardCard();
            this.cardLuong = new QuanLyNhanVien.Controls.DashboardCard();
            this.pnlWelcome = new System.Windows.Forms.Panel();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblWelcomeMsg = new System.Windows.Forms.Label();
            this.pbBigIcon = new System.Windows.Forms.PictureBox();
            this.tableLayout.SuspendLayout();
            this.pnlWelcome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBigIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.BackColor = System.Drawing.Color.Transparent;
            this.tableLayout.ColumnCount = 3;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayout.Controls.Add(this.cardNhanVien, 0, 0);
            this.tableLayout.Controls.Add(this.cardBoPhan, 1, 0);
            this.tableLayout.Controls.Add(this.cardLuong, 2, 0);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 1;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Size = new System.Drawing.Size(1177, 120);
            this.tableLayout.TabIndex = 0;
            // 
            // cardNhanVien
            // 
            this.cardNhanVien.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.cardNhanVien.Cursor = System.Windows.Forms.Cursors.Default;
            this.cardNhanVien.Dock = System.Windows.Forms.DockStyle.Fill;
            // cardNhanVien
            this.cardNhanVien.Location = new System.Drawing.Point(10, 10);
            this.cardNhanVien.Margin = new System.Windows.Forms.Padding(10);
            this.cardNhanVien.Name = "cardNhanVien";
            this.cardNhanVien.Size = new System.Drawing.Size(372, 100);
            this.cardNhanVien.Subtitle = "Tổng nhân viên";
            this.cardNhanVien.TabIndex = 0;
            this.cardNhanVien.Value = "—";
            // 
            // cardBoPhan
            // 
            this.cardBoPhan.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.cardBoPhan.Cursor = System.Windows.Forms.Cursors.Default;
            this.cardBoPhan.Dock = System.Windows.Forms.DockStyle.Fill;
            // cardBoPhan
            this.cardBoPhan.Location = new System.Drawing.Point(402, 10);
            this.cardBoPhan.Margin = new System.Windows.Forms.Padding(10);
            this.cardBoPhan.Name = "cardBoPhan";
            this.cardBoPhan.Size = new System.Drawing.Size(372, 100);
            this.cardBoPhan.Subtitle = "Bộ phận";
            this.cardBoPhan.TabIndex = 1;
            this.cardBoPhan.Value = "—";
            // 
            // cardLuong
            // 
            this.cardLuong.AccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.cardLuong.Cursor = System.Windows.Forms.Cursors.Default;
            this.cardLuong.Dock = System.Windows.Forms.DockStyle.Fill;
            // cardLuong
            this.cardLuong.Location = new System.Drawing.Point(794, 10);
            this.cardLuong.Margin = new System.Windows.Forms.Padding(10);
            this.cardLuong.Name = "cardLuong";
            this.cardLuong.Size = new System.Drawing.Size(373, 100);
            this.cardLuong.Subtitle = "Bảng lương tháng này";
            this.cardLuong.TabIndex = 2;
            this.cardLuong.Value = "—";
            // 
            // pnlWelcome
            // 
            this.pnlWelcome.BackColor = System.Drawing.Color.Transparent;
            this.pnlWelcome.Controls.Add(this.lblHint);
            this.pnlWelcome.Controls.Add(this.lblWelcomeMsg);
            this.pnlWelcome.Controls.Add(this.pbBigIcon);
            this.pnlWelcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWelcome.Location = new System.Drawing.Point(0, 120);
            this.pnlWelcome.Name = "pnlWelcome";
            this.pnlWelcome.Padding = new System.Windows.Forms.Padding(0, 40, 0, 0);
            this.pnlWelcome.Size = new System.Drawing.Size(1177, 591);
            this.pnlWelcome.TabIndex = 1;
            // 
            // lblHint
            // 
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblHint.Location = new System.Drawing.Point(0, 240);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(1177, 40);
            this.lblHint.TabIndex = 2;
            this.lblHint.Text = "Chọn chức năng từ menu bên trái để bắt đầu";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblWelcomeMsg
            // 
            this.lblWelcomeMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWelcomeMsg.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblWelcomeMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.lblWelcomeMsg.Location = new System.Drawing.Point(0, 160);
            this.lblWelcomeMsg.Name = "lblWelcomeMsg";
            this.lblWelcomeMsg.Size = new System.Drawing.Size(1177, 80);
            this.lblWelcomeMsg.TabIndex = 1;
            this.lblWelcomeMsg.Text = "HỆ THỐNG QUẢN LÝ NHÂN VIÊN\r\nQUÁN ĂN";
            this.lblWelcomeMsg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pbBigIcon
            // 
            this.pbBigIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbBigIcon.Location = new System.Drawing.Point(0, 40);
            this.pbBigIcon.Name = "pbBigIcon";
            this.pbBigIcon.Size = new System.Drawing.Size(1177, 120);
            this.pbBigIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbBigIcon.TabIndex = 0;
            this.pbBigIcon.TabStop = false;
            // 
            // FormDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(1177, 711);
            this.Controls.Add(this.pnlWelcome);
            this.Controls.Add(this.tableLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDashboard";
            this.Text = "Dashboard";
            this.tableLayout.ResumeLayout(false);
            this.pnlWelcome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbBigIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private QuanLyNhanVien.Controls.DashboardCard cardNhanVien;
        private QuanLyNhanVien.Controls.DashboardCard cardBoPhan;
        private QuanLyNhanVien.Controls.DashboardCard cardLuong;
        private System.Windows.Forms.Panel pnlWelcome;
        private System.Windows.Forms.PictureBox pbBigIcon;
        private System.Windows.Forms.Label lblWelcomeMsg;
        private System.Windows.Forms.Label lblHint;
    }
}
