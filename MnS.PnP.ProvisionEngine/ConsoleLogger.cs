using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public class ConsoleLogger : ILogger
    {
        public void LogMessage(string message, LogType logType)
        {
            try
            {
                switch (logType)
                {
                    case LogType.ErrorAndAbort:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(string.Format("{0}{1}", Environment.NewLine, message));
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(Constants.AbortCommand);
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;
                    case LogType.ErrorAndContinue:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(string.Format("{0}{1}", Environment.NewLine, message));
                        break;
                    case LogType.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(string.Format("{0}{1}", Environment.NewLine, message));
                        break;
                    case LogType.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("{0}{1}", Environment.NewLine, message));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void LogMessage(object message)
        {
        }
    }
}
