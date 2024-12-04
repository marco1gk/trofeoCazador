using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.ServicioDelJuego;

namespace trofeoCazador.Recursos.ElementosPartida
{
    /*public class Mazo
    {
        public List<Carta> Cartas { get; set; }
        public Mazo()
        {
            Cartas = new List<Carta>();
        }

        public void InicializarMazo()
        {
            // Lista temporal para crear todas las cartas como instancias separadas
            var cartasTemporales = new List<Carta>();

            // Contador para los IDs de las cartas
            int idCounter = 1;

            // Agregar cartas al mazo asignando IDs únicos
            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta1", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 5)
                                                 .Select(_ => new Carta("Carta2", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 7)
                                                 .Select(_ => new Carta("Carta3", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 9)
                                                 .Select(_ => new Carta("Carta4", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 11)
                                                 .Select(_ => new Carta("Carta5", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 13)
                                                 .Select(_ => new Carta("Carta6", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta7", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 2)
                                                 .Select(_ => new Carta("Carta8", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png")));

            // Asignar el mazo completo
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
    }*/
}
