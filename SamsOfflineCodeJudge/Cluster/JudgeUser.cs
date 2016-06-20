using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class JudgeUser
    {
        public string Id { get; set; }
        public List<JudgeUnit> Units { get; set; } = new List<JudgeUnit>();
        public List<UserReport> Reports { get; set; } = new List<UserReport>();
    }
}
