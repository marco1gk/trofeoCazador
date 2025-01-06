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

    namespace trofeoCazador.Vistas.PartidaJuego
    {
        public partial class XAMLTablero : Page, IServicioPartidaCallback
        {
            private ServicioPartidaClient cliente;
            private List<JugadorPartida> jugadores;
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
            if (!Thread.CurrentThread.IsBackground && Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("El constructor XAMLTablero debe ejecutarse en un subproceso STA.");
            }

            InitializeComponent();
            SetupClient();

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.jugadores = jugadores;
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

                    cliente.RegistrarJugadorInvitado(invitado); 
                }
                else
                {
                    cliente.RegistrarJugador(jugadorActual.NombreUsuario); 
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
                        nombreUsuario => GetIdPlayer(nombreUsuario)
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
                // Manejar error según sea necesario (mostrar mensaje al usuario, reintentar, etc.)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }
        private int GetIdPlayer(string username)
        {
            Console.WriteLine("SE ENTRA A GetIdPlayer");
            GestionCuentaServicioClient userManagerClient = new GestionCuentaServicioClient();
            int idPlayer = -1;

            try
            {
                idPlayer = userManagerClient.ObtenerIdJugadorPorNombreUsuario(username);
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                //  NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                // NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }

            return idPlayer;
        }




        private void Mazo_MouseDown(object sender, MouseButtonEventArgs e)
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
            public void MostrarJugadores()
            {
                var jugadoresEnPartida = jugadores
                    .Where(j => j.NombreUsuario != jugadorActual.NombreUsuario)
                    .ToList();

                var areasJugadores = new[]
                {
                    new { Nombre = NombreJugador2, Imagen = Jugador2Imagen, Area = AreaJugador2 },
                    new { Nombre = NombreJugador3, Imagen = Jugador3Imagen, Area = AreaJugador3 },
                    new { Nombre = NombreJugador4, Imagen = Jugador4Imagen, Area = AreaJugador4 }
                };

                for (int i = 0; i < jugadoresEnPartida.Count && i < areasJugadores.Length; i++)
                {
                    var jugador = jugadoresEnPartida[i];
                    var area = areasJugadores[i];

                    area.Nombre.Text = jugador.NombreUsuario;
                    string rutaImagen = ObtenerRutaImagenPerfil(jugador.NumeroFotoPerfil);
                    area.Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                    area.Area.Visibility = Visibility.Visible;
                }
            }


            private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
            {
                try
                {
                    cliente.CrearPartida(jugadores.ToArray(), idPartida);
                    cliente.RepartirCartas(idPartida);
                    cliente.EmpezarTurno(idPartida);
                }
                catch (EndpointNotFoundException ex)
                {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                //ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (TimeoutException ex)
                {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                //ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
                {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                //NavigationService.Navigate(new XAMLInicioSesion());
                }

                catch (FaultException)
                {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                //NavigationService.Navigate(new XAMLInicioSesion());
                }
                catch (CommunicationException ex)
                {

                VentanasEmergentes.CrearMensajeVentanaServidorError();
                //ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                //ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
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


        /*private void ActualizarZonaFichas()
        {
            Metodos.MostrarMensaje($"Nombre jugador 2: {NombreJugador2.Text}");
            if (NombreJugador2.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador2.ItemsSource = FichasEnMano;
            }
            else if (NombreJugador3.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador3.ItemsSource = FichasEnMano;
            }
            else if (NombreJugador4.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador4.ItemsSource = FichasEnMano;
            }
        }*/

        //DADO

        private void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cliente.LanzarDado(idPartida, jugadorActual.NombreUsuario);
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
            cliente.TomarFichaMesa(idPartida, fichaSeleccionada.IdFicha);
            await Task.Delay(1000);

            ManejarDecisionContinuarTurno();
        }
        private async Task ManejarFichaDuplicada()
        {
            await Task.Delay(1000);
            bool tieneBlammo = VerificarCartaEnMano("Carta6");
            bool tieneNanners = VerificarCartaEnMano("Carta5");

            if (tieneBlammo || tieneNanners)
            {
                bool decision = await DecisionSalvarTurno();
                if(decision)
                {
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.SalvarTurno), jugadorTurnoActual);
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
            bool cartaTomada = false;
            MouseButtonEventHandler mouseDownHandler = null;

            mouseDownHandler = (s, e) =>
            {
                cartaTomada = true;
                ZonaMazoCartas.MouseDown -= mouseDownHandler;
            };
            ZonaMazoCartas.MouseDown += mouseDownHandler;

            while (!cartaTomada)
            {
                await Task.Delay(100);
            }

            return cartaTomada;
        }


        private async Task FinalizarTurno()
        {
            await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
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
                        MoverCartaTipoEspecificoAEscondite(cartaSeleccionada, CartasEnMazo.Last().Tipo);
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
            cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, carta.IdCarta, idPartida);
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
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
            }
        }

        private void FichasItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fichaEnMesaSeleccionada = (e.OriginalSource as FrameworkElement)?.DataContext as Ficha;

            if (fichaEnMesaSeleccionada != null)
            {
                cliente.TomarFichaMesa(idPartida, fichaEnMesaSeleccionada.IdFicha);
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
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
                    // Deshabilitar clicks para evitar múltiples selecciones
                    HabilitarClickEnAreasJugadores(false);
                    cliente.RobarCartaAJugador(jugadorObjetivo.NombreUsuario, idPartida, CartaDuplicacionActiva);
                }
            }
        }

        private JugadorPartida ObtenerJugadorDesdeArea(StackPanel area)
        {
            if (area == AreaJugador2) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador2.Text);
            if (area == AreaJugador3) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador3.Text);
            if (area == AreaJugador4) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador4.Text);
            return null;
        }

        private async void AccionFichaRevelarCarta()
        {
            cliente.RevelarCartaMazo(idPartida);
            await Task.Delay(100);
            cliente.PreguntarGuardarCartaEnEscondite(idPartida);
            //cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);

        }

        private void MoverCartaTipoEspecificoAEscondite(CartaCliente cartaSeleccionada, string tipoCartaRevelada)
        {
            if(cartaSeleccionada.Tipo == tipoCartaRevelada)
            {
                cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta, idPartida);
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta incorrecta", "La carta seleccionada no es del tipo de la carta que fue revelada.");
            }
        }
        private void AccionFichaRobarCartaAJugador()
        {
            // Desuscribirse para evitar duplicados
            AreaJugador2.MouseDown -= RobarCartaDeJugador;
            AreaJugador3.MouseDown -= RobarCartaDeJugador;
            AreaJugador4.MouseDown -= RobarCartaDeJugador;
            AreaJugador2.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador3.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador4.MouseDown -= RobarCartaEsconditeDeJugador;

            // Mostrar mensaje y habilitar áreas
            VentanasEmergentes.CrearVentanaEmergente("Selecciona un jugador", "Haz clic en el jugador al que deseas robar una carta.");
            HabilitarClickEnAreasJugadores(true);

            // Asignar eventos una vez
            AreaJugador2.MouseDown += RobarCartaDeJugador;
            AreaJugador3.MouseDown += RobarCartaDeJugador;
            AreaJugador4.MouseDown += RobarCartaDeJugador;
        }

        private void AccionFichaRobarOGuardar()
        {
            cartasATomarMazo = CartaDuplicacionActiva ? 2 : 1;
            cartasAGuardarEscondite = CartaDuplicacionActiva ? 2 : 1;

            VentanasEmergentes.CrearVentanaEmergente(
                "Acción de ficha",
                $"Puedes realizar 1 de las siguientes acciones:\n1) Tomar {cartasATomarMazo} carta(s) del mazo\n2) Guardar {cartasAGuardarEscondite} carta(s) en el escondite."
            );

            ZonaMazoCartas.IsEnabled = true;
            cartasTomadasMazo = 0;
            cartasGuardadasEscondite = 0;
            ZonaMazoCartas.MouseDown += ZonaMazoCartas_MouseDown;
            CartasManoItemsControl.MouseDown += CartasManoItemsControl_MouseDown;
            cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite), jugadorTurnoActual);
        }

        private void ZonaMazoCartas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cartasTomadasMazo++;
            if (cartasTomadasMazo == cartasATomarMazo)
            {
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                ZonaMazoCartas.IsEnabled = false;
                ZonaMazoCartas.MouseDown -= ZonaMazoCartas_MouseDown;
                CartasManoItemsControl.MouseDown -= CartasManoItemsControl_MouseDown;
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
            }
            CartaDuplicacionActiva = false;
        }

        private void CartasManoItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cartasGuardadasEscondite++;
            if (cartasGuardadasEscondite == cartasAGuardarEscondite)
            {
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                ZonaMazoCartas.IsEnabled = false;
                ZonaMazoCartas.MouseDown -= ZonaMazoCartas_MouseDown;
                CartasManoItemsControl.MouseDown -= CartasManoItemsControl_MouseDown;
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);

            }
            CartaDuplicacionActiva = false;
        }



        private void AccionFichaGuardarCartasEscondite()
        {
            if (CartasEnMano.Any())
            {
                cartasAGuardarEscondite = CartaDuplicacionActiva ? 4 : 2;
                cartasGuardadasEscondite = 0;

                VentanasEmergentes.CrearVentanaEmergente(
                    "Acción ficha",
                    $"Puedes guardar {cartasAGuardarEscondite} cartas en el escondite."
                );

                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverAlEscondite), jugadorTurnoActual);
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

        // Método explícito para manejar MouseDown
        private void CartasManoItemsControl_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            cartasGuardadasEscondite++;

            if (cartasGuardadasEscondite == cartasAGuardarEscondite || !CartasEnMano.Any())
            {
                // Desuscripción del evento
                CartasManoItemsControl.MouseDown -= CartasManoItemsControl_MouseDown1;
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                CartaDuplicacionActiva = false;
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
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
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                }
            }
            else
            {
                if (CartaDuplicacionActiva)
                {
                    cartasATomarMazo = cartasATomarMazo * 2;
                }

                cartasTomadasMazo = 0;

                VentanasEmergentes.CrearVentanaEmergente(
                    "Tomar cartas del mazo",
                    $"Puedes tomar {cartasATomarMazo} cartas del mazo."
                );

                ZonaMazoCartas.IsEnabled = true;

                // Suscribir el evento
                ZonaMazoCartas.MouseDown += ZonaMazoCartas_MouseDown1;
            }
        }

        // Método explícito para manejar MouseDown
        private void ZonaMazoCartas_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            cartasTomadasMazo++;

            if (cartasTomadasMazo == cartasATomarMazo || EsMazoVacio())
            {
                // Desuscribir el evento
                ZonaMazoCartas.MouseDown -= ZonaMazoCartas_MouseDown1;

                ZonaMazoCartas.IsEnabled = false;

                if (fichaSeleccionada != null)
                {
                    cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
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

        //NOTIFICACIONES
        public void NotificarTurnoIniciado(string jugadorTurnoActual)
        {
            this.jugadorTurnoActual = jugadorTurnoActual;
            modoSeleccionActual = ModoSeleccionCarta.CartasSinTurno;
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
                cliente.DevolverFichaAMesa(ficha.IdFicha, idPartida);
            }
            DadoImagen.IsEnabled = false;
            dado.DadoLanzado -= ManejarResultadoDado;
            modoSeleccionActual = ModoSeleccionCarta.CartasSinTurno;
            
        }

        public void NotificarResultadoAccion(string action, bool success)
        {
            //TODO
        }

        public void NotificarPartidaCreada(string idPartida)
        {
            this.idPartida = idPartida;
            DadoImagen.IsEnabled = false;
       //     ZonaMazoCartas.IsEnabled = false;
            FichasManoItemsControl.IsEnabled = false;
            CargarFichas();
            modoSeleccionActual = ModoSeleccionCarta.MoverAlEscondite;
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

        /*public void NotificarIntentoRoboCarta(string jugadorObjetivoRobo)
        {
            if (jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                MessageBoxResult decision = DecisionDefenderseRobo();

                if (decision == MessageBoxResult.Yes)
                {
                    CartasManoItemsControl.IsEnabled = true;
                    modoSeleccionActual = ModoSeleccionCarta.DefenderRobo;
                }
                else
                {
                    cliente.RobarCarta(jugadorObjetivoRobo, idPartida);
                }
            }
        }*/

        public async void NotificarIntentoRoboCarta(string nombreUsuarioAtacante)
        {
            if (jugadorActual.NombreUsuario == nombreUsuarioAtacante)
            {
                bool decision = await DecisionDefenderseRobo();

                if (decision)
                {
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo), jugadorActual.NombreUsuario);
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
                        cliente.RobarCarta(idPartida, nombreUsuarioAtacante); // Continúa con el flujo de robo en el servidor.
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
                cliente.DevolverFichaAMesa(4, idPartida);
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
                    cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.DefenderRobo), jugadorActual.NombreUsuario);
                }
                else
                {
                    cliente.RobarCartaEscondite(idPartida, nombreUsuarioAtacante); // Continúa con el flujo de robo en el servidor.
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
                cliente.LanzarDado(idPartida, jugadorTurnoActual);
            });
        }

        public void NotificarPreguntaJugadores(string jugadorTurnoActual)
        {
            ManejarDecisionGuardarCartaEnEscondite();
        }

        public void NotificarNumeroJugadoresGuardaronCarta(int numeroJugadores)
        {
            cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, CartasEnMazo.Last().IdCarta);
            cliente.OcultarCartaMazo(idPartida);
            cartasATomarMazo = numeroJugadores;
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

       

        private async void ManejarDecisionGuardarCartaEnEscondite()
        {
            bool decision = await DecisionGuardarCartaEnEscondite();

            if (decision)
            {
                cliente.EnviarDecision(idPartida, true);
                cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.MoverCartaTipoEspecificoAEscondite), jugadorActual.NombreUsuario);
            }
            else
            {
                cliente.EnviarDecision(idPartida, false);
                modoSeleccionActual = ModoSeleccionCarta.AccionCartasEnTurno;
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
            cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            RobarCartaEsconditeAJugador();

        }

        private void UsarCarta2(int idCarta)
        {
            if (jugadorDecidioParar)
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                cliente.ObligarATirarDado(idPartida);
                jugadorDecidioParar = false;
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
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                SeleccionarCartaDescarte(idCarta);
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente("Carta no disponible", "No hay cartas en el descarte por lo que no puede ocupar esta carta");
            }
        }

        private void UsarCarta4(int idCarta)
        {
            cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            CartaDuplicacionActiva = true;
        }

        private async void UsarCarta5(int idCarta)
        {
            cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            DadoImagen.IsEnabled = false;
            cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
            await ResolverFichas();
            await FinalizarTurno();
        }

        private void UsarCarta6(int idCarta)
        {
            cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
            cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
            ManejarDecisionContinuarTurno();
        }


        private void UsarCarta7(int idCarta)
        {
            cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);

            VentanasEmergentes.CrearVentanaEmergente("Acción de carta", "Puedes tomar 2 cartas del Mazo, da clic 2 veces en él.");
            ZonaMazoCartas.IsEnabled = true;
            cartasTomadasMazo = 0;

            void OnMouseDownHandler(object sender, MouseButtonEventArgs e)
            {
                cartasTomadasMazo++;

                if (cartasTomadasMazo == 2)
                {
                    ZonaMazoCartas.IsEnabled = false;
                    if(jugadorActual.NombreUsuario == jugadorTurnoActual)
                    {
                        cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.AccionCartasEnTurno), jugadorTurnoActual);
                    }
                    else
                    {
                        cliente.EstablecerModoSeleccionCarta(idPartida, Array.IndexOf(modosSeleccionCarta, ModoSeleccionCarta.CartasSinTurno), jugadorActual.NombreUsuario);
                    }

                    ZonaMazoCartas.MouseDown -= OnMouseDownHandler;

                    if(fichaSeleccionada != null)
                    {
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                    }
                }
            }
            ZonaMazoCartas.MouseDown += OnMouseDownHandler;
        }

        private void UsarCarta8(int idCarta)
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

        private async Task<bool> DecisionDefenderseRobo()
        {
            var ventanaDeDecision = VentanasEmergentes.CrearVentanaDeDecision("Decision de defensa", "Has sido elegido como objeto de robo. ¿Quieres defenderte?");
            return await ventanaDeDecision.MostrarDecision();
        }

        private void RobarCartaEsconditeAJugador()
        {
            // Desuscribirse para evitar duplicados
            AreaJugador2.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador3.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador4.MouseDown -= RobarCartaEsconditeDeJugador;
            AreaJugador2.MouseDown -= RobarCartaDeJugador;
            AreaJugador3.MouseDown -= RobarCartaDeJugador;
            AreaJugador4.MouseDown -= RobarCartaDeJugador;

            // Mostrar mensaje y habilitar áreas
            Metodos.MostrarMensaje("Haz clic en el jugador al que deseas robar una carta de su escondite.");
            HabilitarClickEnAreasJugadores(true);

            // Asignar eventos una vez
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
                    // Deshabilitar clicks para evitar múltiples selecciones
                    HabilitarClickEnAreasJugadores(false);
                    cliente.RobarCartaEsconditeAJugador(jugadorObjetivo.NombreUsuario, idPartida, CartaDuplicacionActiva);
                }
            }
        }

        private void SeleccionarCartaDescarte(int idCartaRecientementeDescartada)
        {
            // Filtrar cartas elegibles
            var cartasElegibles = CartasDescarte.Where(c => c.IdCarta != idCartaRecientementeDescartada).ToList();

            if (!cartasElegibles.Any())
            {
                MessageBox.Show("No hay cartas disponibles en el descarte.");
                return;
            }

            // Mostrar el diálogo
            var dialogo = new DialogoSeleccionCarta(cartasElegibles);
            if (dialogo.ShowDialog() == true)
            {
                var cartaSeleccionada = dialogo.CartaSeleccionada;
                Metodos.MostrarMensaje($"Carta seleccionada: {cartaSeleccionada.Tipo}");
                cliente.TomarCartaDeDescarte(idPartida, jugadorActual.NombreUsuario, cartaSeleccionada.IdCarta);
            }
        }
    }
}


