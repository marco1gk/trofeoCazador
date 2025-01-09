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
        private SingletonSesion sesion = SingletonSesion.Instancia;

        private static bool ComprobarEstadoRed()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private async Task EjecutarAccionConSeguridad(Func<Task> accion, string mensajeError = "Error inesperado")
        {
            try
            {
                await accion();
            }
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (CommunicationException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearVentanaEmergente("Error", mensajeError);
            }
        }

        private async Task SuscribirUsuarioAlDiccionarioDeAmigosEnLínea()
        {
            if (!ComprobarEstadoRed())
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                return;
            }

            await EjecutarAccionConSeguridad(async () =>
            {
                try
                {
                    InstanceContext contexto = new InstanceContext(this);
                    GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);
                    await gestorSolicitudesAmistadCliente.AgregarADiccionarioAmistadesEnLineaAsync(sesion.NombreUsuario);
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

            }, "No se pudo suscribir al diccionario de amigos en línea.");
        }

        private async void BtnEnviarSolicitud_Click(object sender, RoutedEventArgs e)
        {
            if (!ComprobarEstadoRed())
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
                return;
            }

            await EjecutarAccionConSeguridad(async () =>
            {
                await Task.Run(() => EnviarSolicitud());
            }, "No se pudo enviar la solicitud.");
        }

        private void EnviarSolicitud()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int idJugador = sesion.JugadorId;
                string mensaje;
                string nombreDeUsuarioJugadorSolicitado = tbNombreDeUsuarioEnviarSolicitud.Text.Trim();

                if (ValidarEnviarSolicitud(idJugador, nombreDeUsuarioJugadorSolicitado))
                {
                    AgregarSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
                    EnviarSolicitudAmistad(nombreDeUsuarioJugadorSolicitado);

                    mensaje = Properties.Resources.lbInvitacionEnviada + " " + nombreDeUsuarioJugadorSolicitado;
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, mensaje);

                    tbNombreDeUsuarioEnviarSolicitud.Text = string.Empty;
                }
                else
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloSolicitudAmistad, Properties.Resources.lbProblemasInvitacion);
                }
            });
        }


        private static void AgregarSolicitudAmistad(int idJugador, string nombreDeUsuarioJugadorSolicitado)
        {
            try
            {
                GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
                gestorAmistadCliente.AgregarSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
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

        private void EnviarSolicitudAmistad(string nombreDeUsuarioJugadorSolicitado)
        {
            
            try
            {
                InstanceContext contexto = new InstanceContext(this);
                GestorDeSolicitudesDeAmistadClient gestorAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);
                gestorAmistadCliente.EnviarSolicitudAmistad(sesion.NombreUsuario, nombreDeUsuarioJugadorSolicitado);
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

        private bool ValidarEnviarSolicitud(int idJugador, string nombreDeUsuarioJugadorSolicitado)
        {
            bool esSolicitudValida = false;

            if (UtilidadesDeValidacion.EsNombreUsuarioValido(nombreDeUsuarioJugadorSolicitado))
            {
                GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
                try
                {
                    esSolicitudValida = gestorAmistadCliente.ValidarEnvioSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
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
            else
            {
                
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
            XAMLUsuarioConectadoControl AmigoItem = new XAMLUsuarioConectadoControl(nombreUsuario);
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
            catch (EndpointNotFoundException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
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
            XAMLSolicitudAmistad solicitudAmistadItem = CrearSolicitudAmistadItemControl(nombreUsuario);

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

        private XAMLSolicitudAmistad CrearSolicitudAmistadItemControl(string nombreUsuario)
        {
            string idItem = "lbSolicitud";
            string idUsuarioItem = idItem + nombreUsuario;
            XAMLSolicitudAmistad solicitudAmistadItem = new XAMLSolicitudAmistad(nombreUsuario);
            solicitudAmistadItem.Name = idUsuarioItem;
            solicitudAmistadItem.BotonUsado += SolicitudAmistrarItem_BtnClicked;

            return solicitudAmistadItem;
        }

        private void SolicitudAmistrarItem_BtnClicked(object sender, ArgumentosDeEventoDeClicDeBotón e)
        {
            string btnAceptar = "Aceptar";
            string btnRechazar = "Rechazar";

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
                CambiarEstadoJugador(nombreUsuarioEmisor, true);

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

        private void RechazarSolicitudAmistad(string nombreUsuario)
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);

            try
            {
                gestorSolicitudesAmistadCliente.RechazarSolicitudAmistad(sesion.JugadorId, nombreUsuario);
                RemoverSolicitudAmistadStackPanel(nombreUsuario);
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

        private void RemoverSolicitudAmistadStackPanel(string nombreUsuario)
        {
            string idItem = "lbSolicitud";
            string idSolicitudAmistadItem = idItem + nombreUsuario;

            XAMLSolicitudAmistad solicitudAmistadItemRemover = BuscarControlElementoSolicitudDeAmistadPorId(idSolicitudAmistadItem);

            if (solicitudAmistadItemRemover != null)
            {
                stackPanelSolicitudesAmistad.Children.Remove(solicitudAmistadItemRemover);
            }
        }

        //Se decidio que este metodo regrese null debido a que solo tiene un sentido, si es nulo es porque no existe
        private XAMLSolicitudAmistad BuscarControlElementoSolicitudDeAmistadPorId(string idSolicitudAmistadItem)
        {
            foreach (XAMLSolicitudAmistad item in stackPanelSolicitudesAmistad.Children)
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

            XAMLUsuarioConectadoControl elementoUsuarioEnLíneaParaEliminar = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

            if (elementoUsuarioEnLíneaParaEliminar != null)
            {
                stackPanelAmigos.Children.Remove(elementoUsuarioEnLíneaParaEliminar);
            }
        }


    }
}
