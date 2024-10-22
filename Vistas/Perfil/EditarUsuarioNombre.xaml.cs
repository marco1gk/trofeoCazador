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

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarUsuarioNombre : Page
    {
        public EditarUsuarioNombre()
        {
            InitializeComponent();
            CargarUsuarioJugador(ObtenerIdJugador());
        }

        private int ObtenerIdJugador()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            return sesion.JugadorId;
        }

        private void CargarUsuarioJugador(int idJugador)
        {
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = new JugadorDataContract();

            if (jugador != null)
            {
                NombreUsuarioActualLabel.Content = jugador.NombreUsuario;
            }
        }

        private void btnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoNombre = NuevoNombreUsuarioTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                MessageBox.Show("Por favor, ingresa un nuevo nombre de usuario.");
                return;
            }

            try
            {
                GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
                bool resultado = proxy.EditarNombreUsuario(ObtenerIdJugador(), nuevoNombre);

                if (resultado)
                {
                    MessageBox.Show("Nombre de usuario actualizado con éxito.");
                    this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Hubo un problema al actualizar el nombre de usuario.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servicio: " + ex.Message);
            }
        }
    }
}
