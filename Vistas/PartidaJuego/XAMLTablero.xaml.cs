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
using System.Threading;
using System.Text;
using trofeoCazador.Vistas.Victoria;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;
using trofeoCazador.Vistas.SalaEspera;
using System.Diagnostics;

namespace trofeoCazador.Vistas.PartidaJuego
{
    public partial class XAMLTablero : Page, IServicioPartidaCallback
    {
        private ServicioPartidaClient cliente;
        private List<JugadorPartida> listaDeJugadores;
        private string idPartida;
        private JugadorDataContract jugadorActual = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        private ObservableCollection<CartaCliente> CartasEnMano { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<Ficha> Fichas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEstaticas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEnMano { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<CartaCliente> CartasDescarte { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<CartaCliente> CartasEnMazo { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<CartaCliente> CartasEnEscondite { get; set; } = new ObservableCollection<CartaCliente>();
        private Dado dado;
        private string jugadorTurnoActual;
        private bool CartaDuplicacionActiva = false;
        private bool jugadorDecidioParar = false;
        private Ficha fichaSeleccionada;
        private int cartasATomarMazo = 0;
        private int cartasAGuardarEscondite = 0;
        private int cartasTomadasMazo = 0;
        private int cartasGuardadasEscondite = 0;
        private ModoSeleccionCarta[] modosSeleccionCarta = Enum.GetValues(typeof(ModoSeleccionCarta)).Cast<ModoSeleccionCarta>().ToArray();
        private bool cartaTomada = false;
        private TaskCompletionSource<bool> jugadorGuardoCartaEnEscondite;
        private string tipoCartaRevelada;
        private const string CARTA_NO_DISPONIBLE = "Carta no disponible";

        private enum ModoSeleccionCarta
        {
            MoverAlEscondite,
            DefenderRobo,
            SalvarTurno,
            AccionCartasEnTurno,
            MoverCartaTipoEspecificoAEscondite,
            CartasSinTurno
        }

        private ModoSeleccionCarta modoSeleccionActual;
        public XAMLTablero(List<JugadorPartida> jugadores, string idPartida)
        {
            ValidarSubproceso();
            InitializeComponent();
            SetupClient();

            Application.Current.Dispatcher.Invoke(() =>
            {
                InicializarPropiedades(jugadores, idPartida);
                ManejarRegistroJugador();
                ConfigurarInterfaz();
            });
        }

        private static void ValidarSubproceso()
        {
            if (!Thread.CurrentThread.IsBackground && Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("El constructor XAMLTablero debe ejecutarse en un subproceso STA.");
            }
        }

        private void InicializarPropiedades(List<JugadorPartida> jugadores, string idPartida)
        {
            this.listaDeJugadores = jugadores;
            CargarFichasEstaticas();
            dado = new Dado(DadoImagen);
            this.idPartida = idPartida;
        }

        private void ManejarRegistroJugador()
        {
            if (jugadorActual.EsInvitado)
            {
                RegistrarJugadorInvitado();
            }
            else
            {
                RegistrarJugadorNormal();
            }
        }

        private void RegistrarJugadorInvitado()
        {
            JugadorPartida invitado = new JugadorPartida
            {
                NombreUsuario = jugadorActual.NombreUsuario,
                EsInvitado = true
            };
            try
            {
                cliente.RegistrarJugadorInvitado(invitado);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void RegistrarJugadorNormal()
        {
            try
            {
                cliente.RegistrarJugador(jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void ConfigurarInterfaz()
        {
            MostrarJugadores();
            FichasItemsControl.ItemsSource = Fichas;
            FichasManoItemsControl.ItemsSource = FichasEnMano;
            ZonaDescarte.ItemsSource = CartasDescarte;
            CartasManoItemsControl.ItemsSource = CartasEnMano;
            ZonaMazoCartas.ItemsSource = CartasEnMazo;
            CartasEsconditeItemsControl.ItemsSource = CartasEnEscondite;
        }


        public void NotificarJugadorDesconectado(string nombreUsuario)
        {
            string mensaje = Properties.Resources.lbAbandonarPartida + " " + nombreUsuario;
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, mensaje);
            listaDeJugadores = listaDeJugadores.Where(j => j.NombreUsuario != nombreUsuario).ToList();
            MostrarJugadores();

        }
        public void MostrarJugadores()
        {
            var jugadoresEnPartida = listaDeJugadores
                .Where(j => j.NombreUsuario != jugadorActual.NombreUsuario)
                .ToList();

            var areasDeJugadores = new[]
            {
        new { Nombre = NombreJugador2, Imagen = Jugador2Imagen, Area = AreaJugador2 },
        new { Nombre = NombreJugador3, Imagen = Jugador3Imagen, Area = AreaJugador3 },
        new { Nombre = NombreJugador4, Imagen = Jugador4Imagen, Area = AreaJugador4 }
             };

            for (int i = 0; i < areasDeJugadores.Length; i++)
            {
                var areaDeJugador = areasDeJugadores[i];

                if (i < jugadoresEnPartida.Count)
                {
                    var jugador = jugadoresEnPartida[i];

                    areaDeJugador.Nombre.Text = jugador.NombreUsuario;
                    string rutaImagen = ObtenerRutaImagenPerfil(jugador.NumeroFotoPerfil);
                    areaDeJugador.Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                    areaDeJugador.Area.Visibility = Visibility.Visible;
                }
                else
                {
                    areaDeJugador.Nombre.Text = string.Empty;
                    areaDeJugador.Imagen.Source = null;
                    areaDeJugador.Area.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SetupClient()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            cliente = new ServicioPartidaClient(instanceContext);
        }


        public void NotificarResultadosJuego(Dictionary<string, int> puntajes, string ganador, int puntajeGanador)
        {
            if (puntajes == null || !puntajes.Any())
                throw new ArgumentException("El diccionario de puntajes no puede ser nulo o vacío.");
            if (string.IsNullOrEmpty(ganador))
                throw new ArgumentException("El nombre del ganador no puede ser nulo o vacío.");

            try
            {
                var jugadores = puntajes.Keys
                    .ToDictionary(
                        nombreUsuario => nombreUsuario,
                        nombreUsuario =>
                        {
                            var jugadorId = ObtenerJugadorId(nombreUsuario);
                            Console.WriteLine($"Jugador: {nombreUsuario}, ID: {jugadorId}");
                            return jugadorId;
                        }
                    );

                var scoreboard = jugadores
                    .Select(kv => new KeyValuePair<JugadorDataContract, int>(
                        new JugadorDataContract
                        {
                            JugadorId = kv.Value,
                            NombreUsuario = kv.Key
                        },
                        puntajes[kv.Key]
                    ))
                    .ToArray();

                var paginaVictoria = new XAMLVictoria(idPartida, scoreboard, puntajeGanador);
                NavigationService?.Navigate(paginaVictoria);
            }
            catch (FaultException ex)
            {
                Console.WriteLine($"Error del servicio: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

        private int ObtenerJugadorId(string nombreUsuario)
        {
            GestionCuentaServicioClient gestorAmigos = new GestionCuentaServicioClient();
            int idJugador = -1;
            try
            {
                idJugador = gestorAmigos.ObtenerIdJugadorPorNombreUsuario(nombreUsuario);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            return idJugador;
        }




        private void Mazo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EsMazoVacio())
                {
                    cliente.FinalizarJuego(idPartida);
                }
                else
                {
                    var cartaSuperior = ObtenerCartaSuperiorDelMazo();
                    cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, cartaSuperior.IdCarta);
                }
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
        {
            try
            {
                cliente.CrearPartida(listaDeJugadores.ToArray(), idPartida);
                cliente.RepartirCartas(idPartida);
                cliente.EmpezarTurno(idPartida);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                Debug.WriteLine($"Error de conexión: {ex.Message}");
                CerrarSesion();
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                Debug.WriteLine($"Tiempo de espera excedido: {ex.Message}");
                CerrarSesion();
            }
            catch (CommunicationObjectFaultedException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                Debug.WriteLine($"El cliente ha fallado: {ex.Message}");
                CerrarSesion();
            }
            catch (CommunicationException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                Debug.WriteLine($"Error de comunicación: {ex.Message}");
                CerrarSesion();
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                CerrarSesion();
            }
        }

        private void CerrarSesion()
        {
            try
            {
                if (cliente != null)
                {
                    cliente.Abort();
                    cliente = null;
                }
                
                NavigationService.Navigate(new Uri("/trofeoCazador;component/Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cerrar la sesión: {ex.Message}");
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
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

        private void CargarFichas()
        {
            for (int i = 1; i <= 6; i++)
            {
                Fichas.Add(new Ficha { IdFicha = i, RutaImagenFicha = $"/Recursos/ElementosPartida/ImagenesPartida/Fichas/Ficha{i}.png" });
            }
        }

        private void CargarFichasEstaticas()
        {
            for (int i = 1; i <= 6; i++)
            {
                FichasEstaticas.Add(new Ficha { IdFicha = i, RutaImagenFicha = $"/Recursos/ElementosPartida/ImagenesPartida/Fichas/Ficha{i}.png" });
            }
        }
        private void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cliente.LanzarDado(idPartida, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                Debug.WriteLine($"Error de conexión: {ex.Message}");
                CerrarSesion();
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                Debug.WriteLine($"Tiempo de espera excedido: {ex.Message}");
                CerrarSesion();
            }
            catch (CommunicationObjectFaultedException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                Debug.WriteLine($"El cliente ha fallado: {ex.Message}");
                CerrarSesion();
            }
            catch (CommunicationException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                Debug.WriteLine($"Error de comunicación: {ex.Message}");
                CerrarSesion();
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                CerrarSesion();
            }
        }
    
        private static string ObtenerRutaImagenPerfil(int numeroFotoPerfil)
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

        private async void ManejarResultadoDado(int resultadoDado)
        {
            await Task.Delay(1000);
            Ficha fichaObtenida = SeleccionarFichaPorResultado(resultadoDado);

            if (PuedeTomarFicha(fichaObtenida))
            {
                try
                {
                    await cliente.TomarFichaMesaAsync(idPartida, fichaObtenida.IdFicha);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                await Task.Delay(1000);
                await ManejarDecisionContinuarTurno();
            }
            else
            {
                await ManejarFichaDuplicada();
            }
        }

        private Ficha SeleccionarFichaPorResultado(int resultadoDado)
        {
            return Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);
        }

        private static bool PuedeTomarFicha(Ficha fichaSeleccionada)
        {
            return fichaSeleccionada != null;
        }

        private async Task ManejarFichaDuplicada()
        {
            await Task.Delay(1000);

            if (JugadorTieneCartasClave())
            {
                bool decision = await SolicitarDecisionSalvarTurno();
                if (decision)
                {
                    SalvarTurno();
                }
                else
                {
                    await ManejarTurnoSinOpciones();
                }
            }
            else
            {
                await ManejarTurnoSinOpciones();
            }
        }

        private bool JugadorTieneCartasClave()
        {
            return VerificarCartaEnMano("Carta6") || VerificarCartaEnMano("Carta5");
        }

        private static async Task<bool> SolicitarDecisionSalvarTurno()
        {
            return await DecisionSalvarTurno();
        }

        private void SalvarTurno()
        {
            try
            {
                cliente.EstablecerModoSeleccionCarta(
                idPartida,
                Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.SalvarTurno),
                jugadorTurnoActual
            );
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }


        private bool VerificarCartaEnMano(string tipoCarta)
        {
            return CartasEnMano.Any(c => c.Tipo == tipoCarta);
        }

        private async Task ManejarTurnoSinOpciones()
        {
            VentanasEmergentes.CrearVentanaEmergente("Ficha repetida", "Esta ficha ya la tenías, por lo que termina tu turno. Pero puedes tomar una carta del mazo.");
            ZonaMazoCartas.IsEnabled = true;
            await EsperarTomaDeCarta();
            ZonaMazoCartas.IsEnabled = false;
            FinalizarTurno();
        }

        private async Task<bool> EsperarTomaDeCarta()
        {
            SuscribirEventoTomaDeCarta();

            cartaTomada = await EsperarCartaTomada();

            DesuscribirEventoTomaDeCarta();

            return cartaTomada;
        }

        private void SuscribirEventoTomaDeCarta()
        {
            ZonaMazoCartas.MouseDown += CartaTomada;
        }

        private void DesuscribirEventoTomaDeCarta()
        {
            ZonaMazoCartas.MouseDown -= CartaTomada;
        }

        private async Task<bool> EsperarCartaTomada()
        {
            while (!cartaTomada)
            {
                await Task.Delay(100);
            }
            return cartaTomada;
        }

        private void CartaTomada(object sender, MouseButtonEventArgs e)
        {
            cartaTomada = true;
        }



        private void FinalizarTurno()
        {
            try
            {
                cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }


        private Task ResolverFichas()
        {
            FichasManoItemsControl.IsEnabled = true;
            return Task.Run(async () =>
            {
                while (FichasEnMano.Count > 0)
                {
                    await Task.Delay(100);
                }
            });

        }

        private static async Task<bool> DecisionSalvarTurno()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decision de turno", "Obtuviste una ficha repetida, puedes usar una carta para salvarte.");
            return await ventanaDeDecision.MostrarDecision();
        }


        private async void SeleccionarCarta(object sender, MouseButtonEventArgs e)
        {
            var cartaSeleccionada = ObtenerCartaSeleccionada(sender);
            if (cartaSeleccionada != null)
            {
                switch (modoSeleccionActual)
                {
                    case ModoSeleccionCarta.MoverAlEscondite:
                        MoverCartaAEscondite(cartaSeleccionada);
                        break;

                    case ModoSeleccionCarta.DefenderRobo:
                        UsarCartaBloqueoRobo(cartaSeleccionada);
                        break;

                    case ModoSeleccionCarta.SalvarTurno:
                        await UsarCartaSalvacionTurno(cartaSeleccionada);
                        break;

                    case ModoSeleccionCarta.AccionCartasEnTurno:
                        UsarCartaEnTurno(cartaSeleccionada);
                        break;

                    case ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite:
                        MoverCartaTipoEspecificoAEscondite(cartaSeleccionada, tipoCartaRevelada);
                        CompletarGuardarCartaEscondite();
                        break;

                    case ModoSeleccionCarta.CartasSinTurno:
                        UsarCartaSinTurno(cartaSeleccionada);
                        break;

                    default:
                        Metodos.MostrarMensaje("Acción no válida.");
                        break;
                }
            }
            else
            {
                Metodos.MostrarMensaje("Debe seleccionar una carta");
            }
        }

        private static CartaCliente ObtenerCartaSeleccionada(object sender)
        {
            return (sender as Border)?.DataContext as CartaCliente;
        }

        private void MoverCartaAEscondite(CartaCliente carta)
        {
            try
            {
                cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, carta.IdCarta, idPartida);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }



        private bool EsMazoVacio()
        {
            return CartasEnMazo.Count() == 1;
        }

        private CartaCliente ObtenerCartaSuperiorDelMazo()
        {
            var cartaPenultima = CartasEnMazo[CartasEnMazo.Count - 2];
            return cartaPenultima;

        }


        private async void Ficha_MouseDown(object sender, MouseButtonEventArgs e)
        {
            fichaSeleccionada = (sender as Border)?.DataContext as Ficha;
            if (fichaSeleccionada != null)
            {
                await EjecutarAccionFicha(fichaSeleccionada);
            }
        }

        private async Task EjecutarAccionFicha(Ficha fichaSeleccionada)
        {
            switch (fichaSeleccionada.IdFicha)
            {
                case 1:
                    AccionFichaTomarCartasMazo();
                    break;

                case 2:
                    AccionFichaGuardarCartasEscondite();
                    break;

                case 3:
                    AccionFichaRobarOGuardar();
                    break;

                case 4:
                    AccionFichaRobarCartaAJugador();
                    break;

                case 5:
                    await AccionFichaRevelarCarta();
                    break;

                case 6:
                    AccionFichaCambiarFicha();
                    break;
                default:
                    Metodos.MostrarMensaje("Opcion incorrecta.");
                    break;
            }
        }

        private void AccionFichaCambiarFicha()
        {
            if (CartaDuplicacionActiva)
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta sin efecto", "Esta carta no tiene efecto en esta ficha.");
                CartaDuplicacionActiva = false;
            }

            if (Fichas.Count > 0)
            {
                VentanasEmergentes.CrearVentanaEmergenteNoModal("Selecciona una ficha", "Selecciona una ficha disponible en la mesa para intercambiarla.");

                FichasItemsControl.IsEnabled = true;
                FichasItemsControl.MouseDown += FichasItemsControl_MouseDown;
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Ficha sin efecto", "Esta ficha no tiene efecto ya que no queda alguna ficha por la cual intercambiarla.");
                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }
        }

        private void FichasItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fichaEnMesaSeleccionada = (e.OriginalSource as FrameworkElement)?.DataContext as Ficha;

            if (fichaEnMesaSeleccionada != null)
            {
                try
                {
                    cliente.TomarFichaMesa(idPartida, fichaEnMesaSeleccionada.IdFicha);
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                FichasItemsControl.IsEnabled = false;
                FichasItemsControl.MouseDown -= FichasItemsControl_MouseDown;
            }
        }

        private void HabilitarClickEnAreasJugadores(bool habilitar)
        {
            AreaJugador2.IsEnabled = habilitar;
            AreaJugador3.IsEnabled = habilitar;
            AreaJugador4.IsEnabled = habilitar;
        }

        private void RobarCartaDeJugador(object sender, MouseButtonEventArgs e)
        {
            var areaSeleccionada = sender as StackPanel;
            if (areaSeleccionada != null)
            {
                var jugadorObjetivo = ObtenerJugadorDesdeArea(areaSeleccionada);
                if (jugadorObjetivo != null)
                {
                    HabilitarClickEnAreasJugadores(false);

                    try
                    {
                        cliente.RobarCartaAJugador(jugadorObjetivo.NombreUsuario, idPartida, CartaDuplicacionActiva);
                    }
                    catch (EndpointNotFoundException)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    }
                    catch (TimeoutException)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    }
                    catch (CommunicationException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                    }
                    catch (Exception)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    }
                    
                }
            }
        }

        private async Task AccionFichaRevelarCarta()
        {
            try
            {
                await cliente.RevelarCartaMazoAsync(idPartida);
                await Task.Delay(100);
                await cliente.PreguntarGuardarCartaEnEsconditeAsync(idPartida);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }



        private void MoverCartaTipoEspecificoAEscondite(CartaCliente cartaSeleccionada, string tipoCartaRevelada)
        {
            Console.WriteLine($"Carta seleccionada tipo = {cartaSeleccionada.Tipo} /nTipo carta revelada: {tipoCartaRevelada}");
            if (cartaSeleccionada.Tipo == tipoCartaRevelada)
            {
                try
                {
                    cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta, idPartida);
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta incorrecta", "La carta seleccionada no es del tipo de la carta que fue revelada.");
            }
        }

        private void AccionFichaRobarCartaAJugador()
        {
            DesuscribirEventosRobarCarta();
            VentanasEmergentes.CrearVentanaEmergente("Selecciona un jugador", "Haz clic en el jugador al que deseas robar una carta.");
            HabilitarClickEnAreasJugadores(true);
            SuscribirEventosRobarCarta();
        }

        private void DesuscribirEventosRobarCarta()
        {
            AreaJugador2.MouseDown -= RobarCartaDeJugador;
            AreaJugador3.MouseDown -= RobarCartaDeJugador;
            AreaJugador4.MouseDown -= RobarCartaDeJugador;
            AreaJugador2.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador3.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador4.MouseDown -= RobarCartaEsconditeDeJugador;
        }

        private void SuscribirEventosRobarCarta()
        {
            AreaJugador2.MouseDown += RobarCartaDeJugador;
            AreaJugador3.MouseDown += RobarCartaDeJugador;
            AreaJugador4.MouseDown += RobarCartaDeJugador;
        }


        private void AccionFichaRobarOGuardar()
        {
            ConfigurarAccionFicha();
            VentanasEmergentes.CrearVentanaEmergente(
                "Acción de ficha",
                $"Puedes realizar 1 de las siguientes acciones:\n1) Tomar {cartasATomarMazo} carta(s) del mazo\n2) Guardar {cartasAGuardarEscondite} carta(s) en el escondite."
            );
            HabilitarInteraccionMazo();
            SuscribirEventosAccionFicha();

            try
            {

            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }

            cliente.EstablecerModoSeleccionCarta(
                idPartida,
                Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite),
                jugadorTurnoActual
            );
        }

        private void ConfigurarAccionFicha()
        {
            cartasATomarMazo = CartaDuplicacionActiva ? 2 : 1;
            cartasAGuardarEscondite = CartaDuplicacionActiva ? 2 : 1;
            cartasTomadasMazo = 0;
            cartasGuardadasEscondite = 0;
        }
        private void HabilitarInteraccionMazo()
        {
            ZonaMazoCartas.IsEnabled = true;
        }

        private void SuscribirEventosAccionFicha()
        {
            ZonaMazoCartas.MouseDown += ZonaMazoCartas_MouseDown;
            CartasManoItemsControl.MouseDown += CartasManoItemsControl_MouseDown;
        }

        private void ZonaMazoCartas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cartasTomadasMazo++;
            if (SeHanTomadoTodasLasCartasMazo())
            {
                FinalizarAccionRobarCartas();
            }
        }

        private bool SeHanTomadoTodasLasCartasMazo()
        {
            return cartasTomadasMazo == cartasATomarMazo;
        }

        private void FinalizarAccionRobarCartas()
        {
            try
            {
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                DeshabilitarInteraccionMazo();
                DesuscribirEventos();
                cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno),
                    jugadorTurnoActual
                );
                CartaDuplicacionActiva = false;
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }


        private void DeshabilitarInteraccionMazo()
        {
            ZonaMazoCartas.IsEnabled = false;
        }

        private void DesuscribirEventos()
        {
            ZonaMazoCartas.MouseDown -= ZonaMazoCartas_MouseDown;
            CartasManoItemsControl.MouseDown -= CartasManoItemsControl_MouseDown;
        }

        private void CartasManoItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cartasGuardadasEscondite++;
            if (SeHanGuardadoTodasLasCartas())
            {
                FinalizarAccionGuardarCartas();
            }
        }

        private bool SeHanGuardadoTodasLasCartas()
        {
            return cartasGuardadasEscondite == cartasAGuardarEscondite;
        }

        private void FinalizarAccionGuardarCartas()
        {
            try
            {
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                DeshabilitarInteraccionMazo();
                DesuscribirEventos();
                cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno),
                    jugadorTurnoActual
                );
                CartaDuplicacionActiva = false;
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
                
        }


        private void AccionFichaGuardarCartasEscondite()
        {
            try
            {
                if (CartasEnMano.Any())
                {
                    ConfigurarCartasAGuardar();
                    VentanasEmergentes.CrearVentanaEmergente(
                        "Acción ficha",
                        $"Puedes guardar {cartasAGuardarEscondite} cartas en el escondite."
                    );

                    cliente.EstablecerModoSeleccionCarta(
                        idPartida,
                        Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite),
                        jugadorTurnoActual
                    );

                    CartasManoItemsControl.MouseDown += CartasManoItemsControl_MouseDown1;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(
                        "Sin cartas en mano",
                        "No tiene cartas que pueda guardar en el escondite."
                    );
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                    CartaDuplicacionActiva = false;
                }
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }


        private void ConfigurarCartasAGuardar()
        {
            cartasAGuardarEscondite = CartaDuplicacionActiva ? 4 : 2;
            cartasGuardadasEscondite = 0;
        }

        private void CartasManoItemsControl_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            cartasGuardadasEscondite++;
            if (cartasGuardadasEscondite == cartasAGuardarEscondite || !CartasEnMano.Any())
            {
                FinalizarAccionGuardarCartasEscondite();
            }
        }

        private void FinalizarAccionGuardarCartasEscondite()
        {
            CartasManoItemsControl.MouseDown -= CartasManoItemsControl_MouseDown1;

            try
            {
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                CartaDuplicacionActiva = false;
                cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno),
                    jugadorTurnoActual
                );
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }


        private void TomarCartasMazo()
        {
            if (cartasATomarMazo == 0)
            {
                VentanasEmergentes.CrearVentanaEmergente(
                    "",
                    "Nadie guardó una carta en el escondite, por lo que no puedes tomar cartas del mazo."
                );

                if (fichaSeleccionada != null)
                {
                    try
                    {
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                    }
                    catch (EndpointNotFoundException)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    }
                    catch (TimeoutException)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    }
                    catch (CommunicationException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                    }
                    catch (Exception)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    }
                    
                }
            }
            else
            {
                ConfigurarCartasParaTomar();
                VentanasEmergentes.CrearVentanaEmergente(
                    "Tomar cartas del mazo",
                    $"Puedes tomar {cartasATomarMazo} cartas del mazo."
                );
                HabilitarInteraccionMazo();
                ZonaMazoCartas.MouseDown += ZonaMazoCartas_MouseDown1;
            }
        }


        private void ConfigurarCartasParaTomar()
        {
            if (CartaDuplicacionActiva)
            {
                cartasATomarMazo *= 2;
            }

            cartasTomadasMazo = 0;
        }

        private void ZonaMazoCartas_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            cartasTomadasMazo++;

            if (cartasTomadasMazo == cartasATomarMazo || EsMazoVacio())
            {
                ZonaMazoCartas.MouseDown -= ZonaMazoCartas_MouseDown1;

                ZonaMazoCartas.IsEnabled = false;

                if (fichaSeleccionada != null)
                {
                    try
                    {
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                    }
                    catch (EndpointNotFoundException)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    }
                    catch (TimeoutException)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    }
                    catch (CommunicationException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                    }
                    catch (Exception)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    }
                    
                }
                CartaDuplicacionActiva = false;
            }
        }

        private void AccionFichaTomarCartasMazo()
        {
            cartasATomarMazo = 2;
            TomarCartasMazo();
        }

        private void VerEscondite_Click(object sender, RoutedEventArgs e)
        {
            if (CartasEsconditeItemsControl.Visibility == Visibility.Collapsed)
            {
                CartasEsconditeItemsControl.Visibility = Visibility.Visible;

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                CartasEsconditeItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
            else
            {
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                fadeOut.Completed += (s, ev) => CartasEsconditeItemsControl.Visibility = Visibility.Collapsed;
                CartasEsconditeItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        public void NotificarTurnoIniciado(string jugadorTurnoActual)
        {
            this.jugadorTurnoActual = jugadorTurnoActual;
            modoSeleccionActual = ModoSeleccionCarta.CartasSinTurno;
            //modoSeleccionActual = ModoSeleccionCarta.MoverAlEscondite;
            jugadorDecidioParar = false;
            if (jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                dado.DadoLanzado -= ManejarResultadoDado;
                dado.DadoLanzado += ManejarResultadoDado;
                DadoImagen.IsEnabled = true;
                modoSeleccionActual = ModoSeleccionCarta.AccionCartasEnTurno;
            }
        }
        public void NotificarTurnoTerminado(string nombreUsuario)
        {
            foreach (var ficha in FichasEnMano)
            {
                try
                {
                    cliente.DevolverFichaAMesa(ficha.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }

            DadoImagen.IsEnabled = false;
            dado.DadoLanzado -= ManejarResultadoDado;
            modoSeleccionActual = ModoSeleccionCarta.CartasSinTurno;
        }

        public void NotificarPartidaCreada(string idPartida)
        {
            this.idPartida = idPartida;
            DadoImagen.IsEnabled = false;
            ZonaMazoCartas.IsEnabled = false;
            FichasManoItemsControl.IsEnabled = false;
            CargarFichas();
            modoSeleccionActual = ModoSeleccionCarta.CartasSinTurno;
            //modoSeleccionActual = ModoSeleccionCarta.MoverAlEscondite;
        }

        public void NotificarResultadoDado(string nombreJugador, int resultadoDado)
        {
            dado.LanzarDado(resultadoDado);
        }

        public void NotificarCartasEnMano(Carta[] cartasRepartidas)
        {
            foreach (var carta in cartasRepartidas)
            {
                CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
                CartasEnMano.Add(cartaCliente);
            }
            foreach (var carta in CartasEnMano)
            {
                Console.WriteLine($"IDCarta: {carta.IdCarta} RutaImagen: {carta.RutaImagen}");
            }
        }

        public void NotificarCartasEnMazo(Carta[] cartasEnMazo)
        {
            foreach (var carta in cartasEnMazo)
            {
                CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
                CartasEnMazo.Add(cartaCliente);
            }
            foreach (var carta in CartasEnMazo)
            {
                Console.WriteLine($"Carta en mazo: {carta.IdCarta}");
            }
        }

        public void NotificarCartaAgregadaAMano(Carta carta)
        {
            CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
            CartasEnMano.Add(cartaCliente);
        }

        public void NotificarCartaTomadaMazo(int idCarta)
        {
            var cartaTomadaMazo = CartasEnMazo.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasEnMazo.Remove(cartaTomadaMazo);
        }

        public void NotificarCartaTomadaDescarte(int idCarta)
        {
            var cartaTomadaDescarte = CartasDescarte.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasDescarte.Remove(cartaTomadaDescarte);
        }

        public void NotificarFichaTomadaMesa(string jugadorTurnoActual, int idFicha)
        {
            var fichaObtenida = Fichas.FirstOrDefault(f => f.IdFicha == idFicha);
            Fichas.Remove(fichaObtenida);

            if (jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                FichasEnMano.Add(fichaObtenida);
            }
        }

        public void NotificarCartaAgregadaADescarte(Carta cartaUtilizada)
        {
            CartaCliente carta = new CartaCliente { IdCarta = cartaUtilizada.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(cartaUtilizada.IdRutaImagen), Tipo = cartaUtilizada.Tipo };
            CartasDescarte.Add(carta);
        }

        public void NotificarCartaUtilizada(int idCartaUtilizada)
        {
            var cartaUtilizada = CartasEnMano.FirstOrDefault(c => c.IdCarta == idCartaUtilizada);
            CartasEnMano.Remove(cartaUtilizada);
        }

        public void NotificarFichaDevuelta(int idFicha, string nombreJugadorTurnoActual)
        {
            if (jugadorActual.NombreUsuario == nombreJugadorTurnoActual)
            {
                var ficha = FichasEnMano.FirstOrDefault(f => f.IdFicha == idFicha);
                FichasEnMano.Remove(ficha);
                Fichas.Add(ficha);
            }
            else
            {
                var fichaDevuelta = FichasEstaticas.FirstOrDefault(f => f.IdFicha == idFicha);
                Fichas.Add(fichaDevuelta);
            }
        }

        public void NotificarCartaAgregadaAEscondite(int idCarta)
        {
            var carta = CartasEnMano.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasEnMano.Remove(carta);
            CartasEnEscondite.Add(carta);
        }

        public async void NotificarIntentoRobo(string nombreJugadorDefensor)
        {
            if (jugadorActual.NombreUsuario == nombreJugadorDefensor)
            {
                bool decision = await DecisionDefenderseRobo();

                try
                {
                    if (decision)
                    {
                        await cliente.EstablecerModoSeleccionCartaAsync(
                            idPartida,
                            Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo),
                            jugadorActual.NombreUsuario
                        );
                    }
                    else
                    {
                        int cartasARobar = CartaDuplicacionActiva ? 2 : 1;

                        for (int i = 0; i < cartasARobar; i++)
                        {
                            await cliente.RobarCartaAsync(idPartida, nombreJugadorDefensor);
                        }
                    }
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }
            CartaDuplicacionActiva = false;
        }


        public void NotificarCartaRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual)
        {
            if (jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                var cartaRobada = CartasEnMano.FirstOrDefault(c => c.IdCarta == carta.IdCarta);
                CartasEnMano.Remove(cartaRobada);
            }

            if (jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                CartaCliente cartaRobada = new CartaCliente { IdCarta = carta.IdCarta, Tipo = carta.Tipo, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
                CartasEnMano.Add(cartaRobada);

                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                
            }
            CartaDuplicacionActiva = false;
        }

        public async void NotificarIntentoRoboCartaEscondite(string nombreJugadorAtacante)
        {
            if (jugadorActual.NombreUsuario == nombreJugadorAtacante)
            {
                bool decision = await DecisionDefenderseRobo();

                try
                {
                    if (decision)
                    {
                        await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo), jugadorActual.NombreUsuario);
                    }
                    else
                    {
                        await cliente.RobarCartaEsconditeAsync(idPartida, nombreJugadorAtacante);
                    }
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }
            CartaDuplicacionActiva = false;
        }


        public void NotificarCartaEsconditeRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual)
        {
            if (jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                var cartaRobada = CartasEnEscondite.FirstOrDefault(c => c.IdCarta == carta.IdCarta);
                CartasEnEscondite.Remove(cartaRobada);
            }

            if (jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                CartaCliente cartaRobada = new CartaCliente { IdCarta = carta.IdCarta, Tipo = carta.Tipo, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
                CartasEnMano.Add(cartaRobada);
            }
            CartaDuplicacionActiva = false;
        }


        public void NotificarTiroDadoForzado(string jugadorTurnoActual)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    cliente.LanzarDado(idPartida, jugadorTurnoActual);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                
            });
        }


        public async void NotificarPreguntaJugadores(string jugadorTurnoActual, string tipoCartaRevelada)
        {
            this.tipoCartaRevelada = tipoCartaRevelada;
            await ManejarDecisionGuardarCartaEnEscondite();
        }

        public void NotificarNumeroJugadoresGuardaronCarta(int numeroJugadoresGuardaronCarta)
        {
            try
            {
                cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, CartasEnMazo[CartasEnMazo.Count - 1].IdCarta);
                cliente.OcultarCartaMazo(idPartida);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            cartasATomarMazo = numeroJugadoresGuardaronCarta;
            TomarCartasMazo();
        }


        public void NotificarPararTirarDado()
        {
            jugadorDecidioParar = true;
            if (jugadorActual.NombreUsuario != jugadorTurnoActual)
            {
                VentanasEmergentes.CrearVentanaEmergente("Jugador paro de tirar", "El jugador en turno ha decidido dejar de tirar el dado.");
            }
        }

        public void NotificarActualizacionDecisionTurno()
        {
            jugadorDecidioParar = false;
        }

        public void NotificarModoSeleccionCarta(int idModoSeleccionCarta)
        {
            modoSeleccionActual = modosSeleccionCarta[idModoSeleccionCarta];
        }
        public void NotificarMazoRevelado()
        {
            CartasEnMazo.Remove(CartasEnMazo[CartasEnMazo.Count - 1]);
        }

        public void NotificarMazoOculto(Carta cartaParteTrasera)
        {
            CartaCliente carta = new CartaCliente { Tipo = cartaParteTrasera.Tipo, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(cartaParteTrasera.IdRutaImagen), IdCarta = cartaParteTrasera.IdCarta };
            CartasEnMazo.Add(carta);
        }

        public void CompletarGuardarCartaEscondite()
        {
            if (jugadorGuardoCartaEnEscondite != null && jugadorGuardoCartaEnEscondite.Task.IsCompleted)
            {
                jugadorGuardoCartaEnEscondite.SetResult(true);
            }
        }
        private async Task ManejarDecisionGuardarCartaEnEscondite()
        {
            bool decision = await DecisionGuardarCartaEnEscondite();
            try
            {
                if (decision)
                {
                    jugadorGuardoCartaEnEscondite = new TaskCompletionSource<bool>();
                    await cliente.EnviarDecisionAsync(idPartida, true);
                    await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite), jugadorActual.NombreUsuario);
                    await jugadorGuardoCartaEnEscondite.Task;
                    await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
                }
                else
                {
                    await cliente.EnviarDecisionAsync(idPartida, false);
                    await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite), jugadorTurnoActual);
                }
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }
        private static async Task<bool> DecisionGuardarCartaEnEscondite()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decision guardar carta", "¿Quieres guardar una carta en el escondite que sea del tipo que ha sido revelada?");
            return await ventanaDeDecision.MostrarDecision();
        }

        private void UsarCartaBloqueoRobo(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta7":
                    UsarCarta7(cartaSeleccionada.IdCarta);
                    break;
                case "Carta8":
                    UsarCarta8(cartaSeleccionada.IdCarta);
                    break;
                default:
                    VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "La carta seleccionada no sirve para bloquear un robo.");
                    break;

            }
        }
        private JugadorPartida ObtenerJugadorDesdeArea(StackPanel area)
        {
            if (area == AreaJugador2) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador2.Text);
            if (area == AreaJugador3) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador3.Text);
            if (area == AreaJugador4) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador4.Text);
            return null;
        }
        private async Task UsarCartaSalvacionTurno(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta5":
                    await UsarCarta5(cartaSeleccionada.IdCarta);
                    break;
                case "Carta6":
                    await UsarCarta6(cartaSeleccionada.IdCarta);
                    break;
                default:
                    VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "La carta seleccionada no sirve para salvar tu turno.");
                    break;

            }
        }

        private void UsarCartaEnTurno(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta1":
                    UsarCarta1(cartaSeleccionada.IdCarta);
                    break;

                case "Carta3":
                    UsarCarta3(cartaSeleccionada.IdCarta);
                    break;

                case "Carta4":
                    UsarCarta4(cartaSeleccionada.IdCarta);
                    break;

                default:
                    VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "La accion de esta carta no se puede aplicar en este momento");
                    break;
            }
        }

        private void UsarCartaSinTurno(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta2":
                    UsarCarta2(cartaSeleccionada.IdCarta);
                    break;
                default:
                    VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "La accion de esta carta no se puede aplicar en este momento");
                    break;
            }
        }

        private async Task ManejarDecisionContinuarTurno()
        {
            bool decision = await DecisionContinuarTurno();
            if (!decision)
            {
                try
                {
                    await cliente.DejarTirarDadoAsync(idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                DadoImagen.IsEnabled = false;
                await ResolverFichas();
                FinalizarTurno();
            }
        }

        private static async Task<bool> DecisionContinuarTurno()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decisión de turno", "¿Quieres continuar tirando?");
            return await ventanaDeDecision.MostrarDecision();
        }
        private void UsarCarta1(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }

            RobarCartaEsconditeAJugador();
        }


        private void UsarCarta2(int idCarta)
        {
            if (jugadorDecidioParar)
            {
                try
                {
                    cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                    cliente.ObligarATirarDado(idPartida);
                    cliente.ActualizarDecisionTurno(idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "Para poder ocupar esta carta el jugador en turno debe haber decidido dejar de tirar el dado.");
            }
        }


        private void UsarCarta3(int idCarta)
        {
            if (CartasDescarte.Any())
            {
                try
                {
                    cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                    SeleccionarCartaDescarte(idCarta);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(CARTA_NO_DISPONIBLE, "No hay cartas en el descarte por lo que no puede ocupar esta carta");
            }
        }

        private void UsarCarta4(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                CartaDuplicacionActiva = true;
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private async Task UsarCarta5(int idCarta)
        {
            try
            {
                await cliente.UtilizarCartaAsync(idPartida, idCarta, jugadorActual.NombreUsuario);
                DadoImagen.IsEnabled = false;
                await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
                await ResolverFichas();
                FinalizarTurno();
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private async Task UsarCarta6(int idCarta)
        {
            try
            {
                await cliente.UtilizarCartaAsync(idPartida, idCarta, jugadorActual.NombreUsuario);
                await cliente.EstablecerModoSeleccionCartaAsync(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
                await ManejarDecisionContinuarTurno();
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void UsarCarta7(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                VentanasEmergentes.CrearVentanaEmergente(
                    "Acción de carta",
                    "Puedes tomar 2 cartas del Mazo, da clic 2 veces en él."
                );
                ConfigurarZonaMazoParaTomaDeCartas();
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void ConfigurarZonaMazoParaTomaDeCartas()
        {
            ZonaMazoCartas.IsEnabled = true;
            cartasTomadasMazo = 0;

            void OnMouseDownHandler(object sender, MouseButtonEventArgs e)
            {
                cartasTomadasMazo++;
                if (cartasTomadasMazo == 2)
                {
                    FinalizarTomaDeCartas();
                    ZonaMazoCartas.MouseDown -= OnMouseDownHandler;
                }
            }

            ZonaMazoCartas.MouseDown += OnMouseDownHandler;
        }

        private void FinalizarTomaDeCartas()
        {
            ZonaMazoCartas.IsEnabled = false;

            if (jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno),
                    jugadorTurnoActual
                );
            }
            else
            {
                cliente.EstablecerModoSeleccionCarta(
                        idPartida,
                        Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno),
                        jugadorActual.NombreUsuario
                );
            }

            if (fichaSeleccionada != null)
            {
                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }
                
            }
        }

        private void UsarCarta8(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                cliente.UtilizarCartaDefensiva(idPartida, jugadorActual.NombreUsuario);

                if (jugadorActual.NombreUsuario == jugadorTurnoActual)
                {
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
                }
                else
                {
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
                }
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private static async Task<bool> DecisionDefenderseRobo()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decision de defensa", "Has sido elegido como objeto de robo. ¿Quieres defenderte?");
            return await ventanaDeDecision.MostrarDecision();
        }

        private void RobarCartaEsconditeAJugador()
        {
            AreaJugador2.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador3.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador4.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador2.MouseDown -= RobarCartaDeJugador;
            AreaJugador3.MouseDown -= RobarCartaDeJugador;
            AreaJugador4.MouseDown -= RobarCartaDeJugador;

            Metodos.MostrarMensaje("Haz clic en el jugador al que deseas robar una carta de su escondite.");
            HabilitarClickEnAreasJugadores(true);

            AreaJugador2.MouseDown += RobarCartaEsconditeDeJugador;
            AreaJugador3.MouseDown += RobarCartaEsconditeDeJugador;
            AreaJugador4.MouseDown += RobarCartaEsconditeDeJugador;
        }

        private void RobarCartaEsconditeDeJugador(object sender, MouseButtonEventArgs e)
        {
            var areaSeleccionada = sender as StackPanel;
            if (areaSeleccionada != null)
            {
                var jugadorObjetivo = ObtenerJugadorDesdeArea(areaSeleccionada);
                if (jugadorObjetivo != null)
                {
                    HabilitarClickEnAreasJugadores(false);

                    try
                    {
                        cliente.RobarCartaEsconditeAJugador(jugadorObjetivo.NombreUsuario, idPartida, CartaDuplicacionActiva);
                    }
                    catch (EndpointNotFoundException)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    }
                    catch (TimeoutException)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    }
                    catch (CommunicationException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                    }
                    catch (Exception)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    }
                    
                }
            }
        }


        private void SeleccionarCartaDescarte(int idCartaRecientementeDescartada)
        {
            var cartasElegibles = CartasDescarte.Where(c => c.IdCarta != idCartaRecientementeDescartada).ToList();

            if (!cartasElegibles.Any())
            {
                VentanasEmergentes.CrearVentanaEmergente(
                    "Sin cartas disponibles",
                    "No hay cartas disponibles en el descarte."
                );
            }

            var dialogo = new DialogoSeleccionCarta(cartasElegibles);
            if (dialogo.ShowDialog() == true)
            {
                var cartaSeleccionada = dialogo.CartaSeleccionada;

                try
                {
                    cliente.TomarCartaDeDescarte(idPartida, jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta);
                }
                catch (EndpointNotFoundException)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                }
                catch (TimeoutException)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                }
                catch (CommunicationException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (Exception)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                }

            }
        }

    }
}

