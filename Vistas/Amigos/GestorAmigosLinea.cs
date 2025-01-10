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
      
        private const string ESTADO_EN_LINEA = "#61FF00";
        private const string ESTADO_FUERA_DE_LINEA = "#FF5A5E59";
       
        private void CargarAmigosJugador()
        {
            stackPanelAmigos.Children.Clear();
            try
            {
                GestorAmistadClient gestorAmistadCliente = new GestorAmistadClient();
                string[] nombreUsuarioAmigosJugador = gestorAmistadCliente.ObtenerListaNombresUsuariosAmigos(sesion.JugadorId);
                AñadirUsuariosListaAmigos(nombreUsuarioAmigosJugador);

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

        private void CambiarEstadoAmigos(string[] nombreLinea, bool estaEnLinea)
        {
            foreach (string nombreEnLinea in nombreLinea)
            {
                CambiarEstadoJugador(nombreEnLinea, estaEnLinea);
            }
        }

        public void CambiarEstadoJugador(string nombreUsuario, bool enLinea)
        {
            string idLabel = "lb";
            string idUsuarioItem = idLabel + nombreUsuario;
            XAMLUsuarioConectadoControl usuarioLineaItem = BuscarControlElementoUsuarioActivoPorId(idUsuarioItem);

            if (usuarioLineaItem == null)
            {
                Console.WriteLine($"Control no encontrado para el usuario: {nombreUsuario}");
                return;
            }

            SolidColorBrush colorEstadoJugador = enLinea
                ? GestorColores.CreateColorFromHexadecimal(ESTADO_EN_LINEA)
                : GestorColores.CreateColorFromHexadecimal(ESTADO_FUERA_DE_LINEA);
            Console.WriteLine($"Estado de {nombreUsuario}: {(enLinea ? "En línea" : "Fuera de línea")}, Color: {colorEstadoJugador}");

            usuarioLineaItem.Dispatcher.Invoke(() =>
            {
                if (usuarioLineaItem.rectanguloEstadoJugador != null)
                {
                    Console.WriteLine("Actualizando color del rectángulo...");
                    usuarioLineaItem.rectanguloEstadoJugador.Fill = colorEstadoJugador;
                    usuarioLineaItem.rectanguloEstadoJugador.InvalidateVisual();

                }
                else
                {
                    Console.WriteLine("El rectángulo de estado es nulo.");
                }
            });
        }

        //Se decidio que este metodo regrese null debido a que solo tiene un sentido, si es nulo es porque no existe
        private XAMLUsuarioConectadoControl BuscarControlElementoUsuarioActivoPorId(string idUsuarioItem)
        {

            foreach (XAMLUsuarioConectadoControl item in stackPanelAmigos.Children)
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
                AñadirUsuarioListaAmigos(nombreUsuario, ESTADO_FUERA_DE_LINEA);
            }
        }


        private void AñadirUsuarioListaAmigos(string nombreUsuario, string estadoConexionJugador)
        {
            XAMLUsuarioConectadoControl usuarioConectadoItem = CrearControlElementoUsuarioActivo(nombreUsuario, estadoConexionJugador);
            stackPanelAmigos.Children.Add(usuarioConectadoItem);
        }


        XAMLUsuarioConectadoControl CrearControlElementoUsuarioActivo(string nombreUsuario, string colorHexadecimal)
        {
            string idItem = "lb";
            string idUsuarioItem = idItem + nombreUsuario;
            XAMLUsuarioConectadoControl usuarioConectadoItem = new XAMLUsuarioConectadoControl(nombreUsuario);
            usuarioConectadoItem.Name = idUsuarioItem;
            usuarioConectadoItem.BotonUsado += ElementoUsuarioEnLínea_BotónEliminarAmigoClickeado;
            SolidColorBrush colorJugadorConectado = GestorColores.CreateColorFromHexadecimal(colorHexadecimal);
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
            catch (EndpointNotFoundException)
            {
                Console.WriteLine("entra al primero");
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException)
            {
                Console.WriteLine("entra al segundo");
                VentanasEmergentes.CrearVentanaMensajeTimeOut();
            }
            catch (FaultException<HuntersTrophyExcepcion>)
            {

                Console.WriteLine("entra al tercero");
                VentanasEmergentes.CrearErrorMensajeVentanaBaseDatos();
            }
            catch (CommunicationException)
            {

                Console.WriteLine("entra al cuarto");
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {

                Console.WriteLine("entra al quinto");
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
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
        }

        public void NotificarInvitacionSala(string nombreInvitador, string codigoSalaEspera)
        {
            string descripcionVentana = Properties.Resources.lbDescripcionInvitacion+" "+nombreInvitador+" "+"a la sala "+codigoSalaEspera;
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloInvitacionAmigo, descripcionVentana);
        }

        public void SincronizarEstado(Dictionary<string, string> estadoJuego)
        {
            throw new NotImplementedException();
        }
    }
  



}
