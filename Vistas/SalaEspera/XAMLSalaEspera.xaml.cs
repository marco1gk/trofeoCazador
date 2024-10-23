using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            SingletonSesion sesion = SingletonSesion.Instancia;
            InitializeComponent();
            SetupClient();
            LobbyPlayer lb = new LobbyPlayer();
            lb.Username = sesion.NombreUsuario;
             
            client.CreateLobby(lb);
            
            // Ejemplo: Llamar al servicio para unirte a un lobby al cargar la página
            //JoinLobbyAsHost("59ZY34"); // Código de ejemplo de un lobby
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            client.sendMessage("a ver");

        }
        // Configuración del cliente WCF
        private void SetupClient()
        {
            // Crear el contexto para manejar los callbacks
            InstanceContext instanceContext = new InstanceContext(this);

            // Crear el cliente WCF usando el contexto
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

        public void NotifyPlayersInLobby(string lobbyCode, LobbyPlayer[] lobbyPlayers)
        {
            throw new NotImplementedException();
        }
    }
}
