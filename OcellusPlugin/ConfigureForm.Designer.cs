namespace ConfigureForm
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_email = new System.Windows.Forms.TextBox();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_validation_email = new System.Windows.Forms.Label();
            this.lbl_validation_password = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_Voice = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_Startup = new System.Windows.Forms.TextBox();
            this.txt_EDDN_Updates = new System.Windows.Forms.TextBox();
            this.txt_Updates = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 488);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Email Address:";
            // 
            // txt_email
            // 
            this.txt_email.Location = new System.Drawing.Point(41, 504);
            this.txt_email.Name = "txt_email";
            this.txt_email.Size = new System.Drawing.Size(398, 20);
            this.txt_email.TabIndex = 4;
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(41, 580);
            this.txt_password.Name = "txt_password";
            this.txt_password.PasswordChar = '*';
            this.txt_password.Size = new System.Drawing.Size(398, 20);
            this.txt_password.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 564);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(279, 640);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(97, 21);
            this.btnSubmit.TabIndex = 7;
            this.btnSubmit.Text = "Save";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(103, 640);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 21);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(37, 408);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Log in with your Elite: Dangerous account";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_validation_email
            // 
            this.lbl_validation_email.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_validation_email.ForeColor = System.Drawing.Color.DarkRed;
            this.lbl_validation_email.Location = new System.Drawing.Point(41, 527);
            this.lbl_validation_email.Name = "lbl_validation_email";
            this.lbl_validation_email.Size = new System.Drawing.Size(398, 19);
            this.lbl_validation_email.TabIndex = 7;
            this.lbl_validation_email.Text = "Invalid Email Address";
            this.lbl_validation_email.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_validation_email.Visible = false;
            // 
            // lbl_validation_password
            // 
            this.lbl_validation_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_validation_password.ForeColor = System.Drawing.Color.DarkRed;
            this.lbl_validation_password.Location = new System.Drawing.Point(41, 603);
            this.lbl_validation_password.Name = "lbl_validation_password";
            this.lbl_validation_password.Size = new System.Drawing.Size(398, 18);
            this.lbl_validation_password.TabIndex = 8;
            this.lbl_validation_password.Text = "Enter your password";
            this.lbl_validation_password.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_validation_password.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(37, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(34, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(293, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Most text to speech is done inside of VoiceAttack.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(34, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(439, 32);
            this.label5.TabIndex = 10;
            this.label5.Text = "There are however a few places where things happen independently of voice command" +
    "s.  Set the text for these Text-to-Speech actions here.";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmb_Voice
            // 
            this.cmb_Voice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Voice.FormattingEnabled = true;
            this.cmb_Voice.Location = new System.Drawing.Point(37, 178);
            this.cmb_Voice.Name = "cmb_Voice";
            this.cmb_Voice.Size = new System.Drawing.Size(401, 21);
            this.cmb_Voice.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(37, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Voice:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 220);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Text to Speech on startup:";
            // 
            // txt_Startup
            // 
            this.txt_Startup.Location = new System.Drawing.Point(37, 236);
            this.txt_Startup.Name = "txt_Startup";
            this.txt_Startup.Size = new System.Drawing.Size(401, 20);
            this.txt_Startup.TabIndex = 1;
            // 
            // txt_EDDN_Updates
            // 
            this.txt_EDDN_Updates.Location = new System.Drawing.Point(37, 292);
            this.txt_EDDN_Updates.Name = "txt_EDDN_Updates";
            this.txt_EDDN_Updates.Size = new System.Drawing.Size(401, 20);
            this.txt_EDDN_Updates.TabIndex = 2;
            // 
            // txt_Updates
            // 
            this.txt_Updates.Location = new System.Drawing.Point(37, 347);
            this.txt_Updates.Name = "txt_Updates";
            this.txt_Updates.Size = new System.Drawing.Size(401, 20);
            this.txt_Updates.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(37, 276);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(175, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Text to Speech for EDDN Updates:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(37, 331);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(224, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Text to Speech for available Ocellus Updates:";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(37, 434);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(401, 50);
            this.label10.TabIndex = 19;
            this.label10.Text = "These credentials are stored only on this computer and are used only to log into " +
    "the Official Fronter Companion API server.  Your credentials are never shared wi" +
    "th Ocellus.io or anyone else.";
            // 
            // Login
            // 
            this.AcceptButton = this.btnSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(479, 685);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt_Updates);
            this.Controls.Add(this.txt_EDDN_Updates);
            this.Controls.Add(this.txt_Startup);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmb_Voice);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbl_validation_password);
            this.Controls.Add(this.lbl_validation_email);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_email);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ocellus Assistant Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_email;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_validation_email;
        private System.Windows.Forms.Label lbl_validation_password;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmb_Voice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_Startup;
        private System.Windows.Forms.TextBox txt_EDDN_Updates;
        private System.Windows.Forms.TextBox txt_Updates;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}