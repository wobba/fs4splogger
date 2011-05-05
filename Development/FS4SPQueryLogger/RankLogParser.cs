using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mAdcOW.FS4SPQueryLogger
{
    internal class RankLogParser
    {
        public static string Parse(string ranklog)
        {
            var hits = ranklog.Split(new[] { "###/" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sbLog = new StringBuilder();
            foreach (string hit in hits)
            {
                string[] hitLines = Regex.Split(hit, @"\n");
                string url = hitLines.Where(l => l.StartsWith("#url ")).FirstOrDefault();
                if (string.IsNullOrEmpty(url)) continue;
                url = url.Replace("#url ", "").Trim();
                string title = hitLines.Where(l => l.StartsWith("#title ")).FirstOrDefault();
                if (!string.IsNullOrEmpty(title))
                {
                    title = title.Replace("#title ", "").Trim();
                }
                string hitNumber = hitLines.Where(l => l.StartsWith("####")).First().Trim(' ', '#');
                string report = RankReport(hit);
                sbLog.AppendLine("Hit: " + hitNumber);
                sbLog.AppendLine("Title: " + title);
                sbLog.AppendLine("Url: " + url);
                sbLog.AppendLine(report);
                sbLog.AppendLine("############################");
            }
            return sbLog.ToString();
        }

        private static string RankReport(string ranklog)
        {
            StringBuilder rankLogReport = new StringBuilder();
            var lines = Regex.Split(ranklog, @"\n");
            foreach (string line in lines)
            {
                if (line.StartsWith("#ranklog"))
                {
                    //$res = [regex]::Split( $rank, "\sOP\s" ) 
                    var res = Regex.Split(line, @"\sOP\s");
                    string ph = string.Empty;

                    //##
                    foreach (var r in res)
                    {
                        if (r.Contains("firstocc=") && !r.Contains("msynthcat"))
                        {
                            Match match = Regex.Match(r, @"\:(.+?) TERMINDEX");
                            string term = string.Empty;
                            if (match.Success)
                            {
                                term = match.Groups[1].Value;
                                term = term.Replace("T", "").Replace("L", "");
                            }
                            string calc = string.Empty;
                            match = Regex.Match(r, @"\(\(\(\((.+?)\)\*");
                            if (match.Success)
                            {
                                calc = match.Groups[1].Value;
                            }

                            double fnum = Convert.ToDouble(calc.Substring(0, calc.IndexOf('+')));
                            var firstocc = r.Substring(r.IndexOf(" firstocc=") + 10);
                            firstocc = firstocc.Substring(0, firstocc.IndexOf(' '));

                            calc = calc.Substring(calc.IndexOf("+") + 1);
                            double nnum = Convert.ToDouble(calc.Substring(0, calc.IndexOf("+")));
                            var numoccs = r.Substring(r.IndexOf(" numoccs=") + 9);
                            numoccs = numoccs.Substring(0, numoccs.IndexOf(' '));

                            calc = calc.Substring(calc.IndexOf("+") + 1);
                            double eenum = Convert.ToInt32(calc.Substring(0, calc.IndexOf("+")));

                            var extnumoccs = r.Substring(r.LastIndexOf(" extnumoccs=") + 12);
                            extnumoccs = extnumoccs.Substring(0, extnumoccs.IndexOf(' '));
                            double xnum = Convert.ToDouble(calc.Substring(calc.IndexOf("+") + 1));

                            var contextmap = r.Substring(r.LastIndexOf(" contextmap=") + 12);
                            contextmap = contextmap.Substring(0, contextmap.IndexOf(' '));
                            var level = Context( contextmap);
                            var allboost = r.Substring(r.LastIndexOf(' ') + 1);

                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var context = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var proximity = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var comcontx = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var oppboost = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('=')));

                            double xsum = context / (fnum + nnum + eenum + xnum);

                            rankLogReport.AppendFormat("Query term: {0}\r\n", term);
                            rankLogReport.AppendFormat("  Context score...............: {0}\r\n", context);
                            if (fnum != 0)
                            {
                                fnum = Math.Round(fnum * xsum, 0);
                                rankLogReport.AppendFormat("    First occurence position/score......: {0}/{1}\r\n",
                                                           firstocc, fnum);
                            }
                            if (nnum != 0)
                            {
                                nnum = Math.Round(nnum * xsum, 0);
                                rankLogReport.AppendFormat("    Number of hits/score in context.....: {0}/{1}\r\n",
                                                           numoccs, nnum);
                            }


                            if (eenum != 0)
                            {
                                eenum = Math.Round(eenum * xsum, 0);
                                rankLogReport.AppendFormat("    Anchor & query text hits/score in...: {0}/{1}\r\n",
                                                           extnumoccs, eenum);
                            }

                            if (xnum != 0)
                            {
                                xnum = Math.Round(xnum * xsum, 0);
                                rankLogReport.AppendFormat("    Context level/score.................: {0}/{1}\r\n",
                                                           level, xnum);
                            }
                            if (proximity != 0)
                            {
                                rankLogReport.AppendFormat("  Proximity score.............: {0}\r\n", proximity);
                            }
                            if (comcontx != 0)
                            {
                                rankLogReport.AppendFormat("  Common-context score........: {0}\r\n", comcontx);
                            }
                            if (oppboost != 0)
                            {
                                rankLogReport.AppendFormat("  Operator score..............: {0}\r\n", oppboost);
                            }
                        }
                        else if (r.StartsWith("PHRASE,"))
                        {
                            Match match = Regex.Match(r, @"\:(.+?) TERMINDEX");
                            if (match.Success)
                            {
                                ph = match.Groups[1].Value;
                                ph = ph.Replace("T", "");
                                ph = ph.Replace("L", "");
                                ph = ph.Replace("bt1.bidx", "");
                            }
                        }
                        else if (r.StartsWith("XRANK,"))
                        {
                            long rank = Convert.ToInt64(r.Substring(r.LastIndexOf('=') + 1));
                            if (rank > 2147483648)
                            {
                                rank = rank - 4294967296;
                                ph = " Term: " + ph + " (subtracted from Context boost)";
                            }
                            else
                            {
                                ph = "     Term: " + ph;
                            }
                            rankLogReport.AppendFormat("XRANK score...................: {0} {1}\r\n", rank, ph);
                        }
                        else if (r.StartsWith("XRANKTERM,"))
                        {
                            //   $ph = @()                 
                            Match match = Regex.Match(r, @"\.bidx(.+?)T");
                            if (match.Success)
                            {
                                ph = match.Groups[1].Value;
                                long rank = Convert.ToInt64(r.Substring(r.LastIndexOf('=') + 1));
                                if (rank > 2147483648)
                                {
                                    rank = rank - 4294967296;
                                    ph = " Term: " + ph + " (subtracted from Context boost)";
                                }
                                else
                                {
                                    ph = "     Term: " + ph;
                                }
                                rankLogReport.AppendFormat("XRANK score...................: {0} {1}\r\n", rank, ph);
                            }
                        }
                        else if (r.StartsWith("STATICRANK,"))
                        {
                            var rank = r.Substring(r.LastIndexOf('=') + 1);

                            //var rank = r.Substring(r.LastIndexOf(")=") + 2);
                            //rank = rank.Substring(0, rank.IndexOf(')'));

                            rankLogReport.AppendFormat("Static rank score.............: {0}\r\n", rank);
                        }

                        else if (r.StartsWith("FRESHNESS,"))
                        {
                            var rank = r.Substring(r.LastIndexOf(")=") + 2);
                            var totrank = r.Substring(r.LastIndexOf('=') + 1);

                            rank = rank.Substring(0, rank.IndexOf(')'));
                            totrank = totrank.Substring(0, totrank.LastIndexOf(' '));

                            rankLogReport.AppendFormat("Freshness score...............: {0}\r\n", rank);
                            rankLogReport.AppendFormat("Total Rank score.........: {0}\r\n", totrank);
                        }
                    }
                }
            }
            return rankLogReport.ToString();
        }

        static string Context(string inp)
        {
            uint input = uint.Parse(inp);
            uint digit = 2147483648;
            string map = "";

            for (var i = 0; i < 32; i++)
            {
                if ((input & digit) != 0)
                {
                    map = map + (32 - i) + ", ";
                }
                digit = digit >> 1;
            }
            map = map.Substring(0, map.LastIndexOf(','));
            return map;
        }
    }
}