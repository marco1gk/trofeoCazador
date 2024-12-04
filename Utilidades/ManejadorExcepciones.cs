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
        private static readonly ILogger _logger = ManejadorLogger.ObtenerLogger();

        public static void ManejarErrorExcepcion(Exception ex, NavigationService servicioNavegacion)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (servicioNavegacion != null)
            {
                servicioNavegacion.Navigate(new XAMLInicioSesion());
            }
        }

        public static void ManejarFatalExcepcion(Exception ex, NavigationService servicioNavegacion)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (servicioNavegacion != null)
            {
                servicioNavegacion.Navigate(new XAMLInicioSesion());
            }
        } 
        public static void ManejarComponenteErrorExcepcion(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarComponenteFatalExcepcion(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }



    }
}
