﻿using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace mAdcOW.FS4SPQueryLogger
{
    public partial class Form1 : Form
    {
        private bool _isLogging;
        private FileLogger _logger;
        private readonly Action<LogEntry> _act;
        private LogEntry _lastEntry;

        public Form1()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FASTSEARCH")))
            {
                MessageBox.Show("FAST for SharePoint is not installed");
                Environment.Exit(0);
            }

            InitializeComponent();
            _act = query => queryList.Invoke(new MethodInvoker(
                                                 () => queryList.Items.Add(query)));
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var entry = (LogEntry)queryList.SelectedItem;
            if (webBrowser1.Document.Body != null)
            {
                // set the y-position for the element
                webBrowser1.Document.Window.ScrollTo(0, entry.Ypos);
            }
        }

        private void LogButtonClick(object sender, EventArgs e)
        {
            if (!_isLogging)
            {
                _isLogging = true;
                logButton.Text = "Stop Logging";
                queryList.DisplayMember = "Query";
                _logger = new FileLogger(qrServerLocation.Text.Trim(new[] {' ', '/'}));
                _logger.Start(_act);
            }
            else
            {
                _isLogging = false;
                logButton.Text = "Start Logging";
                _logger.Stop();
            }
        }

        private void QueryListSelectedIndexChanged(object sender, EventArgs e)
        {
            xmlSaveButton.Enabled = true;
            var entry = (LogEntry) queryList.SelectedItem;
            if (entry == null) return;            
            if (_lastEntry != null && webBrowser1.Document.Body != null)
            {
                // save the current Y position
                _lastEntry.Ypos = webBrowser1.Document.Body.ScrollTop;
            }
            _lastEntry = entry;
            webBrowser1.DocumentText = entry.Html;            
        }

        private void XmlSaveButtonClick(object sender, EventArgs e)
        {
            if (queryList.SelectedItem == null)
            {
                MessageBox.Show("Pick a query in the list before exporting");
                return;
            }
            if (saveXmlDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveXmlDialog.OpenFile()))
                {
                    var entry = (LogEntry) queryList.SelectedItem;
                    writer.Write(entry.Xml);
                }
            }
        }

        private void clearQueryListButton_Click(object sender, EventArgs e)
        {
            queryList.Items.Clear();
            xmlSaveButton.Enabled = false;
            webBrowser1.DocumentText = "";
        }
    }
}