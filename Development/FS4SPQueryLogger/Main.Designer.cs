using System.Drawing;

namespace mAdcOW.FS4SPQueryLogger
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.logButton = new System.Windows.Forms.Button();
            this.queryList = new System.Windows.Forms.ListBox();
            this.resultXmlBrowser = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.xmlSaveButton = new System.Windows.Forms.Button();
            this.saveXmlDialog = new System.Windows.Forms.SaveFileDialog();
            this.clearQueryListButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.qrServerLocation = new System.Windows.Forms.TextBox();
            this.txtQueryBreakDown = new System.Windows.Forms.TextBox();
            this.fqlBrowser = new System.Windows.Forms.WebBrowser();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.xml = new System.Windows.Forms.TabPage();
            this.fql = new System.Windows.Forms.TabPage();
            this.rankLog = new System.Windows.Forms.TabPage();
            this.txtRankLog = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.intervalMs = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.logfileName = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.xml.SuspendLayout();
            this.fql.SuspendLayout();
            this.rankLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // logButton
            // 
            this.logButton.BackColor = System.Drawing.SystemColors.Control;
            this.logButton.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logButton.Location = new System.Drawing.Point(13, 13);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(108, 23);
            this.logButton.TabIndex = 0;
            this.logButton.Text = "Start Monitoring";
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
            this.queryList.Size = new System.Drawing.Size(286, 293);
            this.queryList.TabIndex = 1;
            this.queryList.SelectedIndexChanged += new System.EventHandler(this.QueryListSelectedIndexChanged);
            // 
            // resultXmlBrowser
            // 
            this.resultXmlBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultXmlBrowser.Location = new System.Drawing.Point(3, 3);
            this.resultXmlBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.resultXmlBrowser.Name = "resultXmlBrowser";
            this.resultXmlBrowser.Size = new System.Drawing.Size(565, 414);
            this.resultXmlBrowser.TabIndex = 2;
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
            this.label2.Size = new System.Drawing.Size(73, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "XML Output";
            // 
            // xmlSaveButton
            // 
            this.xmlSaveButton.BackColor = System.Drawing.SystemColors.Control;
            this.xmlSaveButton.Enabled = false;
            this.xmlSaveButton.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xmlSaveButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.xmlSaveButton.Location = new System.Drawing.Point(379, 53);
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
            this.clearQueryListButton.Click += new System.EventHandler(this.ClearQueryListButtonClick);
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
            // txtQueryBreakDown
            // 
            this.txtQueryBreakDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtQueryBreakDown.Location = new System.Drawing.Point(12, 395);
            this.txtQueryBreakDown.Multiline = true;
            this.txtQueryBreakDown.Name = "txtQueryBreakDown";
            this.txtQueryBreakDown.Size = new System.Drawing.Size(287, 134);
            this.txtQueryBreakDown.TabIndex = 9;
            // 
            // fqlBrowser
            // 
            this.fqlBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fqlBrowser.Location = new System.Drawing.Point(3, 3);
            this.fqlBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.fqlBrowser.Name = "fqlBrowser";
            this.fqlBrowser.Size = new System.Drawing.Size(565, 414);
            this.fqlBrowser.TabIndex = 11;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.xml);
            this.tabControl1.Controls.Add(this.fql);
            this.tabControl1.Controls.Add(this.rankLog);
            this.tabControl1.Location = new System.Drawing.Point(306, 83);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(579, 446);
            this.tabControl1.TabIndex = 12;
            // 
            // xml
            // 
            this.xml.Controls.Add(this.resultXmlBrowser);
            this.xml.Location = new System.Drawing.Point(4, 22);
            this.xml.Name = "xml";
            this.xml.Padding = new System.Windows.Forms.Padding(3);
            this.xml.Size = new System.Drawing.Size(571, 420);
            this.xml.TabIndex = 0;
            this.xml.Text = "Result Xml";
            // 
            // fql
            // 
            this.fql.Controls.Add(this.fqlBrowser);
            this.fql.Location = new System.Drawing.Point(4, 22);
            this.fql.Name = "fql";
            this.fql.Padding = new System.Windows.Forms.Padding(3);
            this.fql.Size = new System.Drawing.Size(571, 420);
            this.fql.TabIndex = 1;
            this.fql.Text = "FQL";
            // 
            // rankLog
            // 
            this.rankLog.Controls.Add(this.txtRankLog);
            this.rankLog.Location = new System.Drawing.Point(4, 22);
            this.rankLog.Name = "rankLog";
            this.rankLog.Size = new System.Drawing.Size(571, 420);
            this.rankLog.TabIndex = 2;
            this.rankLog.Text = "Rank Log";
            this.rankLog.UseVisualStyleBackColor = true;
            // 
            // txtRankLog
            // 
            this.txtRankLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRankLog.Location = new System.Drawing.Point(0, 0);
            this.txtRankLog.Multiline = true;
            this.txtRankLog.Name = "txtRankLog";
            this.txtRankLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRankLog.Size = new System.Drawing.Size(571, 420);
            this.txtRankLog.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 379);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Query Info";
            // 
            // intervalMs
            // 
            this.intervalMs.Font = new System.Drawing.Font("Berlin Sans FB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intervalMs.Location = new System.Drawing.Point(203, 15);
            this.intervalMs.Name = "intervalMs";
            this.intervalMs.Size = new System.Drawing.Size(42, 22);
            this.intervalMs.TabIndex = 15;
            this.intervalMs.Text = "500";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(127, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Interval (ms)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(515, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Log file:";
            // 
            // logfileName
            // 
            this.logfileName.AutoSize = true;
            this.logfileName.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logfileName.Location = new System.Drawing.Point(566, 18);
            this.logfileName.Name = "logfileName";
            this.logfileName.Size = new System.Drawing.Size(0, 13);
            this.logfileName.TabIndex = 17;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 542);
            this.Controls.Add(this.logfileName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.intervalMs);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtQueryBreakDown);
            this.Controls.Add(this.qrServerLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.clearQueryListButton);
            this.Controls.Add(this.xmlSaveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.queryList);
            this.Controls.Add(this.logButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "FS4SP Query Logger - by Mikael Svenson (@mikaelsvenson - miksvenson@gmail.com)";
            this.tabControl1.ResumeLayout(false);
            this.xml.ResumeLayout(false);
            this.fql.ResumeLayout(false);
            this.rankLog.ResumeLayout(false);
            this.rankLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.ListBox queryList;
        private System.Windows.Forms.WebBrowser resultXmlBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button xmlSaveButton;
        private System.Windows.Forms.SaveFileDialog saveXmlDialog;
        private System.Windows.Forms.Button clearQueryListButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox qrServerLocation;
        private System.Windows.Forms.TextBox txtQueryBreakDown;
        private System.Windows.Forms.WebBrowser fqlBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage xml;
        private System.Windows.Forms.TabPage fql;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage rankLog;
        private System.Windows.Forms.TextBox txtRankLog;
        private System.Windows.Forms.TextBox intervalMs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label logfileName;
    }
}

