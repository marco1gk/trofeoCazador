﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using trofeoCazador.ServicioDelJuego;
using System.ServiceModel;
using System.Windows.Input;
using trofeoCazador.Vistas.PartidaJuego;
using trofeoCazador.Vistas.InicioSesion;
using System.Windows.Media.Imaging;
using trofeoCazador.Vistas.SalaEspera;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using trofeoCazador.Utilidades;
using System.Diagnostics;

    namespace trofeoCazador.Vistas.SalaEspera
    {
        public partial class XAMLSalaEspera : Page, ISalaEsperaServicioCallback, INotifyPropertyChanged
        {
            private bool esAnfitrion;
            public event PropertyChangedEventHandler PropertyChanged;
            private string anfitrion;
            bool uniendoSalaEspera = false;
            private string codigoLobbyGuardado;
            private SalaEsperaServicioClient cliente;
            private string codigoSalaEsperaActual;
            private int numeroJugadoresSalaEspera;
            public ObservableCollection<JugadorSalaEspera> AmigosDisponibles { get; set; } = new ObservableCollection<JugadorSalaEspera>();
            public ObservableCollection<JugadorSalaEspera> JugadoresEnSala { get; set; }


        private void GuardarEstadoLobby()
        {
            if (!string.IsNullOrEmpty(codigoSalaEsperaActual))
            {
                codigoLobbyGuardado = codigoSalaEsperaActual;
            }
        }
        public void NotificarIniciarPartida(JugadorPartida[] jugadores)
        {
            Console.WriteLine("NotificarIniciarPartida");
            Console.WriteLine("Jugadores recibidos en el cliente:");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.NombreUsuario}");
                Console.WriteLine($"Foto de perfil: {jugador.NumeroFotoPerfil}");
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Console.WriteLine("Comprobando NavigationService...");

                if (this.NavigationService == null)
                {
                    Console.WriteLine("NavigationService es null, intentando obtener el Frame principal.");

                    Frame frame = Application.Current.MainWindow.Content as Frame;
                    if (frame != null)
                    {
                        Console.WriteLine("Frame principal encontrado, navegando al tablero.");
                        XAMLTablero tablero = new XAMLTablero(jugadores.ToList(), codigoSalaEsperaActual);
                        frame.Navigate(tablero);
                    }
                    else
                    {
                        Console.WriteLine("No se pudo obtener el Frame principal, navegación fallida.");
                    }
                }
                else
                {
                    Console.WriteLine("NavigationService está disponible, navegando al tablero.");
                    XAMLTablero tablero = new XAMLTablero(jugadores.ToList(), codigoSalaEsperaActual);
                    this.NavigationService.Navigate(tablero);
                }
            });
        }

        public XAMLSalaEspera()
            {
                InitializeComponent();
                JugadoresEnSala = new ObservableCollection<JugadorSalaEspera>();
                AmigosDisponibles = new ObservableCollection<JugadorSalaEspera>();
                DataContext = this;
                SetupClient();
                CargarAmigosJugador();

            }

        private async void BtnEnviarMensaje(object sender, RoutedEventArgs e)
        {
            string mensaje = tbxMessage.Text.Trim();

            if (string.IsNullOrEmpty(mensaje))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbMensajeFaltante, Properties.Resources.lbMensajeFaltanteDescripcion);
                return;
            }

            try
            {
                SetupClient();

                if (cliente != null)
                {
                    await Task.Run(() => cliente.MandarMensaje(mensaje));
                    tbxMessage.Clear();
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
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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

        private void CloseClient()
        {
            if (cliente != null)
            {
                try
                {
                    if (cliente.State == CommunicationState.Opened)
                    {
                        cliente.Close();
                    }
                    else
                    {
                        cliente.Abort();
                    }
                }
                catch
                {
                    cliente.Abort();
                }
                finally
                {
                    cliente = null;
                }
            }
        }

        private void BtnCrearLobby_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SingletonSesion sesion = SingletonSesion.Instancia;
                string nombreUsuario = sesion.NombreUsuario;
                anfitrion = nombreUsuario;
                EsAnfitrion = true;
                int numeroFotoPerfil = sesion.NumeroFotoPerfil;

                JugadorSalaEspera jugador = new JugadorSalaEspera
                {
                    NombreUsuario = nombreUsuario,
                    NumeroFotoPerfil = numeroFotoPerfil
                };

                cliente.CrearSalaEspera(jugador);
                AjustarVisibilidadAfitrion();
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        public void AjustarVisibilidadAfitrion()
        {
            stackPanelOpciones.Visibility = Visibility.Collapsed;
            stackPanelJugadores.Visibility = Visibility.Visible;
            gridChat.Visibility = Visibility.Visible;
            btnIniciarPartida.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            stackPanelAmigos.Visibility = Visibility.Visible;
            btnEnviarInvitacion.Visibility = Visibility.Visible;

        }
        
            private async void BtnUnirseLobby_Click(object sender, RoutedEventArgs e)
            {

                SetupClient();

                if (cliente == null)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    return;
                }
                btnCrearLobby.Visibility = Visibility.Collapsed;
                txtCodigoLobby.Visibility = Visibility.Visible;
                btnEnviarInvitacion.Visibility = Visibility.Visible;

                if (uniendoSalaEspera) return;

                uniendoSalaEspera = true;

                try
                {
                    SingletonSesion sesion = SingletonSesion.Instancia;
                    string nombreUsuario = sesion.NombreUsuario;
                    int numeroFotoPerfil = sesion.NumeroFotoPerfil;
                    JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = nombreUsuario, NumeroFotoPerfil = numeroFotoPerfil };

                    string codigoSalaEspera = txtCodigoLobby.Text.Trim();

                    if (string.IsNullOrEmpty(codigoSalaEspera))
                    {
                        VentanasEmergentes.CrearSalaEsperaNoEncontradaMensajeVentana();
                        return;
                    }
                    List<string> codigos = (await Task.Run(() => cliente.ObtenerCodigosGenerados())).ToList();

                    if (codigos.Contains(codigoSalaEspera))
                    {
                        cliente.UnirseSalaEspera(codigoSalaEspera, lb);

                        stackPanelOpciones.Visibility = Visibility.Collapsed;
                        stackPanelJugadores.Visibility = Visibility.Visible;
                        gridChat.Visibility = Visibility.Visible;
                        btnSalir.Visibility = Visibility.Visible;
                        btnIniciarPartida.Visibility = Visibility.Visible;

                        codigoSalaEsperaActual = codigoSalaEspera;
                        txtCodigoLobby.Visibility = Visibility.Collapsed;

                    }
                    else
                    {
                        VentanasEmergentes.CrearSalaEsperaNoEncontradaMensajeVentana();
                    }
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
                }
                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
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
                finally
                {
                    uniendoSalaEspera = false;
                }
            }

            private void VerificarYReiniciarCliente()
            {
                if (cliente == null || cliente.State == CommunicationState.Faulted)
                {
                    try
                    {
                        ReiniciarCliente();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al reiniciar el cliente: {ex.Message}",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            private void ReiniciarCliente()
            {
                try
                {
                    if (cliente != null)
                    {
                        cliente.Abort(); 
                    }

                    InstanceContext instanciaContexto = new InstanceContext(this);
                    cliente = new SalaEsperaServicioClient(instanciaContexto);
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
                    ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
            }
            private void SetupClient()
            {
                Debug.WriteLine("SetupClient llamado.");

                if (cliente != null)
                {
                    Debug.WriteLine($"Estado del cliente antes de verificar: {cliente.State}");

                    if (cliente.State == CommunicationState.Faulted)
                    {
                        Debug.WriteLine("Cliente en estado Faulted. Cerrando...");
                        GuardarEstadoLobby();
                        CloseClient();
                    }
                    else
                    {
                        Debug.WriteLine("Cliente ya configurado. Saliendo de SetupClient.");
                        return;
                    }
                }

                Debug.WriteLine("Creando nueva instancia de cliente.");
                cliente = new SalaEsperaServicioClient(new InstanceContext(this));
                cliente.Open();
                Debug.WriteLine("Cliente abierto correctamente.");

                RestaurarEstadoLobby();
            }

            private void RestaurarEstadoLobby()
            {
                Debug.WriteLine($"Intentando restaurar estado del lobby con código: {codigoLobbyGuardado}");

                if (!string.IsNullOrEmpty(codigoLobbyGuardado))
                {
                    try
                    {
                        SingletonSesion sesion = SingletonSesion.Instancia;
                        string nombreUsuario = sesion.NombreUsuario;
                        int numeroFotoPerfil = sesion.NumeroFotoPerfil;

                        JugadorSalaEspera jugador = new JugadorSalaEspera
                        {
                            NombreUsuario = nombreUsuario,
                            NumeroFotoPerfil = numeroFotoPerfil
                        };
                        Debug.WriteLine($"el nombre del jugador es " + jugador.NombreUsuario);
                        cliente.UnirseSalaEspera(codigoLobbyGuardado, jugador);
                        Debug.WriteLine($"Restaurado con éxito al lobby: {codigoLobbyGuardado}");
                        codigoSalaEsperaActual = codigoLobbyGuardado;
                    
                        cliente.ObtenerCodigosGenerados();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error al restaurar estado del lobby: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("No hay código de lobby guardado para restaurar.");
                }
            }
        private async void BtnClicIniciarPartida(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(codigoSalaEsperaActual))
            {
                MessageBox.Show("Código de lobby no disponible. Asegúrate de haber creado o unido a un lobby.");
                return;
            }

            try
            {
                VerificarYReiniciarCliente();

                await Task.Run(() =>
                {
                    try
                    {
                        cliente.IniciarPartida(codigoSalaEsperaActual);
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    }
                    catch (TimeoutException ex)
                    {
                        VentanasEmergentes.CrearVentanaMensajeTimeOut();
                        ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
                    }
                    catch (FaultException<HuntersTrophyExcepcion>)
                    {
                        VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                    }

                    catch (FaultException)
                    {
                        VentanasEmergentes.CrearMensajeVentanaServidorError();
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
                });
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
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }

        public void NotificarJugadorSalioSalaEspera(string nombreUsuario)
        {
            Dispatcher.Invoke(() =>
            {
                var jugador = JugadoresEnSala.FirstOrDefault(j => j.NombreUsuario == nombreUsuario);
                if (jugador != null)
                {
                    JugadoresEnSala.Remove(jugador);
                    numeroJugadoresSalaEspera--;

                }
            });
        }

        public async Task ExpulsarJugadorSalaEsperaAsync(string nombreUsuario)
        {

            if (nombreUsuario == anfitrion)
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloUps,Properties.Resources.lbDescripcionNoExpulsarAnfitrion);
                return;
            }
            InstanceContext contexto = new InstanceContext(this);
            SalaEsperaServicioClient lobbyManagerClientExpulsar = new SalaEsperaServicioClient(contexto);

            try
            {
                await lobbyManagerClientExpulsar.ExpulsarJugadorSalaEsperaAsync(codigoSalaEsperaActual, nombreUsuario);
                SingletonSesion sesion = SingletonSesion.Instancia;
                if (sesion.NombreUsuario == nombreUsuario)
                {
                    NotificarExpulsadoSalaEspera();
                }

            }
            catch (EndpointNotFoundException )
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException )
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (CommunicationException )
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }

        }
        public void SalirSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            try
            {
                cliente.SalirSalaEspera(codigoSalaEspera, nombreUsuario);
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
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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
        }
   
        private void JugadorControl_JugadorExpulsado(object sender, string nombreUsuario)
        {
            ExpulsarJugadorSalaEsperaAsync(nombreUsuario);

        }

        public void UnirseComoInvitado(string codigoSalaEspera)
        {
            AjustarVisibilidad();

            try
            {
                string nombreUsuarioInvitado = "Invitado";

                JugadorSalaEspera invitado = new JugadorSalaEspera
                {
                    NombreUsuario = nombreUsuarioInvitado,
                    EsInvitado = true,
                    NumeroFotoPerfil = 1
                };

                cliente.UnirseSalaEspera(codigoSalaEspera, invitado);
                string mensaje = Properties.Resources.lbInvitadoUnido + nombreUsuarioInvitado;
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbInvitadoUnion, mensaje);

                
                Dispatcher.Invoke(() =>
                {
                    JugadoresEnSala.Add(invitado);
                });
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
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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
        }

        public void AjustarVisibilidad()
        {
            btnCrearLobby.Visibility = Visibility.Collapsed;
            txtCodigoLobby.Visibility = Visibility.Collapsed;
            btnEnviarInvitacion.Visibility = Visibility.Visible;
            stackPanelOpciones.Visibility = Visibility.Collapsed;
            stackPanelJugadores.Visibility = Visibility.Visible;
            gridChat.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            btnIniciarPartida.Visibility = Visibility.Visible;
        }



        public void UnirseSalaEsperaComoHost(string codigoSalaEspera)
        {
            try
            {
                cliente.UnirSalaEsperaComoAnfitrion(codigoSalaEspera);
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
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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
        }

       
        public void RecibirMensaje(string nombreUsuario, string mensaje)
        {
            Dispatcher.Invoke(() =>
            {
               
                stackPanelMessages.Children.Add(new TextBlock { Text = $"{nombreUsuario}: {mensaje}" });
            });
        }
        public void NotificarJugadoresEnSalaEspera(string codigoSalaEspera, JugadorSalaEspera[] jugadores)
        {
            Dispatcher.Invoke(() =>
            {
               
                JugadoresEnSala.Clear(); 
                foreach (var jugador in jugadores)
                {
                    JugadoresEnSala.Add(jugador); 
                }
            });
        }
       

        public void NotificarAnfritionJugadorSalioSalaEspera()
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                NavigationService.Navigate(new XAMLSalaEspera()); 
            });
        }



        private async void BtnSalirLobby(object sender, RoutedEventArgs e)
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string nombreUsuario = sesion.NombreUsuario;

            if (string.IsNullOrEmpty(codigoSalaEsperaActual))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbNoSalaEspera,Properties.Resources.lbDescripcionNoHaySalaEspera);
                return;
            }

            try
            {
                await Task.Run(() => cliente.SalirSalaEspera(codigoSalaEsperaActual, nombreUsuario));
                
                codigoSalaEsperaActual = null;

                Dispatcher.Invoke(() => JugadoresEnSala.Clear());

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.Navigate(new XAMLInicioSesion());
                }
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
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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
        }

        public void NotificarInvitacionSala(string nombreInvitador, string codigoSalaEspera)
        {
            string mensaje = Properties.Resources.lbInvitacionAmigoRecibida + " " + nombreInvitador + " " + Properties.Resources.lbDescripcionInvitacionsala + " " + codigoSalaEspera;
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico,mensaje);
        }
        private void BtnInvitarAmigo_Click(object sender, RoutedEventArgs e)
        {
            var amigoSeleccionado = comboBoxAmigos.SelectedItem as JugadorSalaEspera;

            if (amigoSeleccionado != null)
            {
                try
                {
                    string codigoSala = txtCodigoLobby.Text;
                    string nombreUsuario = SingletonSesion.Instancia.NombreUsuario;
                    string detallesVentana;
                    cliente.InvitarAmigoASala(codigoSalaEsperaActual, amigoSeleccionado.NombreUsuario, nombreUsuario);
                    detallesVentana = Properties.Resources.lbInvitarAmigoSala +" "+ amigoSeleccionado.NombreUsuario;
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloInvitacionEnviada, detallesVentana);
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
                }
                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
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
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloInvitacionAmigo, Properties.Resources.lbSeleccionaAmigo);
            }
        }

        private void CargarAmigosJugador()
        {
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
            int idJugadorActual = SingletonSesion.Instancia.JugadorId;
            SingletonSesion sesion = SingletonSesion.Instancia;
            try
            {
                string[] nombreUsuarioAmigosJugador = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId);

                AmigosDisponibles.Clear();
                foreach (var amigo in nombreUsuarioAmigosJugador)
                {
                    JugadorSalaEspera amigoSala = new JugadorSalaEspera { NombreUsuario = amigo };
                    AmigosDisponibles.Add(amigoSala);
                    Console.WriteLine($"Amigo: {amigo}");
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
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
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

        private void BtnCrearVentanaInvitacion_Click(object sender, RoutedEventArgs e)
        {
            VentanasEmergentes.CrearVentanaInvitacionSalaEspera(codigoSalaEsperaActual);
        }


        public void NotificarSalaEsperaCreada(string codigoSalaEspera)
        {
            codigoSalaEsperaActual = codigoSalaEspera;
        }

        public void NotificarJugadorSeUnioSalaEspera(JugadorSalaEspera jugador, int numeroJugadoresSalaEspera)
        {
            Dispatcher.Invoke(() =>
            {
                JugadoresEnSala.Add(jugador);
                this.numeroJugadoresSalaEspera = ++numeroJugadoresSalaEspera;
            });
        }

        public void NotificarSalaEsperaLlena()
        {
            VentanasEmergentes.CrearVentanaMensajeSalaEsperaLlena();
        }

        public void NotificarSalaEsperaNoExiste()
        {
            VentanasEmergentes.CrearSalaEsperaNoEncontradaMensajeVentana();
        }

        public void NotificarExpulsadoSalaEspera()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloExpulsion, Properties.Resources.lbDetallesExpulsion);
                NavigationService.Navigate(new XAMLSalaEspera());
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool EsAnfitrion
        {
            get => esAnfitrion;
            set
            {
                esAnfitrion = value;
                OnPropertyChanged(nameof(EsAnfitrion));
            }
        }

        public void NotificarPuedeIniciarPartida(bool puedeIniciar)
        {
            Dispatcher.Invoke(() => {
                btnIniciarPartida.Visibility = puedeIniciar ? Visibility.Visible : Visibility.Collapsed;
            });
        }
    }
}
