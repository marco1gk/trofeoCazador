using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Recursos.Cartas;
using trofeoCazador.Recursos.Fichas;
using System.Windows.Media.Animation;
using trofeoCazador.Recursos.Dado;
using System.Windows.Input;
using trofeoCazador.Recursos;
using System.ServiceModel;
using System.Threading.Tasks;


namespace trofeoCazador.Vistas.PartidaJuego
{
    public partial class XAMLTablero : Page, IJuegoServiceCallback
    {
        private JuegoServiceClient client;
        private List<LobbyPlayer> jugadores;
        private List<MatchPlayer> jugadoresPartida;
        private string codigoPartida;
        private JugadorDataContract jugadorActual = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        private Mazo mazo;  // Usamos la clase Mazo
        private Dictionary<string, ObservableCollection<Carta>> cartasDeJugadores = new Dictionary<string, ObservableCollection<Carta>>();
        public ObservableCollection<Ficha> Fichas { get; set; } = new ObservableCollection<Ficha>();
        private Dado dado;

        public XAMLTablero(List<LobbyPlayer> jugadores, string codigoPartida)
        {
            InitializeComponent();
            this.jugadores = jugadores;
            mazo = new Mazo();  // Inicializamos el mazo
            mazo.InicializarMazo();  // Llamamos al método para inicializar las cartas
            CargarFichas();
            dado = new Dado(DadoImagen);
            BarajarYRepartirCartas(jugadores);
            this.codigoPartida = codigoPartida;
        }

        public void MostrarJugadores()
        {
            var jugadoresEnPartida = jugadores
                .Where(j => j.Username != jugadorActual.NombreUsuario)
                .ToList();

            if (jugadoresEnPartida.Count > 0)
            {
                NombreJugador2.Text = jugadoresEnPartida[0].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[0].NumeroFotoPerfil);
                Jugador2Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador2.Visibility = Visibility.Visible;
            }
            if (jugadoresEnPartida.Count > 1)
            {
                NombreJugador3.Text = jugadoresEnPartida[1].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[1].NumeroFotoPerfil);
                Jugador3Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador3.Visibility = Visibility.Visible;
            }
            if (jugadoresEnPartida.Count > 2)
            {
                NombreJugador4.Text = jugadoresEnPartida[2].Username;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[2].NumeroFotoPerfil);
                Jugador4Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador4.Visibility = Visibility.Visible;
            }
        }

        private void BarajarYRepartirCartas(List<LobbyPlayer> jugadores)
        {
            Console.WriteLine("Barajando el mazo de cartas...");
            mazo.Barajar();  // Barajamos el mazo

            int[] cartasPorJugador = { 3, 4, 5, 6 };

            for (int i = 0; i < jugadores.Count; i++)
            {
                var jugador = jugadores[i];
                int cantidadCartas = cartasPorJugador[i];

                // Inicializar la colección de cartas para el jugador
                cartasDeJugadores[jugador.Username] = new ObservableCollection<Carta>();

                // Repartir las cartas
                var cartasRepartidas = mazo.RepartirCartas(cantidadCartas);
                foreach (var carta in cartasRepartidas)
                {
                    cartasDeJugadores[jugador.Username].Add(carta);
                }

                Console.WriteLine($"Cartas repartidas a {jugador.Username}:");
                foreach (var carta in cartasDeJugadores[jugador.Username])
                {
                    Console.WriteLine($"- {carta.Tipo}");
                }
            }

            // Si el jugador actual tiene cartas, las mostramos en el ItemsControl
            if (jugadores.Count > 0 && cartasDeJugadores.ContainsKey(jugadorActual.NombreUsuario))
            {
                CartasManoItemsControl.ItemsSource = cartasDeJugadores[jugadorActual.NombreUsuario];
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

        private void CargarFichas()
        {
            for (int i = 1; i <= 6; i++)
            {
                Fichas.Add(new Ficha { RutaImagenFicha = $"/Recursos/ImagenesPartida/Fichas/Ficha{i}.png" });
            }
            FichasItemsControl.ItemsSource = Fichas;
        }

        private async void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dado.LanzarDado(); // Lanza el dado y obtiene el resultado
            MessageBoxResult decision = MessageBox.Show(
                "Obtuviste... ¿Quieres continuar o parar?",
                "Decisión de turno",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (decision == MessageBoxResult.Yes)
            {
                // El jugador decide continuar
                Console.WriteLine("El jugador decide continuar.");
                // Opcionalmente, puedes llamar algún método para realizar una acción extra.
            }
            else
            {
                // El jugador decide terminar su turno
                Console.WriteLine("El jugador decide parar.");
                await Task.Run(() => client.EndTurn(codigoPartida, jugadorActual.NombreUsuario));
            }
        }

        public void NotifyTurnStarted(string playerId)
        {
            if (playerId == jugadorActual.NombreUsuario)
            {
                Console.WriteLine("Es tu turno.");
                DadoImagen.IsEnabled = true; // Habilita el dado solo para el jugador actual
            }
            else
            {
                Console.WriteLine($"Es el turno de {playerId}.");
                DadoImagen.IsEnabled = false; // Deshabilita el dado para los demás
            }
        }

        public void NotifyTurnEnded(string playerId)
        {
            Console.WriteLine($"El turno de {playerId} ha terminado.");
            if (playerId == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = false; // Deshabilita el dado al terminar el turno
                MessageBox.Show("Tu turno ha terminado.", "Fin de turno", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void NotifyActionResult(string action, bool success)
        {
            //TODO
        }

        private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
        {
            client.StartMatch(jugadoresPartida.ToArray(), codigoPartida);
        }
    }
}

