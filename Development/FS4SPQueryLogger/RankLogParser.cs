using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mAdcOW.FS4SPQueryLogger
{
    /// <summary>
    /// Code copied from http://gallery.technet.microsoft.com/scriptcenter/16448603-ab52-4e28-a1a4-4e4d9ddb4dd9
    /// and converted to C# with minor modifications
    /// </summary>
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

                    bool haveFreshness = false;
                    string staticRankScore = string.Empty;
                    string finalRankScore = string.Empty;
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

                            double occuranceScore = Convert.ToDouble(calc.Substring(0, calc.IndexOf('+')));
                            var firstOccurancePosition = r.Substring(r.IndexOf(" firstocc=") + 10);
                            firstOccurancePosition = firstOccurancePosition.Substring(0, firstOccurancePosition.IndexOf(' '));

                            calc = calc.Substring(calc.IndexOf("+") + 1);
                            double scoreInContext = Convert.ToDouble(calc.Substring(0, calc.IndexOf("+")));
                            var hitsInContext = r.Substring(r.IndexOf(" numoccs=") + 9);
                            hitsInContext = hitsInContext.Substring(0, hitsInContext.IndexOf(' '));

                            calc = calc.Substring(calc.IndexOf("+") + 1);
                            double scoreInAnchorAndQuery = Convert.ToInt32(calc.Substring(0, calc.IndexOf("+")));

                            var hitsInAnchorAndQuery = r.Substring(r.LastIndexOf(" extnumoccs=") + 12);
                            hitsInAnchorAndQuery = hitsInAnchorAndQuery.Substring(0, hitsInAnchorAndQuery.IndexOf(' '));
                            double scoreInLevel = Convert.ToDouble(calc.Substring(calc.IndexOf("+") + 1));

                            var contextmap = r.Substring(r.LastIndexOf(" contextmap=") + 12);
                            contextmap = contextmap.Substring(0, contextmap.IndexOf(' '));
                            var importanceLevel = Context( contextmap);
                            var allboost = r.Substring(r.LastIndexOf(' ') + 1);

                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var contextScore = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var proximityScore = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var commonContextScore = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('+')));
                            allboost = allboost.Substring(allboost.IndexOf('+') + 1);
                            var operatorScore = Convert.ToInt32(allboost.Substring(0, allboost.IndexOf('=')));

                            double xsum = contextScore / (occuranceScore + scoreInContext + scoreInAnchorAndQuery + scoreInLevel);

                            rankLogReport.AppendFormat("Query term: {0}\r\n", term);
                            rankLogReport.AppendFormat("  Context score...............: {0}\r\n", contextScore);
                            if (occuranceScore != 0)
                            {
                                occuranceScore = Math.Round(occuranceScore * xsum, 0);
                                rankLogReport.AppendFormat("    First occurence position/score......: {0}/{1}\r\n",
                                                           firstOccurancePosition, occuranceScore);
                            }
                            if (scoreInContext != 0)
                            {
                                scoreInContext = Math.Round(scoreInContext * xsum, 0);
                                rankLogReport.AppendFormat("    Number of hits/score................: {0}/{1}\r\n",
                                                           hitsInContext, scoreInContext);
                            }


                            if (scoreInAnchorAndQuery != 0)
                            {
                                scoreInAnchorAndQuery = Math.Round(scoreInAnchorAndQuery * xsum, 0);
                                rankLogReport.AppendFormat("    Anchor & query text hits/score in...: {0}/{1}\r\n",
                                                           hitsInAnchorAndQuery, scoreInAnchorAndQuery);
                            }

                            if (scoreInLevel != 0)
                            {
                                scoreInLevel = Math.Round(scoreInLevel * xsum, 0);
                                rankLogReport.AppendFormat("    Importance level/score.................: {0}/{1}\r\n",
                                                           importanceLevel, scoreInLevel);
                            }
                            if (proximityScore != 0)
                            {
                                rankLogReport.AppendFormat("  Proximity score.............: {0}\r\n", proximityScore);
                            }
                            if (commonContextScore != 0)
                            {
                                rankLogReport.AppendFormat("  Common-context score........: {0}\r\n", commonContextScore);
                            }
                            if (operatorScore != 0)
                            {
                                rankLogReport.AppendFormat("  Operator score..............: {0}\r\n", operatorScore);
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
                            staticRankScore = r.Substring(r.LastIndexOf('=') + 1);
                            int spacePos = staticRankScore.IndexOf(' ');
                            if (spacePos > 0) staticRankScore=  staticRankScore.Substring(0, spacePos);

                            //var rank = r.Substring(r.LastIndexOf(")=") + 2);
                            //rank = rank.Substring(0, rank.IndexOf(')'));

                            rankLogReport.AppendFormat("Static rank score.............: {0}\r\n", staticRankScore);
                        }
                        else if (r.StartsWith("FRESHNESS,"))
                        {
                            haveFreshness = true;
                            var freshnessScore = r.Substring(r.LastIndexOf(")=") + 2);
                            finalRankScore = r.Substring(r.LastIndexOf('=') + 1);

                            freshnessScore = freshnessScore.Substring(0, freshnessScore.IndexOf(')'));
                            finalRankScore = finalRankScore.Substring(0, finalRankScore.LastIndexOf(' '));

                            rankLogReport.AppendFormat("Freshness score...............: {0}\r\n", freshnessScore);
                        }
                    }
                    if( !haveFreshness)
                    {
                        finalRankScore = staticRankScore;
                    }
                    rankLogReport.AppendFormat("Total Rank score.........: {0}\r\n", finalRankScore);
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