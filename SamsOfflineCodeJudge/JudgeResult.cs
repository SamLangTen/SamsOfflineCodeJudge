using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class JudgeResult
    {
        public long MaximumRAM { get; set; }
        public double TotalTime { get; set; }
        public string Output { get; set; }
        public JudgeResultEnum Result { get; set; }
        public int ExitCode { get; set; }
    }
    public enum JudgeResultEnum
    {
        Accepted,
        PresentationError,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        WrongAnswer,
        RuntimeError,
        CompileError
    }

}
