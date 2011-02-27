using System;
using System.IO;
using System.Windows.Forms;

namespace mAdcOW.FS4SPQueryLogger
{
    public partial class Form1 : Form
    {
        private bool _isLogging;
        private readonly FileLogger _logger = new FileLogger();
        private readonly Action<LogEntry> _act;

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
        }

        private void LogButtonClick(object sender, EventArgs e)
        {
            if (!_isLogging)
            {
                _isLogging = true;
                logButton.Text = "Stop Logging";
                queryList.DisplayMember = "Query";
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
            var entry = (LogEntry)queryList.SelectedItem;
            if (entry == null) return;
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
                    var entry = (LogEntry)queryList.SelectedItem;
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