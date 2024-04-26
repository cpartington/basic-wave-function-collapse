using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hackathon
{
    public static class Logger
    {
        public static readonly bool PrintDebugLogs = false;

        public static void Debug(string message)
        {
            if (PrintDebugLogs) System.Diagnostics.Debug.WriteLine(message);
        }

        public static void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
