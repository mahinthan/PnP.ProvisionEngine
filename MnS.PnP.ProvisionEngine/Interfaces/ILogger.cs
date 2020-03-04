using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public interface ILogger
    {
        void LogMessage(string message, LogType logType);
        void LogMessage(object sPConnectionAttempt);
    }

    public enum LogType
    {
        ErrorAndAbort,
        ErrorAndContinue,
        Info,
        Success
    }
}
