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
using System.Windows.Navigation;
using System.Windows.Shapes;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.VentanasReutilizables
{
    public partial class VentanaCorreoJugador : Window
    {
        public VentanaCorreoJugador()
        {
            InitializeComponent();
        }
        private void BtnClicIngresarCorreo_Click(object sender, RoutedEventArgs e)
        {
            string correo = tbCorreo.Text.Trim();

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            try
            {
                string codigoRecuperacion = proxy.EnviarCodigoConfirmacion(correo);
                if (!string.IsNullOrEmpty(codigoRecuperacion))
                {
                    ValidarCodigoRegistro validarCodigo = new ValidarCodigoRegistro(null, correo, codigoRecuperacion);
                    validarCodigo.Show();
                    this.Close();
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloCorreoNoReconocido, Properties.Resources.lbDescripcionCorreoIncorrecto);

                }
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
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
                ManejadorExcepciones.ManejarErrorExcepcion(ex, this);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, this);
            }

            
        }
    }
}
