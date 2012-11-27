using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace mAdcOW.FS4SPQueryLogger
{
    /// <summary>
    /// Code copied from http://gallery.technet.microsoft.com/scriptcenter/16448603-ab52-4e28-a1a4-4e4d9ddb4dd9
    /// and converted to C# with minor modifications
    /// </summary>
    public class RankLogParser
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
                string report = RankReport(hit).ToString();
                sbLog.AppendLine("Hit: " + hitNumber);
                sbLog.AppendLine("Title: " + title);
                sbLog.AppendLine("Url: " + url);
                sbLog.AppendLine(report);
                sbLog.AppendLine("############################");
            }
            return sbLog.ToString();
        }

        private static RankLog RankReport(string ranklog)
        {
            RankLog rankLog = new RankLog();

            var lines = Regex.Split(ranklog, @"\n");
            foreach (string line in lines)
            {
                if (!line.StartsWith("#ranklog"))
                    continue;

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
                        Match match = Regex.Match(r, @",(\w+.[^:]+):('[^']+') TERMINDEX");
                        string term = string.Empty;
                        string termEx = string.Empty;
                        if (match.Success)
                        {
                            termEx = match.Groups[1].Value;

                            termEx = termEx.Replace(".complete", " ").Replace("bcatcontent.bidx", "");

                            term = match.Groups[2].Value;
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

                        QueryTerm e = new QueryTerm();

                        e.Term = term;
                        e.TermEx = termEx;
                        e.ContextScore = contextScore;

                        e.OccuranceScore = occuranceScore;
                        e.Xsum = xsum;
                        e.ScoreInContext = scoreInContext;
                        e.HitsInContext = hitsInContext;
                        e.ScoreInAnchorAndQuery = scoreInAnchorAndQuery;
                        e.HitsInAnchorAndQuery = hitsInAnchorAndQuery;
                        e.ScoreInLevel = scoreInLevel;
                        e.ImportanceLevel = importanceLevel;

                        e.FirstOccurancePosition = firstOccurancePosition;
                        e.OccuranceScore = occuranceScore;

                        rankLog.DynamicRank.QueryTerms.Add(e);

                        if(proximityScore > 0)
                        rankLog.DynamicRank.ProximityScore = proximityScore;

                        if(commonContextScore > 0)
                        rankLog.DynamicRank.CommonContextScore = commonContextScore;

                        if(operatorScore > 0)
                        rankLog.DynamicRank.OperatorScore = operatorScore;
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
                        XRankItem item = new XRankItem();
                        item.ph = ph;
                        item.rank = rank;

                        rankLog.DynamicRank.xRankItems.Add(item);
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

                            XRankItem item = new XRankItem();
                            item.ph = ph;
                            item.rank = rank;

                            rankLog.DynamicRank.xRankItems.Add(item);
                        }
                    }
                    else if (r.StartsWith("STATICRANK,"))
                    {
                        staticRankScore = r.Substring(r.LastIndexOf(' ') + 1);
                        int spacePos = staticRankScore.IndexOf(' ');
                        if (spacePos > 0)
                            staticRankScore=  staticRankScore.Substring(0, spacePos);

                        StaticRank staticRank = new StaticRank();
                        
                        staticRank.UrlDepthRank = ParseWeightedValue(r, "batvurldepthrank");
                        staticRank.SiteRank = ParseWeightedValue(r, "batvsiterank");
                        staticRank.DocRank = ParseWeightedValue(r, "batvdocrank");
                        staticRank.HwBoost = ParseWeightedValue(r, "batvhwboost");

                        staticRank.Components = staticRankScore;

                        //TODO Verify sum

                        rankLog.StaticRank = staticRank;
                    }
                    else if (r.StartsWith("FRESHNESS,"))
                    {
                        haveFreshness = true;
                        var freshnessScore = r.Substring(r.LastIndexOf(")=") + 2);
                        finalRankScore = r.Substring(r.LastIndexOf('=') + 1);

                        freshnessScore = freshnessScore.Substring(0, freshnessScore.IndexOf(')'));
                        int freshness = 0;
                        Int32.TryParse(freshnessScore, out freshness);
                        
                        finalRankScore = finalRankScore.Substring(0, finalRankScore.LastIndexOf(' '));

                        rankLog.DynamicRank.Freshness = freshness;
                        rankLog.TotalScore = finalRankScore;
                    }
                }
                if( !haveFreshness)
                {
                    rankLog.TotalScore = staticRankScore;
                }
            }

            return rankLog;
        }

        static WeightedValue ParseWeightedValue(string input, string pattern)
        {
            var x = GiveSubstring(input, pattern, '>', ')');
            WeightedValue weightedValue = new WeightedValue();

            var starIndex = x.IndexOf('*');
            if (starIndex == -1)
                return weightedValue;

            try
            {
                weightedValue.Value = float.Parse(x.Substring(0, starIndex).Trim());
                weightedValue.Weight = float.Parse(x.Substring(starIndex + 1, x.Length - starIndex - 1));
            }
            catch
            {
            }
            return weightedValue;
        }

        static string GiveSubstring(string all, string query, char from, char to)
        {
            int subIndex = all.IndexOf(query);
            if (subIndex > 0)
            {
                int toIndex = all.IndexOf(to, subIndex);
                int fromIndex = all.IndexOf(from, subIndex);

                var substring = all.Substring(fromIndex+1, toIndex - fromIndex-1);

                return substring;
            }
            return string.Empty;
        }

        // To be integrated with FAST via PowerShell or direct API
        private static string GetPropertyByMappingLevel(int priorityLevel)
        {
            var mappings = new Dictionary<int, string>();
            mappings.Add(8, "anchortext;assocqueries");
            mappings.Add(7, "Title; DocSubject");
            mappings.Add(5, "Keywords; DocKeywords");
            mappings.Add(4, "urlkeywords");
            mappings.Add(3, "description");
            mappings.Add(2, "Author; CreatedBY; ModifiedBy; MetadataAuthor; WorkEmail");
            mappings.Add(1, "body; crawledpropertiescontent");

            if (mappings.ContainsKey(priorityLevel))
            {
                return mappings[priorityLevel];
            }

            return string.Empty;
        }

        static Dictionary<int, string> Context(string inp)
        {
            uint input = uint.Parse(inp);
            uint digit = 2147483648;
            var map = new Dictionary<int, string>();

            for (var i = 0; i < 32; i++)
            {
                int level = (32 - i);
                if ((input & digit) != 0)
                {
                    map.Add(level, GetPropertyByMappingLevel(level));
                }
                digit = digit >> 1;
            }

            return map;
        }
    }

    public class RankLog
    {
        public StaticRank StaticRank = new StaticRank();
        public DynamicRank DynamicRank = new DynamicRank();

        public string TotalScore;

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendFormat("Total Rank score.........: {0} sum of\r\n", TotalScore);

            s.Append(StaticRank);
            s.Append(DynamicRank);
            

            return s.ToString();
        }
    }

    public class WeightedValue
    {
        public float Value = 0;
        public float Weight = 0;

        public int Score
        {
            get { return (int)(Value*Weight); }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1} * Weight({2})", Score, Value, Weight);
        }
    }

    public class StaticRank
    {
        public WeightedValue DocRank = new WeightedValue();
        public WeightedValue SiteRank = new WeightedValue();
        public WeightedValue UrlDepthRank = new WeightedValue();
        public WeightedValue HwBoost = new WeightedValue();

        public string Components = string.Empty;

        public int Score
        {
            get { return (int) (DocRank.Score + SiteRank.Score + HwBoost.Score + UrlDepthRank.Score); }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            
            s.AppendFormat("Static rank score..............: {0} sum of\r\n", Score);
            s.AppendFormat("   Url Depth Rank...................: {0}\r\n", UrlDepthRank);
            s.AppendFormat("   Site Rank........................: {0}\r\n", SiteRank);
            s.AppendFormat("   Doc Rank.........................: {0}\r\n", DocRank);
            s.AppendFormat("   HW Boost.........................: {0}\r\n", HwBoost);

            return s.ToString();
        }
    }

    public class DynamicRank
    {
        public int Freshness;
        
        public int ProximityScore;
        public int CommonContextScore;
        public int OperatorScore;

        public List<XRankItem> xRankItems = new List<XRankItem>();
        public List<QueryTerm> QueryTerms = new List<QueryTerm>();

        public int Score
        {
            get 
            {
                int score = 0;

                foreach (var xRankItem in xRankItems)
                {
                    score += (int)xRankItem.rank;
                }

                foreach (var term in QueryTerms)
                {
                    score += term.Score;
                }

                score += Freshness;
                score += ProximityScore;
                score += CommonContextScore;
                score += OperatorScore;

                return score;
            }
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendFormat("Dynamic rank score.............: {0} sum of \r\n", Score);
            s.AppendFormat("   Freshness score..................: {0}\r\n", Freshness);
            
            if (ProximityScore != 0)
            s.AppendFormat("   Proximity score..................: {0}\r\n", ProximityScore);
            
            if (CommonContextScore != 0)
            s.AppendFormat("   Common-context score.............: {0}\r\n", CommonContextScore);
            
            if (OperatorScore != 0)
            s.AppendFormat("   Operator score...................: {0}\r\n", OperatorScore);

            foreach (var item in xRankItems)
            {
                s.Append(item);
            }

            foreach (var item in QueryTerms)
            {
                s.Append(item);
            }

            return s.ToString();
        }
    }

    public class XRankItem
    {
        public double rank;
        public string ph;

        public override string ToString()
        {
            if(rank != 0)
                return string.Format("   XRANK score......................: {0} {1}\r\n", rank, ph);

            return string.Empty;
        }
    }

    public class QueryTerm
    {
        public string Term;
        public string TermEx;
        public int ContextScore;

        public double OccuranceScore;
        public string FirstOccurancePosition;

        public double Xsum;

        public double ScoreInContext;
        public string HitsInContext;
 
        public double ScoreInAnchorAndQuery;
        public string HitsInAnchorAndQuery;
        
        public double ScoreInLevel;
        public Dictionary<int, string> ImportanceLevel = new Dictionary<int, string>();

        public int Score
        {
            get { return ContextScore; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.AppendFormat("   Query term: {0}, {1}\r\n", Term, TermEx);
            s.AppendFormat("      Context score.................: {0} sum of\r\n", ContextScore);
            
            if (OccuranceScore != 0)
            {
                s.AppendFormat("      First occurence score .............: {0} (at position {1})\r\n", Math.Round(OccuranceScore * Xsum, 0), FirstOccurancePosition);
            }

            if (ScoreInContext != 0)
            {
                s.AppendFormat("      Hits score.........................: {0} (with {1} hits)\r\n", Math.Round(ScoreInContext * Xsum, 0), HitsInContext);
            }

            if (ScoreInAnchorAndQuery != 0)
            {
                s.AppendFormat("      Anchor & query text score..........: {0} (with {1} hits)\r\n", Math.Round(ScoreInAnchorAndQuery * Xsum, 0), HitsInAnchorAndQuery);
            }

            if (ScoreInLevel != 0)
            {
                s.AppendFormat("      Importance score...................: {0} (with hits in)\r\n",  Math.Round(ScoreInLevel * Xsum, 0));

                foreach (var level in ImportanceLevel.Keys)
                {
                    s.AppendFormat("           Level {0}..........................: {1}\r\n", level, ImportanceLevel[level]);
                }
            }

            return s.ToString();
        }
    }
}