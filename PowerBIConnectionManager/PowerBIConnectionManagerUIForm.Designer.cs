namespace Microsoft.Samples.SqlServer.SSIS.PowerBIConnectionManager
{
    partial class PowerBIConnectionManagerUIForm
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
            this.ClientIdTxt = new System.Windows.Forms.TextBox();
            this.RedirectUriTxt = new System.Windows.Forms.TextBox();
            this.ResourceUriTxt = new System.Windows.Forms.TextBox();
            this.OAuthTxt = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.UserNameTxt = new System.Windows.Forms.TextBox();
            this.PasswordTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PowerBIDataSetsTxt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OkBtn = new System.Windows.Forms.Button();
            this.TestBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClientIdTxt
            // 
            this.ClientIdTxt.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ClientIdTxt.Location = new System.Drawing.Point(133, 20);
            this.ClientIdTxt.Name = "ClientIdTxt";
            this.ClientIdTxt.Size = new System.Drawing.Size(244, 20);
            this.ClientIdTxt.TabIndex = 0;
            // 
            // RedirectUriTxt
            // 
            this.RedirectUriTxt.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RedirectUriTxt.Location = new System.Drawing.Point(133, 46);
            this.RedirectUriTxt.Name = "RedirectUriTxt";
            this.RedirectUriTxt.Size = new System.Drawing.Size(244, 20);
            this.RedirectUriTxt.TabIndex = 1;
            // 
            // ResourceUriTxt
            // 
            this.ResourceUriTxt.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.ResourceUriTxt.Location = new System.Drawing.Point(133, 72);
            this.ResourceUriTxt.Name = "ResourceUriTxt";
            this.ResourceUriTxt.Size = new System.Drawing.Size(244, 20);
            this.ResourceUriTxt.TabIndex = 2;
            // 
            // OAuthTxt
            // 
            this.OAuthTxt.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.OAuthTxt.Location = new System.Drawing.Point(133, 98);
            this.OAuthTxt.Name = "OAuthTxt";
            this.OAuthTxt.Size = new System.Drawing.Size(244, 20);
            this.OAuthTxt.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.PasswordTxt);
            this.groupBox1.Controls.Add(this.UserNameTxt);
            this.groupBox1.Location = new System.Drawing.Point(35, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 84);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credentials";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.PowerBIDataSetsTxt);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ClientIdTxt);
            this.groupBox2.Controls.Add(this.RedirectUriTxt);
            this.groupBox2.Controls.Add(this.OAuthTxt);
            this.groupBox2.Controls.Add(this.ResourceUriTxt);
            this.groupBox2.Location = new System.Drawing.Point(35, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 164);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Power BI Connection Settings";
            // 
            // UserNameTxt
            // 
            this.UserNameTxt.ForeColor = System.Drawing.SystemColors.WindowText;
            this.UserNameTxt.Location = new System.Drawing.Point(133, 19);
            this.UserNameTxt.Name = "UserNameTxt";
            this.UserNameTxt.Size = new System.Drawing.Size(244, 20);
            this.UserNameTxt.TabIndex = 1;
            // 
            // PasswordTxt
            // 
            this.PasswordTxt.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PasswordTxt.Location = new System.Drawing.Point(133, 45);
            this.PasswordTxt.Name = "PasswordTxt";
            this.PasswordTxt.Size = new System.Drawing.Size(244, 20);
            this.PasswordTxt.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Client ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Redirect Uri";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Resource Uri";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "OAuth2 Authority Uri";
            // 
            // PowerBIDataSetsTxt
            // 
            this.PowerBIDataSetsTxt.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.PowerBIDataSetsTxt.Location = new System.Drawing.Point(133, 124);
            this.PowerBIDataSetsTxt.Name = "PowerBIDataSetsTxt";
            this.PowerBIDataSetsTxt.Size = new System.Drawing.Size(244, 20);
            this.PowerBIDataSetsTxt.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Power BI DataSets";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Username";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Password";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(367, 307);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // OkBtn
            // 
            this.OkBtn.Location = new System.Drawing.Point(286, 307);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 7;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(35, 307);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(109, 23);
            this.TestBtn.TabIndex = 8;
            this.TestBtn.Text = "Test Connection";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // PowerBIConnectionManagerUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 342);
            this.Controls.Add(this.TestBtn);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PowerBIConnectionManagerUIForm";
            this.Text = "PowerBIConnectionManagerUIForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ClientIdTxt;
        private System.Windows.Forms.TextBox RedirectUriTxt;
        private System.Windows.Forms.TextBox ResourceUriTxt;
        private System.Windows.Forms.TextBox OAuthTxt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox PasswordTxt;
        private System.Windows.Forms.TextBox UserNameTxt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PowerBIDataSetsTxt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button TestBtn;
    }
}