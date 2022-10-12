using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BioVirtualMachine
{
    /// <summary>
    /// Implementation of the ILogger interface that supports logging to console and file. It is configured by XML file.
    /// </summary>
    class Logger : ILogger
    {
        // List of lists for the logger configuration
        // The inner list format it {XML entry path, XML argument name, variable type in this class, variable name in this class}
        private List<List<string>> configuration = new List<List<string>>()
        {
            new List<string>(){ "/logger/console/enabled", "value", "bool", "consoleEnabled"},
            new List<string>(){ "/logger/console/logDEBUG", "value", "bool", "consoleLogDEBUG"},
            new List<string>(){ "/logger/console/logINFO", "value", "bool", "consoleLogINFO"},
            new List<string>(){ "/logger/console/logWARNING", "value", "bool", "consoleLogWARNING"},
            new List<string>(){ "/logger/console/logERROR", "value", "bool", "consoleLogERROR"},
            new List<string>(){ "/logger/console/logFATAL", "value", "bool", "consoleLogFATAL"},
            new List<string>(){ "/logger/console/logTRACE", "value", "bool", "consoleLogTRACE"},
            new List<string>(){ "/logger/file/enabled", "value", "bool", "fileEnabled"},
            new List<string>(){ "/logger/file/logDEBUG", "value", "bool", "fileLogDEBUG"},
            new List<string>(){ "/logger/file/logINFO", "value", "bool", "fileLogINFO"},
            new List<string>(){ "/logger/file/logWARNING", "value", "bool", "fileLogWARNING"},
            new List<string>(){ "/logger/file/logERROR", "value", "bool", "fileLogERROR"},
            new List<string>(){ "/logger/file/logFATAL", "value", "bool", "fileLogFATAL"},
            new List<string>(){ "/logger/file/logTRACE", "value", "bool", "fileLogTRACE"},
            new List<string>(){ "/logger/file/filePath", "value", "string", "filePath" }
        };


        /// <summary>
        /// Enable logging to console.
        /// </summary>
        public bool consoleEnabled { get; set; } = true;
        public bool consoleLogDEBUG { get; set; } = true;
        public bool consoleLogINFO { get; set; } = true;
        public bool consoleLogWARNING { get; set; } = true;
        public bool consoleLogERROR { get; set; } = true;
        public bool consoleLogFATAL { get; set; } = true;
        public bool consoleLogTRACE { get; set; } = true;

        /// <summary>
        /// Enable logging to file.
        /// </summary>
        public bool fileEnabled { get; set; } = false;
        public bool fileLogDEBUG { get; set; } = false;
        public bool fileLogINFO { get; set; } = false;
        public bool fileLogWARNING { get; set; } = false;
        public bool fileLogERROR { get; set; } = false;
        public bool fileLogFATAL { get; set; } = false;
        public bool fileLogTRACE { get; set; } = false;
        public string filePath { get; set; } = "";


        /// <summary>
        /// Creates a default logger (all levels, console only).
        /// </summary>
        public Logger()
        {

        }


        /// <summary>
        /// Creates a logger according to the configuration passed as file.
        /// </summary>
        /// <param name="configurationFile">XML configuration file.</param>
        public Logger(ref FileStream configurationFile)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(configurationFile);
                foreach (var entry in configuration)
                {
                    XmlNode node = xmlDocument.DocumentElement.SelectSingleNode(entry[0]);
                    if (node == null)
                    {
                        throw new LoggerException("The entry '" + entry[0] + "' could not be found in the XML file.");
                    }
                    string attributeValue = node.Attributes[entry[1]]?.InnerText; // Exception risk here
                    switch (entry[2])
                    {
                        case "bool":
                            if (attributeValue == "true")
                            {
                                this.GetType().GetProperty(entry[3]).SetValue(this, true);
                            }
                            else if (attributeValue == "false")
                            {
                                this.GetType().GetProperty(entry[3]).SetValue(this, false);
                            }
                            else
                            {
                                throw new LoggerException("The value of the bool entry '" + entry[0] + "' is not valid. Only 'true' and 'false' are accepted.");
                            }
                            break;
                        case "string":
                            this.GetType().GetProperty(entry[3]).SetValue(this, attributeValue);
                            break;
                        case "int":
                            this.GetType().GetProperty(entry[3]).SetValue(this, Convert.ToInt32(attributeValue));
                            break;
                        default:
                            throw new LoggerException("The value type '" + entry[2] + "' of entry '" + entry[0] + "' could not be found in the XML file.");
                    }
                }
            }
            catch (XmlException ex)
            {
                throw new LoggerException("The XML configration file is not valid or malformed. " + ex.Message);
            }
            catch (LoggerException)
            {
                throw;
            }

            // Creating empity logfile
            try
            {
                if (fileEnabled)
                {
                    FileStream logFile = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    logFile.Close();
                }
            }
            catch (Exception ex)
            {
                throw new LoggerException("Impossible to create or open the file '" + filePath + "'." + ex.Message);
            }
        }


        /// <summary>
        /// Severity levels:
        /// DEBUG: Additional information about application behavior for cases when that information is necessary to diagnose problems.
        /// INFO: Application events for general purposes.
        /// WARN: Application events that may be an indication of a problem.
        /// ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data.
        /// FATAL: A critical error that results in the termination of an application.
        /// TRACE: Used to mark the entry and exit of functions, for purposes of performance profiling. 
        /// </summary>
        private enum Level { DEBUG, INFO, WARNING, ERROR, FATAL, TRACE };


        public void Info(string message)
        {
            Log(Level.INFO, message);
        }


        public void Info(string format, params object[] parameters)
        {
            Log(Level.INFO, format, parameters);
        }


        public void Debug(string message)
        {
            Log(Level.DEBUG , message);
        }


        public void Debug(string format, params object[] parameters)
        {
            Log(Level.DEBUG, format, parameters);
        }


        public void Warning(string message)
        {
            Log(Level.WARNING, message);
        }


        public void Warning(string format, params object[] parameters)
        {
            Log(Level.WARNING, format, parameters);
        }


        public void Error(string message)
        {
            Log(Level.ERROR, message);
        }


        public void Error(string format, params object[] parameters)
        {
            Log(Level.ERROR, format, parameters);
        }


        public void Error(Exception ex, string message)
        {
            Log(Level.ERROR, ex, message);
        }


        public void Error(Exception ex, string format, params object[] parameters)
        {
            Log(Level.ERROR, ex, format, parameters);
        }


        public void Fatal(string message)
        {
            Log(Level.FATAL, message);
        }


        public void Fatal(string format, params object[] parameters)
        {
            Log(Level.FATAL, format, parameters);
        }


        public void Fatal(Exception ex, string message)
        {
            Log(Level.FATAL, ex, message);
        }


        public void Fatal(Exception ex, string format, params object[] parameters)
        {
            Log(Level.FATAL, ex, format, parameters);
        }


        public void Trace(string message)
        {
            Log(Level.TRACE, message);
        }


        public void Trace(string format, params object[] parameters)
        {
            Log(Level.TRACE, format, parameters);
        }


        private void Log(Level level, string message)
        {
            if (ConsoleAndLevelEnabled(level))
            {
                PrintLoggerTag(level);
                Console.WriteLine(IndentTextAccordingToLevel(level, message));
            }
            if (FileAndLevelEnabled(level))
            {
                string text = GetLoggerTag(level) + IndentTextAccordingToLevel(level, message) + '\n';
                LogToFile(text);
            }
        }


        private void Log(Level level, string format, params object[] parameters)
        {
            if (ConsoleAndLevelEnabled(level))
            {
                PrintLoggerTag(level);
                Console.WriteLine(IndentTextAccordingToLevel(level, format), parameters);
            }
            if (FileAndLevelEnabled(level))
            {
                string text = GetLoggerTag(level) + IndentTextAccordingToLevel(level, String.Format(format, parameters)) + '\n';
                LogToFile(text);
            }
        }


        private void Log(Level level, Exception ex, string message)
        {
            if (ConsoleAndLevelEnabled(level))
            {
                PrintLoggerTag(level);
                Console.WriteLine(IndentTextAccordingToLevel(level, message));
                Console.WriteLine(IndentTextAccordingToLevel(level, "An exception was trown. Exception info:"));
                Console.WriteLine(IndentTextAccordingToLevel(level, ex.ToString()));
            }
            if (FileAndLevelEnabled(level))
            {
                string text = GetLoggerTag(level) + IndentTextAccordingToLevel(level, message) + '\n';
                text += IndentTextAccordingToLevel(level, "An exception was trown. Exception info:") + '\n';
                text += IndentTextAccordingToLevel(level, ex.ToString()) + '\n';
                LogToFile(text);
            }
        }


        private void Log(Level level, Exception ex, string format, params object[] parameters)
        {
            if (ConsoleAndLevelEnabled(level))
            {
                PrintLoggerTag(level);
                Console.WriteLine(IndentTextAccordingToLevel(level, format), parameters);
                Console.WriteLine(IndentTextAccordingToLevel(level, "An exception was trown. Exception info:"));
                Console.WriteLine(IndentTextAccordingToLevel(level, ex.ToString()));
            }
            if (FileAndLevelEnabled(level))
            {
                string text = GetLoggerTag(level) + IndentTextAccordingToLevel(level, String.Format(format, parameters)) + '\n';
                text += IndentTextAccordingToLevel(level, "An exception was trown. Exception info:") + '\n';
                text += IndentTextAccordingToLevel(level, ex.ToString()) + '\n';
                LogToFile(text);
            }
        }


        private void LogToFile(string text)
        {
            try
            {
                    FileStream logFile = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                    StreamWriter logFileStream = new StreamWriter(logFile);
                    logFileStream.Write(text);
                    logFileStream.Close();
                    logFile.Close();
            }
            catch (Exception ex)
            {
                throw new LoggerException("Impossible to open or write the file '" + filePath + "'." + ex.Message);
            }
        }


        private bool ConsoleAndLevelEnabled(Level level)
        {
            return consoleEnabled && ((level == Level.DEBUG && consoleLogDEBUG) || 
                                      (level == Level.INFO && consoleLogINFO) ||
                                      (level == Level.WARNING && consoleLogWARNING) ||
                                      (level == Level.ERROR && consoleLogERROR) ||
                                      (level == Level.FATAL && consoleLogFATAL) ||
                                      (level == Level.TRACE && consoleLogTRACE));
        }


        private bool FileAndLevelEnabled(Level level)
        {
            return fileEnabled && ((level == Level.DEBUG && fileLogDEBUG) ||
                                      (level == Level.INFO && fileLogINFO) ||
                                      (level == Level.WARNING && fileLogWARNING) ||
                                      (level == Level.ERROR && fileLogERROR) ||
                                      (level == Level.FATAL && fileLogFATAL) ||
                                      (level == Level.TRACE && fileLogTRACE));
        }


        private void PrintLoggerTag(Level level)
        {
            switch (level)
            {
                case Level.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Level.INFO:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case Level.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Level.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Level.FATAL:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Level.TRACE:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                default:
                    break;
            }
            Console.Write(Enum.GetName(typeof(Level), level) + ": ");
            System.Reflection.MethodBase method = new StackTrace().GetFrame(3).GetMethod();
            string className = method.ReflectedType.Name;
            Console.WriteLine("Form " + className + ": ");
            Console.ResetColor();
        }


        private string GetLoggerTag(Level level)
        {
            string loggerTag = "";
            loggerTag += Enum.GetName(typeof(Level), level) + ": ";
            System.Reflection.MethodBase method = new StackTrace().GetFrame(3).GetMethod();
            string className = method.ReflectedType.Name;
            loggerTag += "Form " + className + ": \n";
            return loggerTag;
        }


        private string IndentTextAccordingToLevel(Level level, string text)
        {
            string indent = new string(' ', Enum.GetName(typeof(Level), level).Length + 2);
            string newLineAndIndent = '\n' + indent;
            return indent + text.Replace("\n", newLineAndIndent);
        }


        public override string ToString()
        {
            string answer = "";
            answer += "consoleEnabled = " + consoleEnabled.ToString() + '\n';
            answer += "consoleLogDEBUG = " + consoleLogDEBUG.ToString() + '\n';
            answer += "consoleLogINFO = " + consoleLogINFO.ToString() + '\n';
            answer += "consoleLogWARNING = " + consoleLogWARNING.ToString() + '\n';
            answer += "consoleLogERROR = " + consoleLogERROR.ToString() + '\n';
            answer += "consoleLogFATAL = " + consoleLogFATAL.ToString() + '\n';
            answer += "consoleLogTRACE = " + consoleLogTRACE.ToString() + '\n';
            answer += "fileEnabled = " + fileEnabled.ToString() + '\n';
            answer += "fileLogDEBUG = " + fileLogDEBUG.ToString() + '\n';
            answer += "fileLogINFO = " + fileLogINFO.ToString() + '\n';
            answer += "fileLogWARNING = " + fileLogWARNING.ToString() + '\n';
            answer += "fileLogERROR = " + fileLogERROR.ToString() + '\n';
            answer += "fileLogFATAL = " + fileLogFATAL.ToString() + '\n';
            answer += "fileLogTRACE = " + fileLogTRACE.ToString() + '\n';
            answer += "filePath = " + filePath.ToString(); // + '\n';
            return answer;
        }
    }

    public class LoggerException : Exception
    {
        public LoggerException(string message) : base(message)
        {
        }
    }
}
