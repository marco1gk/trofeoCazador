using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using trofeoCazador.ServicioDelJuego;
using System.ServiceModel;
using System.Windows.Input;

namespace trofeoCazador.Vistas.SalaEspera
{
    /// <summary>
    /// Lógica de interacción para XAMLSalaEspera.xaml
    /// </summary>
    public partial class XAMLSalaEspera : Page, ILobbyManagerCallback
    {
        private LobbyManagerClient client;

        public XAMLSalaEspera()
        {
            InitializeComponent();
            SetupClient();  
            UnirseOcrearLobby();  
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
                if (!string.IsNullOrEmpty(existingLobbyCode))
                {
                    client.JoinLobby(existingLobbyCode, lb);  
                    MessageBox.Show($"Unido al lobby con código: {existingLobbyCode}");
                }
                else
                {
                    client.CreateLobby(lb);
                    MessageBox.Show("No se encontró un lobby, creando uno nuevo.");
                }
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

                await Task.Run(() => client.sendMessage(message)); 
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
            MessageBox.Show($"Lobby creado con el código: {lobbyCode}");
        }

        public void NotifyPlayersInLobby(string lobbyCode, List<LobbyPlayer> lobbyPlayers)
        {
            MessageBox.Show($"Jugadores en el lobby {lobbyCode}: {string.Join(", ", lobbyPlayers.Select(p => p.Username))}");
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

        public void NotifyStartOfMatch()
        {
            MessageBox.Show("¡La partida ha comenzado!");
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

    }
}
