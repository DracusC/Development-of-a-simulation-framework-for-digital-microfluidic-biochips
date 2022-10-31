using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class InactiveLogger : ILogger
    {

        public void Info(string message) { }
        public void Info(string format, params object[] parameters) { }

        public void Debug(string message) { }
        public void Debug(string format, params object[] parameters) { }
        
        public void Warning(string message) { }
        public void Warning(string format, params object[] parameters) { }

        public void Error(string message) { }
        public void Error(string format, params object[] parameters) { }
        public void Error(Exception ex, string message) { }
        public void Error(Exception ex, string format, params object[] parameters) { }

        public void Fatal(string message) { }
        public void Fatal(string format, params object[] parameters) { }
        public void Fatal(Exception ex, string message) { }
        public void Fatal(Exception ex, string format, params object[] parameters) { }

        public void Trace(string message) { }
        public void Trace(string format, params object[] parameters) { }
    }
}
