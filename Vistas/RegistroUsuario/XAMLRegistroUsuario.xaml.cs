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
using System.ServiceModel;

namespace trofeoCazador.Vistas.RegistroUsuario
{
    public partial class XAMLRegistroUsuario : Page
    {
        public XAMLRegistroUsuario()
        {
            InitializeComponent();

            List<ImagenPerfil> imagenesPerfil = new List<ImagenPerfil>
            {
                new ImagenPerfil { Id = 1, NombreImagen = "", RutaImagen = "/Recursos/FotosPerfil/abeja.jpg" },
                new ImagenPerfil { Id = 2, NombreImagen = "", RutaImagen = "/Recursos/FotosPerfil/cazador.jpg" },
        
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

        private void BtnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            string errores = ValidarCampos();
            if (!string.IsNullOrEmpty(errores))
            {
                MostrarVentanaErroresValidacion();
                return;
            }

            string codigoEnviado = ProcesarCuenta();

            if (string.IsNullOrEmpty(codigoEnviado))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbDescripcionProblemasCodigo);
                return;
            }

            bool? resultadoValidacion = ValidarCodigo(codigoEnviado);

            if (resultadoValidacion == true)
            {
                CrearCuentaExitosa();
            }
        }

        private void MostrarVentanaErroresValidacion()
        {
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbTituloErroresValidacion);
        }

        //Se decidio que este metodo regrese null debido a que solo tiene un sentido, si es nulo es porque no existe
        private string ProcesarCuenta()
        {
            try
            {
                using (GestionCuentaServicioClient proxy = new GestionCuentaServicioClient())
                {
                    if (proxy.ExisteCorreo(tbCorreo.Text.Trim()))
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloCorreoEligido, Properties.Resources.lbTituloErroresValidacion);
                        return null;
                    }

                    if (proxy.ExisteNombreUsuario(tbUsuario.Text.Trim()))
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbDescripcionNombreUsado);
                        return null;
                    }

                    return proxy.EnviarCodigoConfirmacion(tbCorreo.Text);
                }
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                Console.WriteLine($"Error de conexión: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                Console.WriteLine($"Timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            return null;
        }

        private bool? ValidarCodigo(string codigoEnviado)
        {
            int numeroImagenPerfil = cbImagenPerfil.SelectedIndex + 1;

            JugadorDataContract jugador = new JugadorDataContract
            {
                NombreUsuario = tbUsuario.Text,
                NumeroFotoPerfil = numeroImagenPerfil,
                ContraseniaHash = PbContraseña.Password,
                Correo = tbCorreo.Text
            };

            ValidarCodigoRegistro ventanaValidacion = new ValidarCodigoRegistro(jugador, null, codigoEnviado);
            return ventanaValidacion.ShowDialog();
        }
        private void CrearCuentaExitosa()
        {
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCuentaCreada);

            try
            {
                NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
            }
            catch (UriFormatException uriEx)
            {
                Console.WriteLine($"Error en el formato de la URI: {uriEx.Message}");
            }
            catch (InvalidOperationException invOpEx)
            {
                Console.WriteLine($"Operación de navegación no válida: {invOpEx.Message}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error inesperado al navegar: {ex.Message}");
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
