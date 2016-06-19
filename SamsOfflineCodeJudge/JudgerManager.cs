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
        public static SystemUser RunningUser { get; set; }
        public static int MaximumTime { get; set; } = 20000;
    }
}
