using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLAmigos : Page, IGestorDeSolicitudesDeAmistadCallback
    {
        SingletonSesion sesion = SingletonSesion.Instancia;

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
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void EnviarSolicitud()
        {
            lbErrorNombreDeUsuarioSolicitudAmistad.Visibility = Visibility.Visible;

            string nombreDeUsuarioJugadorSolicitado = tbxNombreDeUsuarioEnviarSolicitud.Text.Trim();
            int idJugador = sesion.JugadorId;

            if (ValidarEnviarSolicitud(idJugador, nombreDeUsuarioJugadorSolicitado))
            {
                AgregarSolicitudAmistad(idJugador, nombreDeUsuarioJugadorSolicitado);
                EnviarSolicitudAmistad(nombreDeUsuarioJugadorSolicitado);

                Console.WriteLine("");

                tbxNombreDeUsuarioEnviarSolicitud.Text = string.Empty;
            }
            else
            {
             //   EmergentWindows.CreateEmergentWindow(Properties.Recursos.lbFriendRequest,
               //     Properties.Resources.tbkFriendRequestErrorDescription);
            }
        }

        private void AgregarSolicitudAmistad(int idJugador, string nombreDeUsuarioJugadorSolicitado)
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
            
            stackPanelFriends.Children.Clear();

            
            string[] nombresAmigos = ObtenerListaAmigos();

            // Agrega cada amigo a la lista en la interfaz
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
            }//GetListUsernameFriends
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return nombresUsuarioAmigo;
        }

        private void AgregarUsuarioListaAmigos(string nombreUsuario)
        {
            XAMLActiveUserItemControl AmigoItem = new XAMLActiveUserItemControl(nombreUsuario);
            stackPanelFriends.Children.Add(AmigoItem);
        }


        private void BtnSolicitudDeAmigos_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("antes d mostrar");
            MostrarSolicitudesAmistad();
            Console.WriteLine("despues d mostrar");
        }

        private void MostrarSolicitudesAmistad()
        {
            Console.WriteLine("Mostrando solicitudes de amistad...");
            vistaDesplazableSolicitudesAmistad.Visibility = Visibility.Visible;
            vistaDesplazableAmigos.Visibility = Visibility.Visible;

            stackPanelFriendsRequest.Children.Clear();
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

            try//GetUsernamePlayersRequesters
            {
                nombreUsuarioJugadores = gestorAmistadCliente.ObtenerNombresUsuariosSolicitantes(sesion.JugadorId);
                Console.WriteLine($"Solicitudes encontradas: {nombreUsuarioJugadores?.Length ?? 0}");
            }
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
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
                stackPanelFriendsRequest.Children.Add(solicitudAmistadItem);
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
            solicitudAmistadItem.ButtonClicked += SolicitudAmistradItem_BtnClicked;

            return solicitudAmistadItem;
        }

        private void SolicitudAmistradItem_BtnClicked(object sender, ArgumentosDeEventoDeClicDeBotón e)
        {
            string btnAccept = "Accept";
            string btnReject = "Reject";

            if (e.NombreBoton.Equals(btnAccept))
            {
                AceptarSolicitudAmistad(e.NombreUsuario);
            }

            if (e.NombreBoton.Equals(btnReject))
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
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void RemoverSolicitudAmistadStackPanel(string nombreUsuario)
        {
            string idItem = "lbRequest";
            string idSolicitudAmistadItem = idItem + nombreUsuario;

            XAMLFriendRequestItemComponent solicitudAmistadItemRemover = BuscarControlElementoSolicitudDeAmistadPorId(idSolicitudAmistadItem);

            if (solicitudAmistadItemRemover != null)
            {
                stackPanelFriendsRequest.Children.Remove(solicitudAmistadItemRemover);
            }
        }

        private XAMLFriendRequestItemComponent BuscarControlElementoSolicitudDeAmistadPorId(string idSolicitudAmistadItem)
        {
            foreach (XAMLFriendRequestItemComponent item in stackPanelFriendsRequest.Children)
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
                stackPanelFriends.Children.Remove(elementoUsuarioEnLíneaParaEliminar);
            }
        }


    }
}
