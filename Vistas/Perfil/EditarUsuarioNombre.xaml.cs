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
        private JugadorDataContract Jugador;
        public EditarUsuarioNombre()
        {
            InitializeComponent();
            Jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
            CargarUsuarioJugador();
        }

        private void CargarUsuarioJugador()
        {
            if (Jugador != null)
            {
                NombreUsuarioActualLabel.Content = Jugador.NombreUsuario;
            }
        }

        private void BtnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoNombreUsuario = NuevoNombreUsuarioTextBox.Text.Trim();
            int longitudValidaNombreUsuario = 50;

            if (!Metodos.ValidarEntradaVacia(nuevoNombreUsuario))
            {
                Metodos.MostrarMensaje("Por favor, debe ingresar un nuevo nombre de usuario.");
                return;
            }

            if (!Metodos.ValidarLongitudDeEntrada(nuevoNombreUsuario, longitudValidaNombreUsuario))
            {
                Metodos.MostrarMensaje("El nombre de usuario no puede tener más de 50 caracteres.");
                return;
            }

            if (!Metodos.ValidarEntradaIgual(Jugador.NombreUsuario, nuevoNombreUsuario))
            {
                Metodos.MostrarMensaje("El nuevo nombre de usuario es igual al actual.");
                return;
            }

            try
            {

                SingletonSesion sesion = SingletonSesion.Instancia;
            GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
            bool resultado = proxy.EditarNombreUsuario(sesion.JugadorId, nuevoNombreUsuario);

                if (resultado)
                {
                    Metodos.MostrarMensaje("Nombre de usuario actualizado con éxito.");
                    this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                }
                else
                {
                    Metodos.MostrarMensaje("Hubo un problema al actualizar el nombre de usuario.");
                }
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
          //      ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
                //       ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
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
                // ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
                //ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }
    }
}
