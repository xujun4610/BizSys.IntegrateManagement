namespace BizSys.IntegrateManagement.ServiceTest
{
    partial class Form2
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEndPoint = new System.Windows.Forms.TextBox();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtCondition = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbResource = new System.Windows.Forms.ComboBox();
            this.cmbMethods = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(176, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Resource";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(26, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "EndPoint";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(634, 22);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Parameters";
            // 
            // txtEndPoint
            // 
            this.txtEndPoint.Location = new System.Drawing.Point(21, 36);
            this.txtEndPoint.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtEndPoint.Name = "txtEndPoint";
            this.txtEndPoint.Size = new System.Drawing.Size(156, 21);
            this.txtEndPoint.TabIndex = 4;
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(636, 36);
            this.txtParameters.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(266, 21);
            this.txtParameters.TabIndex = 6;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(290, 102);
            this.txtResult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(566, 329);
            this.txtResult.TabIndex = 8;
            // 
            // txtCondition
            // 
            this.txtCondition.Location = new System.Drawing.Point(21, 102);
            this.txtCondition.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtCondition.Multiline = true;
            this.txtCondition.Name = "txtCondition";
            this.txtCondition.Size = new System.Drawing.Size(246, 329);
            this.txtCondition.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(19, 87);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Condition";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(288, 87);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "Resutl";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(290, 66);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 18);
            this.button1.TabIndex = 12;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbResource
            // 
            this.cmbResource.FormattingEnabled = true;
            this.cmbResource.Location = new System.Drawing.Point(178, 36);
            this.cmbResource.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbResource.Name = "cmbResource";
            this.cmbResource.Size = new System.Drawing.Size(362, 20);
            this.cmbResource.TabIndex = 13;
            this.cmbResource.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            this.cmbResource.TextUpdate += new System.EventHandler(this.TextUpdate);
            this.cmbResource.SelectedValueChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // cmbMethods
            // 
            this.cmbMethods.FormattingEnabled = true;
            this.cmbMethods.Location = new System.Drawing.Point(544, 36);
            this.cmbMethods.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbMethods.Name = "cmbMethods";
            this.cmbMethods.Size = new System.Drawing.Size(86, 20);
            this.cmbMethods.TabIndex = 14;
            this.cmbMethods.SelectedIndexChanged += new System.EventHandler(this.cmbMethods_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(542, 22);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "Method";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(364, 66);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(56, 18);
            this.btnCopy.TabIndex = 16;
            this.btnCopy.Text = "复制";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 439);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbMethods);
            this.Controls.Add(this.cmbResource);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCondition);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtParameters);
            this.Controls.Add(this.txtEndPoint);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEndPoint;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtCondition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cmbResource;
        private System.Windows.Forms.ComboBox cmbMethods;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCopy;
    }
}