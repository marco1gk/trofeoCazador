using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
namespace trofeoCazador.Utilidades
{
    public static class Utilidades
    {
        public static string ConstruirRutaAbsoluta(string rutaRelativa)
        {
            string rutaAbsoluta = "";

            if (rutaRelativa != null)
            {
                rutaAbsoluta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
            }

            return rutaAbsoluta;
        }


    }
}
