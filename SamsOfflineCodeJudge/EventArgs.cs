using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class CompilationFinishedEventArgs:EventArgs
    {
        public bool IsCompilationSucceeded;
    }
    public class JudgementFinishedEventArgs:EventArgs
    {
        public int Index;
    }
}
