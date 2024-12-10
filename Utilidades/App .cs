using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace trofeoCazador
{
    public partial class App : Application
    {
        public static void CambiarIdioma(string codigoCultura)
        {
            CultureInfo nuevaCultura = new CultureInfo(codigoCultura);
            Thread.CurrentThread.CurrentCulture = nuevaCultura;
            Thread.CurrentThread.CurrentUICulture = nuevaCultura;
        }
    }
}