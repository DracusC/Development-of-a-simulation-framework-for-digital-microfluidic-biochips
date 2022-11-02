using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    /// <summary>
    /// Interface for the logger.
    /// </summary>
    interface ILogger
    {
        /// <summary>
        /// Logs INFO level message (INFO: Application events for general purposes).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Info(string message);


        /// <summary>
        /// Logs INFO level message (INFO: Application events for general purposes).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Info(string format, params object[] parameters);


        /// <summary>
        /// Logs DEBUG level message (DEBUG: Additional information about application behavior for cases when that information is necessary to diagnose problems).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Debug(string message);


        /// <summary>
        /// Logs DEBUG level message (DEBUG: Additional information about application behavior for cases when that information is necessary to diagnose problems).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Debug(string format, params object[] parameters);


        /// <summary>
        /// Logs WARNING level message (WARNING: Application events that may be an indication of a problem).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Warning(string message);


        /// <summary>
        /// Logs WARNING level message (WARNING: Application events that may be an indication of a problem).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Warning(string format, params object[] parameters);


        /// <summary>
        /// Logs ERROR level message (ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Error(string message);


        /// <summary>
        /// Logs ERROR level message (ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Error(string format, params object[] parameters);


        /// <summary>
        /// Logs ERROR level message (ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data).
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="message">Message to be logged.</param>
        void Error(Exception ex, string message);


        /// <summary>
        /// Logs ERROR level message (ERROR: Typically logged in the catch block a try/catch block, includes the exception and contextual data).
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Error(Exception ex, string format, params object[] parameters);


        /// <summary>
        /// Logs FATAL level message (FATAL: A critical error that results in the termination of an application).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Fatal(string message);


        /// <summary>
        /// Logs FATAL level message (FATAL: A critical error that results in the termination of an application).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Fatal(string format, params object[] parameters);


        /// <summary>
        /// Logs FATAL level message (FATAL: A critical error that results in the termination of an application).
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="message">Message to be logged.</param>
        void Fatal(Exception ex, string message);


        /// <summary>
        /// Logs FATAL level message (FATAL: A critical error that results in the termination of an application).
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Fatal(Exception ex, string format, params object[] parameters);


        /// <summary>
        /// Logs TRACE level message (TRACE: Used to mark the entry and exit of functions, for purposes of performance profiling).
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Trace(string message);


        /// <summary>
        /// Logs TRACE level message (TRACE: Used to mark the entry and exit of functions, for purposes of performance profiling).
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An object to write using format.</param>
        void Trace(string format, params object[] parameters);

    }
}
