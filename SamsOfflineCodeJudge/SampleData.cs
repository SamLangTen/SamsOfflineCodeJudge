using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class DataPair
    {
        public string InputData { get; set; }
        public string OutputData { get; set; }
    }
    /// <summary>
    /// A problem defined by user
    /// </summary>
    public class SampleData
    {
        public SampleData()
        {
            Datas = new List<DataPair>();
        }
        /// <summary>
        /// Limit Time (ms)
        /// </summary>
        public double LimitTime { get; set; }
        /// <summary>
        /// Limit RAM (bytes)
        /// </summary>
        public long LimitRAM { get; set; }
        public List<DataPair> Datas { get; set; }
    }
}
