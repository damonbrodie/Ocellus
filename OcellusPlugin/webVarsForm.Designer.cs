namespace WebVars
{
    partial class EditWebVars
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
            this.txtJson1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtJson2 = new System.Windows.Forms.TextBox();
            this.txtJson3 = new System.Windows.Forms.TextBox();
            this.txtJson4 = new System.Windows.Forms.TextBox();
            this.txtJson5 = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.btnTest3 = new System.Windows.Forms.Button();
            this.btnTest4 = new System.Windows.Forms.Button();
            this.btnTest5 = new System.Windows.Forms.Button();
            this.dataGridTest = new System.Windows.Forms.DataGridView();
            this.Attribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerStatus = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabResponse = new System.Windows.Forms.TabPage();
            this.txtNumVariables = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtWarnings = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabVariables = new System.Windows.Forms.TabPage();
            this.tabJson = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTest)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabResponse.SuspendLayout();
            this.tabVariables.SuspendLayout();
            this.tabJson.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtJson1
            // 
            this.txtJson1.Location = new System.Drawing.Point(13, 174);
            this.txtJson1.Name = "txtJson1";
            this.txtJson1.Size = new System.Drawing.Size(305, 20);
            this.txtJson1.TabIndex = 0;
            this.txtJson1.TextChanged += new System.EventHandler(this.txtJson1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "List of JSON subscriptions (URLs):";
            // 
            // txtJson2
            // 
            this.txtJson2.Location = new System.Drawing.Point(13, 226);
            this.txtJson2.Name = "txtJson2";
            this.txtJson2.Size = new System.Drawing.Size(305, 20);
            this.txtJson2.TabIndex = 2;
            this.txtJson2.TextChanged += new System.EventHandler(this.txtJson2_TextChanged);
            // 
            // txtJson3
            // 
            this.txtJson3.Location = new System.Drawing.Point(13, 276);
            this.txtJson3.Name = "txtJson3";
            this.txtJson3.Size = new System.Drawing.Size(305, 20);
            this.txtJson3.TabIndex = 4;
            this.txtJson3.TextChanged += new System.EventHandler(this.txtJson3_TextChanged);
            // 
            // txtJson4
            // 
            this.txtJson4.Location = new System.Drawing.Point(13, 327);
            this.txtJson4.Name = "txtJson4";
            this.txtJson4.Size = new System.Drawing.Size(305, 20);
            this.txtJson4.TabIndex = 6;
            this.txtJson4.TextChanged += new System.EventHandler(this.txtJson4_TextChanged);
            // 
            // txtJson5
            // 
            this.txtJson5.Location = new System.Drawing.Point(13, 383);
            this.txtJson5.Name = "txtJson5";
            this.txtJson5.Size = new System.Drawing.Size(305, 20);
            this.txtJson5.TabIndex = 8;
            this.txtJson5.TextChanged += new System.EventHandler(this.txtJson5_TextChanged);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(243, 439);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 11;
            this.btnSubmit.Text = "Save";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(82, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnTest1
            // 
            this.btnTest1.Enabled = false;
            this.btnTest1.Location = new System.Drawing.Point(324, 172);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(75, 23);
            this.btnTest1.TabIndex = 1;
            this.btnTest1.Text = "Test";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.btnTest1_Click);
            // 
            // btnTest2
            // 
            this.btnTest2.Enabled = false;
            this.btnTest2.Location = new System.Drawing.Point(324, 224);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(75, 23);
            this.btnTest2.TabIndex = 3;
            this.btnTest2.Text = "Test";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // btnTest3
            // 
            this.btnTest3.Enabled = false;
            this.btnTest3.Location = new System.Drawing.Point(324, 274);
            this.btnTest3.Name = "btnTest3";
            this.btnTest3.Size = new System.Drawing.Size(75, 23);
            this.btnTest3.TabIndex = 5;
            this.btnTest3.Text = "Test";
            this.btnTest3.UseVisualStyleBackColor = true;
            this.btnTest3.Click += new System.EventHandler(this.btnTest3_Click);
            // 
            // btnTest4
            // 
            this.btnTest4.Enabled = false;
            this.btnTest4.Location = new System.Drawing.Point(324, 325);
            this.btnTest4.Name = "btnTest4";
            this.btnTest4.Size = new System.Drawing.Size(75, 23);
            this.btnTest4.TabIndex = 7;
            this.btnTest4.Text = "Test";
            this.btnTest4.UseVisualStyleBackColor = true;
            this.btnTest4.Click += new System.EventHandler(this.btnTest4_Click);
            // 
            // btnTest5
            // 
            this.btnTest5.Enabled = false;
            this.btnTest5.Location = new System.Drawing.Point(324, 381);
            this.btnTest5.Name = "btnTest5";
            this.btnTest5.Size = new System.Drawing.Size(75, 23);
            this.btnTest5.TabIndex = 9;
            this.btnTest5.Text = "Test";
            this.btnTest5.UseVisualStyleBackColor = true;
            this.btnTest5.Click += new System.EventHandler(this.btnTest5_Click);
            // 
            // dataGridTest
            // 
            this.dataGridTest.AllowUserToAddRows = false;
            this.dataGridTest.AllowUserToDeleteRows = false;
            this.dataGridTest.AllowUserToResizeRows = false;
            this.dataGridTest.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridTest.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.Value});
            this.dataGridTest.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridTest.Location = new System.Drawing.Point(0, 0);
            this.dataGridTest.Name = "dataGridTest";
            this.dataGridTest.RowHeadersVisible = false;
            this.dataGridTest.Size = new System.Drawing.Size(391, 441);
            this.dataGridTest.TabIndex = 14;
            // 
            // Attribute
            // 
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.MinimumWidth = 10;
            this.Attribute.Name = "Attribute";
            this.Attribute.Width = 150;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 10;
            this.Value.Name = "Value";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(15, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = " Server Status:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtServerStatus
            // 
            this.txtServerStatus.Location = new System.Drawing.Point(103, 70);
            this.txtServerStatus.Name = "txtServerStatus";
            this.txtServerStatus.ReadOnly = true;
            this.txtServerStatus.Size = new System.Drawing.Size(253, 20);
            this.txtServerStatus.TabIndex = 15;
            this.txtServerStatus.Text = " ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabResponse);
            this.tabControl1.Controls.Add(this.tabVariables);
            this.tabControl1.Controls.Add(this.tabJson);
            this.tabControl1.Location = new System.Drawing.Point(415, 9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(399, 467);
            this.tabControl1.TabIndex = 16;
            // 
            // tabResponse
            // 
            this.tabResponse.Controls.Add(this.txtNumVariables);
            this.tabResponse.Controls.Add(this.label7);
            this.tabResponse.Controls.Add(this.txtWarnings);
            this.tabResponse.Controls.Add(this.label6);
            this.tabResponse.Controls.Add(this.txtServerStatus);
            this.tabResponse.Controls.Add(this.label2);
            this.tabResponse.Location = new System.Drawing.Point(4, 22);
            this.tabResponse.Name = "tabResponse";
            this.tabResponse.Size = new System.Drawing.Size(391, 441);
            this.tabResponse.TabIndex = 2;
            this.tabResponse.Text = "Response";
            this.tabResponse.UseVisualStyleBackColor = true;
            // 
            // txtNumVariables
            // 
            this.txtNumVariables.Location = new System.Drawing.Point(103, 222);
            this.txtNumVariables.Name = "txtNumVariables";
            this.txtNumVariables.ReadOnly = true;
            this.txtNumVariables.Size = new System.Drawing.Size(40, 20);
            this.txtNumVariables.TabIndex = 19;
            this.txtNumVariables.Text = " ";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(15, 225);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "# Variables:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtWarnings
            // 
            this.txtWarnings.Location = new System.Drawing.Point(103, 144);
            this.txtWarnings.Name = "txtWarnings";
            this.txtWarnings.ReadOnly = true;
            this.txtWarnings.Size = new System.Drawing.Size(253, 20);
            this.txtWarnings.TabIndex = 17;
            this.txtWarnings.Text = " ";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(34, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Warnings:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabVariables
            // 
            this.tabVariables.Controls.Add(this.dataGridTest);
            this.tabVariables.Location = new System.Drawing.Point(4, 22);
            this.tabVariables.Name = "tabVariables";
            this.tabVariables.Padding = new System.Windows.Forms.Padding(3);
            this.tabVariables.Size = new System.Drawing.Size(391, 441);
            this.tabVariables.TabIndex = 0;
            this.tabVariables.Text = "Variables";
            this.tabVariables.UseVisualStyleBackColor = true;
            // 
            // tabJson
            // 
            this.tabJson.Controls.Add(this.label5);
            this.tabJson.Controls.Add(this.txtJson);
            this.tabJson.Location = new System.Drawing.Point(4, 22);
            this.tabJson.Name = "tabJson";
            this.tabJson.Padding = new System.Windows.Forms.Padding(3);
            this.tabJson.Size = new System.Drawing.Size(391, 441);
            this.tabJson.TabIndex = 1;
            this.tabJson.Text = "JSON";
            this.tabJson.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "Source JSON Output:";
            // 
            // txtJson
            // 
            this.txtJson.Location = new System.Drawing.Point(16, 50);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(358, 370);
            this.txtJson.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(30, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(358, 67);
            this.label3.TabIndex = 17;
            this.label3.Text = "Web Variables allow you to subscribe to different feeds from the internet that pr" +
    "ovide dynamically changing variables that you can acces in VoiceAttack.";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkRed;
            this.label4.Location = new System.Drawing.Point(45, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(306, 40);
            this.label4.TabIndex = 18;
            this.label4.Text = "Use Web Variables with caution in your VoiceAttack Profile.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EditWebVars
            // 
            this.AcceptButton = this.btnSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(823, 485);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnTest5);
            this.Controls.Add(this.btnTest4);
            this.Controls.Add(this.btnTest3);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.btnTest1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtJson5);
            this.Controls.Add(this.txtJson4);
            this.Controls.Add(this.txtJson3);
            this.Controls.Add(this.txtJson2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtJson1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditWebVars";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ocellus Web Variables";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EditWebVars_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTest)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabResponse.ResumeLayout(false);
            this.tabResponse.PerformLayout();
            this.tabVariables.ResumeLayout(false);
            this.tabJson.ResumeLayout(false);
            this.tabJson.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtJson1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtJson2;
        private System.Windows.Forms.TextBox txtJson3;
        private System.Windows.Forms.TextBox txtJson4;
        private System.Windows.Forms.TextBox txtJson5;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTest1;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.Button btnTest3;
        private System.Windows.Forms.Button btnTest4;
        private System.Windows.Forms.Button btnTest5;
        private System.Windows.Forms.DataGridView dataGridTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabResponse;
        private System.Windows.Forms.TabPage tabVariables;
        private System.Windows.Forms.TabPage tabJson;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNumVariables;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtWarnings;
    }
}