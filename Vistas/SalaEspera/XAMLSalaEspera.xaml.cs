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

namespace trofeoCazador.Vistas.SalaEspera
{
    /// <summary>
    /// Lógica de interacción para XAMLSalaEspera.xaml
    /// </summary>
    public partial class XAMLSalaEspera : Page, ILobbyManagerCallback
    {
        private LobbyManagerClient client;
        private string codigoLobbyActual;

        public XAMLSalaEspera()
        {
            InitializeComponent();
            SetupClient();  
          //  UnirseOcrearLobby();  
        }
        private async void BtnCrearLobby_Click(object sender, RoutedEventArgs e)
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            LobbyPlayer lb = new LobbyPlayer { Username = username };

            try
            {
                // Crear un lobby nuevo
                client.CreateLobby(lb);
                MessageBox.Show("Se ha creado un nuevo lobby.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el lobby: {ex.Message}");
            }
        }

        private async void BtnUnirseLobby_Click(object sender, RoutedEventArgs e)
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            LobbyPlayer lb = new LobbyPlayer { Username = username };

            // Mostrar el campo de texto para que el usuario introduzca el código del lobby
            txtCodigoLobby.Visibility = Visibility.Visible;

            // Obtener el código ingresado
            string codigoLobby = txtCodigoLobby.Text.Trim();

            if (string.IsNullOrEmpty(codigoLobby))
            {
                MessageBox.Show("Por favor, ingresa un código de lobby válido.");
                return;
            }

            try
            {
                // Intentar unirse a un lobby existente con el código proporcionado
                client.JoinLobby(codigoLobby, lb);
                MessageBox.Show($"Unido al lobby con código: {codigoLobby}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al unirse al lobby: {ex.Message}");
            }
        }
        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        private async void UnirseOcrearLobby()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            LobbyPlayer lb = new LobbyPlayer { Username = username };

            try
            {
                // Intentar unirse a un lobby existente
                string existingLobbyCode = await BuscarLobbyExistente();
                //if (!string.IsNullOrEmpty(existingLobbyCode))
              //  {
                   // client.JoinLobby(existingLobbyCode, lb);  
                 //   MessageBox.Show($"Unido al lobby con código: {existingLobbyCode}");
               // }
                //else
                //{
                    client.CreateLobby(lb);
                    MessageBox.Show("No se encontró un lobby, creando uno nuevo.");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task<string> BuscarLobbyExistente()
        {
            // Simulación de una consulta al servicio para buscar lobbies existentes
            return await Task.Run(() =>
            {
                try
                {
                    // Llama al servicio para obtener el código del primer lobby disponible
                    return client.BuscarLobbyDisponible();
                }
                catch (Exception)
                {
                    return null; 
                }
            });
        }

        private async void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = tbxMessage.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Por favor, escribe un mensaje antes de enviar.");
                return;
            }

            try
            {
                if (client == null)
                {
                    SetupClient(); 
                }

                await Task.Run(() => client.SendMessage(message)); 
                tbxMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje: {ex.Message}");
            }
        }

        public void JoinExistingLobby(string lobbyCode, string username)
        {
            LobbyPlayer newPlayer = new LobbyPlayer { Username = username };
            client.JoinLobby(lobbyCode, newPlayer);
        }

      
        private void SetupClient()
        {
            
            InstanceContext instanceContext = new InstanceContext(this);
            client = new LobbyManagerClient(instanceContext);
        }

        public void JoinLobbyAsHost(string lobbyCode)
        {
            try
            {
                client.JoinLobbyAsHost(lobbyCode);
                MessageBox.Show($"Intentando unirse al lobby {lobbyCode} como anfitrión...");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al unirse al lobby: {ex.Message}");
            }
        }

        public void ExitLobby(string lobbyCode, string username)
        {
            try
            {
                client.ExitLobby(lobbyCode, username);
                MessageBox.Show($"{username} ha salido del lobby {lobbyCode}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al salir del lobby: {ex.Message}");
            }
        }

        
        public void NotifyLobbyCreated(string lobbyCode)
        {
            codigoLobbyActual = lobbyCode;
            MessageBox.Show($"Lobby creado con el código: {lobbyCode}");
            btnIniciarPartida.Visibility = Visibility.Visible;
        }

        public void NotifyPlayersInLobby(string lobbyCode, List<LobbyPlayer> lobbyPlayers)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Jugadores en el lobby {lobbyCode}: {string.Join(", ", lobbyPlayers.Select(p => p.Username))}");
            });
        }

        public void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby)
        {
            MessageBox.Show($"{lobbyPlayer.Username} se unió. Jugadores en el lobby: {numOfPlayersInLobby}");
        }

        public void NotifyPlayerLeftLobby(string username)
        {
            MessageBox.Show($"{username} ha salido del lobby.");
        }

        public void NotifyHostPlayerLeftLobby()
        {
            MessageBox.Show("El anfitrión ha dejado el lobby.");
        }

        public void NotifyStartMatch(LobbyPlayer[] jugadores)
        {
            Console.WriteLine("Jugadores recibidos en el cliente:");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.Username}");
            }

            XAMLTablero tablero = new XAMLTablero();
            tablero.MostrarJugadores(jugadores.ToList());
            this.NavigationService.Navigate(tablero);
        }

        public void NotifyLobbyIsFull()
        {
            MessageBox.Show("El lobby está lleno.");
        }

        public void NotifyLobbyDoesNotExist()
        {
            MessageBox.Show("El lobby no existe.");
        }

        public void NotifyExpulsedFromLobby()
        {
            MessageBox.Show("Has sido expulsado del lobby.");
        }

        public void ReceiveMessage(string username, string message)
        {
            Dispatcher.Invoke(() =>
            {
               
                stackPanelMessages.Children.Add(new TextBlock { Text = $"{username}: {message}" });
            });
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            
            Dispatcher.Invoke(() =>
            {
                stackPanelMessages.Children.Clear();

                foreach (var player in lobbyPlayers)
                {
                    stackPanelMessages.Children.Add(new TextBlock { Text = $"{player.Username} está en el lobby {lobbyCode}" });
                }
            });
        }

        public void NotifyCanStartGame(bool canStart)
        {
            // Se hace visible el boton una vez que haya al menos 2 jugadores
            Dispatcher.Invoke(() => {
                btnIniciarPartida.Visibility = canStart ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private async void BtnClicIniciarPartida(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(codigoLobbyActual))
            {
                try
                {
                    await Task.Run(() => client.StartGame(codigoLobbyActual));
                    
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
    }
}
