using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
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
using trofeoCazador.VentanasReutilizables;

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
            // Validar los campos ingresados
            string errores = ValidateFields();

            if (!string.IsNullOrEmpty(errores))
            {
                MessageBox.Show(errores, "Errores en la validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener ejecución si hay errores en los datos ingresados
            }

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();

            // Verificar si el correo ya está registrado
            if (proxy.ExisteCorreo(tbCorreo.Text.Trim()))
            {
                MessageBox.Show("Este correo ya está registrado. Por favor, elige otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución si el correo está registrado
            }

            // Verificar si el nombre de usuario ya está registrado
            if (proxy.ExisteNombreUsuario(tbUsuario.Text.Trim()))
            {
                MessageBox.Show("Este nombre de usuario ya está en uso. Por favor, elige otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución si el nombre de usuario está registrado
            }

            // Enviar el código de verificación por correo
            string codigoEnviado = proxy.EnviarCodigoConfirmacion(tbCorreo.Text);

            if (string.IsNullOrEmpty(codigoEnviado))
            {
                MessageBox.Show("No se pudo enviar el código de verificación. Intenta de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Detener la ejecución si hay un error enviando el código
            }

            // Abrir la ventana para validar el código
            JugadorDataContract jugador = new JugadorDataContract
            {
                NombreUsuario = tbUsuario.Text,
                NumeroFotoPerfil = 1,
                ContraseniaHash = PbContraseña.Password,
                Correo = tbCorreo.Text
            };

            ValidarCodigoRegistro ventanaValidacion = new ValidarCodigoRegistro(jugador, codigoEnviado);
            ventanaValidacion.ShowDialog(); // Mostrar la ventana de validación
        }





        public string ValidateFields()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            StringBuilder errores = new StringBuilder();
           

            ValidadPropiedadesContraseña();

            if (!UtilidadesDeValidacion.EsCorreoValido(tbCorreo.Text.Trim()))
            {
              //  tbCorreo.Style = (Style)FindResource(errorTextBoxStyle);
                errores.AppendLine("El correo electrónico no es válido.");
            }

            if (!UtilidadesDeValidacion.EsNombreUsuarioValido(tbUsuario.Text.Trim()))
            {
                //tbUsuario.Style = (Style)FindResource(errorTextBoxStyle);
                errores.AppendLine("El nombre de usuario no es válido.");
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(PbContraseña.Password.Trim()))
            {
                //PbContraseña.Style = (Style)FindResource(errorPasswordBoxStyle);
                errores.AppendLine("La contraseña no es válida.");
            }

            return errores.ToString();
        }



        private void ValidadPropiedadesContraseña()
        {

            lbRequerimientoLongitud.Foreground = Brushes.Red;
            lbCaracterEspecial.Foreground = Brushes.Red;
            lbRequerimientoMayuscula.Foreground = Brushes.Red;
            lbRequerimientoMinuscula.Foreground = Brushes.Red;
            lbRequerimientoNumero.Foreground = Brushes.Red;
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
