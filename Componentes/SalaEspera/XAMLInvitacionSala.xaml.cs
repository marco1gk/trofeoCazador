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
    /// <summary>
    /// Lógica de interacción para XAMLInvitacionSala.xaml
    /// </summary>
    public partial class XAMLInvitacionSala : Window
    {
        private readonly Window _ventanaPrincipal;
        private readonly string _salaEsperaCodigo;

        public XAMLInvitacionSala(string codigoSalaEspera)
        {
            InitializeComponent();

            _ventanaPrincipal = Application.Current.MainWindow;
            _salaEsperaCodigo = codigoSalaEspera;

            ConfigurarVentanaEmergente();
        }

        private void ConfigurarVentanaEmergente()
        {
            this.Owner = _ventanaPrincipal;
            tbkcodigoSalaEspera.Text = _salaEsperaCodigo;
            EstablecerMedidasVentana();
            EstablecerCentroVentana();
        }

        private void EstablecerMedidasVentana()
        {
            this.Width = _ventanaPrincipal.Width;
            this.Height = _ventanaPrincipal.Height;
        }

        private void EstablecerCentroVentana()
        {
            double centroX = _ventanaPrincipal.Left + (_ventanaPrincipal.Width - this.Width) / 2;
            double centroY = _ventanaPrincipal.Top + (_ventanaPrincipal.Height - this.Height) / 2;
            this.Left = centroX;
            this.Top = centroY;
        }

        private void ImgCerrar_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BtnInvitarPorCorreo_Click(object sender, RoutedEventArgs e)
        {
            borderInviteByCode.Visibility = Visibility.Collapsed;
            borderInviteByEmail.Visibility = Visibility.Visible;
        }

        private void BtnCopiarCodigo_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_salaEsperaCodigo);
        }

        private void BtnEnviarInvitacion_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCorreo())
            {
                ServicioDelJuego.GestorInvitacionClient invitacionGestionCliente = new ServicioDelJuego.GestorInvitacionClient();

                try
                {
                    invitacionGestionCliente.EnviarInvitacionCorreo(_salaEsperaCodigo, tbxCorreoAmigo.Text.Trim());
                }
                catch (EndpointNotFoundException ex)
                {
                    VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                    ManejadorExcepciones.HandleComponentErrorException(ex);
                }
                catch (TimeoutException ex)
                {
                    VentanasEmergentes.CrearVentanaMensajeTimeOut();
                    ManejadorExcepciones.HandleComponentErrorException(ex);
                }
                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                }
                catch (CommunicationException ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
                    ManejadorExcepciones.HandleComponentErrorException(ex);
                }
                catch (Exception ex)
                {
                    VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                    ManejadorExcepciones.HandleComponentFatalException(ex);
                }
            }
        }

        private void TbxCorreoAmigo_ObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tbxCorreoAmigo.Text == (string)tbxCorreoAmigo.Tag)
            {
                tbxCorreoAmigo.Text = string.Empty;
                tbxCorreoAmigo.Foreground = Brushes.Black;
            }
        }

        private void TbxCorreoAmigo_PerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxCorreoAmigo.Text))
            {
                tbxCorreoAmigo.Text = (string)tbxCorreoAmigo.Tag;
            }
        }

        private bool ValidarCorreo()
        {
            EstablecerEstilosPorDefecto();
            bool valido = true;
            string errorTextBoxEstilo = "errorTextBoxEstilo";

            if (!UtilidadesDeValidacion.EsCorreoValido(tbxCorreoAmigo.Text) || tbxCorreoAmigo.Text.Equals(tbxCorreoAmigo.Tag))
            {
                tbxCorreoAmigo.Style = (Style)FindResource(errorTextBoxEstilo);
                valido = false;
            }

            return valido;
        }

        private void EstablecerEstilosPorDefecto()
        {
            string estiloNormalTextBox = "estiloNormalTextBox";
            tbxCorreoAmigo.Style = (Style)FindResource(estiloNormalTextBox);
        }
    }
}
