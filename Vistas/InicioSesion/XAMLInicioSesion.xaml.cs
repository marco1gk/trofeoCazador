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
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.InicioSesion
{
    /// <summary>
    /// Interaction logic for XAMLInicioSesion.xaml
    /// </summary>
    public partial class XAMLInicioSesion : Page
    {
        public XAMLInicioSesion()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRegistrarCuenta(object sender, RoutedEventArgs e)
        {
            // Navegar a la página de registro de usuario
            if (this.NavigationService == null)
            {
                return;//veo  veo
            }
            this.NavigationService.Navigate(new Uri("Vistas/RegistroUsuario/XAMLRegistroUsuario.xaml", UriKind.Relative));

        }



        private void BtnIniciarSesion(object sender, RoutedEventArgs e)
        {
            string contraseña = ContrasenaPasswordBox.Password;
            string usuario = UsuarioTextBox.Text;

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();

            // Validar las credenciales del jugador
            JugadorDataContract jugador = proxy.ValidarInicioSesion(usuario, contraseña);

            if (jugador != null)
            {
                // Asignar los datos del jugador al SingletonSesion
                SingletonSesion sesion = SingletonSesion.Instancia;
                sesion.JugadorId = jugador.JugadorId;
                sesion.NombreUsuario = jugador.NombreUsuario;
                sesion.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
                sesion.Correo = jugador.Correo;
               // proxy.EnviarCodigoConfirmacion("vaomarco052@gmail.com");

                Console.WriteLine("El correo es: " + jugador.Correo);

                // Navegar al menú inicial (vestíbulo)
                this.NavigationService.Navigate(new Uri("Vistas/Menu/XAMLMenu.xaml", UriKind.Relative));
              //  proxy.EditarCorreo(sesion.JugadorId, "esteEsElNuevoCorreo");
               // proxy.EditarNombreUsuario(sesion.JugadorId, "esteEsElNuevoNombreDeUsuario");
            }
            else
            {
                // Manejar el fallo del inicio de sesión
                Console.WriteLine("El inicio de sesión falló o los datos no fueron recuperados correctamente.");
            }
        }

        private void EstablecerEstilosPorDefecto()
        {
            string estiloTextBoxNormal = "NormalTextBoxStyle";

            UsuarioTextBox.Style = (Style)FindResource(estiloTextBoxNormal);
            ContrasenaPasswordBox.Style = (Style)FindResource(estiloTextBoxNormal);
            lbCredencialesIncorrectas.Visibility = Visibility.Hidden;
        }

        private bool ValidarCampos()
        {
            EstablecerEstilosPorDefecto();
            bool esValido = true;
            string estiloErrorTextBox = "ErrorTextBoxStyle";

            if (UtilidadesDeValidacion.EsNombreUsuarioValido(UsuarioTextBox.Text) || UsuarioTextBox.Text.Equals(UsuarioTextBox.Tag))
            {
                UsuarioTextBox.Style = (Style)FindResource(estiloErrorTextBox);
                esValido = false;
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(ContrasenaPasswordBox.Password) || ContrasenaPasswordBox.Password.Equals(ContrasenaPasswordBox.Tag))
            {
                ContrasenaPasswordBox.Style = (Style)FindResource(estiloErrorTextBox);
                esValido = false;
            }

            return esValido;
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
