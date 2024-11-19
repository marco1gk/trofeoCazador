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
        private LobbyManagerClient client;
        private string codigoSalaEsperaActual;
        public ObservableCollection<JugadorSalaEspera> JugadoresEnSala { get; set; }
       
        public XAMLSalaEspera()
        {
            InitializeComponent();
            JugadoresEnSala = new ObservableCollection<JugadorSalaEspera>();
            DataContext = this; // Establecer el contexto de datos para la página
            SetupClient();
            
                                //  UnirseOcrearLobby();  
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
            int numeroFotoPerfil = sesion.NumeroFotoPerfil;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = nombreUsuario, NumeroFotoPerfil = numeroFotoPerfil};

            try
            {
                // Crear un lobby nuevo
                client.CrearSalaEspera(lb);
                btnIniciarPartida.Visibility = Visibility.Visible;
                MessageBox.Show("Se ha creado un nuevo lobby.");
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
           
            if (isJoiningLobby) return;  // Prevenir recursión

            isJoiningLobby = true;
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            int numeroFotoPerfil = sesion.NumeroFotoPerfil;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = username, NumeroFotoPerfil = numeroFotoPerfil };

            // Mostrar el campo de texto para que el usuario introduzca el código del lobby
            txtCodigoLobby.Visibility = Visibility.Visible;

            // Obtener el código ingresado
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
                client.UnirseSalaEspera(codigoLobby, lb);
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
        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        private async void UnirseOcrearLobby()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            string username = sesion.NombreUsuario;
            JugadorSalaEspera lb = new JugadorSalaEspera { NombreUsuario = username };

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
                    client.CrearSalaEspera(lb);
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
                    return client.BuscarSalaEsperaDisponible();
                }
                catch (Exception)
                {
                    return null; 
                }
            });
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
                if (client == null)
                {
                    SetupClient(); 
                }

                await Task.Run(() => client.MandarMensaje(message)); 
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
            client.UnirseSalaEspera(lobbyCode, newPlayer);
        }

      
        private void SetupClient()
        {
            
            InstanceContext instanceContext = new InstanceContext(this);
            client = new LobbyManagerClient(instanceContext);
        }

        public void UnirseSalaEsperaComoHost(string lobbyCode)
        {
            try
            {
                client.UnirSalaEsperaComoAnfitrion(lobbyCode);
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
                client.SalirSalaEspera(lobbyCode, username);
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





        public void NotificarJugadorSeUnioSalaEspera(JugadorSalaEspera jugador, int numOfPlayersInLobby)
        {
            Dispatcher.Invoke(() =>
            {
                JugadoresEnSala.Add(jugador);
                MessageBox.Show($"{jugador.NombreUsuario} se unió. Jugadores en el lobby: {numOfPlayersInLobby}");
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
                }
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
                // Sincronizar la colección `JugadoresEnSala` con los datos recibidos
                JugadoresEnSala.Clear(); // Limpiar la colección antes de agregar nuevos jugadores
                foreach (var jugador in jugadores)
                {
                    JugadoresEnSala.Add(jugador); // Agregar jugadores a la colección
                }
            });
        }




        public void NotificarPuedeIniciarPartida(bool canStart)
        {
            // Se hace visible el boton una vez que haya al menos 2 jugadores
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
                    await Task.Run(() => client.IniciarPartida(codigoSalaEsperaActual));
                    
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
                await Task.Run(() => client.SalirSalaEspera(codigoSalaEsperaActual, username));

                // Limpiar datos locales
                codigoSalaEsperaActual = null;

                // Limpiar jugadores en la lista
                Dispatcher.Invoke(() => JugadoresEnSala.Clear());

                // Navegar a la página anterior
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
