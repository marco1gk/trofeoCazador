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

        private string jugadorTurnoActual;
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
            //dado.DadoLanzado += ManejarResultadoDado;
            this.idPartida = idPartida;
            MostrarJugadores();
            cliente.RegistrarJugador(jugadorActual.NombreUsuario);
            DadoImagen.IsEnabled = false;
            //FichasItemsControl.ItemsSource = Fichas;
            FichasManoItemsControl.ItemsSource = FichasEnMano;
            //ZonaFichasJugador2.ItemsSource = FichasEnMano;
            //ZonaFichasJugador3.ItemsSource = FichasEnMano;
            //ZonaFichasJugador4.ItemsSource = FichasEnMano;
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

        private void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cliente.LanzarDado(idPartida, jugadorActual.NombreUsuario);
        }

        public void NotificarTurnoIniciado(string nombreUsuario)
        {
            ResetearFichas();
            jugadorTurnoActual = nombreUsuario;
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = true;
            }
            else
            {
                DadoImagen.IsEnabled = false;
            }
        }

        public void NotificarTurnoTerminado(string nombreUsuario)
        {
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = false;
                ResetearFichas();
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

        public void NotificarResultadoDado(string nombreUsuario, int resultadoDado)
        {
            dado.LanzarDado(resultadoDado);
            dado.DadoLanzado += ManejarResultadoDado;

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
            await Task.Delay(500);

            if (jugadorTurnoActual == jugadorActual.NombreUsuario)
            {
                Ficha fichaSeleccionada = Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);
                if (!FichasEnMano.Contains(fichaSeleccionada) && fichaSeleccionada != null)
                {
                    FichasEnMano.Add(fichaSeleccionada);
                    Fichas.Remove(fichaSeleccionada);

                    await Task.Delay(500);

                    MessageBoxResult decision = MessageBox.Show(
                        "Obtuviste una ficha. ¿Quieres continuar o parar?",
                        "Decisión de turno",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (decision != MessageBoxResult.Yes)
                    {
                        await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
                        
                    }
                }
                else
                {
                    Metodos.MostrarMensaje("Esta ficha ya la tenias, por lo que termina tu turno");
                    await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
                }

            }
            else
            {
                ZonaFichasJugador2.ItemsSource = FichasEnMano;
                Ficha fichaSeleccionada = Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);
                Fichas.Remove(fichaSeleccionada);
            }
            
        }

        private void ActualizarZonaFichas()
        {
            Metodos.MostrarMensaje($"Nombre jugador 2: {NombreJugador2.Text}");
            if(NombreJugador2.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador2.ItemsSource = FichasEnMano;
            }else if(NombreJugador3.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador3.ItemsSource = FichasEnMano;
            }else if(NombreJugador4.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador4.ItemsSource = FichasEnMano;
            }
        }


        private void ResetearFichas()
        {
            foreach (var ficha in FichasEnMano.ToList()) 
            {
                Fichas.Add(ficha);
            }

            FichasEnMano.Clear();

        }

        public void NotificarResultadoDado(string nombreUsuario, int resultado)
        {
            throw new NotImplementedException();
        }
    }
}


