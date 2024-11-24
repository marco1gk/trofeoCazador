using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = ManejadorLogger.GetLogger();

        public static void HandleErrorException(Exception ex, NavigationService navigationService)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (navigationService != null)
            {
                navigationService.Navigate(new XAMLInicioSesion());
            }
        }

        public static void HandleFatalException(Exception ex, NavigationService navigationService)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (navigationService != null)
            {
                navigationService.Navigate(new XAMLInicioSesion());
            }
        }

        public static void HandleComponentErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void HandleComponentFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }



    }
}
