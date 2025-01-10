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

            if (!UtilidadesDeValidacion.EsCorreoValido(correo))
            {
                VentanasEmergentes.CrearVentanaEmergente("", Properties.Resources.lbDescripcionCorreoIncorrecto);
                tbCorreo.Clear();
                return;
            }
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
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }            
        }
    }
}
