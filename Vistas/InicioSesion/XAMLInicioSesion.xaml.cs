using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.VentanasReutilizables;

namespace trofeoCazador.Vistas.InicioSesion
{
 
    public partial class XAMLInicioSesion : Page
    {
        private const string FUENTE_SECUNDARIA = "Inter";

        public XAMLInicioSesion()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CorreoJugador ventanaCorreo = new CorreoJugador();
            ventanaCorreo.Show();

        }

        private void BtnRegistrarCuenta(object sender, RoutedEventArgs e)
        {

            if (this.NavigationService == null)
            {
                return; 
            }
            this.NavigationService.Navigate(new Uri("Vistas/RegistroUsuario/XAMLRegistroUsuario.xaml", UriKind.Relative));
        }

        private void BtnIniciarSesion(object sender, RoutedEventArgs e)
        {
            lbCredencialesIncorrectas.Visibility = Visibility.Hidden;

            if (!ValidarCampos())
            {
                lbCredencialesIncorrectas.Visibility = Visibility.Visible;
                return; 
            }

            string contraseña = ContrasenaPasswordBox.Password;
            string usuario = UsuarioTextBox.Text;

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = proxy.ValidarInicioSesion(usuario, contraseña);
            

            if (jugador != null)
            {
                SingletonSesion sesion = SingletonSesion.Instancia;
                sesion.JugadorId = jugador.JugadorId;
                sesion.NombreUsuario = jugador.NombreUsuario;
                sesion.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
                sesion.Correo = jugador.Correo;

                this.NavigationService.Navigate(new Uri("Vistas/Menu/XAMLMenu.xaml", UriKind.Relative));
            }
            else
            {
                lbCredencialesIncorrectas.Visibility = Visibility.Visible;
                Console.WriteLine("El inicio de sesión falló o los datos no fueron recuperados correctamente.");
            }
        }


        private bool ValidarCampos()
        {
            bool esValido = true;
            Console.WriteLine("Validar campos antes de entrar a condicion" + esValido);

            if (!UtilidadesDeValidacion.EsCorreoValido(UsuarioTextBox.Text) || UsuarioTextBox.Text.Equals(UsuarioTextBox.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos correo" + esValido);
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(ContrasenaPasswordBox.Password) || ContrasenaPasswordBox.Password.Equals(ContrasenaPasswordBox.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos contraseña" + esValido);
            }

            return esValido;
        }
        private void TbxUsuarioObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (UsuarioTextBox.Text == (string)UsuarioTextBox.Tag)
            {
                UsuarioTextBox.Text = string.Empty;
                UsuarioTextBox.Foreground = Brushes.Black;
                UsuarioTextBox.FontFamily = new FontFamily(FUENTE_SECUNDARIA);
                UsuarioTextBox.FontWeight = FontWeights.Bold;
            }
        }
        private void TbxUsuarioPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsuarioTextBox.Text))
            {
                UsuarioTextBox.Text = (string)UsuarioTextBox.Tag;
            }
        }

        private void PbContraseñaObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (ContrasenaPasswordBox.Password == (string)ContrasenaPasswordBox.Tag)
            {
                ContrasenaPasswordBox.Password = string.Empty;
                ContrasenaPasswordBox.Foreground = Brushes.Black;
            }
        }

        private void PbContraseñaPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContrasenaPasswordBox.Password))
            {
                ContrasenaPasswordBox.Password = (string)ContrasenaPasswordBox.Tag;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void lbClicRecuperarContrasena(object sender, MouseButtonEventArgs e)
        {
            CorreoJugador ventanaCorreo = new CorreoJugador();
            ventanaCorreo.Show();
        }
    }
}
