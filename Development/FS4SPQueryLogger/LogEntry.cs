﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace mAdcOW.FS4SPQueryLogger
{
    class LogEntry
    {
        public string Query { get; set; }
        public string Xml { get; set; }
        public NameValueCollection HttpParams { get; set; }
        public int Ypos { get; set; }
        public string RankLog { get; set; }
        public string FileName;

        public string GetQueryInfo()
        {            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Sort: " + HttpParams["sortby"]);
            sb.AppendLine("Timezone Offset: " + HttpParams["qtf_parsekw:timezone"]);
            sb.AppendLine("Language: " + HttpParams["language"]);
            sb.AppendLine("Lemmatization: " + HttpParams["qtf_lemmatize"]);
            
            XDocument doc = XDocument.Parse(Xml);
            var res = from cp in doc.Descendants("QUERYTRANSFORM")
                      where cp.Attribute("NAME").Value == "FastQT_Navigation"
                      select cp.Attribute("QUERY").Value;
            var nav = res.FirstOrDefault();
            if (nav != null)
            {
                var navigators = nav.Split(new[] {"$ "}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string navigator in navigators)
                {
                    string incExc = navigator.StartsWith("+") ? "INC" : "EXC";
                    string currentNav = navigator.Trim(' ', '$', '+', '-').Replace("\"", "").Replace("^", "");
                    sb.Append("Refiner: ");
                    sb.Append(incExc);
                    sb.Append(" ");
                    sb.Append(currentNav);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public string GetOriginalFqlAsHtml()
        {
            XDocument doc = XDocument.Parse(Xml);
            var res = from cp in doc.Descendants("QUERYTRANSFORM")
                      where cp.Attribute("NAME").Value == "Original query"
                      select cp.Attribute("QUERY").Value;
            var rawFql = res.FirstOrDefault();
            if (string.IsNullOrEmpty(rawFql)) return string.Empty;
            return "<html><body><pre>" + FormatFql(rawFql) + "</pre></body></html>";
        }

        const string IndentStr = "    ";
        public string Indent(int number)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < number; i++)
            {
                builder.Append(IndentStr);
            }
            return builder.ToString();
        }


        public string FormatFql(string rawFql)
        {
            // rawFql = 'and(or(author:"james",byline:"james"), topic:"news")';
            var indentLvl = 0;
            var builder = new StringBuilder();

            foreach (var chr in rawFql)
            {
                switch (chr)
                {
                    case '(':
                        indentLvl += 1;
                        builder.Append(chr);
                        builder.Append(Environment.NewLine);
                        builder.Append(Indent(indentLvl));
                        break;

                    case ')':
                        indentLvl -= 1;
                        builder.Append(Environment.NewLine);
                        builder.Append(Indent(indentLvl));
                        builder.Append(chr);
                        break;

                    case ',':
                        builder.Append(chr);
                        builder.Append(Environment.NewLine);
                        builder.Append(Indent(indentLvl));
                        break;

                    default:
                        builder.Append(chr);
                        break;
                }
            }
            return builder.ToString();
        }
    }
}
