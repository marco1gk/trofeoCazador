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
using trofeoCazador.Vistas;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Amigos
{

    public partial class XAMLAmigos : Page
    {
        private readonly GestorUsuariosConectadosClient cliente;
        public XAMLAmigos()
        {
            InitializeComponent();
            InstanceContext contexto = new InstanceContext(this);
            cliente = new GestorUsuariosConectadosClient(contexto);
            MostrarComoUsuarioActivo();
            MostrarDatos();
            
        }

        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        public void DesregistrarUsuarioActual()
        {
            if (cliente != null)
            {
                cliente.DesregistrarUsuarioDeUsuariosEnLinea(SingletonSesion.Instancia.NombreUsuario);
                Console.WriteLine($"Usuario {SingletonSesion.Instancia.NombreUsuario} desregistrado.");
            }
        }
        public void MostrarComoUsuarioActivo()
        {

            try
            {
                cliente.RegistrarUsuarioAUsuariosConectados(SingletonSesion.Instancia.JugadorId, SingletonSesion.Instancia.NombreUsuario);

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
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
            
        }

        private void MostrarDatos()
        {
            try
            {
                CargarAmigosJugador();

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