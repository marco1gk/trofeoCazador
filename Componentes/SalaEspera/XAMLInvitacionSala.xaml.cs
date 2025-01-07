using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.Amigos;

namespace trofeoCazador.Componentes.SalaEspera
{
    public partial class XAMLInvitacionSala : Window
    {
        private readonly Window ventanaPrincipal;
        private readonly string salaEsperaCodigo;

        public XAMLInvitacionSala(string codigoSalaEspera)
        {
            InitializeComponent();
            ventanaPrincipal = Application.Current.MainWindow;
            salaEsperaCodigo = codigoSalaEspera;
            ConfigurarVentanaEmergente();
        }

        private void ConfigurarVentanaEmergente()
        {
            this.Owner = ventanaPrincipal;
            tbCodigoSalaEspera.Text = salaEsperaCodigo;
            EstablecerMedidasVentana();
            EstablecerCentroVentana();
        }

        private void EstablecerMedidasVentana()
        {
            this.Width = ventanaPrincipal.Width;
            this.Height = ventanaPrincipal.Height;
        }

        private void EstablecerCentroVentana()
        {
            double centroX = ventanaPrincipal.Left + (ventanaPrincipal.Width - this.Width) / 2;
            double centroY = ventanaPrincipal.Top + (ventanaPrincipal.Height - this.Height) / 2;
            this.Left = centroX;
            this.Top = centroY;
        }

        private void ImgCerrar_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BtnInvitarPorCorreo_Click(object sender, RoutedEventArgs e)
        {
            bordeInvitarPorCodigo.Visibility = Visibility.Collapsed;
            bordeInvitarPorCorrreo.Visibility = Visibility.Visible;
        }

        private void BtnCopiarCodigo_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(salaEsperaCodigo);
        }

        private void BtnEnviarInvitacion_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCorreo())
            {
                ServicioDelJuego.GestorInvitacionClient invitacionGestionCliente = new ServicioDelJuego.GestorInvitacionClient();

                try
                {
                    invitacionGestionCliente.EnviarInvitacionCorreo(salaEsperaCodigo, tbCorreoAmigo.Text.Trim());
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                }
                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (CommunicationException ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                    ManejadorExcepciones.ManejarComponenteFatalExcepcion(ex);
                }
            }
        }

        private void TbCorreoAmigo_ObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tbCorreoAmigo.Text == (string)tbCorreoAmigo.Tag)
            {
                tbCorreoAmigo.Text = string.Empty;
                tbCorreoAmigo.Foreground = Brushes.Black;
            }
        }

        private void TbCorreoAmigo_PerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCorreoAmigo.Text))
            {
                tbCorreoAmigo.Text = (string)tbCorreoAmigo.Tag;
            }
        }

        private bool ValidarCorreo()
        {
            bool valido = true;
            string errorTextBoxEstilo = "errorTextBoxEstilo";

            if (!UtilidadesDeValidacion.EsCorreoValido(tbCorreoAmigo.Text) || tbCorreoAmigo.Text.Equals(tbCorreoAmigo.Tag))
            {
                tbCorreoAmigo.Style = (Style)FindResource(errorTextBoxEstilo);
                valido = false;
            }

            return valido;
        }

    }
}
