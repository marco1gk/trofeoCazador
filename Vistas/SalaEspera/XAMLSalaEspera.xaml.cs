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

namespace trofeoCazador.Vistas.SalaEspera
{
    public partial class XAMLSalaEspera : Page, ILobbyManagerCallback
    {
        string anfitrion;
        private LobbyManagerClient cliente;
        private string codigoSalaEsperaActual;
        private int numeroJugadoresSalaEspera = 1;
        public ObservableCollection<JugadorSalaEspera> JugadoresEnSala { get; set; }
       
        public XAMLSalaEspera()
        {
            InitializeComponent();
            JugadoresEnSala = new ObservableCollection<JugadorSalaEspera>();
            DataContext = this; 
            SetupClient();
        }
        private void JugadorControl_JugadorExpulsado(object sender, string nombreUsuario)
        {
            ExpulsarJugadorSalaEsperaAsync(nombreUsuario);
            MessageBox.Show($"{nombreUsuario} ha sido expulsado.");
            
        }

        private async void BtnCrearLobby_Click(object sender, RoutedEventArgs e)
        {
            stackPanelOpciones.Visibility = Visibility.Collapsed;
            stackPanelJugadores.Visibility = Visibility.Visible;
            gridChat.Visibility = Visibility.Visible;
            btnIniciarPartida.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            
            SingletonSesion sesion = SingletonSesion.Instancia;
            string nombreUsuario = sesion.NombreUsuario;
             anfitrion = nombreUsuario;
            int numeroFotoPerfil = sesion.NumeroFotoPerfil;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = nombreUsuario, NumeroFotoPerfil = numeroFotoPerfil};

            try
            {
                // Crear un lobby nuevo
<<<<<<< HEAD
                client.CrearSalaEspera(lb);
                btnIniciarPartida.Visibility = Visibility.Visible;
                MessageBox.Show("Se ha creado un nuevo lobby.");
=======
                cliente.CrearSalaEspera(lb);
                MessageBox.Show("Se ha creado un nuevo lobby POR ."+nombreUsuario   );
>>>>>>> expulsar jugador
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el lobby: {ex.Message}");
            }
        }
        bool isJoiningLobby = false;
        private async void BtnUnirseLobby_Click(object sender, RoutedEventArgs e)
        {
            btnCrearLobby.Visibility = Visibility.Collapsed;
            txtCodigoLobby.Visibility = Visibility.Visible;
           
            if (isJoiningLobby) return;  

            isJoiningLobby = true;
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            int numeroFotoPerfil = sesion.NumeroFotoPerfil;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = username, NumeroFotoPerfil = numeroFotoPerfil };

            txtCodigoLobby.Visibility = Visibility.Visible;
            string codigoLobby = txtCodigoLobby.Text.Trim();

            if (string.IsNullOrEmpty(codigoLobby))
            {
                MessageBox.Show("Por favor, ingresa un código de lobby válido.");
                isJoiningLobby = false;
                return;
            }

            try
            {
                Console.WriteLine("antes de unirse a sala");
                // Intentar unirse a un lobby existente con el código proporcionado
                cliente.UnirseSalaEspera(codigoLobby, lb);
                Console.WriteLine("después de unirse a sala");
                stackPanelOpciones.Visibility = Visibility.Collapsed;
                stackPanelJugadores.Visibility = Visibility.Visible;
                gridChat.Visibility = Visibility.Visible;
                btnSalir.Visibility = Visibility.Visible;
                btnIniciarPartida.Visibility = Visibility.Visible;
                // Actualizar el código de la sala actual
                codigoSalaEsperaActual = codigoLobby;
                txtCodigoLobby.Visibility= Visibility.Collapsed;
                MessageBox.Show($"Te has unido al lobby con código: {codigoLobby}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show($"Error al unirse al lobby: {ex.Message}");
            }
            finally
            {
                isJoiningLobby = false;  // Resetear la bandera
            }

        }
        private async void BtnEnviarMensaje(object sender, RoutedEventArgs e)
        {
            string message = tbxMessage.Text.Trim();

            if (string.IsNullOrEmpty(message))
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

                await Task.Run(() => cliente.MandarMensaje(message)); 
                tbxMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }

        public void UnirseSalaEsperaExistente(string lobbyCode, string username)
        {
            JugadorSalaEspera newPlayer = new JugadorSalaEspera { NombreUsuario = username };
            cliente.UnirseSalaEspera(lobbyCode, newPlayer);
        }

      
        private void SetupClient()
        {
            
            InstanceContext instanceContext = new InstanceContext(this);
            cliente = new LobbyManagerClient(instanceContext);
        }

        public void UnirseSalaEsperaComoHost(string lobbyCode)
        {
            try
            {
                cliente.UnirSalaEsperaComoAnfitrion(lobbyCode);
                MessageBox.Show($"Intentando unirse al lobby {lobbyCode} como anfitrión...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al unirse al lobby: {ex.Message}");
            }
        }

        public void SalirSalaEspera(string lobbyCode, string username)
        {
            try
            {
                cliente.SalirSalaEspera(lobbyCode, username);
                MessageBox.Show($"{username} ha salido del lobby {lobbyCode}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al salir del lobby: {ex.Message}");
            }
        }

        public void NotificarSalaEsperaCreada(string lobbyCode)
        {
            codigoSalaEsperaActual = lobbyCode;
            MessageBox.Show($"Lobby creado con el código: {lobbyCode}");
        }

        public async Task ExpulsarJugadorSalaEsperaAsync(string username)
        {
            InstanceContext context = new InstanceContext(this);
            LobbyManagerClient lobbyManagerClientExpulse = new LobbyManagerClient(context);

            try
            {
                await lobbyManagerClientExpulse.ExpulsarJugadorSalaEsperaAsync(codigoSalaEsperaActual, username);
                SingletonSesion sesion = SingletonSesion.Instancia;
                if (sesion.NombreUsuario == username)
                {
                    NotificarExpulsadoSalaEspera();
                }

            }
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
    
        }

        public void NotificarJugadorSeUnioSalaEspera(JugadorSalaEspera jugador, int numOfPlayersInLobby)
        {
            Dispatcher.Invoke(() =>
            {
                JugadoresEnSala.Add(jugador);
                MessageBox.Show($"{jugador.NombreUsuario} se unió. Jugadores en el lobby: {numOfPlayersInLobby}");
                numeroJugadoresSalaEspera = ++numOfPlayersInLobby;
    });
        }
        public void NotificarJugadorSalioSalaEspera(string username)
        {
            Dispatcher.Invoke(() =>
            {
                var jugador = JugadoresEnSala.FirstOrDefault(j => j.NombreUsuario == username);
                if (jugador != null)
                {
                    JugadoresEnSala.Remove(jugador); // Eliminar al jugador de la colección
                    numeroJugadoresSalaEspera--;

                }
            });
<<<<<<< HEAD
        }






        public void NotificarIniciarPartida(JugadorPartida[] jugadores)
=======
        } 
        public void NotificarIniciarPartida(JugadorSalaEspera[] jugadores)
>>>>>>> expulsar jugador
        {
            Console.WriteLine("Jugadores recibidos en el cliente:");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.NombreUsuario}");
                Console.WriteLine($"Foto de perfil: {jugador.NumeroFotoPerfil}");
            }

            XAMLTablero tablero = new XAMLTablero(jugadores.ToList(), codigoSalaEsperaActual);
            this.NavigationService.Navigate(tablero);
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

        public void RecibirMensaje(string username, string message)
        {
            Dispatcher.Invoke(() =>
            {
               
                stackPanelMessages.Children.Add(new TextBlock { Text = $"{username}: {message}" });
            });
        }
        public void NotificarJugadoresEnSalaEspera(string lobbyCode, JugadorSalaEspera[] jugadores)
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

        public void NotificarPuedeIniciarPartida(bool canStart)
        {
            Dispatcher.Invoke(() => {
                btnIniciarPartida.Visibility = canStart ? Visibility.Visible : Visibility.Collapsed;
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
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al iniciar la partida: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Código de lobby no disponible. Asegúrate de haber creado o unido a un lobby.");
            }
        }

        public void NotificarAnfritionJugadorSalioSalaEspera()
        {
     
            NavigationService.Navigate(new XAMLSalaEspera());
        }

        private async void BtnSalirLobby(object sender, RoutedEventArgs e)
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;

            if (string.IsNullOrEmpty(codigoSalaEsperaActual))
            {
                MessageBox.Show("No estás en un lobby.");
                return;
            }

            try
            {
                await Task.Run(() => cliente.SalirSalaEspera(codigoSalaEsperaActual, username));
                
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error al salir del lobby: {ex.Message}");
            }
        }
    }
}
