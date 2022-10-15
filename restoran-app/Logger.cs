using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace restoran_app
{
    public static class Logger
    {
        // We will disable logForm in release builds, or only show errors
        public static LogForm logForm = new LogForm();
        private static void logWithPrefix(string prefix, string format, params object[] args)
        {
            string log = string.Format(format, args).Trim();
            logForm.textBox.Text += DateTime.Now.ToString() + " " + prefix + ": " + log + "\n";
        }
        public static void LogInfo(string format, params object[] args)
        {
#if DEBUG
            logWithPrefix("INFO", format, args);
#endif
        }
        public static void LogError(string format, params object[] args)
        {
            logWithPrefix("ERROR", format, args);
        }
        public static void ShowLogForm()
        {
            if (!logForm.Visible)
            {
                logForm.Show();
            }
        }
    }
}
