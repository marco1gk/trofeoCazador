using System;
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

namespace trofeoCazador.Vistas.SalaEspera
{
    public partial class XAMLSalaEspera : Page, ISalaEsperaServicioCallback, INotifyPropertyChanged
    {
        private bool esAnfitrion;
        public event PropertyChangedEventHandler PropertyChanged;
        private string anfitrion;
        bool isJoiningLobby = false;
        private SalaEsperaServicioClient cliente;
        private string codigoSalaEsperaActual;
        private int numeroJugadoresSalaEspera;
        public ObservableCollection<JugadorSalaEspera> AmigosDisponibles { get; set; } = new ObservableCollection<JugadorSalaEspera>();
        public ObservableCollection<JugadorSalaEspera> JugadoresEnSala { get; set; }



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

        public XAMLSalaEspera()
        {
            InitializeComponent();
            JugadoresEnSala = new ObservableCollection<JugadorSalaEspera>();
            AmigosDisponibles = new ObservableCollection<JugadorSalaEspera>();
            DataContext = this;
            SetupClient();
            CargarAmigosJugador();

        }

        public void NotificarPuedeIniciarPartida(bool puedeIniciar )
        {
            Dispatcher.Invoke(() => {
                btnIniciarPartida.Visibility = puedeIniciar ? Visibility.Visible : Visibility.Collapsed;
            });
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
                    MessageBox.Show($"Error al reiniciar el cliente: {ex.Message}",
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
                MessageBox.Show("El anfitrión no puede expulsarse a sí mismo.");
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
        public void SalirSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            try
            {
                cliente.SalirSalaEspera(codigoSalaEspera, nombreUsuario);
                MessageBox.Show($"{nombreUsuario} ha salido del lobby {codigoSalaEspera}.");
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

                stackPanelOpciones.Visibility = Visibility.Collapsed;
                stackPanelJugadores.Visibility = Visibility.Visible;
                gridChat.Visibility = Visibility.Visible;
                btnIniciarPartida.Visibility = Visibility.Visible;
                btnSalir.Visibility = Visibility.Visible;
                stackPanelAmigos.Visibility = Visibility.Visible;
                btnEnviarInvitacion.Visibility = Visibility.Visible;

                MessageBox.Show($"Se ha creado un nuevo lobby por {nombreUsuario}");
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                Console.WriteLine($"Error de conexión: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                Console.WriteLine($"Timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

        public void UnirseComoInvitado(string codigoSalaEspera)
        {
            btnCrearLobby.Visibility = Visibility.Collapsed;
            txtCodigoLobby.Visibility = Visibility.Collapsed;
            btnEnviarInvitacion.Visibility = Visibility.Visible;
            stackPanelOpciones.Visibility = Visibility.Collapsed;
            stackPanelJugadores.Visibility = Visibility.Visible;
            gridChat.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            btnIniciarPartida.Visibility = Visibility.Visible;

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

                MessageBox.Show($"Te has unido al lobby {codigoSalaEspera} como invitado con el nombre {nombreUsuarioInvitado}.");

                
                Dispatcher.Invoke(() =>
                {
                    JugadoresEnSala.Add(invitado);
                });
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }




        private async void BtnUnirseLobby_Click(object sender, RoutedEventArgs e)
        {
            btnCrearLobby.Visibility = Visibility.Collapsed;
            txtCodigoLobby.Visibility = Visibility.Visible;
            btnEnviarInvitacion.Visibility = Visibility.Visible;

            if (isJoiningLobby) return;

            isJoiningLobby = true;

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

                    MessageBox.Show($"Te has unido al lobby con código: {codigoSalaEspera}");
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
                isJoiningLobby = false; 
            }
        }

        private async void BtnEnviarMensaje(object sender, RoutedEventArgs e)
        {
            string mensaje = tbxMessage.Text.Trim();

            if (string.IsNullOrEmpty(mensaje))
            {
                MessageBox.Show("Por favor, escribe un mensaje antes de enviar.");
                return;
            }

            try
            {
                if (cliente == null)
                {
                    SetupClient(); 
                }

                await Task.Run(() => cliente.MandarMensaje(mensaje)); 
                tbxMessage.Clear();
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


        private void SetupClient()
        {
            try
            {
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

            catch (FaultException )
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

        public void UnirseSalaEsperaComoHost(string codigoSalaEspera)
        {
            try
            {
                cliente.UnirSalaEsperaComoAnfitrion(codigoSalaEspera);
                MessageBox.Show($"Intentando unirse al lobby {codigoSalaEspera} como anfitrión...");
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

       

        public void NotificarSalaEsperaCreada(string codigoSalaEspera)
        {
            codigoSalaEsperaActual = codigoSalaEspera;
            MessageBox.Show($"Lobby creado con el código: {codigoSalaEspera}");
        }

    

        public void NotificarJugadorSeUnioSalaEspera(JugadorSalaEspera jugador, int numeroJugadoresSalaEspera)
        {
            Dispatcher.Invoke(() =>
            {
                JugadoresEnSala.Add(jugador);
                MessageBox.Show($"{jugador.NombreUsuario} se unió. Jugadores en el lobby: {numeroJugadoresSalaEspera}");
                this.numeroJugadoresSalaEspera = ++numeroJugadoresSalaEspera;
    });
        }

        public void NotificarIniciarPartida(JugadorPartida[] jugadores)
        {
            Console.WriteLine("Jugadores recibidos en el cliente:");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.NombreUsuario}");
                Console.WriteLine($"Foto de perfil: {jugador.NumeroFotoPerfil}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLTablero tablero = new XAMLTablero(jugadores.ToList(), codigoSalaEsperaActual);
                this.NavigationService.Navigate(tablero);
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
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloExpulsion,Properties.Resources.lbDetallesExpulsion);
                NavigationService.Navigate(new XAMLSalaEspera());
            });
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
            Console.WriteLine($"Has sido invitado por {nombreInvitador} a unirte a la sala: {codigoSalaEspera}.");
         
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
            string[] nombreUsuarioAmigosJugador = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId);

            AmigosDisponibles.Clear(); 
            foreach (var amigo in nombreUsuarioAmigosJugador)
            {
                JugadorSalaEspera amigoSala = new JugadorSalaEspera { NombreUsuario = amigo };
                AmigosDisponibles.Add(amigoSala);
                Console.WriteLine($"Amigo: {amigo}");
            }
        }

        private void BtnCrearVentanaInvitacion_Click(object sender, RoutedEventArgs e)
        {
            VentanasEmergentes.CrearVentanaInvitacionSalaEspera(codigoSalaEsperaActual);
        }

        
    }
}
