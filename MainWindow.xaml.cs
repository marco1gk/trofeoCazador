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
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            // Crear instancia del cliente que se comunica con el servicio
            Servidor.GestionUsuarioClient proxy = new Servidor.GestionUsuarioClient();

            // Crear un nuevo jugador con valores fijos para pruebas
            Jugador jugador = new Jugador
            {
                usuario = "PruebaJugador", // Nombre de usuario fijo
                fechaNacimiento = new DateTime(1990, 1, 1), // Fecha de nacimiento fija
                partidasJugadas = 10, // Asignar un número de partidas jugadas fijo
                partidasGanadas = 5, // Asignar un número de partidas ganadas fijo
                fechaRegistro = DateTime.Now // Fecha de registro actual
            };

            // Llamar al servicio para agregar el jugador
            int jugadorId = proxy.agregarJugador(jugador);

            if (jugadorId > 0)
            {
                MessageBox.Show("Jugador creado con éxito. ID: " + jugadorId);
            }
            else
            {
                MessageBox.Show("Error al crear el jugador.");
            }

            // Cerrar el cliente del servicio
            proxy.Close();
        }
    }
}
