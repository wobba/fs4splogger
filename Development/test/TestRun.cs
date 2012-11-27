using System;
using System.IO;
using FS4SPQueryLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mAdcOW.FS4SPQueryLogger;

namespace test
{
    [TestClass]
    public class RankLogTest
    {
        [TestMethod]
        public void RankLog()
        {
            using (TextReader r = new StreamReader("ranklog_bo.txt"))
            {
                string str = r.ReadToEnd();
                var result = RankLogParser.Parse(str);

                Console.WriteLine(result);
            }
        }

        [TestMethod]
        public void OldRankLog()
        {
            using (TextReader r = new StreamReader("ranklog1.txt"))
            {
                string str = r.ReadToEnd();
                var result = OldRankLogParser.Parse(str);

                Console.WriteLine(result);
            }
        }
    }
}
