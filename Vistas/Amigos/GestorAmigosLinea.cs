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
            stackPanelFriends.Children.Clear();
            GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
           
            string[] nombreUsuarioAmigosJugador = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId);
            AñadirUsuariosListaAmigos(nombreUsuarioAmigosJugador);
        }

        public void MostrarComoUsuarioActivo()
        {
            InstanceContext contexto = new InstanceContext(this);
            GestorUsuariosConectadosClient cliente = new GestorUsuariosConectadosClient(contexto);

            cliente.RegistrarUsuarioAUsuariosConectados(sesion.JugadorId, sesion.NombreUsuario);
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

            Dispatcher.Invoke(() =>
            {
                XAMLActiveUserItemControl usuarioEnLineaItem = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

                if (usuarioEnLineaItem != null)
                {
                    usuarioEnLineaItem.IsConnected = estaEnLinea;

              
                    Console.WriteLine($"Estado de conexión para {nombreUsuario}: {usuarioEnLineaItem.IsConnected}");
                    Console.WriteLine($"Estado actualizado en GUI: {usuarioEnLineaItem.rectangleStatusPlayer.Fill}");
                }
                else
                {
                    Console.WriteLine($"No se encontró el control para {nombreUsuario}");
                }
            });

        }

        private XAMLActiveUserItemControl BuscarControlElementoUsuarioActivoPorId(string idUsuarioItem)
        {
            foreach (XAMLActiveUserItemControl item in stackPanelFriends.Children.OfType<XAMLActiveUserItemControl>())
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
                Console.WriteLine($"Añadiendo usuario {nombreUsuario} a la lista de amigos");
                AñadirUsuarioListaAmigos(nombreUsuario, ESTADOFUERADELINEA);
            }
        }

        private void AñadirUsuarioListaAmigos(string nombreUsuario, string estadoConexiónJugador)
        {
            
            var controlExistente = stackPanelFriends.Children
                .OfType<XAMLActiveUserItemControl>()
                .FirstOrDefault(ctrl => ctrl.Name == "lb" + nombreUsuario);

            if (controlExistente != null)
            {
                Console.WriteLine($"El control para {nombreUsuario} ya existe. Actualizando estado.");
            
                controlExistente.IsConnected = estadoConexiónJugador == ESTADOENLINEA;
            }
            else
            {
                Console.WriteLine($"Creando control para {nombreUsuario}");
                
                XAMLActiveUserItemControl elementoUsuarioEnLínea = CrearControlElementoUsuarioActivo(nombreUsuario, estadoConexiónJugador);
                stackPanelFriends.Children.Add(elementoUsuarioEnLínea);
                Console.WriteLine($"Control añadido: {elementoUsuarioEnLínea.Name}");
            }
        }





        XAMLActiveUserItemControl CrearControlElementoUsuarioActivo(string nombreUsuario, string colorHexadecimal)
        {
            XAMLActiveUserItemControl usuarioEnLineaItem = new XAMLActiveUserItemControl(nombreUsuario)
            {
                Name = "lb" + nombreUsuario
            };

            usuarioEnLineaItem.IsConnected = false; 
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
                VentanasEmergentes.CreateConnectionFailedMessageWindow();
                ManejadorExcepciones.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                VentanasEmergentes.CreateTimeOutMessageWindow();
                ManejadorExcepciones.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {
                VentanasEmergentes.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException )
            {
                VentanasEmergentes.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLInicioSesion());
            }
            catch (CommunicationException ex)
            {

                VentanasEmergentes.CreateServerErrorMessageWindow();
                ManejadorExcepciones.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                VentanasEmergentes.CreateUnexpectedErrorMessageWindow();
                ManejadorExcepciones.HandleFatalException(ex, NavigationService);
            }
        }

        public void NotificarUsuarioConectado(string nombreUsuario)
        {
            bool esEnLinea = true;
            CambiarEstadoJugador(nombreUsuario, esEnLinea);
            Console.WriteLine(nombreUsuario + " se ha conectado");
        }

        public void NotificarUsuarioDesconectado(string nombreUsuario)
        {
            bool esEnLinea = false;
            CambiarEstadoJugador(nombreUsuario, esEnLinea);
            Console.WriteLine(nombreUsuario+" se ha desconectado");
        }

        public void NotificarAmigosEnLinea(string[] nombresUsuariosEnLinea)
        {
            bool esEnLinea = true;

            CambiarEstadoAmigos(nombresUsuariosEnLinea, esEnLinea);
            SuscribirUsuarioAlDiccionarioDeAmigosEnLínea();
        }



        public void Ping()
        {
            Console.WriteLine("es el ping");
        }

        public void NotificarInvitacionSala(string nombreInvitador, string codigoSalaEspera)
        {
            Console.WriteLine("te invito un we");
            MessageBox.Show($"{nombreInvitador} te ha invitado a a la sala con codigo " + codigoSalaEspera);
        }
    }
    public static class Utilities
    {
        public static SolidColorBrush CrearColorDeHexadecimal(string coloHexadecimal)
        {
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(coloHexadecimal));
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error al convertir el color {coloHexadecimal}: {ex.Message}");
                return Brushes.Gray; 
            }
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
