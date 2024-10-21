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


namespace trofeoCazador.Vistas.Perfil
{
    public partial class XAMLPerfil : Page
    {
        /*public XAMLPerfil()
        {
            InitializeComponent();
            CargarDatosJugador(1); // Reemplaza 1 con el ID del jugador que desees cargar
        }

        private void CargarDatosJugador(int idJugador)
        {
            try
            {
                // Crear una instancia del cliente del servicio
                var cliente = new gestionUsuarioServicio();

                // Obtener los datos del jugador
                Jugadorr jugador = cliente.obtenerJugador(idJugador);

                if (jugador != null)
                {
                    // Asignar los valores a los Label
                    Label usuarioLabel = (Label)this.FindName("UsuarioLabel");
                    Label emailLabel = (Label)this.FindName("EmailLabel");
                    Label globalNameLabel = (Label)this.FindName("GlobalNameLabel");

                    usuarioLabel.Content = jugador.usuario;
                    emailLabel.Content = jugador.correo; // Asumiendo que tienes la cuenta asociada
                    globalNameLabel.Content = jugador.; // Asumiendo que quieres mostrar el nombre
                }
                else
                {
                    MessageBox.Show("Jugador no encontrado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}");
            }
        }*/
    }
}