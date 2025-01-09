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
            private string[] nombresJugadoresModoSeleccion;
            private bool cartaTomada = false;
            private TaskCompletionSource<bool> jugadorGuardoCartaEnEscondite;
            private string tipoCartaRevelada;

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
        private void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cliente.LanzarDado(idPartida, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                NavigationService.Navigate(new XAMLInicioSesion());

            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLSalaEspera());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);

            }
        }
        public XAMLTablero(List<JugadorPartida> jugadores, string idPartida)
        {
            if (!Thread.CurrentThread.IsBackground && Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("El constructor XAMLTablero debe ejecutarse en un subproceso STA.");
            }
            InitializeComponent();
            SetupClient();

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.listaDeJugadores = jugadores;
                CargarFichasEstaticas();
                dado = new Dado(DadoImagen);
                this.idPartida = idPartida;

                if (jugadorActual.EsInvitado)
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
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (CommunicationException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }

                }
                else
                {
                    try
                    {
                        cliente.RegistrarJugador(jugadorActual.NombreUsuario);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (CommunicationException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                }

                MostrarJugadores();

                FichasItemsControl.ItemsSource = Fichas;
                FichasManoItemsControl.ItemsSource = FichasEnMano;
                ZonaDescarte.ItemsSource = CartasDescarte;
                CartasManoItemsControl.ItemsSource = CartasEnMano;
                ZonaMazoCartas.ItemsSource = CartasEnMazo;
                CartasEsconditeItemsControl.ItemsSource = CartasEnEscondite;
            });
        }

        public void NotificarJugadorDesconectado(string nombreUsuario)
        {
            MessageBox.Show("se salio el cabron de " + nombreUsuario);
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

            var servicioDelJuegoClient = new GestionCuentaServicioClient();

            try
            {
                var jugadores = puntajes.Keys
                    .ToDictionary(
                        nombreUsuario => nombreUsuario,
                        nombreUsuario => ObtenerJugadorId(nombreUsuario)
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }

            return idJugador;
        }




        private void Mazo_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (EsMazoVacio())
                {
                    try
                    {
                        cliente.FinalizarJuego(idPartida);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLSalaEspera());
                    }
                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }

                }
                else
                {
                    var cartaSuperior = ObtenerCartaSuperiorDelMazo();
                    try
                    {
                        cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, cartaSuperior.IdCarta);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLSalaEspera());
                    }
                    catch (CommunicationException ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }

                }
            }
          
            private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
            {
                try
                {
                    cliente.CrearPartida(listaDeJugadores.ToArray(), idPartida);
                    cliente.RepartirCartas(idPartida);
                    cliente.EmpezarTurno(idPartida);
                    btnRepartirCartas.Visibility= Visibility.Collapsed;
                }
                catch (EndpointNotFoundException ex)
                {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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

        private async void ManejarResultadoDado(int resultadoDado)
        {
            await Task.Delay(1000);
            Ficha fichaSeleccionada = SeleccionarFichaPorResultado(resultadoDado);
            
            if (PuedeTomarFicha(fichaSeleccionada))
            {
                await TomarFicha(fichaSeleccionada);
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

        private bool PuedeTomarFicha(Ficha fichaSeleccionada)
        {
            return fichaSeleccionada != null;
        }

        private async Task TomarFicha(Ficha fichaSeleccionada)
        {
            try
            {
                cliente.TomarFichaMesa(idPartida, fichaSeleccionada.IdFicha);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            await Task.Delay(1000);

            ManejarDecisionContinuarTurno();
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

        private async Task<bool> SolicitarDecisionSalvarTurno()
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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

            bool cartaTomadaDelMazo = await EsperarTomaDeCarta();

            ZonaMazoCartas.IsEnabled = false;
            await FinalizarTurno();
        }

        private async Task<bool> EsperarTomaDeCarta()
        {
            SuscribirEventoTomaDeCarta();

            bool cartaTomada = await EsperarCartaTomada();

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



        private async Task FinalizarTurno()
        {
            try
            {
                await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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

        private async Task<bool> DecisionSalvarTurno()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decision de turno", "Obtuviste una ficha repetida, puedes usar una carta para salvarte.");
            return await ventanaDeDecision.MostrarDecision();
        }
        

        private void SeleccionarCarta(object sender, MouseButtonEventArgs e)
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
                        UsarCartaSalvacionTurno(cartaSeleccionada);
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

        private CartaCliente ObtenerCartaSeleccionada(object sender)
        {
            return (sender as Border)?.DataContext as CartaCliente;
        }

        private void MoverCartaAEscondite(CartaCliente carta)
        {
            try
            {
                cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, carta.IdCarta, idPartida);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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


        private void Ficha_MouseDown(object sender, MouseButtonEventArgs e)
        {
            fichaSeleccionada = (sender as Border)?.DataContext as Ficha;
            if (fichaSeleccionada != null)
            {
                EjecutarAccionFicha(fichaSeleccionada);
            }
        }

        private void EjecutarAccionFicha(Ficha fichaSeleccionada)
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
                    AccionFichaRevelarCarta();
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
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                    }
                }
            }
        }

        private JugadorPartida ObtenerJugadorDesdeArea(StackPanel area)
        {
            if (area == AreaJugador2) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador2.Text);
            if (area == AreaJugador3) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador3.Text);
            if (area == AreaJugador4) return listaDeJugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador4.Text);
            return null;
        }

        private async void AccionFichaRevelarCarta()
        {
            try
            {
                cliente.RevelarCartaMazo(idPartida);
                await Task.Delay(100);
                cliente.PreguntarGuardarCartaEnEscondite(idPartida);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
  
        }

        private void MoverCartaTipoEspecificoAEscondite(CartaCliente cartaSeleccionada, string tipoCartaRevelada)
        {
            Console.WriteLine($"Carta seleccionada tipo = {cartaSeleccionada.Tipo} /nTipo carta revelada: {tipoCartaRevelada}");
            if(cartaSeleccionada.Tipo == tipoCartaRevelada)
            {
                try
                {
                    cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta, idPartida);
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                cliente.EstablecerModoSeleccionCarta(
                idPartida,
                Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite),
                jugadorTurnoActual
            );
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }

        private void AccionFichaGuardarCartasEscondite()
        {
            if (CartasEnMano.Any())
            {
                ConfigurarCartasAGuardar();
                VentanasEmergentes.CrearVentanaEmergente(
                "Acción ficha",
                $"Puedes guardar {cartasAGuardarEscondite} cartas en el escondite."
            );
                try
                {
                    cliente.EstablecerModoSeleccionCarta(
                        idPartida,
                        Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite),
                        jugadorTurnoActual
                    );
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }

                CartasManoItemsControl.MouseDown += CartasManoItemsControl_MouseDown1;
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(
                "Sin cartas en mano",
                "No tiene cartas que pueda guardar en el escondite."
                );

                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
                CartaDuplicacionActiva = false;
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
            jugadorDecidioParar = false;
            if(jugadorActual.NombreUsuario == jugadorTurnoActual)
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
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
        }

        public void NotificarResultadoDado(string nombreUsuario, int resultadoDado)
        {
            dado.LanzarDado(resultadoDado);
        }
 
        public void NotificarCartasEnMano(Carta[] cartasRepartidas)
        {
            foreach(var carta in cartasRepartidas)
            {
                CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen)};
                CartasEnMano.Add(cartaCliente);
            }
            foreach(var carta in CartasEnMano)
            {
                Console.WriteLine($"IDCarta: {carta.IdCarta} RutaImagen: {carta.RutaImagen}");
            }
        }

        public void NotificarCartasEnMazo(Carta[] cartasEnMazo)
        {
            foreach(var carta in cartasEnMazo)
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
            var cartaTomada = CartasEnMazo.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasEnMazo.Remove(cartaTomada);
        }

        public void NotificarCartaTomadaDescarte(int idCarta)
        {
            var cartaTomada = CartasDescarte.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasDescarte.Remove(cartaTomada);
        }

        public void NotificarFichaTomadaMesa(string jugadorTurnoActual, int idFicha)
        {
            var fichaSeleccionada = Fichas.FirstOrDefault(f => f.IdFicha == idFicha);
            Fichas.Remove(fichaSeleccionada);

            if(jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                FichasEnMano.Add(fichaSeleccionada);
            }
        }

        public void NotificarCartaAgregadaADescarte(Carta cartaUtilizada)
        {
            CartaCliente carta = new CartaCliente { IdCarta = cartaUtilizada.IdCarta, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(cartaUtilizada.IdRutaImagen), Tipo = cartaUtilizada.Tipo};
            CartasDescarte.Add(carta);
        }

        public void NotificarCartaUtilizada(int idCartaUtilizada)
        {
            var cartaUtilizada = CartasEnMano.FirstOrDefault(c => c.IdCarta == idCartaUtilizada);
            CartasEnMano.Remove(cartaUtilizada);
        }

        public void NotificarFichaDevuelta(int idFicha, string nombreJugadorTurnoActual)
        {
            if(jugadorActual.NombreUsuario == nombreJugadorTurnoActual)
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

        public async void NotificarIntentoRoboCarta(string nombreUsuarioAtacante)
        {
            if (jugadorActual.NombreUsuario == nombreUsuarioAtacante)
            {
                bool decision = await DecisionDefenderseRobo();

                if (decision)
                {
                    try
                    {
                        cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo), jugadorActual.NombreUsuario);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                    }
                    
                }
                else
                {
                    int cartasARobar = 1;
                    if (CartaDuplicacionActiva)
                    {
                        cartasARobar = 2;
                    }
                    for (int i = 0; i < cartasARobar; i++)
                    {
                        try
                        {
                            cliente.RobarCarta(idPartida, nombreUsuarioAtacante);
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                            ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                        }
                        catch (TimeoutException ex)
                        {
                            VentanasEmergentes.CrearVentanaMensajeTimeOut();
                            ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                        }
                        catch (FaultException<HuntersTrophyExcepcion>)
                        {
                            VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                            NavigationService.Navigate(new XAMLInicioSesion());
                        }

                        catch (FaultException)
                        {
                            VentanasEmergentes.CrearMensajeVentanaServidorError();
                            NavigationService.Navigate(new XAMLInicioSesion());
                        }
                        catch (CommunicationException ex)
                        {

                            VentanasEmergentes.CrearMensajeVentanaServidorError();
                            ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                        }
                        catch (Exception ex)
                        {
                            VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                            ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                        }
                        
                    }
                }
            }
            CartaDuplicacionActiva = false;
        }

        public void NotificarCartaRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual)
        {
            if(jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                var cartaRobada = CartasEnMano.FirstOrDefault(c => c.IdCarta == carta.IdCarta);
                CartasEnMano.Remove(cartaRobada);
            }

            if(jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                CartaCliente cartaRobada = new CartaCliente { IdCarta = carta.IdCarta, Tipo = carta.Tipo, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(carta.IdRutaImagen) };
                CartasEnMano.Add(cartaRobada);

                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            }
            CartaDuplicacionActiva = false;
        }

        public async void NotificarIntentoRoboCartaEscondite(string nombreUsuarioAtacante)
        {
            if (jugadorActual.NombreUsuario == nombreUsuarioAtacante)
            {
                bool decision = await DecisionDefenderseRobo();

                if (decision)
                {
                    try
                    {
                        cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo), jugadorActual.NombreUsuario);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                    }
                    
                }
                else
                {
                    try
                    {
                        cliente.RobarCartaEscondite(idPartida, nombreUsuarioAtacante);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                    }
                    
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
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            });
        }

        public void NotificarPreguntaJugadores(string jugadorTurnoActual, string tipoCartaRevelada)
        {
            this.tipoCartaRevelada = tipoCartaRevelada;
            ManejarDecisionGuardarCartaEnEscondite();
        }

        public void NotificarNumeroJugadoresGuardaronCarta(int numeroJugadoresGuardaronCarta)
        {
            try
            {
                cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, CartasEnMazo.Last().IdCarta);
                cliente.OcultarCartaMazo(idPartida);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            
            cartasATomarMazo = numeroJugadoresGuardaronCarta;
            TomarCartasMazo();
        }

        public void NotificarPararTirarDado()
        {
            jugadorDecidioParar = true;
            if(jugadorActual.NombreUsuario != jugadorTurnoActual)
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
            CartasEnMazo.Remove(CartasEnMazo.Last());
        }

        public void NotificarMazoOculto(Carta cartaParteTrasera)
        {
            CartaCliente carta = new CartaCliente { Tipo = cartaParteTrasera.Tipo, RutaImagen = CartaCliente.ObtenerRutaImagenCarta(cartaParteTrasera.IdRutaImagen), IdCarta = cartaParteTrasera.IdCarta};
            CartasEnMazo.Add(carta);
        }

        public void NotificarResultadoAccion(string accion, bool resultado)
        {

        }

        public void CompletarGuardarCartaEscondite()
        {
            if (jugadorGuardoCartaEnEscondite != null && jugadorGuardoCartaEnEscondite.Task.IsCompleted)
            {
                jugadorGuardoCartaEnEscondite.SetResult(true); 
            }
        }
        private async void ManejarDecisionGuardarCartaEnEscondite()
        {
            bool decision = await DecisionGuardarCartaEnEscondite();
            
            if (decision)
            {
                jugadorGuardoCartaEnEscondite = new TaskCompletionSource<bool>();
                try
                {
                    cliente.EnviarDecision(idPartida, true);
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite), jugadorActual.NombreUsuario);
                    await jugadorGuardoCartaEnEscondite.Task;
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);

                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            }
            else
            {
                try
                {
                    cliente.EnviarDecision(idPartida, false);
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite), jugadorTurnoActual);

                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
   
            }
        }

        private async Task<bool> DecisionGuardarCartaEnEscondite()
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
                    VentanasEmergentes.CrearVentanaEmergente("Carta incorrecta.", "La carta seleccionada no sirve para bloquear un robo.");
                    break;

            }
        }

        private void UsarCartaSalvacionTurno(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta5":
                    UsarCarta5(cartaSeleccionada.IdCarta);
                    break;
                case "Carta6":
                    UsarCarta6(cartaSeleccionada.IdCarta);
                    break;
                default:
                    VentanasEmergentes.CrearVentanaEmergente("Carta incorrecta.", "La carta seleccionada no sirve para salvar tu turno.");
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
                    VentanasEmergentes.CrearVentanaEmergente("Carta no disponible", "La accion de esta carta no se puede aplicar en este momento");
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
                    VentanasEmergentes.CrearVentanaEmergente("Carta no disponible", "La accion de esta carta no se puede aplicar en este momento");
                    break;
            }
        }

        private async void ManejarDecisionContinuarTurno()
        {
            bool decision = await DecisionContinuarTurno();
            if (!decision)
            {
                cliente.DejarTirarDado(idPartida);
                DadoImagen.IsEnabled = false;
                await ResolverFichas();
                await FinalizarTurno();
            }
        }

        private async Task<bool> DecisionContinuarTurno()
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta no disponible", "Para poder ocupar esta carta el jugador en turno debe haber decidido dejar de tirar el dado.");
            }
        }

        private void UsarCarta3(int idCarta)
        {
            if (CartasDescarte.Any())
            {
                try
                {
                    cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                SeleccionarCartaDescarte(idCarta);
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta no disponible", "No hay cartas en el descarte por lo que no puede ocupar esta carta");
            }
        }

        private void UsarCarta4(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            CartaDuplicacionActiva = true;
        }

        private async void UsarCarta5(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                DadoImagen.IsEnabled = false;
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            
            await ResolverFichas();
            await FinalizarTurno();
        }

        private void UsarCarta6(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
                
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            ManejarDecisionContinuarTurno();

        }


        private void UsarCarta7(int idCarta)
        {
            try
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
            
            VentanasEmergentes.CrearVentanaEmergente(
                "Acción de carta",
                "Puedes tomar 2 cartas del Mazo, da clic 2 veces en él."
            );
            ConfigurarZonaMazoParaTomaDeCartas();
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
                try
                {
                    cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno),
                    jugadorTurnoActual
                );
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            }
            else
            {
                try
                {
                    cliente.EstablecerModoSeleccionCarta(
                    idPartida,
                    Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno),
                    jugadorActual.NombreUsuario
                );
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            }

            if (fichaSeleccionada != null)
            {
                try
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }

        private async Task<bool> DecisionDefenderseRobo()
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
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        NavigationService.Navigate(new XAMLInicioSesion());
                    }
                    catch (CommunicationException ex)
                    {

                        VentanasEmergentes.CrearMensajeVentanaServidorError();
                        ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                    }
                    catch (Exception ex)
                    {
                        VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                        ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                    }
                    
                }
            }
        }

        private void SeleccionarCartaDescarte(int idCartaRecientementeDescartada)
        {
            var cartasElegibles = CartasDescarte.Where(c => c.IdCarta != idCartaRecientementeDescartada).ToList();

            if (!cartasElegibles.Any())
            {
                MessageBox.Show("No hay cartas disponibles en el descarte.");
                return;
            }

            var dialogo = new DialogoSeleccionCarta(cartasElegibles);
            if (dialogo.ShowDialog() == true)
            {
                var cartaSeleccionada = dialogo.CartaSeleccionada;
                Metodos.MostrarMensaje($"Carta seleccionada: {cartaSeleccionada.Tipo}");

                try
                {
                    cliente.TomarCartaDeDescarte(idPartida, jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta);
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (FaultException<HuntersTrophyExcepcion>)
                {
                    VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarErrorExcepcionPartida(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
                
            }
        }



    }
}


