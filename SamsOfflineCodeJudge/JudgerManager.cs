using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;

namespace SamsOfflineCodeJudge
{
    public class SystemUser
    {
        public string Username { get; set; }
        public SecureString  Password { get; set; }
    }
    public class JudgerManager
    {
        static JudgerManager()
        {
            LanguageExtensions = new Dictionary<string, string>();
            LanguageExtensions.Add("C", "c");
            LanguageExtensions.Add("C++", "cpp");
            LanguageExtensions.Add("VB.Net", "vb");
            LanguageExtensions.Add("C#", "cs");
            LanguageExtensions.Add("Pascal", "pas");
        }
        public static SystemUser RunningUser { get; set; }
        public static int MaximumTime { get; set; } = 20000;
        public static Dictionary<string, string> LanguageExtensions { get; set; }
    }
}
