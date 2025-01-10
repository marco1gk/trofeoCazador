using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
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
using trofeoCazador.Vistas.Perfil;
using trofeoCazador.Recursos;
using trofeoCazador.Utilidades;
using System.ServiceModel;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarCorreoCodigo : Page
    {
        public EditarCorreoCodigo()
        {
            InitializeComponent();
        }
        GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
        private readonly SingletonSesion sesion = SingletonSesion.Instancia;

        private void BtnClicEnviarCodigo(object sender, RoutedEventArgs e)
        {

            string codigoIngresado = CodigoTextBox.Text.Trim();
            string codigoEnviado = sesion.CodigoVerificacion;
            int longitudMaximaCodigo = 6;

            if (!Metodos.ValidarEntradaVacia(codigoIngresado))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbIngresaCodigo);
                return;
            }

            if(!Metodos.ValidarLongitudDeEntrada(codigoIngresado, longitudMaximaCodigo))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCodigoDigitos);
                return;
            }

            try
            {
                if (proxy.ValidarCodigo(codigoIngresado, codigoEnviado))
                {
                    if (proxy.EditarCorreo(sesion.JugadorId, sesion.NuevoCorreo))
                    {
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloExito, Properties.Resources.lbCorreoCambiado);
                        this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                    }
                    else
                        VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbProblemasCorreo);
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbErrorInesperadoCorreo);
                }
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            
        }

        private void BtnClicSolicitarNuevoCodigo(object sender, RoutedEventArgs e)
        {
            try
            {
                proxy.EnviarCodigoConfirmacion(sesion.Correo);
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }   
        }
    }
}
