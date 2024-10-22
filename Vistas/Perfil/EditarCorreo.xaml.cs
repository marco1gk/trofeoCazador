using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    public partial class XAMLEditarCorreo : Page
    {
        public XAMLEditarCorreo()
        {
            InitializeComponent();
            CargarCorreoActual();
        }

        private void CargarCorreoActual()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = proxy.ObtenerJugador(sesion.JugadorId);
            if (jugador != null)
            {
                CorreoActualLabel.Content = jugador.Correo;
            }
        }

        private void btnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoCorreo = NuevoCorreoTextBox.Text.Trim();

            try
            {
                // Obtener el correo actual del jugador
                SingletonSesion sesion = SingletonSesion.Instancia;
                GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
                JugadorDataContract jugador = proxy.ObtenerJugador(sesion.JugadorId);

                if (jugador != null)
                {
                    string correoActual = jugador.Correo;
                    string codigoVerificacion = proxy.EnviarCodigoConfirmacion(correoActual);
                    if (!string.IsNullOrEmpty(codigoVerificacion))
                    {
                        // Guardar el nuevo correo en la sesión o en algún lugar
                        SingletonSesion.Instancia.NuevoCorreo = nuevoCorreo; // Almacenar el nuevo correo
                        SingletonSesion.Instancia.CodigoVerificacion = codigoVerificacion;

                        // Navegar a la página de verificación
                        this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarCorreoCodigo.xaml", UriKind.Relative));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo: " + ex.Message);
            }
        }

    }
}
