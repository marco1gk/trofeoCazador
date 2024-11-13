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
using trofeoCazador.Recursos;


namespace trofeoCazador.Vistas.Perfil
{
    public partial class XAMLPerfil : Page
    {
        
        public XAMLPerfil()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            InitializeComponent();
            CargarPerfil(sesion.JugadorId);


        }

        private void CargarPerfil(int IdJugador)
        {
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = proxy.ObtenerJugador(IdJugador);
            if (jugador != null)
            {
                UsuarioLabel.Content = jugador.NombreUsuario;
                CorreoLabel.Content = jugador.Correo;
            
                string rutaImagen = ObtenerRutaImagenPerfil(jugador.NumeroFotoPerfil);
                imgPerfil.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
            }
            
        }
        private string ObtenerRutaImagenPerfil(int numeroFotoPerfil)
        {
            switch (numeroFotoPerfil)
            {
                case 1:
                    return "/Recursos/FotosPerfil/abeja.jpg";
                case 2:
                    return "/Recursos/FotosPerfil/cazador.jpg";
                default:
                    return "/Recursos/FotosPerfil/cazador.jpg"; 
            }
        }

        private void BtnClicEditarUsuario(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarUsuarioNombre.xaml", UriKind.Relative));
        }

        private void BtnClicEditarContrasenia(object sender, RoutedEventArgs e)
        {
            NavigationWindow navigationWindow = new NavigationWindow();
            navigationWindow.Content = new EditarContrasenia(null);
            navigationWindow.Show();
        }

        private void BtnClicEditarCorreo(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarCorreo.xaml", UriKind.Relative));
        }

        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}