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
        public static string BuildAbsolutePath(string relativePath)
        {
            string absolutePath = "";

            if (relativePath != null)
            {
                absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            return absolutePath;
        }


    }
}
