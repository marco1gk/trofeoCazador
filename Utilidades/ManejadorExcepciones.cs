using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using trofeoCazador.Vistas.InicioSesion;
using System.Windows;

namespace trofeoCazador.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger logger = ManejadorLogger.ObtenerLogger();

        public static void ManejarErrorExcepcion(Exception ex, NavigationService servicioNavegacion)
        {
            logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (servicioNavegacion != null)
            {
                servicioNavegacion.Navigate(new XAMLInicioSesion());
            }
        }

        public static void ManejarFatalExcepcion(Exception ex, NavigationService servicioNavegacion)
        {
            logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (servicioNavegacion != null)
            {
                servicioNavegacion.Navigate(new XAMLInicioSesion());
            }
        } 
        public static void ManejarComponenteErrorExcepcion(Exception ex)
        {
            logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarComponenteFatalExcepcion(Exception ex)
        {
            logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarErrorExcepcion(Exception ex, Window ventana)
        {
            logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close(); 
                ventana.Show(); 
            }
        }

        public static void ManejarFatalExcepcion(Exception ex, Window ventana)
        {
            logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");

            if (ventana != null)
            {
                ventana.Close();
                ventana.Show();  
            }
        }


    }
}
