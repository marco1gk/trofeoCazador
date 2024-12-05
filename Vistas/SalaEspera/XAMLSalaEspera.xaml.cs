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
    public partial class XAMLSalaEspera : Page, ILobbyManagerCallback
    {
        string anfitrion;
        private LobbyManagerClient cliente;
        private string codigoSalaEsperaActual;
        private JugadorSalaEspera _selectedAmigo;
        private int numeroJugadoresSalaEspera = 1;


        public ObservableCollection<JugadorSalaEspera> JugadoresEnSala { get; set; }

    
        public XAMLSalaEspera()
        {
            InitializeComponent();
            JugadoresEnSala = new ObservableCollection<JugadorSalaEspera>();
            AmigosDisponibles = new ObservableCollection<JugadorSalaEspera>();
            DataContext = this;
            SetupClient();
            CargarAmigosJugador();
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

        private void JugadorControl_JugadorExpulsado(object sender, string nombreUsuario)
        {
            ExpulsarJugadorSalaEsperaAsync(nombreUsuario);
            MessageBox.Show($"{nombreUsuario} ha sido expulsado.");

        }
        public async Task ExpulsarJugadorSalaEsperaAsync(string nombreUsuario)
        {
            InstanceContext contexto = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClientExpulsar = new LobbyManagerClient(contexto);

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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }
        private async void BtnCrearLobby_Click(object sender, RoutedEventArgs e)
        {
            stackPanelOpciones.Visibility = Visibility.Collapsed;
            stackPanelJugadores.Visibility = Visibility.Visible;
            gridChat.Visibility = Visibility.Visible;
            btnIniciarPartida.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            stackPanelAmigos.Visibility = Visibility.Visible;
            btnEnviarInvitacion.Visibility= Visibility.Visible; 
             SingletonSesion sesion = SingletonSesion.Instancia;
            string nombreUsuario = sesion.NombreUsuario;
            anfitrion = nombreUsuario;
            int numeroFotoPerfil = sesion.NumeroFotoPerfil;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = nombreUsuario, NumeroFotoPerfil = numeroFotoPerfil };

            try
            {
  

                cliente.CrearSalaEspera(lb);
                btnIniciarPartida.Visibility = Visibility.Visible;
                MessageBox.Show("Se ha creado un nuevo lobby por "+nombreUsuario   );

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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }
        bool isJoiningLobby = false;
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }

 
        private void SetupClient()
        {
            
            InstanceContext instanciaContexto = new InstanceContext(this);
            cliente = new LobbyManagerClient(instanciaContexto);
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
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

            // Usar Dispatcher para asegurar que XAMLTablero se cree en el hilo principal STA
            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLTablero tablero = new XAMLTablero(jugadores.ToList(), codigoSalaEsperaActual);
                this.NavigationService.Navigate(tablero);
            });
        }

        public void NotificarSalaEsperaLlena()
        {
            MessageBox.Show("El lobby está lleno.");
        }

        public void NotificarSalaEsperaNoExiste()
        {
            MessageBox.Show("El lobby no existe.");
        }

        public void NotificarExpulsadoSalaEspera()
        {
            MessageBox.Show("Has sido expulsado del lobby.");
            NavigationService.Navigate(new XAMLSalaEspera());
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

        public void NotificarPuedeIniciarPartida(bool puedeIniciar )
        {
            Dispatcher.Invoke(() => {
                btnIniciarPartida.Visibility = puedeIniciar ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private async void BtnClicIniciarPartida(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(codigoSalaEsperaActual))
            {
                try
                {
                    await Task.Run(() => cliente.IniciarPartida(codigoSalaEsperaActual));
                    
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
                    VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
            }
            else
            {
                MessageBox.Show("Código de lobby no disponible. Asegúrate de haber creado o unido a un lobby.");
            }
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
                MessageBox.Show("No estás en un lobby.");
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
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

                    cliente.InvitarAmigoASala(codigoSalaEsperaActual, amigoSeleccionado.NombreUsuario, nombreUsuario);

                    MessageBox.Show($"Se envió la invitación a {amigoSeleccionado.NombreUsuario}.", "Invitación enviada", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                    ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un amigo para invitar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        public ObservableCollection<JugadorSalaEspera> AmigosDisponibles { get; set; } = new ObservableCollection<JugadorSalaEspera>();


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

        private void BotonCrearVentanaInvitacion_Click(object sender, RoutedEventArgs e)
        {
            VentanasEmergentes.CrearVentanaInvitacionSalaEspera(codigoSalaEsperaActual);
        }

        
    }
}
