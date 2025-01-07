using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trofeoCazador.Utilidades
{
    public class ArgumentosDeEventoDeClicDeBotón
    {
        public string NombreBoton { get; private set; }
        public string NombreUsuario { get; private set; }

        public ArgumentosDeEventoDeClicDeBotón(string nombreBoton, string nombreUsuario)
        {
            NombreBoton = nombreBoton;
            NombreUsuario = nombreUsuario;
        }
    }
}
