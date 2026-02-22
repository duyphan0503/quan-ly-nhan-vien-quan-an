namespace QuanLyNhanVien.Forms
{
    partial class FormThongKe
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.flowTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNam = new System.Windows.Forms.Label();
            this.cboNam = new System.Windows.Forms.ComboBox();
            this.btnXem = new System.Windows.Forms.Button();
            this.lblTongNam = new System.Windows.Forms.Label();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.colThang = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSoNhanVien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongLuong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongUng = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongBHXH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongThue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTongThucNhan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainLayout.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.flowTop.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlTop, 0, 0);
            this.mainLayout.Controls.Add(this.pnlGrid, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(800, 450);
            this.mainLayout.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(50)))), ((int)(((byte)(68)))));
            this.pnlTop.Controls.Add(this.flowTop);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTop.Location = new System.Drawing.Point(3, 3);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.pnlTop.Size = new System.Drawing.Size(794, 60);
            this.pnlTop.TabIndex = 0;
            // 
            // flowTop
            // 
            this.flowTop.Controls.Add(this.lblNam);
            this.flowTop.Controls.Add(this.cboNam);
            this.flowTop.Controls.Add(this.btnXem);
            this.flowTop.Controls.Add(this.lblTongNam);
            this.flowTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowTop.Location = new System.Drawing.Point(20, 10);
            this.flowTop.Name = "flowTop";
            this.flowTop.Size = new System.Drawing.Size(754, 40);
            this.flowTop.TabIndex = 0;
            // 
            // lblNam
            // 
            this.lblNam.AutoSize = true;
            this.lblNam.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblNam.Location = new System.Drawing.Point(0, 8);
            this.lblNam.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this.lblNam.Name = "lblNam";
            this.lblNam.Size = new System.Drawing.Size(79, 20);
            this.lblNam.TabIndex = 0;
            this.lblNam.Text = "Chọn năm:";
            // 
            // cboNam
            // 
            this.cboNam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.cboNam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboNam.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboNam.ForeColor = System.Drawing.Color.White;
            this.cboNam.Location = new System.Drawing.Point(89, 5);
            this.cboNam.Margin = new System.Windows.Forms.Padding(0, 5, 20, 0);
            this.cboNam.Name = "cboNam";
            this.cboNam.Size = new System.Drawing.Size(100, 25);
            this.cboNam.TabIndex = 1;
            // 
            // btnXem
            // 
            this.btnXem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            this.btnXem.FlatAppearance.BorderSize = 0;
            this.btnXem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.btnXem.Location = new System.Drawing.Point(209, 0);
            this.btnXem.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnXem.Name = "btnXem";
            this.btnXem.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnXem.Size = new System.Drawing.Size(150, 32);
            this.btnXem.TabIndex = 2;
            this.btnXem.Text = "Xem Thống Kê";
            // 
            // lblTongNam
            // 
            this.lblTongNam.AutoSize = true;
            this.lblTongNam.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTongNam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.lblTongNam.Location = new System.Drawing.Point(349, 8);
            this.lblTongNam.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblTongNam.Name = "lblTongNam";
            this.lblTongNam.Size = new System.Drawing.Size(0, 20);
            this.lblTongNam.TabIndex = 3;
            // 
            // pnlGrid
            // 
            this.pnlGrid.Controls.Add(this.dgv);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 68);
            this.pnlGrid.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(800, 382);
            this.pnlGrid.TabIndex = 1;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(37)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(227)))), ((int)(((byte)(161)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.ColumnHeadersHeight = 45;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colThang,
            this.colSoNhanVien,
            this.colTongLuong,
            this.colTongUng,
            this.colTongBHXH,
            this.colTongThue,
            this.colTongThucNhan});
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(50)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dgv.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 35;
            this.dgv.Size = new System.Drawing.Size(800, 382);
            this.dgv.TabIndex = 0;
            // 
            // colThang
            // 
            this.colThang.DataPropertyName = "Thang";
            this.colThang.HeaderText = "Tháng";
            this.colThang.Name = "colThang";
            this.colThang.ReadOnly = true;
            this.colThang.Width = 90;
            // 
            // colSoNhanVien
            // 
            this.colSoNhanVien.DataPropertyName = "SoNhanVien";
            this.colSoNhanVien.HeaderText = "Số NV";
            this.colSoNhanVien.Name = "colSoNhanVien";
            this.colSoNhanVien.ReadOnly = true;
            this.colSoNhanVien.Width = 100;
            // 
            // colTongLuong
            // 
            this.colTongLuong.DataPropertyName = "TongLuong";
            this.colTongLuong.HeaderText = "Tổng Lương";
            this.colTongLuong.Name = "colTongLuong";
            this.colTongLuong.ReadOnly = true;
            this.colTongLuong.Width = 120;
            // 
            // colTongUng
            // 
            this.colTongUng.DataPropertyName = "TongUng";
            this.colTongUng.HeaderText = "Tổng Ứng";
            this.colTongUng.Name = "colTongUng";
            this.colTongUng.ReadOnly = true;
            this.colTongUng.Width = 110;
            // 
            // colTongBHXH
            // 
            this.colTongBHXH.DataPropertyName = "TongBHXH";
            this.colTongBHXH.HeaderText = "BHXH";
            this.colTongBHXH.Name = "colTongBHXH";
            this.colTongBHXH.ReadOnly = true;
            this.colTongBHXH.Width = 100;
            // 
            // colTongThue
            // 
            this.colTongThue.DataPropertyName = "TongThue";
            this.colTongThue.HeaderText = "Thuế";
            this.colTongThue.Name = "colTongThue";
            this.colTongThue.ReadOnly = true;
            this.colTongThue.Width = 100;
            // 
            // colTongThucNhan
            // 
            this.colTongThucNhan.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTongThucNhan.DataPropertyName = "TongThucNhan";
            this.colTongThucNhan.HeaderText = "Thực Nhận";
            this.colTongThucNhan.Name = "colTongThucNhan";
            this.colTongThucNhan.ReadOnly = true;
            // 
            // FormThongKe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FormThongKe";
            this.Text = "Thống Kê Lương";
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.flowTop.ResumeLayout(false);
            this.flowTop.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.FlowLayoutPanel flowTop;
        private System.Windows.Forms.Label lblNam;
        private System.Windows.Forms.ComboBox cboNam;
        private System.Windows.Forms.Button btnXem;
        private System.Windows.Forms.Label lblTongNam;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThang;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSoNhanVien;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongLuong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongUng;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongBHXH;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongThue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTongThucNhan;
    }
}
