using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector
{
    class QDebug
    {
        public static bool Enabled = false;
        public static void write(string text, string mark = "qdebug")
        {
            if (Enabled)
            {
                Debug.WriteLine((mark.Length > 0 ? mark + ": " : "") + text);
            }
        }
    }
}
