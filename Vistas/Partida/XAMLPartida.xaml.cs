using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using trofeoCazador.ServicioDelJuego;

namespace trofeoCazador.Vistas.Partida
{
    /// <summary>
    /// Lógica de interacción para XAMLPartida.xaml
    /// </summary>
    public partial class XAMLPartida : Page
    {
        public XAMLPartida()
        {
            InitializeComponent();
        }

        public void MostrarJugadores(List<LobbyPlayer> jugadores)
        {
            Console.WriteLine("Mostrando jugadores en la página de partida:");

            if (jugadores.Count > 0)
            {
                Console.WriteLine($"Mostrando Jugador 2: {jugadores[0].Username}");
                NombreJugador2.Text = jugadores[0].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadores[0].NumeroFotoPerfil);
                Jugador2Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador2.Visibility = Visibility.Visible;
            }

            if (jugadores.Count > 1)
            {
                Console.WriteLine($"Mostrando Jugador 3: {jugadores[1].Username}");
                NombreJugador3.Text = jugadores[1].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadores[1].NumeroFotoPerfil);
                Jugador3Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador3.Visibility = Visibility.Visible;
            }

            if (jugadores.Count > 2)
            {
                Console.WriteLine($"Mostrando Jugador 4: {jugadores[2].Username}");
                NombreJugador4.Text = jugadores[2].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadores[2].NumeroFotoPerfil);
                Jugador4Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador4.Visibility = Visibility.Visible;
            }
        }

        private string ObtenerRutaImagenPerfil(int numeroFotoPerfil)
        {
            switch (numeroFotoPerfil)
            {
                case 1:
                    return "/Recursos/FotosPerfil/abeja.jpg";
                case 2:
                    return "/Recursos/FotosPerfil/cazador.jpg";
                default:
                    return "/Recursos/FotosPerfil/cazador.jpg";
            }
        }
    }
}
