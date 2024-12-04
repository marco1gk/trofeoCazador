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
            stackPanelAmigos.Children.Clear();
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

        private void CambiarEstadoJugador(string nombreUsuario, bool enLinea)
        {
            string idLabel = "lb";
            string idUsuarioItem = idLabel + nombreUsuario;
            XAMLActiveUserItemControl usuarioLineaItem = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

            if (usuarioLineaItem == null)
            {
                Console.WriteLine($"Control no encontrado para el usuario: {nombreUsuario}");
                return;
            }

            SolidColorBrush statusPlayerColor = enLinea
                ? Utilities.CreateColorFromHexadecimal(ESTADOENLINEA)
                : Utilities.CreateColorFromHexadecimal(ESTADOFUERADELINEA);

            usuarioLineaItem.Dispatcher.Invoke(() =>
            {
                if (usuarioLineaItem.rectanguloEstadoJugador != null)
                {
                    Console.WriteLine("Se está cambiando el color en el hilo de la UI.");
                    usuarioLineaItem.rectanguloEstadoJugador.Fill = statusPlayerColor;
                }
                else
                {
                    Console.WriteLine("El rectángulo de estado es nulo.");
                }
            });
        }


        private XAMLActiveUserItemControl BuscarControlElementoUsuarioActivoPorId(string idUsuarioItem)
        {
            foreach (XAMLActiveUserItemControl item in stackPanelAmigos.Children)
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


        private void AñadirUsuarioListaAmigos(string nombreUsuario, string estadoConexionJugador)
        {
            XAMLActiveUserItemControl usuarioConectadoItem = CrearControlElementoUsuarioActivo(nombreUsuario, estadoConexionJugador);
            stackPanelAmigos.Children.Add(usuarioConectadoItem);
        }


        XAMLActiveUserItemControl CrearControlElementoUsuarioActivo(string nombreUsuario, string colorHexadecimal)
        {
            string idItem = "lb";
            string idUsuarioItem = idItem + nombreUsuario;
            XAMLActiveUserItemControl usuarioConectadoItem = new XAMLActiveUserItemControl(nombreUsuario);
            usuarioConectadoItem.Name = idUsuarioItem;
            usuarioConectadoItem.BotonUsado += ElementoUsuarioEnLínea_BotónEliminarAmigoClickeado;
            SolidColorBrush colorJugadorConectado = Utilities.CreateColorFromHexadecimal(colorHexadecimal);
            usuarioConectadoItem.rectanguloEstadoJugador.Fill = colorJugadorConectado;

            return usuarioConectadoItem;
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
                VentanasEmergentes.CrearMensajeVentanaInesperadoError();
                ManejadorExcepciones.ManejarFatalExcepcion(ex, NavigationService);
            }
        }

        public void NotificarUsuarioConectado(string nombreUsuario)
        {
            bool conectado=true;
            Console.WriteLine($"Recibida notificación de conexión para {nombreUsuario}");
            CambiarEstadoJugador(nombreUsuario, conectado);
        }

        public void NotificarUsuarioDesconectado(string nombreUsuario)
        {
            bool conectado = false;
            Console.WriteLine($"Recibida notificación de desconexión para {nombreUsuario}");
            CambiarEstadoJugador(nombreUsuario, conectado);
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
        
        
            public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
            {
                SolidColorBrush solidColorBrush = null;

                if (hexadecimalColor != null)
                {
                    try
                    {
                        solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
                    }
                    catch (FormatException ex)
                    {
                    Console.WriteLine($"Error al convertir color: {ex.Message}");
                    ManejadorExcepciones.ManejarComponenteErrorExcepcion(ex);
                    }catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }

                return solidColorBrush;
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
