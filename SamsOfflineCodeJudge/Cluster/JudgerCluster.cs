using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class JudgerCluster
    {
        public List<JudgeUnit> JudgeUnits { get; set; } = new List<JudgeUnit>();
        public List<SampleData> SampleDatas { get; set; } = new List<SampleData>();

    }
}
