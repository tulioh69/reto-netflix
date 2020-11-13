using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompensacionV01
{
    public class LoggerBlock
    {
        protected LogWriter logWriter;
        public static void write(string message)
        {
            Logger.Write(new LogEntry()
            {
                Message = message
            });
        }
        public LoggerBlock()
        {
            initLogging();
        }
        private void crearEventSource()
        {
            string sourceEventName = "Application"; // default
            
            LoggingSettings loggingConfiguration = (LoggingSettings)ConfigurationManager.GetSection(LoggingSettings.SectionName);
            foreach (TraceListenerData tld in loggingConfiguration.TraceListeners)
            {
                if(tld.Type.Name == "FormattedEventLogTraceListener")
                {
                    FormattedEventLogTraceListenerData feltc = (FormattedEventLogTraceListenerData)tld;
                    sourceEventName = feltc.Source;
                }
            }
            if (!EventLog.SourceExists(sourceEventName))
            {
                EventLog.CreateEventSource(sourceEventName, sourceEventName);
            }
        }
        private void crearPathFileName()
        {
            string fileNameFormat = "yyMMdd";
            string fileName;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            LoggingSettings loggingConfiguration = (LoggingSettings)config.GetSection(LoggingSettings.SectionName);
            foreach (TraceListenerData tld in loggingConfiguration.TraceListeners)
            {
                if (tld.Type.Name == "RollingFlatFileTraceListener")
                {
                    RollingFlatFileTraceListenerData rfftl = (RollingFlatFileTraceListenerData)tld;
                    string fileNameELibLog = rfftl.FileName;
                    DirectoryInfo di = new DirectoryInfo(fileNameELibLog);
                    string fileNamePath = di.FullName.Replace(di.Name, ""); // D://compensacion//
                    string filenNameN = di.Name.Replace(di.Extension, ""); //compensacion200203   
                    filenNameN = Regex.Replace(filenNameN, "[0-9]{2,}", "");// replace all numbers with empty to leave the text 'compensacion'
                    fileNameFormat = DateTime.Now.ToString(fileNameFormat, CultureInfo.InvariantCulture); //yyyyMMdd
                    string fileNameExt = di.Extension;//.log
                    fileName = Path.Combine(fileNamePath, filenNameN + fileNameFormat + fileNameExt);
                    bool readOnly = rfftl.IsReadOnly();
                    if (rfftl != null && !readOnly)
                    {
                        rfftl.FileName = fileName;
                    }
                }
            }
            Thread.Sleep(500);
            config.Save(ConfigurationSaveMode.Modified);
            Thread.Sleep(500);
        }
        private void initLogging()
        {
            crearEventSource();
            //crearPathFileName();
            LogWriterFactory logWriterFactory = new LogWriterFactory();
            logWriter = logWriterFactory.Create();
            Logger.SetLogWriter(logWriter, false);
        }
        public LogWriter getLogWriter
        {
            get{return logWriter;}
        }
    }
}
