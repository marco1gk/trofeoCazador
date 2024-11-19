using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using trofeoCazador.ServicioDelJuego;
using System.Windows.Media.Animation;
using System.Windows.Input;
using trofeoCazador.Recursos;
using System.ServiceModel;
using System.Threading.Tasks;
using trofeoCazador.Recursos.ElementosPartida;
using System.Windows.Media;

namespace trofeoCazador.Vistas.PartidaJuego
{
    public partial class XAMLTablero : Page, IServicioPartidaCallback
    {
        private ServicioPartidaClient cliente;
        private List<JugadorPartida> jugadores;
        private string idPartida;
        private JugadorDataContract jugadorActual = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        private Mazo mazo;
        private Dictionary<string, ObservableCollection<Carta>> cartasDeJugadores = new Dictionary<string, ObservableCollection<Carta>>();
        public ObservableCollection<Ficha> Fichas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEnMano { get; set; } = new ObservableCollection<Ficha>();

        private Dado dado;

        public XAMLTablero(List<JugadorPartida> jugadores, string idPartida)
        {
            InitializeComponent();
            SetupClient();
            this.jugadores = jugadores;
            mazo = new Mazo();
            mazo.InicializarMazo();
            CargarFichas();
            dado = new Dado(DadoImagen);
            dado.DadoLanzado += ManejarResultadoDado;
            this.idPartida = idPartida;
            MostrarJugadores();
            cliente.RegistrarJugador(jugadorActual.NombreUsuario);
            DadoImagen.IsEnabled = false;
            //FichasItemsControl.ItemsSource = Fichas;
            FichasManoItemsControl.ItemsSource = FichasEnMano;

        }

        private void SetupClient()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            cliente = new ServicioPartidaClient(instanceContext);
        }

        public void MostrarJugadores()
        {
            var jugadoresEnPartida = jugadores
                .Where(j => j.NombreUsuario != jugadorActual.NombreUsuario)
                .ToList();

            if (jugadoresEnPartida.Count > 0)
            {
                NombreJugador2.Text = jugadoresEnPartida[0].NombreUsuario;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[0].NumeroFotoPerfil);
                Jugador2Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador2.Visibility = Visibility.Visible;
            }
            if (jugadoresEnPartida.Count > 1)
            {
                NombreJugador3.Text = jugadoresEnPartida[1].NombreUsuario;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[1].NumeroFotoPerfil);
                Jugador3Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador3.Visibility = Visibility.Visible;
            }
            if (jugadoresEnPartida.Count > 2)
            {
                NombreJugador4.Text = jugadoresEnPartida[2].NombreUsuario;
                string rutaImagen = ObtenerRutaImagenPerfil(jugadoresEnPartida[2].NumeroFotoPerfil);
                Jugador4Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                AreaJugador4.Visibility = Visibility.Visible;
            }
        }

        private void BarajarYRepartirCartas(List<JugadorPartida> jugadores)
        {
            Metodos.MostrarMensaje("Barajando el mazo de cartas...");
            mazo.Barajar();

            int[] cartasPorJugador = { 3, 4, 5, 6 };

            for (int i = 0; i < jugadores.Count; i++)
            {
                var jugador = jugadores[i];
                int cantidadCartas = cartasPorJugador[i];

                cartasDeJugadores[jugador.NombreUsuario] = new ObservableCollection<Carta>();

                var cartasRepartidas = mazo.RepartirCartas(cantidadCartas);
                foreach (var carta in cartasRepartidas)
                {
                    cartasDeJugadores[jugador.NombreUsuario].Add(carta);
                }

                Console.WriteLine($"Cartas repartidas a {jugador.NombreUsuario}:");
                foreach (var carta in cartasDeJugadores[jugador.NombreUsuario])
                {
                    Console.WriteLine($"- {carta.Tipo}");
                }
            }

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
                Fichas.Add(new Ficha { IdFicha = i, RutaImagenFicha = $"/Recursos/ElementosPartida/ImagenesPartida/Fichas/Ficha{i}.png" });
            }
            FichasItemsControl.ItemsSource = Fichas;
        }

        private async void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dado.LanzarDado();
            MessageBoxResult decision = MessageBox.Show(
                "Obtuviste... ¿Quieres continuar o parar?",
                "Decisión de turno",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (decision != MessageBoxResult.Yes)
            {
                //Metodos.MostrarMensaje("El jugador decide parar.");
                //Metodos.MostrarMensaje($"IDPartida que se esta mandando: {idPartida}");
                //Metodos.MostrarMensaje($"Jugador actual que se esta mandando: {jugadorActual.NombreUsuario}");
                await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
                ResetearFichas();
            }
        }

        public void NotificarTurnoIniciado(string nombreUsuario)
        {
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                Metodos.MostrarMensaje("Es tu turno.");
                DadoImagen.IsEnabled = true;
            }
            else
            {
                Metodos.MostrarMensaje($"Es el turno de {nombreUsuario}.");
                DadoImagen.IsEnabled = false;
            }
        }

        public void NotificarTurnoTerminado(string nombreUsuario)
        {
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = false;
                Console.WriteLine("Resetear fichas del jugador porque terminó su turno."); // Log
                ResetearFichas();
            }
            else
            {
                Console.WriteLine($"El turno de {nombreUsuario} terminó, pero no es el jugador actual.");
            }
        }

        public void NotificarResultadoAccion(string action, bool success)
        {
            //TODO
        }

        public void NotificarPartidaCreada(string idPartida)
        {
            this.idPartida = idPartida;
            BarajarYRepartirCartas(jugadores);
        }

        private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
        {
            try
            {
                cliente.CrearPartida(jugadores.ToArray(), idPartida);
                cliente.EmpezarTurno(idPartida);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la partida: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Thickness margenInicialCarta = new Thickness();

        private void CartaMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                if (margenInicialCarta == new Thickness())
                {
                    margenInicialCarta = border.Margin;
                }

                ThicknessAnimation animacionMargen = new ThicknessAnimation
                {
                    From = border.Margin,
                    To = new Thickness(margenInicialCarta.Left, margenInicialCarta.Top - 20, margenInicialCarta.Right, margenInicialCarta.Bottom),
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                border.BeginAnimation(Border.MarginProperty, animacionMargen);
            }
        }

        private void CartaMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                ThicknessAnimation animacionMargen = new ThicknessAnimation
                {
                    From = border.Margin,
                    To = margenInicialCarta,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                border.BeginAnimation(Border.MarginProperty, animacionMargen);
            }
        }

        private void CartaMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = border.BorderBrush == Brushes.Blue ? Brushes.Transparent : Brushes.Blue;
                border.BorderThickness = new Thickness(border.BorderThickness == new Thickness(0) ? 2 : 0);
            }
        }

        private async void ManejarResultadoDado(int resultadoDado)
        {
            Metodos.MostrarMensaje($"Número obtenido: {resultadoDado}");

            {
                // Obtener ficha seleccionada
                Ficha fichaSeleccionada = Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);

                // Verificar si la ficha ya está en las manos del jugador
                if (!FichasEnMano.Contains(fichaSeleccionada))
                {
                    // Agregar ficha a la mano del jugador
                    FichasEnMano.Add(fichaSeleccionada);
                    Fichas.Remove(fichaSeleccionada);
                    //Metodos.MostrarMensaje($"Ficha {resultadoDado} agregada a tu mano.");
                }
                else
                {
                    // Finalizar turno si ya tiene la ficha
                    //Metodos.MostrarMensaje("Ya tienes esta ficha. Finalizando tu turno...");
                    await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
                }
            }
        }

        private void ResetearFichas()
        {
            // Mover todas las fichas de la mano de regreso al área general
            foreach (var ficha in FichasEnMano.ToList()) // Usar ToList para evitar modificaciones durante la iteración
            {
                Fichas.Add(ficha); // Agregar ficha a la colección principal
            }

            // Limpiar la colección de fichas en mano
            FichasEnMano.Clear();

        }

    }
}


