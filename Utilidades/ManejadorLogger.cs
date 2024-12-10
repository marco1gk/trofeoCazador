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
        private static ILogger logger;

        private static void ConfigurarLogger(string rutaDelArchivoDeRegistro)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(rutaDelArchivoDeRegistro)
                .CreateLogger();
        }

        private static string ConstruirRutaDelArchivoDeRegistro()
        {
            string formatoFecha = "dd-MM-yyyy";
            string idArchivoNombre = "Log";
            string caracterSeparador = "_";
            string extensionArchivo = ".txt";
            string rutaRelativaDelArchivoRegistro = "../../Logs\\";

            DateTime fechaActual = DateTime.Today;
            string fecha = fechaActual.ToString(formatoFecha);

            string nombreDelArchivoDeRegistro = idArchivoNombre + caracterSeparador + fecha + extensionArchivo;
            string rutaAbsolutaDelArchivoDeRegistro = Utilidades.ConstruirRutaAbsoluta(rutaRelativaDelArchivoRegistro);
            string rutaDeRegistro = rutaAbsolutaDelArchivoDeRegistro + nombreDelArchivoDeRegistro;

            return rutaDeRegistro;
        }

        public static ILogger ObtenerLogger()
        {
            if (logger == null)
            {
                string rutaDeRegistro = ConstruirRutaDelArchivoDeRegistro();
                ConfigurarLogger(rutaDeRegistro);
            }

            logger = Log.Logger;
            return logger;
        }

        public static void CerrarYVaciar()
        {
            (logger as IDisposable)?.Dispose();
            Log.CloseAndFlush();
            logger = null;
        }
    }
}
