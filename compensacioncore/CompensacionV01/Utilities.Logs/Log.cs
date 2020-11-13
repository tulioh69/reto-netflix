using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Logs
{
    public class Log
    {
        private string logFileName { get; set; }
        private string formatLogFileName { get; set; }
        private string extLogFile { get; set; }
        private string logsRootPath { get; set; }

        public Log() { }
        public Log(string logsRootPath, string logFileName, string formatLogFileName, string extLogFile)
        {
            this.logsRootPath = logsRootPath;
            this.logFileName = logFileName;
            this.formatLogFileName = formatLogFileName;
            this.extLogFile = extLogFile;
        }
        private void validateLogConfig()
        {
            if ((logsRootPath == null || logsRootPath.Length == 0))
            {
                Exception e = new Exception("Parametro logsRootPath no establecido, revisar configuracion "); throw e;
            }
            if ((logFileName == null || logFileName.Length == 0))
            {
                Exception e = new Exception("Parametro logFileName no establecido, revisar configuracion "); throw e;
            }
            if ((formatLogFileName == null || formatLogFileName.Length == 0))
            {
                Exception e = new Exception("Parametro formatLogFileName no establecido, revisar configuracion "); throw e;
            }
            if ((extLogFile == null || extLogFile.Length == 0))
            {
                Exception e = new Exception("Parametro extLogFile no establecido, revisar configuracion "); throw e;
            }
        }
        public void grabarLog(string msj)
        {
            validateLogConfig();
            string nombreLog;
            string archivoLog;

            nombreLog = logFileName + DateTime.Now.ToString(formatLogFileName) + extLogFile;
            archivoLog = logsRootPath + "\\";
            if (!Directory.Exists(logsRootPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(logsRootPath);
            }
            if (!Directory.Exists(archivoLog))
            {
                DirectoryInfo di = Directory.CreateDirectory(archivoLog);
            }
            archivoLog = archivoLog + nombreLog;
            msj = DateTime.Now.ToString() + " " + msj;
            using (StreamWriter sw = File.AppendText(archivoLog))
            {
                sw.WriteLine(msj);
            }
        }
    }
}
