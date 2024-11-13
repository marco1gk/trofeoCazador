using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLAmigos : Page, IGestorUsuariosConectadosCallback
    {
      
        private const string ESTADOENLINEA = "#61FF00";
        private const string ESTADOFUERADELINEA = "#FF5A5E59";
       
        private void CargarAmigosJugador()
        {
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
           
            string[] nombreUsuarioAmigosJugador = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId);
            AñadirUsuariosListaAmigos(nombreUsuarioAmigosJugador);//GetListUsernameFriends
        }

        public void MostrarComoUsuarioActivo()
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorUsuariosConectadosClient cliente = new GestorUsuariosConectadosClient(contexto);

            cliente.RegisterUserToOnlineUsers(sesion.JugadorId, sesion.NombreUsuario);
        }


        private void CambiarEstadoAmigos(string[] nombreLinea, bool estaEnLinea)
        {
            foreach (string nombreEnLinea in nombreLinea)
            {
                CambiarEstadoJugador(nombreEnLinea, estaEnLinea);
            }
        }

        private void CambiarEstadoJugador(string nombreUsuario, bool estaEnLinea)
        {
              Console.WriteLine($"Cambiando estado de {nombreUsuario} a {(estaEnLinea ? "Conectado" : "Desconectado")}");
            string idLabel = "lb";
            string idUsuarioItem = idLabel + nombreUsuario;
            XAMLActiveUserItemControl usuarionEnLineaItem = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

            if (usuarionEnLineaItem != null)
            {
                SolidColorBrush estadoJugadorColor;

                if (estaEnLinea)
                {
                    estadoJugadorColor = Utilities.CrearColorDeHexadecimal(ESTADOENLINEA);
                }
                else
                {
                    estadoJugadorColor = Utilities.CrearColorDeHexadecimal(ESTADOFUERADELINEA);
                }

                usuarionEnLineaItem.rectangleStatusPlayer.Fill = estadoJugadorColor;
            }
        }


        private XAMLActiveUserItemControl BuscarControlElementoUsuarioActivoPorId(string idUsuarioItem)
        {
            foreach (XAMLActiveUserItemControl item in stackPanelFriends.Children)
            {
                if (item.Name == idUsuarioItem)
                {
                    return item;
                }
            }

            return null;
        }

        private void AñadirUsuariosListaAmigos(string[] nombresUsuarioEnLinea)
        {
            foreach (var nombreUsuario in nombresUsuarioEnLinea)
            {
                AñadirUsuarioListaAmigos(nombreUsuario, ESTADOFUERADELINEA);
            }
        }

        private void AñadirUsuarioListaAmigos(string nombreUsuario, string estadoConexiónJugador)
        {
            XAMLActiveUserItemControl elementoUsuarioEnLínea = CrearControlElementoUsuarioActivo(nombreUsuario, estadoConexiónJugador);
            stackPanelFriends.Children.Add(elementoUsuarioEnLínea);
        }

        private XAMLActiveUserItemControl CrearControlElementoUsuarioActivo(string nombreUsuario, string colorHexadecimal)
        {
            string idItem = "lb";
            string idUsuarioItem = idItem + nombreUsuario;
            XAMLActiveUserItemControl usuarioEnLineaItem = new XAMLActiveUserItemControl(nombreUsuario);
            usuarioEnLineaItem.Name = idUsuarioItem;
            usuarioEnLineaItem.ButtonClicked += ElementoUsuarioEnLínea_BotónEliminarAmigoClickeado;
         

            return usuarioEnLineaItem;
        }

        private void ElementoUsuarioEnLínea_BotónEliminarAmigoClickeado(object sender, ArgumentosDeEventoDeClicDeBotón e)
        {
            string btnEliminarAmigo = "DeleteFriend";

            if (e.NombreBoton.Equals(btnEliminarAmigo))
            {
                EliminarAmigo(e.NombreUsuario);
            }
        }

        private void EliminarAmigo(string nombreUsuarioAmigoEliminar)
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient gestorSolicitudesAmistadCliente = new GestorDeSolicitudesDeAmistadClient(contexto);

            try
            {
                
                gestorSolicitudesAmistadCliente.EliminarAmigo(sesion.JugadorId, sesion.NombreUsuario, nombreUsuarioAmigoEliminar);
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

        public void NotifyUserLoggedIn(string nombreUsuario)
        {
            bool esEnLinea = true;
            CambiarEstadoJugador(nombreUsuario, esEnLinea);
        }

        public void NotifyUserLoggedOut(string nombreUsuario)
        {
            bool esEnLinea = false;
            CambiarEstadoJugador(nombreUsuario, esEnLinea);
        }

        public void NotifyOnlineFriends(string[] nombresUsuariosEnLinea)
        {
            bool esEnLinea = true;

            CambiarEstadoAmigos(nombresUsuariosEnLinea, esEnLinea);
            SuscribirUsuarioAlDiccionarioDeAmigosEnLínea();
        }

    }
    public static class Utilities
    {
        public static SolidColorBrush CrearColorDeHexadecimal(string coloHexadecimal)
        {
            SolidColorBrush solidColorBrush = null;

            if (coloHexadecimal != null)
            {
                try
                {
                    solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(coloHexadecimal));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return solidColorBrush;
        }

        public static Image CrearImagenPorPath(string imagenPath)
        {
            string pathAbsoluto = ConstruirPathAbsoluto(imagenPath);

            Image estiloImagen = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(pathAbsoluto));
            estiloImagen.Source = bitmapImage;

            return estiloImagen;
        }

        public static string ConstruirPathAbsoluto(string pathRelativo)
        {
            string pathAbsoluto = "";

            if (pathRelativo != null)
            {
                pathAbsoluto = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathRelativo);
            }

            return pathAbsoluto;
        }

    }



}
//private void BtnSignOff_Click(object sender, RoutedEventArgs e)
//{
//    ExitGameFromLobby();
//    NavigationService.Navigate(new XAMLInicioSesion());
//}

//public void BtnCloseWindow_Click()
//{
//    ExitGameFromLobby();
//}

//private void ExitGameFromLobby()
//{
//    InstanceContext context = new InstanceContext(this);
//    GestorUsuariosConectadosClient client = new GestorUsuariosConectadosClient(context);


//    try
//    {
//        // Asegurarse de eliminar correctamente al jugador de la lista de jugadores activos
//        client.UnregisterUserToOnlineUsers(sesion.NombreUsuario);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex);
//    }
//}