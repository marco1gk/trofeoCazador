using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLAmigos : Page, IGestorDeSolicitudesDeAmistadCallback
    {
        public SingletonSesion sesion = SingletonSesion.Instancia;

        private void SuscribirUsuarioAlDiccionarioDeAmigosEnLínea()
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);
           
            try
            {
                gestorSolicitudesAmistadCliente.AgregarADiccionarioAmistadesEnLinea(sesion.NombreUsuario);
                
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
            catch (FaultException )
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


        private void BtnEnviarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnviarSolicitud();
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


        private void EnviarSolicitud()
        {
            lbErrorNombreDeUsuarioSolicitudAmistad.Visibility = Visibility.Visible;

            string nombreDeUsuarioJugadorSolicitado = tbNombreDeUsuarioEnviarSolicitud.Text.Trim();
            int idJugador = sesion.JugadorId;
            string mensaje;
            if (ValidarEnviarSolicitud(idJugador, nombreDeUsuarioJugadorSolicitado))
            {
                AgregarSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
                EnviarSolicitudAmistad(nombreDeUsuarioJugadorSolicitado);
                mensaje = Properties.Resources.lbInvitacionEnviada + " " + nombreDeUsuarioJugadorSolicitado;
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloSolicitudAmistad,mensaje);
                tbNombreDeUsuarioEnviarSolicitud.Text = string.Empty;
            }
            else
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloSolicitudAmistad,Properties.Resources.lbProblemasInvitacion);

            }
        }

        private static void AgregarSolicitudAmistad(int idJugador, string nombreDeUsuarioJugadorSolicitado)
        {
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();

            gestorAmistadCliente.AgregarSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
        }


        private void EnviarSolicitudAmistad(string nombreDeUsuarioJugadorSolicitado)
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);

            gestorAmistadCliente.EnviarSolicitudAmistad(sesion.NombreUsuario, nombreDeUsuarioJugadorSolicitado);
        }

        private bool ValidarEnviarSolicitud(int idJugador, string nombreDeUsuarioJugadorSolicitado)
        {
            bool esSolicitudValida = false;

            if (UtilidadesDeValidacion.EsNombreUsuarioValido(nombreDeUsuarioJugadorSolicitado))
            {
                GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();

                esSolicitudValida = gestorAmistadCliente.ValidarEnvioSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
            }
            else
            {
                lbErrorNombreDeUsuarioSolicitudAmistad.Visibility = Visibility.Visible;
            }

            return esSolicitudValida;
        }

        private void BtnAmigos_Click(object sender, RoutedEventArgs e)
        {
            vistaDesplazableAmigos.Visibility = Visibility.Visible;
            vistaDesplazableSolicitudesAmistad.Visibility = Visibility.Visible; 
            
            stackPanelAmigos.Children.Clear();

            
            string[] nombresAmigos = ObtenerListaAmigos();

            if (nombresAmigos != null)
            {
                foreach (string nombreUsuarioAmigo in nombresAmigos)
                {
                    AgregarUsuarioListaAmigos(nombreUsuarioAmigo);
                }
            }
        }

        private string[] ObtenerListaAmigos()
        {
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
            string[] nombresUsuarioAmigo = null;

            try
            {
                nombresUsuarioAmigo = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId).ToArray();
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

            return nombresUsuarioAmigo;
        }

        private void AgregarUsuarioListaAmigos(string nombreUsuario)
        {
            XAMLActiveUserItemControl AmigoItem = new XAMLActiveUserItemControl(nombreUsuario);
            stackPanelAmigos.Children.Add(AmigoItem);
        }


        private void BtnSolicitudDeAmigos_Click(object sender, RoutedEventArgs e)
        {
            MostrarSolicitudesAmistad();
        }
        private void ImageenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void MostrarSolicitudesAmistad()
        {
            Console.WriteLine("Mostrando solicitudes de amistad...");
            vistaDesplazableSolicitudesAmistad.Visibility = Visibility.Visible;
            vistaDesplazableAmigos.Visibility = Visibility.Visible;

            stackPanelSolicitudesAmistad.Children.Clear();
            string[] nombresUsuarioJugadores = ObtenerSolicitudesAmistadActuales();

            if (nombresUsuarioJugadores != null && nombresUsuarioJugadores.Length > 0)
            {
                Console.WriteLine($"Número de solicitudes de amistad: {nombresUsuarioJugadores.Length}");
                AgregarUsuariosALaListaDeSolicitudesDeAmigos(nombresUsuarioJugadores);
            }
            else
            {
                Console.WriteLine("No hay solicitudes de amistad pendientes.");
            }
        }



        private string[] ObtenerSolicitudesAmistadActuales()
        {
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
            string[] nombreUsuarioJugadores = null;

            try
            {
                nombreUsuarioJugadores = gestorAmistadCliente.ObtenerNombresUsuariosSolicitantes(sesion.JugadorId);
                Console.WriteLine($"Solicitudes encontradas: {nombreUsuarioJugadores?.Length ?? 0}");
            }
            catch (EndpointNotFoundException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                ManejadorExcepciones.ManejarErrorExcepcion(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
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

            return nombreUsuarioJugadores;
        }


        private void AgregarUsuariosALaListaDeSolicitudesDeAmigos(string[] nombresUsuarioJugadores)
        {
            if (nombresUsuarioJugadores != null)
            {
                foreach (string nombreUsuario in nombresUsuarioJugadores)
                {
                    AgregarUsuariosALaListaDeSolicitudesDeAmigos(nombreUsuario);
                }
            }
        }

        private void AgregarUsuariosALaListaDeSolicitudesDeAmigos(string nombreUsuario)
        {
            XAMLFriendRequestItemComponent solicitudAmistadItem = CrearSolicitudAmistadItemControl(nombreUsuario);

            if (solicitudAmistadItem != null)
            {
                Console.WriteLine($"Agregando solicitud de: {nombreUsuario}");
                stackPanelSolicitudesAmistad.Children.Add(solicitudAmistadItem);
            }
            else
            {
                Console.WriteLine("No se pudo crear el control para la solicitud.");
            }
        }

        private XAMLFriendRequestItemComponent CrearSolicitudAmistadItemControl(string nombreUsuario)
        {
            string idItem = "lbRequest";
            string idUsuarioItem = idItem + nombreUsuario;
            XAMLFriendRequestItemComponent solicitudAmistadItem = new XAMLFriendRequestItemComponent(nombreUsuario);
            solicitudAmistadItem.Name = idUsuarioItem;
            solicitudAmistadItem.BotonUsado += SolicitudAmistradItem_BtnClicked;

            return solicitudAmistadItem;
        }

        private void SolicitudAmistradItem_BtnClicked(object sender, ArgumentosDeEventoDeClicDeBotón e)
        {
            string btnAceptar = "Accept";
            string btnRechazar = "Reject";

            if (e.NombreBoton.Equals(btnAceptar))
            {
                AceptarSolicitudAmistad(e.NombreUsuario);
            }

            if (e.NombreBoton.Equals(btnRechazar))
            {
                RechazarSolicitudAmistad(e.NombreUsuario);
            }
        }

        private void AceptarSolicitudAmistad(string nombreUsuarioEmisor)
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);

            try
            {
                gestorSolicitudesAmistadCliente.AceptarSolicitudAmistad(sesion.JugadorId, sesion.NombreUsuario, nombreUsuarioEmisor);
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
            catch (FaultException )
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

        private void RechazarSolicitudAmistad(string nombreUsuario)
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);

            try
            {
                gestorSolicitudesAmistadCliente.RechazarSolicitudAmistad(sesion.JugadorId, nombreUsuario);
                RemoverSolicitudAmistadStackPanel(nombreUsuario);
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
            catch (FaultException )
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

        private void RemoverSolicitudAmistadStackPanel(string nombreUsuario)
        {
            string idItem = "lbRequest";
            string idSolicitudAmistadItem = idItem + nombreUsuario;

            XAMLFriendRequestItemComponent solicitudAmistadItemRemover = BuscarControlElementoSolicitudDeAmistadPorId(idSolicitudAmistadItem);

            if (solicitudAmistadItemRemover != null)
            {
                stackPanelSolicitudesAmistad.Children.Remove(solicitudAmistadItemRemover);
            }
        }

        private XAMLFriendRequestItemComponent BuscarControlElementoSolicitudDeAmistadPorId(string idSolicitudAmistadItem)
        {
            foreach (XAMLFriendRequestItemComponent item in stackPanelSolicitudesAmistad.Children)
            {
                if (item.Name == idSolicitudAmistadItem)
                {
                    return item;
                }
            }

            return null;
        }

        public void NotificarNuevaSolicitudAmistad(string nombreUsuario)
        {
            AgregarUsuariosALaListaDeSolicitudesDeAmigos(nombreUsuario);
        }

        public void NotificarSolicitudAmistadAceptada(string nombreUsuario)
        {
           AgregarUsuarioListaAmigos(nombreUsuario);
            RemoverSolicitudAmistadStackPanel(nombreUsuario);
        }

        public void NotificarAmigoEliminado(string nombreUsuario)
        {
            EliminarAmigoDeLaListaDeAmigos(nombreUsuario);
        }

        private void EliminarAmigoDeLaListaDeAmigos(string nombreUsuario)
        {
            string idItem = "lb";
            string idUsuarioItem = idItem + nombreUsuario;

            XAMLActiveUserItemControl elementoUsuarioEnLíneaParaEliminar = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

            if (elementoUsuarioEnLíneaParaEliminar != null)
            {
                stackPanelAmigos.Children.Remove(elementoUsuarioEnLíneaParaEliminar);
            }
        }


    }
}
