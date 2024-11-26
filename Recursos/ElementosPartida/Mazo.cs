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
        public List<Carta> Cartas { get; set; }
        public Mazo()
        {
            Cartas = new List<Carta>();
        }
        /*public void InicializarMazo()
        {
            Cartas.Add(new Carta("Carta1", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png"));
            Cartas.Add(new Carta("Carta2", 5, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png"));
            Cartas.Add(new Carta("Carta3", 7, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png"));
            Cartas.Add(new Carta("Carta4", 9, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png"));
            Cartas.Add(new Carta("Carta5", 11, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png"));
            Cartas.Add(new Carta("Carta6", 13, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png"));
            Cartas.Add(new Carta("Carta7", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png"));
            Cartas.Add(new Carta("Carta8", 2, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png"));
            Cartas = Cartas.SelectMany(carta => Enumerable.Repeat(carta, carta.Cantidad)).ToList();
        }*/

        public void InicializarMazo()
        {
            // Lista temporal para crear todas las cartas como instancias separadas
            var cartasTemporales = new List<Carta>();

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta1", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png"), 3)
                                                 .Select(_ => new Carta("Carta1", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta2", 5, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png"), 5)
                                                 .Select(_ => new Carta("Carta2", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta3", 7, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png"), 7)
                                                 .Select(_ => new Carta("Carta3", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png")));
            
            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta4", 9, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png"), 9)
                                                 .Select(_ => new Carta("Carta4", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta5", 11, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png"), 11)
                                                 .Select(_ => new Carta("Carta5", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta6", 13, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png"), 13)
                                                 .Select(_ => new Carta("Carta6", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta7", 3, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png"), 3)
                                                .Select(_ => new Carta("Carta7", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(new Carta("Carta8", 2, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png"), 2)
                                                 .Select(_ => new Carta("Carta8", 1, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png")));


            Cartas = cartasTemporales;
        }
        public void Barajar()
        {
            Random random = new Random();
            for (int i = Cartas.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1); // Elegir un índice aleatorio entre 0 e i
                var temp = Cartas[i];
                Cartas[i] = Cartas[j];
                Cartas[j] = temp;
            }
        }

        public List<Carta> RepartirCartas(int cantidad)
        {
            var cartasRepartidas = Cartas.Take(cantidad).ToList();
            Cartas = Cartas.Skip(cantidad).ToList();
            return cartasRepartidas;
        }
    }
}
