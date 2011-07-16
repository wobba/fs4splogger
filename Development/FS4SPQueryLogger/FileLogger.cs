using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Timers;
using System.Web;

namespace mAdcOW.FS4SPQueryLogger
{
    internal class FileLogger
    {
        private readonly string _logPath;
        private Action<LogEntry> _action;
        private bool _logging;
        private static readonly object _lock = new object();
        private readonly Timer _timer = new Timer(500);
        private long _lastMaxOffset;
        private readonly string _qrLocation;

        public FileLogger(string qrLocation)
        {
            _qrLocation = qrLocation;
            _logPath = Path.Combine(Environment.GetEnvironmentVariable("FASTSEARCH"), @"var\log\querylogs");
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_logging) return;
            lock (_lock)
            {
                string line = ReadLastLine(GetCurrentFile());
                int start = line.IndexOf("GET ");
                if (start == -1) return;

                int end = line.IndexOf(' ', start + 5);
                string query = line.Substring(start, end - start);
                if (query.EndsWith("nolog")) return;
                NameValueCollection param = HttpUtility.ParseQueryString(query);
                query = query.Substring(query.IndexOf('?'));

                DateTime dt = GetQueryDate(line);
                LogEntry entry = new LogEntry
                                     {
                                         Query = dt.ToString("yyyy-MM-dd HH:mm:ss") + " " + param["query"],
                                         HttpParams = query,
                                         Xml = GetQueryResultXml(query),
                                         RankLog = GetQueryRankLog(query)
                                     };
                // do new query and pass back query with QR xml
                _action.EndInvoke(_action.BeginInvoke(entry, null, null));
            }
        }

        private static DateTime GetQueryDate(string line)
        {
            int dateStart = line.IndexOf('[') + 1;
            int dateEnd = line.IndexOf(']');
            string date = line.Substring(dateStart, dateEnd - dateStart);

            DateTime dt;
            if (!DateTime.TryParseExact(date, @"dd\/MMM\/yyyy:HH:mm:ss zz00", null, DateTimeStyles.None, out dt))
            {
                if (!DateTime.TryParseExact(date, @"dd\/MMM\/yyyy:HH:mm:ss +zz00", null, DateTimeStyles.None, out dt))
                {
                    if (!DateTime.TryParseExact(date, @"dd\/MMM\/yyyy:HH:mm:ss +zz00", null, DateTimeStyles.None, out dt))
                    {
                        if (!DateTime.TryParse(date, null, DateTimeStyles.None, out dt))
                        {
                            dt = DateTime.Now;
                        }
                    }
                }
            }
            return dt;
        }

        private string GetQueryResultXml(string query)
        {
            string url = _qrLocation + "/cgi-bin/xsearch" + query + "&nolog";
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }

        private string GetQueryRankLog(string query)
        {
            string url = _qrLocation + "/cgi-bin/search" + query + "&ranklog&nolog";
            WebClient client = new WebClient();
            return client.DownloadString(url);
        }

        private string GetCurrentFile()
        {
            DirectoryInfo di = new DirectoryInfo(_logPath);
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var currentLogfile = files.OrderByDescending(f => f.Name).First();
            return currentLogfile.FullName;
        }

        public void Start(Action<LogEntry> action)
        {
            _action = action;
            _logging = true;
            _timer.Start();
        }

        public void Stop()
        {
            _logging = false;
            _timer.Stop();
        }

        public string ReadLastLine(string path)
        {
            return ReadLastLine(path, Encoding.ASCII, "\n");
        }

        public string ReadLastLine(string path, Encoding encoding, string newline)
        {
            int charsize = encoding.GetByteCount("\n");
            byte[] buffer = encoding.GetBytes(newline);

            using (
                FileStream stream = new FileStream(path, FileMode.Open, FileSystemRights.Read, FileShare.ReadWrite,
                                                   16384, FileOptions.RandomAccess))
            {
                long endpos = stream.Length / charsize;
                if (endpos == _lastMaxOffset) return string.Empty;
                _lastMaxOffset = endpos;

                for (long pos = charsize; pos < endpos; pos += charsize)
                {
                    stream.Seek(-pos, SeekOrigin.End);
                    stream.Read(buffer, 0, buffer.Length);

                    if (encoding.GetString(buffer) == newline && stream.Length != stream.Position)
                    {
                        buffer = new byte[stream.Length - stream.Position];
                        stream.Read(buffer, 0, buffer.Length);
                        return encoding.GetString(buffer);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return encoding.GetString(buffer);
            }
        }
    }
}