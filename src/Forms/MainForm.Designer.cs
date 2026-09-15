namespace NetworkSwitcher
{
    /// <summary>
    /// Автоматически сгенерированный код инициализации главной формы.
    /// </summary>
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnThemeToggle = new System.Windows.Forms.Button();
            this.btnOpenSettings = new System.Windows.Forms.Button();
            this.btnTurnOffAll = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.chkAmnezia = new NetworkSwitcher.ToggleSwitch();
            this.chkWarp = new NetworkSwitcher.ToggleSwitch();
            this.chkSlot2 = new NetworkSwitcher.ToggleSwitch();
            this.chkSlot1 = new NetworkSwitcher.ToggleSwitch();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(225, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Управление сетями";
            // 
            // btnThemeToggle
            // 
            this.btnThemeToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemeToggle.FlatAppearance.BorderSize = 0;
            this.btnThemeToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemeToggle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnThemeToggle.Location = new System.Drawing.Point(352, 12);
            this.btnThemeToggle.Name = "btnThemeToggle";
            this.btnThemeToggle.Size = new System.Drawing.Size(35, 35);
            this.btnThemeToggle.TabIndex = 1;
            this.btnThemeToggle.Text = "🌙";
            this.btnThemeToggle.UseVisualStyleBackColor = true;
            // 
            // btnOpenSettings
            // 
            this.btnOpenSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenSettings.FlatAppearance.BorderSize = 0;
            this.btnOpenSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenSettings.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnOpenSettings.Location = new System.Drawing.Point(393, 13);
            this.btnOpenSettings.Name = "btnOpenSettings";
            this.btnOpenSettings.Size = new System.Drawing.Size(35, 35);
            this.btnOpenSettings.TabIndex = 2;
            this.btnOpenSettings.Text = "🛠";
            this.btnOpenSettings.UseVisualStyleBackColor = true;
            this.btnOpenSettings.Click += new System.EventHandler(this.btnOpenSettings_Click);
            // 
            // btnTurnOffAll
            // 
            this.btnTurnOffAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(64)))), ((int)(((byte)(52)))));
            this.btnTurnOffAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTurnOffAll.FlatAppearance.BorderSize = 0;
            this.btnTurnOffAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTurnOffAll.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTurnOffAll.ForeColor = System.Drawing.Color.White;
            this.btnTurnOffAll.Location = new System.Drawing.Point(25, 235);
            this.btnTurnOffAll.Name = "btnTurnOffAll";
            this.btnTurnOffAll.Size = new System.Drawing.Size(190, 42);
            this.btnTurnOffAll.TabIndex = 7;
            this.btnTurnOffAll.Text = "Отключить всё";
            this.btnTurnOffAll.UseVisualStyleBackColor = false;
            // 
            // btnRestart
            // 
            this.btnRestart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(144)))), ((int)(((byte)(217)))));
            this.btnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestart.FlatAppearance.BorderSize = 0;
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRestart.ForeColor = System.Drawing.Color.White;
            this.btnRestart.Location = new System.Drawing.Point(225, 235);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(190, 42);
            this.btnRestart.TabIndex = 8;
            this.btnRestart.Text = "🔄 Перезапустить";
            this.btnRestart.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(25, 290);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(210, 20);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "🔴 Все службы отключены";
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.txtLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLogs.Location = new System.Drawing.Point(25, 325);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(390, 135);
            this.txtLogs.TabIndex = 10;
            // 
            // chkAmnezia
            // 
            this.chkAmnezia.BackColor = System.Drawing.Color.Transparent;
            this.chkAmnezia.Checked = false;
            this.chkAmnezia.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAmnezia.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.chkAmnezia.Location = new System.Drawing.Point(25, 185);
            this.chkAmnezia.Name = "chkAmnezia";
            this.chkAmnezia.Size = new System.Drawing.Size(390, 35);
            this.chkAmnezia.TabIndex = 6;
            this.chkAmnezia.Text = "AmneziaWG";
            this.chkAmnezia.ThumbColor = System.Drawing.Color.White;
            this.chkAmnezia.TrackColorOff = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.chkAmnezia.TrackColorOn = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(47)))), ((int)(((byte)(190)))));
            // 
            // chkWarp
            // 
            this.chkWarp.BackColor = System.Drawing.Color.Transparent;
            this.chkWarp.Checked = false;
            this.chkWarp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWarp.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.chkWarp.Location = new System.Drawing.Point(25, 145);
            this.chkWarp.Name = "chkWarp";
            this.chkWarp.Size = new System.Drawing.Size(390, 35);
            this.chkWarp.TabIndex = 5;
            this.chkWarp.Text = "Cloudflare WARP";
            this.chkWarp.ThumbColor = System.Drawing.Color.White;
            this.chkWarp.TrackColorOff = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.chkWarp.TrackColorOn = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(47)))), ((int)(((byte)(190)))));
            // 
            // chkSlot2
            // 
            this.chkSlot2.BackColor = System.Drawing.Color.Transparent;
            this.chkSlot2.Checked = false;
            this.chkSlot2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSlot2.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.chkSlot2.Location = new System.Drawing.Point(25, 105);
            this.chkSlot2.Name = "chkSlot2";
            this.chkSlot2.Size = new System.Drawing.Size(390, 35);
            this.chkSlot2.TabIndex = 4;
            this.chkSlot2.Text = "Стратегия 2";
            this.chkSlot2.ThumbColor = System.Drawing.Color.White;
            this.chkSlot2.TrackColorOff = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.chkSlot2.TrackColorOn = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(47)))), ((int)(((byte)(190)))));
            // 
            // chkSlot1
            // 
            this.chkSlot1.BackColor = System.Drawing.Color.Transparent;
            this.chkSlot1.Checked = false;
            this.chkSlot1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSlot1.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.chkSlot1.Location = new System.Drawing.Point(25, 65);
            this.chkSlot1.Name = "chkSlot1";
            this.chkSlot1.Size = new System.Drawing.Size(390, 35);
            this.chkSlot1.TabIndex = 3;
            this.chkSlot1.Text = "Стратегия 1";
            this.chkSlot1.ThumbColor = System.Drawing.Color.White;
            this.chkSlot1.TrackColorOff = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.chkSlot1.TrackColorOn = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(47)))), ((int)(((byte)(190)))));
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 480);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnTurnOffAll);
            this.Controls.Add(this.chkAmnezia);
            this.Controls.Add(this.chkWarp);
            this.Controls.Add(this.chkSlot2);
            this.Controls.Add(this.chkSlot1);
            this.Controls.Add(this.btnOpenSettings);
            this.Controls.Add(this.btnThemeToggle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Network Switcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnThemeToggle;
        private System.Windows.Forms.Button btnOpenSettings;
        private ToggleSwitch chkSlot1;
        private ToggleSwitch chkSlot2;
        private ToggleSwitch chkWarp;
        private ToggleSwitch chkAmnezia;
        private System.Windows.Forms.Button btnTurnOffAll;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtLogs;
    }
}
