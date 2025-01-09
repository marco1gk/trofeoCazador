using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Utilidades
{
    public static class GestorColores
    {
        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            SolidColorBrush solidColorBrush = null;

            if (hexadecimalColor != null)
            {
                try
                {
                    solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Error al convertir color: {ex.Message}");
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }

            return solidColorBrush;
        }

    }
}

