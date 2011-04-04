using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mAdcOW.FS4SPQueryLogger
{
    class LogEntry
    {
        public string Query { get; set; }
        public string Xml { get; set; }
        public string Html { get; set; }
        public string HttpParams { get; set; }
        public int Ypos { get; set; }
    }
}
