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
using trofeoCazador.Servidor;


namespace trofeoCazador
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Servidor.GestionUsuarioClient proxy = new Servidor.GestionUsuarioClient();

                Jugador jugador = new Jugador
                {
                    usuario = "PruebaJugador", 
                    fechaNacimiento = new DateTime(1990, 1, 1),
                    partidasJugadas = 10, 
                    partidasGanadas = 5, 
                    fechaRegistro = DateTime.Now 
                };

               
                int jugadorId = proxy.agregarJugador(jugador);

                if (jugadorId > 0)
                {
                    MessageBox.Show("Jugador creado con éxito. ID: " + jugadorId);
                }
                else
                {
                    MessageBox.Show("Error al crear el jugador.");
                }

             
                proxy.Close();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine($"Error al agregar el jugador y la cuenta: {ex.Message}");
                // Imprimir la cadena de conexión para depuración (solo para propósitos de desarrollo)
                Console.WriteLine("Cadena de conexión utilizada: " + "name=ModeloBDContainer");
               // Código de error
            }


        }
    }
}
