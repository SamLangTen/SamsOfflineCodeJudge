using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class JudgeResult
    {
        public int MaximumRAM { get; set; }
        public int TotalTime { get; set; }
        public bool[] Comparsions { get; set; }
        public JudgeResultEnum Result { get; set; }
    }
    public enum JudgeResultEnum
    {
        Accepted,
        PresentationError,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        WrongAnswer,
        RuntimeError,
        OutputLimitExceeded,
        CompileError
    }

}
