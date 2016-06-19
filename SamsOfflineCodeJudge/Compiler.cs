using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamsOfflineCodeJudge
{
    public class Compiler
    {
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Argument { get; set; }
        public List<string> Languages { get; set; }
    }
}
