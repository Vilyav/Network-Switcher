namespace NetworkSwitcher
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblZapretPath;
        private System.Windows.Forms.TextBox txtZapretPath;
        private System.Windows.Forms.Button btnBrowseZapret;

        private System.Windows.Forms.CheckBox chkUseCustomNames;

        private System.Windows.Forms.Label lblSlot1;
        private System.Windows.Forms.TextBox txtSlot1Name;
        private System.Windows.Forms.TextBox txtSlot1Bat;

        private System.Windows.Forms.Label lblSlot2;
        private System.Windows.Forms.TextBox txtSlot2Name;
        private System.Windows.Forms.TextBox txtSlot2Bat;

        private System.Windows.Forms.Label lblWarpPath;
        private System.Windows.Forms.TextBox txtWarpPath;
        private System.Windows.Forms.Button btnBrowseWarp;

        private System.Windows.Forms.Label lblAmneziaExePath;
        private System.Windows.Forms.TextBox txtAmneziaExePath;
        private System.Windows.Forms.Button btnBrowseAmneziaExe;

        private System.Windows.Forms.Label lblAmneziaConfPath;
        private System.Windows.Forms.TextBox txtAmneziaConfPath;
        private System.Windows.Forms.Button btnBrowseAmneziaConf;

        private System.Windows.Forms.Label lblAmneziaTunnelName;
        private System.Windows.Forms.TextBox txtAmneziaTunnelName;

        private System.Windows.Forms.CheckBox chkAutoStart;

        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblZapretPath = new System.Windows.Forms.Label();
            this.txtZapretPath = new System.Windows.Forms.TextBox();
            this.btnBrowseZapret = new System.Windows.Forms.Button();
            this.chkUseCustomNames = new System.Windows.Forms.CheckBox();
            this.lblSlot1 = new System.Windows.Forms.Label();
            this.txtSlot1Name = new System.Windows.Forms.TextBox();
            this.txtSlot1Bat = new System.Windows.Forms.TextBox();
            this.lblSlot2 = new System.Windows.Forms.Label();
            this.txtSlot2Name = new System.Windows.Forms.TextBox();
            this.txtSlot2Bat = new System.Windows.Forms.TextBox();
            this.lblWarpPath = new System.Windows.Forms.Label();
            this.txtWarpPath = new System.Windows.Forms.TextBox();
            this.btnBrowseWarp = new System.Windows.Forms.Button();
            this.lblAmneziaExePath = new System.Windows.Forms.Label();
            this.txtAmneziaExePath = new System.Windows.Forms.TextBox();
            this.btnBrowseAmneziaExe = new System.Windows.Forms.Button();
            this.lblAmneziaConfPath = new System.Windows.Forms.Label();
            this.txtAmneziaConfPath = new System.Windows.Forms.TextBox();
            this.btnBrowseAmneziaConf = new System.Windows.Forms.Button();
            this.lblAmneziaTunnelName = new System.Windows.Forms.Label();
            this.txtAmneziaTunnelName = new System.Windows.Forms.TextBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblZapretPath
            // 
            this.lblZapretPath.AutoSize = true;
            this.lblZapretPath.Location = new System.Drawing.Point(12, 15);
            this.lblZapretPath.Name = "lblZapretPath";
            this.lblZapretPath.Size = new System.Drawing.Size(81, 13);
            this.lblZapretPath.Text = "Папка Zapret:";
            // 
            // txtZapretPath
            // 
            this.txtZapretPath.Location = new System.Drawing.Point(15, 31);
            this.txtZapretPath.Name = "txtZapretPath";
            this.txtZapretPath.Size = new System.Drawing.Size(380, 20);
            this.txtZapretPath.TabIndex = 0;
            // 
            // btnBrowseZapret
            // 
            this.btnBrowseZapret.Location = new System.Drawing.Point(401, 29);
            this.btnBrowseZapret.Name = "btnBrowseZapret";
            this.btnBrowseZapret.Size = new System.Drawing.Size(35, 23);
            this.btnBrowseZapret.TabIndex = 1;
            this.btnBrowseZapret.Text = "...";
            this.btnBrowseZapret.UseVisualStyleBackColor = true;
            // 
            // chkUseCustomNames
            // 
            this.chkUseCustomNames.AutoSize = true;
            this.chkUseCustomNames.Location = new System.Drawing.Point(15, 62);
            this.chkUseCustomNames.Name = "chkUseCustomNames";
            this.chkUseCustomNames.Size = new System.Drawing.Size(250, 17);
            this.chkUseCustomNames.TabIndex = 2;
            this.chkUseCustomNames.Text = "Использовать кастомные названия и батники";
            this.chkUseCustomNames.UseVisualStyleBackColor = true;
            // 
            // lblSlot1
            // 
            this.lblSlot1.AutoSize = true;
            this.lblSlot1.Location = new System.Drawing.Point(12, 88);
            this.lblSlot1.Name = "lblSlot1";
            this.lblSlot1.Size = new System.Drawing.Size(126, 13);
            this.lblSlot1.Text = "Слот 1 (Название / Батник):";
            // 
            // txtSlot1Name
            // 
            this.txtSlot1Name.Location = new System.Drawing.Point(15, 104);
            this.txtSlot1Name.Name = "txtSlot1Name";
            this.txtSlot1Name.Size = new System.Drawing.Size(185, 20);
            this.txtSlot1Name.TabIndex = 3;
            // 
            // txtSlot1Bat
            // 
            this.txtSlot1Bat.Location = new System.Drawing.Point(210, 104);
            this.txtSlot1Bat.Name = "txtSlot1Bat";
            this.txtSlot1Bat.Size = new System.Drawing.Size(185, 20);
            this.txtSlot1Bat.TabIndex = 4;
            // 
            // lblSlot2
            // 
            this.lblSlot2.AutoSize = true;
            this.lblSlot2.Location = new System.Drawing.Point(12, 133);
            this.lblSlot2.Name = "lblSlot2";
            this.lblSlot2.Size = new System.Drawing.Size(126, 13);
            this.lblSlot2.Text = "Слот 2 (Название / Батник):";
            // 
            // txtSlot2Name
            // 
            this.txtSlot2Name.Location = new System.Drawing.Point(15, 149);
            this.txtSlot2Name.Name = "txtSlot2Name";
            this.txtSlot2Name.Size = new System.Drawing.Size(185, 20);
            this.txtSlot2Name.TabIndex = 5;
            // 
            // txtSlot2Bat
            // 
            this.txtSlot2Bat.Location = new System.Drawing.Point(210, 149);
            this.txtSlot2Bat.Name = "txtSlot2Bat";
            this.txtSlot2Bat.Size = new System.Drawing.Size(185, 20);
            this.txtSlot2Bat.TabIndex = 6;
            // 
            // lblWarpPath
            // 
            this.lblWarpPath.AutoSize = true;
            this.lblWarpPath.Location = new System.Drawing.Point(12, 178);
            this.lblWarpPath.Name = "lblWarpPath";
            this.lblWarpPath.Size = new System.Drawing.Size(107, 13);
            this.lblWarpPath.Text = "Путь к warp-cli.exe:";
            // 
            // txtWarpPath
            // 
            this.txtWarpPath.Location = new System.Drawing.Point(15, 194);
            this.txtWarpPath.Name = "txtWarpPath";
            this.txtWarpPath.Size = new System.Drawing.Size(380, 20);
            this.txtWarpPath.TabIndex = 7;
            // 
            // btnBrowseWarp
            // 
            this.btnBrowseWarp.Location = new System.Drawing.Point(401, 192);
            this.btnBrowseWarp.Name = "btnBrowseWarp";
            this.btnBrowseWarp.Size = new System.Drawing.Size(35, 23);
            this.btnBrowseWarp.TabIndex = 8;
            this.btnBrowseWarp.Text = "...";
            this.btnBrowseWarp.UseVisualStyleBackColor = true;
            // 
            // lblAmneziaExePath
            // 
            this.lblAmneziaExePath.AutoSize = true;
            this.lblAmneziaExePath.Location = new System.Drawing.Point(12, 223);
            this.lblAmneziaExePath.Name = "lblAmneziaExePath";
            this.lblAmneziaExePath.Size = new System.Drawing.Size(123, 13);
            this.lblAmneziaExePath.Text = "Путь к amneziawg.exe:";
            // 
            // txtAmneziaExePath
            // 
            this.txtAmneziaExePath.Location = new System.Drawing.Point(15, 239);
            this.txtAmneziaExePath.Name = "txtAmneziaExePath";
            this.txtAmneziaExePath.Size = new System.Drawing.Size(380, 20);
            this.txtAmneziaExePath.TabIndex = 9;
            // 
            // btnBrowseAmneziaExe
            // 
            this.btnBrowseAmneziaExe.Location = new System.Drawing.Point(401, 237);
            this.btnBrowseAmneziaExe.Name = "btnBrowseAmneziaExe";
            this.btnBrowseAmneziaExe.Size = new System.Drawing.Size(35, 23);
            this.btnBrowseAmneziaExe.TabIndex = 10;
            this.btnBrowseAmneziaExe.Text = "...";
            this.btnBrowseAmneziaExe.UseVisualStyleBackColor = true;
            // 
            // lblAmneziaConfPath
            // 
            this.lblAmneziaConfPath.AutoSize = true;
            this.lblAmneziaConfPath.Location = new System.Drawing.Point(12, 268);
            this.lblAmneziaConfPath.Name = "lblAmneziaConfPath";
            this.lblAmneziaConfPath.Size = new System.Drawing.Size(124, 13);
            this.lblAmneziaConfPath.Text = "Конфиг AmneziaWG (.conf):";
            // 
            // txtAmneziaConfPath
            // 
            this.txtAmneziaConfPath.Location = new System.Drawing.Point(15, 284);
            this.txtAmneziaConfPath.Name = "txtAmneziaConfPath";
            this.txtAmneziaConfPath.Size = new System.Drawing.Size(380, 20);
            this.txtAmneziaConfPath.TabIndex = 11;
            // 
            // btnBrowseAmneziaConf
            // 
            this.btnBrowseAmneziaConf.Location = new System.Drawing.Point(401, 282);
            this.btnBrowseAmneziaConf.Name = "btnBrowseAmneziaConf";
            this.btnBrowseAmneziaConf.Size = new System.Drawing.Size(35, 23);
            this.btnBrowseAmneziaConf.TabIndex = 12;
            this.btnBrowseAmneziaConf.Text = "...";
            this.btnBrowseAmneziaConf.UseVisualStyleBackColor = true;
            // 
            // lblAmneziaTunnelName
            // 
            this.lblAmneziaTunnelName.AutoSize = true;
            this.lblAmneziaTunnelName.Location = new System.Drawing.Point(12, 313);
            this.lblAmneziaTunnelName.Name = "lblAmneziaTunnelName";
            this.lblAmneziaTunnelName.Size = new System.Drawing.Size(128, 13);
            this.lblAmneziaTunnelName.Text = "Имя туннеля AmneziaWG:";
            // 
            // txtAmneziaTunnelName
            // 
            this.txtAmneziaTunnelName.Location = new System.Drawing.Point(15, 329);
            this.txtAmneziaTunnelName.Name = "txtAmneziaTunnelName";
            this.txtAmneziaTunnelName.Size = new System.Drawing.Size(380, 20);
            this.txtAmneziaTunnelName.TabIndex = 13;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(15, 362);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(215, 17);
            this.chkAutoStart.TabIndex = 14;
            this.chkAutoStart.Text = "Запускать вместе с Windows (в трее)";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(15, 395);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 30);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Экспорт";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(135, 395);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(110, 30);
            this.btnImport.TabIndex = 16;
            this.btnImport.Text = "Импорт";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(255, 395);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(181, 30);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 440);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.chkAutoStart);
            this.Controls.Add(this.txtAmneziaTunnelName);
            this.Controls.Add(this.lblAmneziaTunnelName);
            this.Controls.Add(this.btnBrowseAmneziaConf);
            this.Controls.Add(this.txtAmneziaConfPath);
            this.Controls.Add(this.lblAmneziaConfPath);
            this.Controls.Add(this.btnBrowseAmneziaExe);
            this.Controls.Add(this.txtAmneziaExePath);
            this.Controls.Add(this.lblAmneziaExePath);
            this.Controls.Add(this.btnBrowseWarp);
            this.Controls.Add(this.txtWarpPath);
            this.Controls.Add(this.lblWarpPath);
            this.Controls.Add(this.txtSlot2Bat);
            this.Controls.Add(this.txtSlot2Name);
            this.Controls.Add(this.lblSlot2);
            this.Controls.Add(this.txtSlot1Bat);
            this.Controls.Add(this.txtSlot1Name);
            this.Controls.Add(this.lblSlot1);
            this.Controls.Add(this.chkUseCustomNames);
            this.Controls.Add(this.btnBrowseZapret);
            this.Controls.Add(this.txtZapretPath);
            this.Controls.Add(this.lblZapretPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки системы";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}