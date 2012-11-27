using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace mAdcOW.FS4SPQueryLogger
{
    public partial class Main : Form
    {
        private bool _isLogging;
        private FileLogger _logger;
        private readonly Action<LogEntry> _action;
        private LogEntry _lastEntry;

        public Main()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FASTSEARCH")))
            {
                MessageBox.Show("FAST for SharePoint is not installed");
                Environment.Exit(0);
            }

            try
            {
                // Try to read the log folder
                var logPath = Path.Combine(Environment.GetEnvironmentVariable("FASTSEARCH"), @"var\log\querylogs");
                DirectoryInfo di = new DirectoryInfo(logPath);
                FileSystemInfo[] fileSystemInfos = di.GetFileSystemInfos();
                if (fileSystemInfos.Length > 0)
                {
                    using (FileStream stream = new FileStream(fileSystemInfos[0].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.ReadByte();
                    }
                }
                else
                {
                    MessageBox.Show("No log files available");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }

            InitializeComponent();
            _action = logEntry => queryList.Invoke(new MethodInvoker(() =>
            {
                queryList.Items.Add(logEntry);
                logfileName.Text = logEntry.FileName;
            }));
            resultXmlBrowser.DocumentCompleted += WebBrowser1DocumentCompleted;
        }

        private void WebBrowser1DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var entry = (LogEntry)queryList.SelectedItem;
            if (resultXmlBrowser.Document != null && resultXmlBrowser.Document.Body != null)
            {
                // set the y-position for the element
                resultXmlBrowser.Document.Window.ScrollTo(0, entry.Ypos);
            }
            string tempFile = resultXmlBrowser.Url.LocalPath;
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
            }
        }

        private void LogButtonClick(object sender, EventArgs e)
        {
            if (!_isLogging)
            {
                _isLogging = true;
                logButton.Text = "Stop Monitoring";
                queryList.DisplayMember = "Query";
                int interval;
                if (!int.TryParse(intervalMs.Text, out interval)) interval = 500;
                _logger = new FileLogger(qrServerLocation.Text.Trim(new[] { ' ', '/' }), interval, this);
                _logger.Start(_action);
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
            if (_lastEntry != null && resultXmlBrowser.Document != null && resultXmlBrowser.Document.Body != null)
            {
                // save the current Y position
                _lastEntry.Ypos = resultXmlBrowser.Document.Body.ScrollTop;
            }
            _lastEntry = entry;

            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), "xml");
            File.WriteAllText(tempFile, entry.Xml);
            resultXmlBrowser.Navigate(tempFile);

            fqlBrowser.DocumentText = entry.GetOriginalFqlAsHtml();
            txtQueryBreakDown.Text = entry.GetQueryInfo();
            txtRankLog.Text = RankLogParser.Parse(entry.RankLog);
            rankLogRawText.Text = entry.RankLog;
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

        private void ClearQueryListButtonClick(object sender, EventArgs e)
        {
            queryList.Items.Clear();
            xmlSaveButton.Enabled = false;
            resultXmlBrowser.DocumentText = "";
        }
    }
}