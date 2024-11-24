using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.Vistas.Amigos;
using Serilog;

namespace trofeoCazador.Utilidades
{
    public static class ManejadorLogger
    {
        private static ILogger _logger;

        private static void ConfigureLogger(string logFilePath)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@logFilePath)
                .CreateLogger();
        }

        private static string BuildLogFilePath()
        {
            string dateFormat = "dd-MM-yyyy";
            string idFileName = "Log";
            string characterSeparation = "_";
            string fileExtension = ".txt";
            string relativeLogFilePath = "../../Logs\\";

            DateTime currentDate = DateTime.Today;
            string date = currentDate.ToString(dateFormat);

            string logFileName = idFileName + characterSeparation + date + fileExtension;
            string absoluteLogFilePath = Utilidades.BuildAbsolutePath(relativeLogFilePath);
            string logPath = absoluteLogFilePath + logFileName;

            return logPath;
        }

        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                string logPath = BuildLogFilePath();
                ConfigureLogger(logPath);
            }

            _logger = Log.Logger;
            return _logger;
        }

        public static void CloseAndFlush()
        {
            (_logger as IDisposable)?.Dispose();
            Log.CloseAndFlush();
            _logger = null;
        }
    }
}
