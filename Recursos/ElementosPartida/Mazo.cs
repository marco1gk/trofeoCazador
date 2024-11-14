using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.ServicioDelJuego;

namespace trofeoCazador.Recursos.ElementosPartida
{
    public class Mazo
    {
        public List<Carta> Cartas { get; private set; }
        public Mazo()
        {
            Cartas = new List<Carta>();
        }
        public void InicializarMazo()
        {
            // Aquí se inicializan las cartas en el mazo
            Cartas.Add(new Carta("Carta1", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png"));
            Cartas.Add(new Carta("Carta2", 5, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png"));
            Cartas.Add(new Carta("Carta3", 7, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png"));
            Cartas.Add(new Carta("Carta4", 9, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png"));
            Cartas.Add(new Carta("Carta5", 11, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png"));
            Cartas.Add(new Carta("Carta6", 13, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png"));
            Cartas.Add(new Carta("Carta7", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png"));
            Cartas.Add(new Carta("Carta8", 2, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png"));
            // Repite este proceso para todas las cartas
            Cartas = Cartas.SelectMany(carta => Enumerable.Repeat(carta, carta.Cantidad)).ToList();
        }
        public void Barajar()
        {
            Random rnd = new Random();
            Cartas = Cartas.OrderBy(x => rnd.Next()).ToList(); // Baraja el mazo
        }
        public List<Carta> RepartirCartas(int cantidad)
        {
            var cartasRepartidas = Cartas.Take(cantidad).ToList();
            Cartas = Cartas.Skip(cantidad).ToList();
            return cartasRepartidas;
        }
    }
}
