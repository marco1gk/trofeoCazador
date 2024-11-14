using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trofeoCazador.Recursos.ElementosPartida
{
    public class Carta
    {
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public string RutaImagen { get; set; }
        public Carta(string tipo, int cantidad, string rutaImagen)
        {
            Tipo = tipo;
            Cantidad = cantidad;
            RutaImagen = rutaImagen;
        }
    }
}
