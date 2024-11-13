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
    public partial class XAMLRegistroUsuario : Page
    {
        public XAMLRegistroUsuario()
        {
            InitializeComponent();

            List<ImagenPerfil> imagenesPerfil = new List<ImagenPerfil>
            {
                new ImagenPerfil { Id = 1, NombreImagen = "Perfil 1", RutaImagen = "/Recursos/FotosPerfil/abeja.jpg" },
                new ImagenPerfil { Id = 2, NombreImagen = "Perfil 2", RutaImagen = "/Recursos/FotosPerfil/cazador.jpg" },
        
            };

            cbImagenPerfil.ItemsSource = imagenesPerfil;
        }
        private void CbImagenPerfil_Seleccion(object sender, SelectionChangedEventArgs e)
        {
            ImagenPerfil seleccionada = (ImagenPerfil)cbImagenPerfil.SelectedItem;
            if (seleccionada != null)
            {
                int idImagenSeleccionada = seleccionada.Id;
                Console.WriteLine("Id de foto seleccionada"+idImagenSeleccionada+"]");
               
            }
        }

        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
                NavigationService.GoBack();
        }

        private void BtnCrearCuenta(object sender, RoutedEventArgs e)
        {
            string errores = ValidarCampos();

            if (!string.IsNullOrEmpty(errores))
            {
                MessageBox.Show(errores, "Errores en la validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();

            if (proxy.ExisteCorreo(tbCorreo.Text.Trim()))
            {
                MessageBox.Show("Este correo ya está registrado. Por favor, elige otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            if (proxy.ExisteNombreUsuario(tbUsuario.Text.Trim()))
            {
                MessageBox.Show("Este nombre de usuario ya está en uso. Por favor, elige otro.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            string codigoEnviado = proxy.EnviarCodigoConfirmacion(tbCorreo.Text);

            if (string.IsNullOrEmpty(codigoEnviado))
            {
                MessageBox.Show("No se pudo enviar el código de verificación. Intenta de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int numeroImagenPerfil = cbImagenPerfil.SelectedIndex + 1; 

            JugadorDataContract jugador = new JugadorDataContract
            {
                NombreUsuario = tbUsuario.Text,
                NumeroFotoPerfil = numeroImagenPerfil, 
                ContraseniaHash = PbContraseña.Password,
                Correo = tbCorreo.Text
            };
            ValidarCodigoRegistro ventanaValidacion = new ValidarCodigoRegistro(jugador, null, codigoEnviado);
            bool? resultadoValidacion = ventanaValidacion.ShowDialog();

            if (resultadoValidacion == true)
            {
                MessageBox.Show("Cuenta creada exitosamente. Ahora puedes iniciar sesión.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
            }
        }


        public string ValidarCampos()
        {
            StringBuilder errores = new StringBuilder();
            ValidarPropiedadesContraseña();

            if (!UtilidadesDeValidacion.EsCorreoValido(tbCorreo.Text.Trim()))
            {
                errores.AppendLine("El correo electrónico no es válido.");
            }

            if (!UtilidadesDeValidacion.EsNombreUsuarioValido(tbUsuario.Text.Trim()))
            {
                errores.AppendLine("El nombre de usuario no es válido.");
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(PbContraseña.Password.Trim()))
            {
                errores.AppendLine("La contraseña no es válida.");
            }
            if (cbImagenPerfil.SelectedItem == null)
            {
                errores.AppendLine("Por favor, selecciona una imagen de perfil.");
            }

            return errores.ToString();
        }
        private void ValidarPropiedadesContraseña()
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
