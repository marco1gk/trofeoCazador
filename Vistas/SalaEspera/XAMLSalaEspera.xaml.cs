using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using trofeoCazador.ServicioDelJuego;
using System.ServiceModel;

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
            SetupClient();  // Asegúrate de configurar el cliente WCF al inicializar
            UnirseOcrearLobby();  // Intentar unirse a un lobby o crear uno nuevo
        }

        // Método que busca si hay un lobby existente o crea uno nuevo si no hay ninguno
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
                    client.JoinLobby(existingLobbyCode, lb);  // Unirse al lobby existente
                    MessageBox.Show($"Unido al lobby con código: {existingLobbyCode}");
                }
                else
                {
                    // Si no hay un lobby existente, crear uno nuevo
                    client.CreateLobby(lb);
                    MessageBox.Show("No se encontró un lobby, creando uno nuevo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Método para buscar un lobby existente
        private async Task<string> BuscarLobbyExistente()
        {
            // Simulación de una consulta al servicio para buscar lobbies existentes
            return await Task.Run(() =>
            {
                try
                {
                    // Llama al servicio para obtener el código del primer lobby disponible
                    return client.BuscarLobbyDisponible(); // Debes implementar este método en el servicio
                }
                catch (Exception)
                {
                    return null; // Devuelve null si ocurre algún error o no hay lobby
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
                    SetupClient(); // Asegúrate de que el cliente esté inicializado
                }

                await Task.Run(() => client.sendMessage(message)); // Enviar el mensaje de manera asíncrona
                tbxMessage.Clear(); // Limpiar el TextBox después de enviar el mensaje
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

        // Configuración del cliente WCF
        private void SetupClient()
        {
            // Crear el contexto para manejar los callbacks
            InstanceContext instanceContext = new InstanceContext(this);
            client = new LobbyManagerClient(instanceContext);
        }

        // Método para unirse al lobby como host
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

        // Método para salir de un lobby
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

        // Métodos del callback para manejar las notificaciones del servicio
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
                // Asegúrate de que estás actualizando la UI en el hilo correcto
                stackPanelMessages.Children.Add(new TextBlock { Text = $"{username}: {message}" });
            });
        }

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            // Actualizamos la UI para mostrar los jugadores en el lobby
            Dispatcher.Invoke(() =>
            {
                // Limpiamos el stackPanelMessages o algún control similar que esté mostrando los jugadores
                stackPanelMessages.Children.Clear();

                // Mostramos los jugadores en el lobby
                foreach (var player in lobbyPlayers)
                {
                    stackPanelMessages.Children.Add(new TextBlock { Text = $"{player.Username} está en el lobby {lobbyCode}" });
                }
            });
        }

    }
}
