namespace QuanLyNhanVien.Forms
{
    partial class FormConnectionWizard
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
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStepTitle = new System.Windows.Forms.Label();
            this.lblStepDesc = new System.Windows.Forms.Label();
            this.pnlStep1 = new System.Windows.Forms.Panel();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblTip1 = new System.Windows.Forms.Label();
            this.pnlStep2 = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.lblTip2 = new System.Windows.Forms.Label();
            this.pnlStep3 = new System.Windows.Forms.Panel();
            this.pbDiagnostic = new System.Windows.Forms.ProgressBar();
            this.rtbDiagnostic = new System.Windows.Forms.RichTextBox();
            this.pnlStep4 = new System.Windows.Forms.Panel();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnBack = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnCancel = new QuanLyNhanVien.Controls.RoundedButton();
            this.btnNext = new QuanLyNhanVien.Controls.RoundedButton();
            this.lblStepIndicator = new System.Windows.Forms.Label();
            this.pnlCard.SuspendLayout();
            this.pnlStep1.SuspendLayout();
            this.pnlStep2.SuspendLayout();
            this.pnlStep3.SuspendLayout();
            this.pnlStep4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.pnlCard.Controls.Add(this.lblLogo);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblStepTitle);
            this.pnlCard.Controls.Add(this.lblStepDesc);
            this.pnlCard.Controls.Add(this.pnlStep1);
            this.pnlCard.Controls.Add(this.pnlStep2);
            this.pnlCard.Controls.Add(this.pnlStep3);
            this.pnlCard.Controls.Add(this.pnlStep4);
            this.pnlCard.Controls.Add(this.btnBack);
            this.pnlCard.Controls.Add(this.btnCancel);
            this.pnlCard.Controls.Add(this.btnNext);
            this.pnlCard.Controls.Add(this.lblStepIndicator);
            this.pnlCard.Location = new System.Drawing.Point(20, 15);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(540, 490);
            this.pnlCard.TabIndex = 0;
            // 
            // lblLogo
            // 
            this.lblLogo.BackColor = System.Drawing.Color.Transparent;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 32F);
            this.lblLogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.lblLogo.Location = new System.Drawing.Point(0, 10);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(540, 42);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "🔌";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 52);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(540, 24);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "THIẾT LẬP KẾT NỐI CƠ SỞ DỮ LIỆU";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStepTitle
            // 
            this.lblStepTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblStepTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStepTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.lblStepTitle.Location = new System.Drawing.Point(25, 85);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(490, 22);
            this.lblStepTitle.TabIndex = 2;
            // 
            // lblStepDesc
            // 
            this.lblStepDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblStepDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStepDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblStepDesc.Location = new System.Drawing.Point(25, 107);
            this.lblStepDesc.Name = "lblStepDesc";
            this.lblStepDesc.Size = new System.Drawing.Size(490, 18);
            this.lblStepDesc.TabIndex = 3;
            // 
            // pnlStep1
            // 
            this.pnlStep1.BackColor = System.Drawing.Color.Transparent;
            this.pnlStep1.Controls.Add(this.lblServer);
            this.pnlStep1.Controls.Add(this.txtServer);
            this.pnlStep1.Controls.Add(this.lblPort);
            this.pnlStep1.Controls.Add(this.txtPort);
            this.pnlStep1.Controls.Add(this.lblTip1);
            this.pnlStep1.Location = new System.Drawing.Point(25, 135);
            this.pnlStep1.Name = "pnlStep1";
            this.pnlStep1.Size = new System.Drawing.Size(490, 250);
            this.pnlStep1.TabIndex = 4;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.BackColor = System.Drawing.Color.Transparent;
            this.lblServer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblServer.Location = new System.Drawing.Point(0, 10);
            this.lblServer.Name = "lblServer";
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Tên máy chủ hoặc IP:";
            // 
            // txtServer
            // 
            this.txtServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServer.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtServer.ForeColor = System.Drawing.Color.White;
            this.txtServer.Location = new System.Drawing.Point(0, 32);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(490, 27);
            this.txtServer.TabIndex = 1;
            this.txtServer.Text = "localhost";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.BackColor = System.Drawing.Color.Transparent;
            this.lblPort.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblPort.Location = new System.Drawing.Point(0, 72);
            this.lblPort.Name = "lblPort";
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Cổng (Port):";
            // 
            // txtPort
            // 
            this.txtPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPort.ForeColor = System.Drawing.Color.White;
            this.txtPort.Location = new System.Drawing.Point(0, 94);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(120, 27);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "1433";
            // 
            // lblTip1
            // 
            this.lblTip1.BackColor = System.Drawing.Color.Transparent;
            this.lblTip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblTip1.Location = new System.Drawing.Point(0, 140);
            this.lblTip1.Name = "lblTip1";
            this.lblTip1.Size = new System.Drawing.Size(490, 90);
            this.lblTip1.TabIndex = 4;
            this.lblTip1.Text = "💡 Ví dụ:\r\n  • localhost — SQL Server trên máy này\r\n  • 192.168.1.100 — IP của máy chủ\r\n  • MYSERVER\\SQLEXPRESS — Named instance\r\n  • Cổng mặc định SQL Server: 1433";
            // 
            // pnlStep2
            // 
            this.pnlStep2.BackColor = System.Drawing.Color.Transparent;
            this.pnlStep2.Controls.Add(this.lblUsername);
            this.pnlStep2.Controls.Add(this.txtUsername);
            this.pnlStep2.Controls.Add(this.lblPassword);
            this.pnlStep2.Controls.Add(this.txtPassword);
            this.pnlStep2.Controls.Add(this.lblDatabase);
            this.pnlStep2.Controls.Add(this.txtDatabase);
            this.pnlStep2.Controls.Add(this.lblTip2);
            this.pnlStep2.Location = new System.Drawing.Point(25, 135);
            this.pnlStep2.Name = "pnlStep2";
            this.pnlStep2.Size = new System.Drawing.Size(490, 250);
            this.pnlStep2.TabIndex = 5;
            this.pnlStep2.Visible = false;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblUsername.Location = new System.Drawing.Point(0, 10);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Tên đăng nhập SQL:";
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsername.ForeColor = System.Drawing.Color.White;
            this.txtUsername.Location = new System.Drawing.Point(0, 32);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(490, 27);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Text = "sa";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblPassword.Location = new System.Drawing.Point(0, 72);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Mật khẩu:";
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(0, 94);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(490, 27);
            this.txtPassword.TabIndex = 3;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.BackColor = System.Drawing.Color.Transparent;
            this.lblDatabase.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(173)))), ((int)(((byte)(200)))));
            this.lblDatabase.Location = new System.Drawing.Point(0, 134);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.TabIndex = 4;
            this.lblDatabase.Text = "Tên database:";
            // 
            // txtDatabase
            // 
            this.txtDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(78)))));
            this.txtDatabase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDatabase.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDatabase.ForeColor = System.Drawing.Color.White;
            this.txtDatabase.Location = new System.Drawing.Point(0, 156);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(300, 27);
            this.txtDatabase.TabIndex = 5;
            this.txtDatabase.Text = "QuanLyNhanVien";
            // 
            // lblTip2
            // 
            this.lblTip2.BackColor = System.Drawing.Color.Transparent;
            this.lblTip2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTip2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblTip2.Location = new System.Drawing.Point(0, 200);
            this.lblTip2.Name = "lblTip2";
            this.lblTip2.Size = new System.Drawing.Size(490, 30);
            this.lblTip2.TabIndex = 6;
            this.lblTip2.Text = "💡 Tài khoản mặc định: sa / [mật khẩu khi cài đặt SQL Server]";
            // 
            // pnlStep3
            // 
            this.pnlStep3.BackColor = System.Drawing.Color.Transparent;
            this.pnlStep3.Controls.Add(this.pbDiagnostic);
            this.pnlStep3.Controls.Add(this.rtbDiagnostic);
            this.pnlStep3.Location = new System.Drawing.Point(25, 135);
            this.pnlStep3.Name = "pnlStep3";
            this.pnlStep3.Size = new System.Drawing.Size(490, 280);
            this.pnlStep3.TabIndex = 6;
            this.pnlStep3.Visible = false;
            // 
            // pbDiagnostic
            // 
            this.pbDiagnostic.Location = new System.Drawing.Point(0, 0);
            this.pbDiagnostic.Name = "pbDiagnostic";
            this.pbDiagnostic.Size = new System.Drawing.Size(490, 20);
            this.pbDiagnostic.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbDiagnostic.TabIndex = 0;
            // 
            // rtbDiagnostic
            // 
            this.rtbDiagnostic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(37)))));
            this.rtbDiagnostic.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDiagnostic.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbDiagnostic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.rtbDiagnostic.Location = new System.Drawing.Point(0, 28);
            this.rtbDiagnostic.Name = "rtbDiagnostic";
            this.rtbDiagnostic.ReadOnly = true;
            this.rtbDiagnostic.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbDiagnostic.Size = new System.Drawing.Size(490, 245);
            this.rtbDiagnostic.TabIndex = 1;
            this.rtbDiagnostic.Text = "";
            // 
            // pnlStep4
            // 
            this.pnlStep4.BackColor = System.Drawing.Color.Transparent;
            this.pnlStep4.Controls.Add(this.lblResult);
            this.pnlStep4.Location = new System.Drawing.Point(25, 135);
            this.pnlStep4.Name = "pnlStep4";
            this.pnlStep4.Size = new System.Drawing.Size(490, 250);
            this.pnlStep4.TabIndex = 7;
            this.pnlStep4.Visible = false;
            // 
            // lblResult
            // 
            this.lblResult.BackColor = System.Drawing.Color.Transparent;
            this.lblResult.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.lblResult.Location = new System.Drawing.Point(0, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(490, 200);
            this.lblResult.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.CornerRadius = 8;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(214)))), ((int)(((byte)(244)))));
            this.btnBack.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(71)))), ((int)(((byte)(90)))));
            this.btnBack.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(91)))), ((int)(((byte)(112)))));
            this.btnBack.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(50)))), ((int)(((byte)(68)))));
            this.btnBack.Location = new System.Drawing.Point(25, 440);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(130, 36);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "← QUAY LẠI";
            this.btnBack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnBack.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.CornerRadius = 8;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(27)))));
            this.btnCancel.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(139)))), ((int)(((byte)(168)))));
            this.btnCancel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(177)))), ((int)(((byte)(206)))));
            this.btnCancel.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(114)))), ((int)(((byte)(143)))));
            this.btnCancel.Location = new System.Drawing.Point(250, 440);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "THOÁT";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNext
            // 
            this.btnNext.CornerRadius = 8;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(27)))));
            this.btnNext.IdleColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(180)))), ((int)(((byte)(250)))));
            this.btnNext.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.btnNext.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(155)))), ((int)(((byte)(225)))));
            this.btnNext.Location = new System.Drawing.Point(365, 440);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(165, 36);
            this.btnNext.TabIndex = 10;
            this.btnNext.Text = "TIẾP TỤC →";
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStepIndicator
            // 
            this.lblStepIndicator.BackColor = System.Drawing.Color.Transparent;
            this.lblStepIndicator.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStepIndicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(112)))), ((int)(((byte)(134)))));
            this.lblStepIndicator.Location = new System.Drawing.Point(25, 478);
            this.lblStepIndicator.Name = "lblStepIndicator";
            this.lblStepIndicator.Size = new System.Drawing.Size(200, 16);
            this.lblStepIndicator.TabIndex = 11;
            this.lblStepIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormConnectionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(27)))));
            this.ClientSize = new System.Drawing.Size(580, 560);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormConnectionWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trình Hướng Dẫn Kết Nối Database — Quản Lý Nhân Viên";
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.pnlStep1.ResumeLayout(false);
            this.pnlStep1.PerformLayout();
            this.pnlStep2.ResumeLayout(false);
            this.pnlStep2.PerformLayout();
            this.pnlStep3.ResumeLayout(false);
            this.pnlStep4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // ── Controls ──
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.Label lblStepDesc;

        // Step 1: Server
        private System.Windows.Forms.Panel pnlStep1;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblTip1;

        // Step 2: Credentials
        private System.Windows.Forms.Panel pnlStep2;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblTip2;

        // Step 3: Diagnostics
        private System.Windows.Forms.Panel pnlStep3;
        private System.Windows.Forms.RichTextBox rtbDiagnostic;
        private System.Windows.Forms.ProgressBar pbDiagnostic;

        // Step 4: Result
        private System.Windows.Forms.Panel pnlStep4;
        private System.Windows.Forms.Label lblResult;

        // Navigation
        private QuanLyNhanVien.Controls.RoundedButton btnBack;
        private QuanLyNhanVien.Controls.RoundedButton btnNext;
        private QuanLyNhanVien.Controls.RoundedButton btnCancel;
        private System.Windows.Forms.Label lblStepIndicator;
    }
}
