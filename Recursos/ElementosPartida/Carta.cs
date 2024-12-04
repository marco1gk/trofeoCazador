using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trofeoCazador.Recursos.ElementosPartida
{
    public class CartaCliente
    {
        public int IdCarta { get; set; }
        public string Tipo { get; set; }
        //public int Cantidad { get; set; }
        public string RutaImagen { get; set; }
        public double PosicionX { get; set; } 
        public double PosicionY { get; set; }
        public bool Asignada { get; set; }

        /*public CartaCliente(string tipo, int idCarta, string rutaImagen)
        {
            IdCarta = idCarta;
            Tipo = tipo;
            //Cantidad = cantidad;
            RutaImagen = rutaImagen;
            PosicionX = 0;
            PosicionY = 0;
            Asignada = false;
        }*/
    }
}
