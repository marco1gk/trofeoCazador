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
using System.Xml.Linq;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

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
            string contrasenia = PbContraseña.Password;

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = new JugadorDataContract();
   
            jugador.NombreUsuario = nombreUsuario;
            jugador.NumeroFotoPerfil = 1;
            jugador.ContraseniaHash = contrasenia;
            jugador.Correo = correo;
           proxy.AgregarJugador(jugador);
        }

        /*
        private void EstablecerEstilosPredeterminados()
        {
            string estiloNormalTextBox = "NormalTextBoxStyle";
            string estiloNormalContraseña = "NormalPasswordBoxStyle";

            tbCorreo.Style = (Style)FindResource(estiloNormalTextBox);
            tbUsuario.Style = (Style)FindResource(estiloNormalTextBox);
            PbContraseña.Style = (Style)FindResource(estiloNormalContraseña);

            lbExistentEmail.Visibility = Visibility.Hidden;
            lbExistentUsername.Visibility = Visibility.Hidden;
            lbEmailError.Visibility = Visibility.Hidden;

            lbPasswordLengthInstruction.Foreground = Brushes.Red;
            lbPasswordSymbolInstruction.Foreground = Brushes.Red;
            lbPasswordCapitalLetterInstruction.Foreground = Brushes.Red;
            lbPasswordLowerLetterInstruction.Foreground = Brushes.Red;
            lbPasswordNumberInstruction.Foreground = Brushes.Red;

            ImgNameErrorDetails.Visibility = Visibility.Hidden;
            ImgLastNameErrorDetails.Visibility = Visibility.Hidden;
            ImgEmailErrorDetails.Visibility = Visibility.Hidden;
            ImgUsernameErrorDetails.Visibility = Visibility.Hidden;
            ImgPasswordErrorDetails.Visibility = Visibility.Hidden;
        }
        */


        private void ValidadPropiedadesContraseña()
        {
            if (PbContraseña.Password.Trim().Length >= 12)   
            {
                lbRequerimientoLongitud.Foreground = Brushes.GreenYellow;
            }

            if (UtilidadesDeValidacion.EsSimboloValido(PbContraseña.Password))
            {
                lbCaracterEspecial.Foreground = Brushes.GreenYellow;
            }

            if (UtilidadesDeValidacion.EsMayusculaValida(PbContraseña.Password))
            {
                lbRequerimientoMayuscula.Foreground = Brushes.GreenYellow;
            }

            if (UtilidadesDeValidacion.EsMinusculaValida(PbContraseña.Password))
            {
                lbRequerimientoMinuscula.Foreground = Brushes.GreenYellow;
            }

            if (UtilidadesDeValidacion.EsNumeroValido(PbContraseña.Password))
            {
                lbRequerimientoNumero.Foreground = Brushes.GreenYellow;
            }
        }



    }
}
