using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using trofeoCazador.Servidor;


namespace trofeoCazador
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
            /* try
             {
                 // Configurar el cliente del servicio
                 ChannelFactory<IGestionUsuario> channelFactory = new ChannelFactory<IGestionUsuario>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8733/Design_Time_Addresses/trofeoDelCazadorServicio/gestionUsuarioServicio/"));
            
            IGestionUsuario client = channelFactory.CreateChannel();

                // Crear un nuevo objeto Cuentaa para enviar al servicio
                Cuentaa nuevaCuenta = new Cuentaa
                {
                    nombre = "Juan",
                    apellido = "Pérez",
                    correo = "juan.perez@example.com",
                    contrasenia = "password123",
                    fechaRegistro = DateTime.Now
                };

                // Crear un nuevo objeto Jugadorr para enviar al servicio
                Jugadorr nuevoJugador = new Jugadorr
                {
                    usuario = "juanperez",
                    fechaNacimiento = new DateTime(1995, 5, 20),
                    partidasJugadas = null, // Dejarlo nulo si no se han jugado partidas aún
                    partidasGanadas = null, // Dejarlo nulo si no se han ganado partidas aún
                    fechaRegistro = DateTime.Now,
                    CuentaLlaveForanea = nuevaCuenta
                };

                // Llamar al método agregarJugador del servicio
                int idJugadorCreado = client.agregarJugador(nuevoJugador);

                MessageBox.Show($"Jugador creado con éxito. ID: {idJugadorCreado}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cerrar el canal
                channelFactory.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el jugador: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }
    }

        
    }

