using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader
{
    public static class Logger
    {
        private static string LogsFolder
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Websites.Addic7ed.Config)).Location), "logs");
            }
        }

        private static string Logfile
        {
            get
            {
                return Path.Combine(LogsFolder, DateTime.Today.ToLongDateString() + ".txt");
            }
        }

        static Logger()
        {
            if (!Directory.Exists(LogsFolder))
                Directory.CreateDirectory(LogsFolder);
        }

        public static void WriteLine(string format, params object[] args)
        {
            var line = string.Format("[{0}]: {1}" + Environment.NewLine, DateTime.Now.ToLongTimeString(), string.Format(format, args));
            File.AppendAllText(Logfile, line);
        }
    }
}
