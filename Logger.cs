using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hackathon
{
    public static class Logger
    {
        public static readonly bool PrintLogs = true;

        public static void Log(string message)
        {
            if (PrintLogs) System.Diagnostics.Debug.WriteLine(message);
        }

        public static void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
