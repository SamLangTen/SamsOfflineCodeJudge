using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    /// <summary>
    /// A problem defined by user
    /// </summary>
    public class SampleData
    {
        public SampleData()
        {
            Datas = new List<string>();
        }
        /// <summary>
        /// Limit Time (ms)
        /// </summary>
        public int LimitTime { get; set; }
        /// <summary>
        /// Limit RAM (Megabytes)
        /// </summary>
        public int LimitRAM { get; set; }
        public List<string> Datas { get; set; }
    }
}
