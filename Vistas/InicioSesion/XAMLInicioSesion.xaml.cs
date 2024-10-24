using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.InicioSesion
{
    /// <summary>
    /// Interaction logic for XAMLInicioSesion.xaml
    /// </summary>
    public partial class XAMLInicioSesion : Page
    {
        private const string FUENTE_SECUNDARIA = "Inter";

        public XAMLInicioSesion()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Acción del botón si es necesario
        }

        private void BtnRegistrarCuenta(object sender, RoutedEventArgs e)
        {

            // Navegar a la página de registro de usuario
            if (this.NavigationService == null)
            {
                return; // Salimos si no hay un servicio de navegación
            }
            this.NavigationService.Navigate(new Uri("Vistas/RegistroUsuario/XAMLRegistroUsuario.xaml", UriKind.Relative));
        }

        private void BtnIniciarSesion(object sender, RoutedEventArgs e)
        {
            // Ocultar mensaje de error antes de validar
            lbCredencialesIncorrectas.Visibility = Visibility.Hidden;

            // Validamos los campos de entrada
            if (!ValidarCampos())
            {
                // Si la validación falla, mostramos el mensaje de error
                lbCredencialesIncorrectas.Visibility = Visibility.Visible;
                return; // Salimos del método si los campos no son válidos
            }

            // Aquí va la lógica para validar las credenciales
            string contraseña = ContrasenaPasswordBox.Password;
            string usuario = UsuarioTextBox.Text;

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();

            // Asegúrate de obtener el jugador de la validación
            JugadorDataContract jugador = proxy.ValidarInicioSesion(usuario, contraseña);

            if (jugador != null)
            {
                // Asignar los datos del jugador al SingletonSesion
                SingletonSesion sesion = SingletonSesion.Instancia;
                sesion.JugadorId = jugador.JugadorId;
                sesion.NombreUsuario = jugador.NombreUsuario;
                sesion.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
                sesion.Correo = jugador.Correo;
              
                // Navegar al menú inicial (vestíbulo)
                this.NavigationService.Navigate(new Uri("Vistas/Menu/XAMLMenu.xaml", UriKind.Relative));
            }
            else
            {
                // Manejar el fallo del inicio de sesión
                lbCredencialesIncorrectas.Visibility = Visibility.Visible; // Muestra el mensaje de error
                Console.WriteLine("El inicio de sesión falló o los datos no fueron recuperados correctamente.");
            }
        }


        private bool ValidarCampos()
        {
            bool esValido = true;
            Console.WriteLine("Validar campos antes de entrar a condicion" + esValido);

            // Validar nombre de usuario
            if (!UtilidadesDeValidacion.EsCorreoValido(UsuarioTextBox.Text) || UsuarioTextBox.Text.Equals(UsuarioTextBox.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos correo" + esValido);
            }

            // Validar contraseña
            if (!UtilidadesDeValidacion.EsContrasenaValida(ContrasenaPasswordBox.Password) || ContrasenaPasswordBox.Password.Equals(ContrasenaPasswordBox.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos contraseña" + esValido);
            }

            return esValido;
        }

        // Evento que se activa cuando el campo de texto para el nombre de usuario recibe el foco.
        private void TbxUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsuarioTextBox.Text == (string)UsuarioTextBox.Tag)
            {
                UsuarioTextBox.Text = string.Empty;
                UsuarioTextBox.Foreground = Brushes.Black;
                UsuarioTextBox.FontFamily = new FontFamily(FUENTE_SECUNDARIA);
                UsuarioTextBox.FontWeight = FontWeights.Bold;
            }
        }

        // Evento que se activa cuando el campo de texto para el nombre de usuario pierde el foco.
        private void TbxUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsuarioTextBox.Text))
            {
                UsuarioTextBox.Text = (string)UsuarioTextBox.Tag;
            }
        }

        // Evento que se activa cuando el campo de entrada de la contraseña recibe el foco.
        private void PwBxPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ContrasenaPasswordBox.Password == (string)ContrasenaPasswordBox.Tag)
            {
                ContrasenaPasswordBox.Password = string.Empty;
                ContrasenaPasswordBox.Foreground = Brushes.Black;
            }
        }

        // Evento que se activa cuando el campo de entrada de la contraseña pierde el foco.
        private void PwBxPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContrasenaPasswordBox.Password))
            {
                ContrasenaPasswordBox.Password = (string)ContrasenaPasswordBox.Tag;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Acción del botón si es necesario
        }
    }
}
