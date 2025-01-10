using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarUsuarioNombre : Page
    {
        private JugadorDataContract jugador;
        public EditarUsuarioNombre()
        {
            InitializeComponent();
            try
            {
                jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
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
            CargarUsuarioJugador();
        }

        private void CargarUsuarioJugador()
        {
            if (jugador != null)
            {
                NombreUsuarioActualLabel.Content = jugador.NombreUsuario;
            }
        }

        private void BtnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoNombreUsuario = NuevoNombreUsuarioTextBox.Text.Trim();
            int longitudValidaNombreUsuario = 50;

            if (!ValidarNombreUsuario(nuevoNombreUsuario, longitudValidaNombreUsuario))
            {
                return;
            }

            try
            {
                SingletonSesion sesion = SingletonSesion.Instancia;
                GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
                bool resultado = proxy.EditarNombreUsuario(sesion.JugadorId, nuevoNombreUsuario);

                if (resultado)
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbNombreNuevoExitoso);
                    this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbProblemasActualizarNombre);
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

        private bool ValidarNombreUsuario(string nombreUsuario, int longitudMaxima)
        {
            if (!Metodos.ValidarEntradaVacia(nombreUsuario))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbIngresarUsuario);
                return false;
            }

            if (!Metodos.ValidarLongitudDeEntrada(nombreUsuario, longitudMaxima))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCaracteresNombreUsuario);
                return false;
            }

            if (!Metodos.ValidarEntradaIgual(jugador.NombreUsuario, nombreUsuario))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbNombreUsuarioRepetido);
                return false;
            }

            return true;
        }

    }
}
