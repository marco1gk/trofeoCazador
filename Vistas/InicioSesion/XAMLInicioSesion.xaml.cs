using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.VentanasReutilizables;
using trofeoCazador.Vistas.SalaEspera;
using trofeoCazador.Vistas.Amigos;
using System.ServiceModel;
using trofeoCazador.Vistas.Menu;

namespace trofeoCazador.Vistas.InicioSesion
{
    public partial class XAMLInicioSesion : Page
    {
        private const string FUENTE_SECUNDARIA = "Inter";
        private void IdiomaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbIdioma.SelectedItem is ComboBoxItem item)
            {
                string codigoCultura = item.Tag.ToString();
                App.CambiarIdioma(codigoCultura);
                this.NavigationService.Refresh();
            }
        }


        private void BtnCorreo_Click(object sender, RoutedEventArgs e)
        {
            VentanaCorreoJugador ventanaCorreo = new VentanaCorreoJugador();
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
            try
            {
                lbCredencialesIncorrectas.Visibility = Visibility.Hidden;

                if (!ValidarCampos())
                {
                    lbCredencialesIncorrectas.Visibility = Visibility.Visible;
                    return;
                }

                string contraseña = tpContraseña.Password;
                string usuario = tbUsuario.Text;

                GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
                JugadorDataContract jugador = proxy.ValidarInicioSesion(usuario, contraseña);

                if (jugador != null)
                {
                    SingletonSesion sesion = SingletonSesion.Instancia;
                    sesion.JugadorId = jugador.JugadorId;
                    sesion.NombreUsuario = jugador.NombreUsuario;
                    sesion.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
                    sesion.Correo = jugador.Correo;

                    sesion.EstaActivo = true; 

                    XAMLAmigos paginaAmigos = new XAMLAmigos();
                    paginaAmigos.MostrarComoUsuarioActivo(); 

                    XAMLAmigos amigosPage = new XAMLAmigos();

                    this.NavigationService.Navigate(new XAMLMenu(amigosPage));
                }
                else
                {
                    lbCredencialesIncorrectas.Visibility = Visibility.Visible;
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbCredencialesIncorrectas, Properties.Resources.lbDescripcionCredencialesIncorrectas);
                }
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                Console.WriteLine($"Excepción de conexión: {ex.Message}");
                if (this.NavigationService != null)
                {
                    this.NavigationService.RemoveBackEntry(); 
                }
                return; 
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (CommunicationException ex)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }

        }


        private bool ValidarCampos()
        {
            bool esValido = true;
            Console.WriteLine("Validar campos antes de entrar a condicion" + esValido);

            if (!UtilidadesDeValidacion.EsCorreoValido(tbUsuario.Text) || tbUsuario.Text.Equals(tbUsuario.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos correo" + esValido);
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(tpContraseña.Password) || tpContraseña.Password.Equals(tpContraseña.Tag))
            {
                esValido = false;
                Console.WriteLine("Validar campos contraseña" + esValido);
            }

            return esValido;
        }
        private void TbxUsuarioObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tbUsuario.Text == (string)tbUsuario.Tag)
            {
                tbUsuario.Text = string.Empty;
                tbUsuario.Foreground = Brushes.Black;
                tbUsuario.FontFamily = new FontFamily(FUENTE_SECUNDARIA);
                tbUsuario.FontWeight = FontWeights.Bold;
            }
        }
        private void TbxUsuarioPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsuario.Text))
            {
                tbUsuario.Text = (string)tbUsuario.Tag;
            }
        }

        private void PbContraseñaObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tpContraseña.Password == (string)tpContraseña.Tag)
            {
                tpContraseña.Password = string.Empty;
                tpContraseña.Foreground = Brushes.Black;
            }
        }

        private void PbContraseñaPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tpContraseña.Password))
            {
                tpContraseña.Password = (string)tpContraseña.Tag;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
           
        }

        private void lbClicRecuperarContrasena(object sender, MouseButtonEventArgs e)
        {
            VentanaCorreoJugador ventanaCorreo = new VentanaCorreoJugador();
            ventanaCorreo.Show();
        }
    }
}
