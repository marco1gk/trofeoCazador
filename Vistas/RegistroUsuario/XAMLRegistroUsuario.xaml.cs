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

namespace trofeoCazador.Vistas.RegistroUsuario
{
    /// <summary>
    /// Interaction logic for XAMLRegistroUsuario.xaml
    /// </summary>
    public partial class XAMLRegistroUsuario : Page
    {
        public XAMLRegistroUsuario()
        {
            InitializeComponent();
        }

        
        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
                NavigationService.GoBack();
        }

        private void BtnCrearCuenta(object sender, RoutedEventArgs e)
        {
            string nombreUsuario= tbUsuario.Text;
            string correo= tbCorreo.Text;   
            string contrasenia = pbContraseña.Password;

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = new JugadorDataContract();
   
            jugador.NombreUsuario = nombreUsuario;
            jugador.NumeroFotoPerfil = 1;
            jugador.ContraseniaHash = contrasenia;
            jugador.Correo = correo;
            proxy.AgregarJugador(jugador);
        }
    }
}
