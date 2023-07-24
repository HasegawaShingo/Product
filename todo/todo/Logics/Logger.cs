using System.Reflection;
using log4net;

namespace todo.Logics
{
    internal static class Logger
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static void WriteError(string methodName, string message)
        {
            logger.Error($"method name : {methodName} / message : {message}");
        }

        internal static void WriteFatal(string methodName, string message)
        {
            logger.Fatal($"method name : {methodName} / message : {message}");
        }

        internal static void WriteDebug(string methodName, string message)
        {
            logger.Debug($"method name : {methodName} / message : {message}");
        }

        internal static void WriteInfo(string methodName, string message)
        {
            logger.Info($"method name : {methodName} / message : {message}");
        }

        internal static void WriteWarn(string methodName, string message)
        {
            logger.Warn($"method name : {methodName} / message : {message}");
        }
    }
}
