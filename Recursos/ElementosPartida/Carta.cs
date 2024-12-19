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

        public static string ObtenerRutaImagenCarta(int idRutaImagen)
        {
            switch (idRutaImagen)
            {
                case 1:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.jpg";
                case 2:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.jpg";
                case 3:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.jpg";
                case 4:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.jpg";
                case 5:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.jpg";
                case 6:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.jpg";
                case 7:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.jpg";
                case 8:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.jpg";
                case 9:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta_Mazo.png";
                default:
                    return "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta_Mazo.png";
            }
        }
    }
}
