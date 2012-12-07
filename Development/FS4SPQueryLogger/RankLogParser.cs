using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.SharePoint.Search.Extended.Administration;
using Microsoft.SharePoint.Search.Extended.Administration.Schema;
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
                        QualityRank qualityRank = new QualityRank();
                        
                        // batv(name)[num]->num * num
                        Regex reg = new Regex(@"batv([\w\d]+)\[(\d+)\]->([\d.]+)\s\*\s([\d.]+)");
                        var matches = reg.Matches(r);
                        foreach (Match m in matches)
                        {
                            QualityRankComponent c= new QualityRankComponent();
                            c.Name = m.Groups[1].Value;

                            var fastFormat = new NumberFormatInfo() { NumberDecimalSeparator = "." };
                            try
                            {
                                c.Value = float.Parse(m.Groups[3].Value, fastFormat);
                                c.Weight = float.Parse(m.Groups[4].Value, fastFormat);
                            }
                            catch{}
                            qualityRank.Components.Add(c);
                        }

                        // A+B+C=D
                        string normalPattern   = @"\d+\+\d+\+\d+=\d+";
                        // A+B/X+C=D
                        string weightedPattern = @"\d+\+\d+/(\d+)\+\d+=\d+";
                        Match normalScore = Regex.Match(r, normalPattern);
                        if (!normalScore.Success)
                        {
                            //try case when B is reduced
                            Match weightedScore = Regex.Match(r, weightedPattern);
                            if (weightedScore.Success)
                            {
                                qualityRank.Reduction = Int32.Parse(weightedScore.Groups[1].Value);
                            }
                        }

                        rankLog.QualityRank = qualityRank;
                    }
                    else if (r.StartsWith("FRESHNESS,"))
                    {
                        var freshnessScore = r.Substring(r.LastIndexOf(")=") + 2);
                        finalRankScore = r.Substring(r.LastIndexOf('=') + 1);

                        freshnessScore = freshnessScore.Substring(0, freshnessScore.IndexOf(')'));
                        int freshness = 0;
                        Int32.TryParse(freshnessScore, out freshness);
                        
                        finalRankScore = finalRankScore.Substring(0, finalRankScore.LastIndexOf(' '));

                        rankLog.FreshnessRank = freshness;
                        rankLog.TotalScore = finalRankScore;
                    }
                }
            }

            return rankLog;
        }
        
        private static Dictionary<int, string> _mappings = new Dictionary<int, string>(); 
        
        private static string GetPropertyByMappingLevel(int priorityLevel)
        {
            if (_mappings.Count == 0)
            {
                InitFullTextMappings();
            }

            if (_mappings.ContainsKey(priorityLevel))
            {
                return _mappings[priorityLevel];
            }

            return string.Empty;
        }

        private static Dictionary<int, string> InitDefault()
        {
            var mappings = new Dictionary<int, string>();
            mappings.Add(8, "anchortext;assocqueries");
            mappings.Add(7, "Title; DocSubject");
            mappings.Add(5, "Keywords; DocKeywords");
            mappings.Add(4, "urlkeywords");
            mappings.Add(3, "description");
            mappings.Add(2, "Author; CreatedBY; ModifiedBy; MetadataAuthor; WorkEmail");
            mappings.Add(1, "body; crawledpropertiescontent");

            return mappings;
        }

        public static void InitFullTextMappings()
        {
            _mappings = new Dictionary<int, string>();
            try
            {
                SchemaContext schemaContext = new SchemaContext();
                Schema indexSchema = schemaContext.Schema;
                var ind = indexSchema.AllFullTextIndecies.FirstOrDefault(index => index.isDefault);
                
                foreach(int level in Enum.GetValues(typeof(FullTextIndexImportanceLevel)))
                {
                    string prop = string.Empty;
                    foreach (var map in ind.GetMappingsForLevel(level))
                    {
                        prop += map.Name + "; ";
                    }

                    if (!string.IsNullOrEmpty(prop))
                    {
                        _mappings.Add(level, prop);
                    }
                }
            }
            catch
            {
                _mappings = InitDefault();
            }
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
        public QualityRank QualityRank = new QualityRank();
        public DynamicRank DynamicRank = new DynamicRank();
        public int FreshnessRank;

        public string TotalScore;

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendFormat("Total Rank score.........: {0} sum of\r\n", TotalScore);
            s.AppendFormat("Freshness score................: {0}\r\n", FreshnessRank);

            s.Append(QualityRank);
            s.Append(DynamicRank);
            

            return s.ToString();
        }
    }

    public class QualityRankComponent
    {
        public float Value = 0;
        public float Weight = 0;
        public string Name = string.Empty;

        public int Score
        {
            get { return (int)(Value*Weight); }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1} * Weight({2})", Score, Value, Weight);
        }
    }

    public class QualityRank
    {
        public List<QualityRankComponent> Components = new List<QualityRankComponent>();
        public int Reduction = 1;

        public int Score
        {
            get
            {
                return Components.Sum(c=>c.Score) / Reduction;
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            string reductionMessage = string.Empty;
            if (Reduction > 1)
            {
                reductionMessage = string.Format("(={0}/{1}, because Context Score <= 0)", Components.Sum(c=>c.Score), Reduction);
            }

            s.AppendFormat("Quality rank score.............: {0} {1} sum of\r\n", Score, reductionMessage);
            foreach (var component in Components)
            {
                var dots = new string('.', 33 - component.Name.Length); // 33 magic number for alighmnent
                s.AppendFormat("   {0}{1}: {2}\r\n", component.Name, dots, component);
            }

            return s.ToString();
        }
    }

    public class DynamicRank
    {
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

                if (score < 0)
                    score = 0;

                score += ProximityScore;
                score += CommonContextScore;
                score += OperatorScore;

                return score;
            }
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            
            s.AppendFormat("Context & Authority score......: {0} sum of \r\n", Score);
            
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