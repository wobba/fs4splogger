namespace mAdcOW.FS4SPQueryLogger
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.logButton = new System.Windows.Forms.Button();
            this.queryList = new System.Windows.Forms.ListBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.xmlSaveButton = new System.Windows.Forms.Button();
            this.saveXmlDialog = new System.Windows.Forms.SaveFileDialog();
            this.clearQueryListButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.qrServerLocation = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // logButton
            // 
            this.logButton.BackColor = System.Drawing.SystemColors.Control;
            this.logButton.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logButton.Location = new System.Drawing.Point(13, 13);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(92, 23);
            this.logButton.TabIndex = 0;
            this.logButton.Text = "Start Logging";
            this.logButton.UseVisualStyleBackColor = false;
            this.logButton.Click += new System.EventHandler(this.LogButtonClick);
            // 
            // queryList
            // 
            this.queryList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.queryList.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryList.FormattingEnabled = true;
            this.queryList.ItemHeight = 17;
            this.queryList.Location = new System.Drawing.Point(13, 83);
            this.queryList.Name = "queryList";
            this.queryList.Size = new System.Drawing.Size(286, 446);
            this.queryList.TabIndex = 1;
            this.queryList.SelectedIndexChanged += new System.EventHandler(this.QueryListSelectedIndexChanged);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(306, 83);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(579, 446);
            this.webBrowser1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Query List";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(303, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "View XML";
            // 
            // xmlSaveButton
            // 
            this.xmlSaveButton.BackColor = System.Drawing.SystemColors.Control;
            this.xmlSaveButton.Enabled = false;
            this.xmlSaveButton.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlSaveButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.xmlSaveButton.Location = new System.Drawing.Point(370, 53);
            this.xmlSaveButton.Name = "xmlSaveButton";
            this.xmlSaveButton.Size = new System.Drawing.Size(53, 23);
            this.xmlSaveButton.TabIndex = 5;
            this.xmlSaveButton.Text = "Export";
            this.xmlSaveButton.UseVisualStyleBackColor = false;
            this.xmlSaveButton.Click += new System.EventHandler(this.XmlSaveButtonClick);
            // 
            // saveXmlDialog
            // 
            this.saveXmlDialog.Filter = "XML files (*.xml)|*.xml";
            // 
            // clearQueryListButton
            // 
            this.clearQueryListButton.BackColor = System.Drawing.SystemColors.Control;
            this.clearQueryListButton.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearQueryListButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.clearQueryListButton.Location = new System.Drawing.Point(78, 53);
            this.clearQueryListButton.Name = "clearQueryListButton";
            this.clearQueryListButton.Size = new System.Drawing.Size(53, 23);
            this.clearQueryListButton.TabIndex = 6;
            this.clearQueryListButton.Text = "Clear";
            this.clearQueryListButton.UseVisualStyleBackColor = false;
            this.clearQueryListButton.Click += new System.EventHandler(this.clearQueryListButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(303, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "QR Server";
            // 
            // qrServerLocation
            // 
            this.qrServerLocation.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qrServerLocation.Location = new System.Drawing.Point(366, 15);
            this.qrServerLocation.Name = "qrServerLocation";
            this.qrServerLocation.Size = new System.Drawing.Size(134, 22);
            this.qrServerLocation.TabIndex = 8;
            this.qrServerLocation.Text = "http://localhost:13280";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(897, 542);
            this.Controls.Add(this.qrServerLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.clearQueryListButton);
            this.Controls.Add(this.xmlSaveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.queryList);
            this.Controls.Add(this.logButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "FS4SP Query Logger - by Mikael Svenson (@mikaelsvenson)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.ListBox queryList;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button xmlSaveButton;
        private System.Windows.Forms.SaveFileDialog saveXmlDialog;
        private System.Windows.Forms.Button clearQueryListButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox qrServerLocation;
    }
}

