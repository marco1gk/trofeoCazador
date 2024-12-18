using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using trofeoCazador.Recursos;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class XAMLEditarCorreo : Page
    {
        private JugadorDataContract jugador;
        public XAMLEditarCorreo()
        {
            InitializeComponent();
            jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
            CargarCorreoActual();
        }
        private void CargarCorreoActual()
        {
            if (jugador != null)
            {
                CorreoActualLabel.Content = jugador.Correo;
            }
        }
        private void BtnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoCorreo = NuevoCorreoTextBox.Text.Trim();
            int longitudMaximaCorreo = 100;
            try
            {
                if (!Metodos.ValidarEntradaVacia(nuevoCorreo))
                {
                    Metodos.MostrarMensaje("El correo no puede estar vacío.");
                    return;
                }

                if (!Metodos.ValidarEntradaIgual(jugador.Correo, nuevoCorreo))
                {
                    Metodos.MostrarMensaje("El nuevo correo no puede ser el mismo que el actual.");
                    return;
                }

                if (!Metodos.ValidarLongitudDeEntrada(nuevoCorreo, longitudMaximaCorreo))
                {
                    Metodos.MostrarMensaje("El correo no puede exceder los 50 caracteres.");
                    return;
                }

                GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();

                if (jugador != null)
                {
                    string correoActual = jugador.Correo;
                    string codigoVerificacion = proxy.EnviarCodigoConfirmacion(correoActual);
                    if (!string.IsNullOrEmpty(codigoVerificacion))
                    {
                        SingletonSesion.Instancia.NuevoCorreo = nuevoCorreo;
                        SingletonSesion.Instancia.CodigoVerificacion = codigoVerificacion;
                        this.NavigationService.Navigate(new Uri("Vistas/Perfil/EditarCorreoCodigo.xaml", UriKind.Relative));
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
                NavigationService.Navigate(new XAMLInicioSesion());
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
    }
}
