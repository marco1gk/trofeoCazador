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

            // Lista de imágenes de perfil
            List<ImagenPerfil> imagenesPerfil = new List<ImagenPerfil>
    {
        new ImagenPerfil { Id = 1, NombreImagen = "Perfil 1", RutaImagen = "/Recursos/FotosPerfil/abeja.jpg" },
        new ImagenPerfil { Id = 2, NombreImagen = "Perfil 2", RutaImagen = "/Recursos/FotosPerfil/cazador.jpg" },
        
    };

            // Asignar la lista como fuente de datos para el ComboBox
            cbImagenPerfil.ItemsSource = imagenesPerfil;
        }
        private void CbImagenPerfil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener la imagen seleccionada
            ImagenPerfil seleccionada = (ImagenPerfil)cbImagenPerfil.SelectedItem;
            if (seleccionada != null)
            {
                int idImagenSeleccionada = seleccionada.Id;
                // Aquí puedes usar el ID para asociarlo con el jugador
                MessageBox.Show($"Has seleccionado la imagen con ID: {idImagenSeleccionada}");
            }
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

            // Capturar la imagen seleccionada por el usuario
            int numeroImagenPerfil = cbImagenPerfil.SelectedIndex + 1; // Asumiendo que las imágenes están numeradas del 1 al 3

            // Abrir la ventana para validar el código
            JugadorDataContract jugador = new JugadorDataContract
            {
                NombreUsuario = tbUsuario.Text,
                NumeroFotoPerfil = numeroImagenPerfil, // Guardar la imagen seleccionada
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

            // Validar que se haya seleccionado una imagen de perfil
            if (cbImagenPerfil.SelectedItem == null)
            {
                errores.AppendLine("Por favor, selecciona una imagen de perfil.");
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

    public class ImagenPerfil
    {
        public int Id { get; set; } // ID de la imagen
        public string NombreImagen { get; set; } // Nombre o etiqueta de la imagen
        public string RutaImagen { get; set; } // Ruta a la imagen
    }

}
