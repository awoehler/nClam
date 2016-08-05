using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nClam
{
    public class ClamServerThreadStats
    {
        public int live { get; set; }
        public int idle { get; set; }
        public int max { get; set; }
        public int idleTimeout { get; set; }
        public ClamServerThreadStats(Dictionary<string, string> stats)
        {
            if (stats.ContainsKey("live"))
            {
                live = int.Parse(stats["live"]);
            }
            if (stats.ContainsKey("idle"))
            {
                idle = int.Parse(stats["idle"]);
            }
            if (stats.ContainsKey("max"))
            {
                max = int.Parse(stats["max"]);
            }
            if (stats.ContainsKey("idle-timeout"))
            {
                idleTimeout = int.Parse(stats["idle-timeout"]);
            }
        }
    }
    public class ClamServerMemoryStats
    {
        public Int64 heap { get; set; }
        public Int64 mmap { get; set; }
        public Int64 used { get; set; }
        public Int64 free { get; set; }
        public Int64 releasable { get; set; }
        public int pools { get; set; }
        public Int64 pools_used  { get; set; }
        public Int64 pools_total { get; set; }

        public ClamServerMemoryStats( Dictionary<string,string> stats )
        {
            if( stats.ContainsKey("heap") )
            {
                if( isMB(stats["heap"] ) )
                {
                    heap = toMB( stats["heap"] );
                }
            }
            if (stats.ContainsKey("mmap"))
            {
                if (isMB(stats["mmap"]))
                {
                    mmap = toMB(stats["mmap"]);
                }
            }
            if (stats.ContainsKey("used"))
            {
                if (isMB(stats["used"]))
                {
                    used = toMB(stats["used"]);
                }
            }
            if (stats.ContainsKey("free"))
            {
                if (isMB(stats["free"]))
                {
                    free = toMB(stats["free"]);
                }
            }
            if (stats.ContainsKey("releasable"))
            {
                if (isMB(stats["releasable"]))
                {
                    releasable = toMB(stats["releasable"]);
                }
            }
            if (stats.ContainsKey("pools"))
            {
                if (isMB(stats["pools"]))
                {
                    pools = (int)toMB(stats["pools"]);
                }
            }
            if (stats.ContainsKey("pools_used"))
            {
                if (isMB(stats["pools_used"]))
                {
                    pools_used = toMB(stats["pools_used"]);
                }
            }
            if (stats.ContainsKey("pools_total"))
            {
                if (isMB(stats["pools_total"]))
                {
                    pools_total = toMB(stats["pools_total"]);
                }
            }
        }

        bool isMB( string value )
        {
            if( value.Last() == 'M' )
            {
                return true;
            }
            return false;
        }

        Int64 toMB( string value )
        {
            decimal v = decimal.Parse(value.Replace('M', ' '));
            return  Convert.ToInt64( v * 1024 * 1024 ) ;
        }
    }
    public class ClamServerStats
    {
        public string state { get; private set; }
        public int pools { get; private set; }
        public int queueSize { get; private set; }
        //public Dictionary<string,string> threads { get; set; }
        public ClamServerThreadStats threads { get; set; }
        public ClamServerMemoryStats memstats { get; set; }

        public ClamServerStats(string stats )
        {
            DictionaryToStats(toDictionary(stats, ':'));
        }
        public ClamServerStats(Dictionary<string, string> stats)
        {
            DictionaryToStats(stats);
        }
        /// <summary>
        /// Setup the appropriate values
        /// </summary>
        /// <param name="stats"></param>
        private void DictionaryToStats( Dictionary<string,string> stats) { 
            if( stats.ContainsKey("POOLS") )
            {
                pools = int.Parse(stats["POOLS"]);
            }
            if( stats.ContainsKey("STATE") )
            {
                state = stats["STATE"];
            }
            if (stats.ContainsKey("THREADS"))
            {
                threads = new ClamServerThreadStats( toDictionary(stats["THREADS"]) );
            }
            if (stats.ContainsKey("QUEUE"))
            {
                queueSize = int.Parse(stats["QUEUE"].Replace("items", ""));
            }
            if (stats.ContainsKey("MEMSTATS"))
            {
                memstats = new ClamServerMemoryStats( toDictionary( stats["MEMSTATS"] ));
            }
        }

        /// <summary>
        /// Expects token separated pairs. If the number is odd the last item will be skipped.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Dictionary<string,string> toDictionary( string content, char token=' ' )
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            string[] tokens = content.Split(new[] { token }, StringSplitOptions.RemoveEmptyEntries);
            int nTokens = tokens.Length;
            for( var i=1; i < nTokens; i += 2 )
            {
                response.Add(tokens[i - 1].Trim(), tokens[i].Trim());
            }
            return response;
        }
    }
}
