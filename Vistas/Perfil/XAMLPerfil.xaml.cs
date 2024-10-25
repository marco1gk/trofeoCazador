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

                // Cargar la imagen de perfil según el número de foto del jugador
                string rutaImagen = ObtenerRutaImagenPerfil(jugador.NumeroFotoPerfil);
                imgPerfil.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
            }
            
        }
        private string ObtenerRutaImagenPerfil(int numeroFotoPerfil)
        {
            // Definir las rutas de las imágenes según el número de foto de perfil
            switch (numeroFotoPerfil)
            {
                case 1:
                    return "/Recursos/FotosPerfil/abeja.jpg";
                case 2:
                    return "/Recursos/FotosPerfil/cazador.jpg";
                // Agregar más casos según las imágenes disponibles
                default:
                    return "/Recursos/FotosPerfil/cazador.jpg"; // Imagen por defecto
            }
        }

        private void btnClicEditarUsuario(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarUsuarioNombre.xaml", UriKind.Relative));
        }

        private void btnClicEditarContrasenia(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarContrasenia.xaml", UriKind.Relative));
        }

        private void btnClicEditarCorreo(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarCorreo.xaml", UriKind.Relative));
        }
    }
}