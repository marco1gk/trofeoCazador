using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.VentanasReutilizables;
using System.Windows;
using trofeoCazador.Componentes.SalaEspera;

namespace trofeoCazador.Utilidades
{//todo
    //falta pasar los mensajes a resources
    //asi se hace Properties.Resources.lb...
    public static class VentanasEmergentes
    {
        public static void CrearVentanaEmergente(string tituloVentanaEmergente, string descripcionVentanaEmergente)
        {
            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergente.Show();
        }
        public static void CrearVentanaInvitacionSalaEspera(string codigoSalaEspera)
        {
            XAMLInvitacionSala componenteInvitacionSalaEspera = new XAMLInvitacionSala(codigoSalaEspera);

            componenteInvitacionSalaEspera.Show();
        }
        public static void CrearVentanaEmergenteNoModal(string tituloVentanaEmergente, string descripcionVentanaEmergente)
        {
            XAMLVentanaEmergente ventanaEmergenteNoModal = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descripcionVentanaEmergente
            );

            ventanaEmergenteNoModal.Show();
        }

        public static void CrearConexionFallidaMensajeVentana()
        {
            string tituloVentanaEmergente = "Fallo la conexion";
            string descripcionVentanaEmergente = "No se pudo conectar con el servidor. Por favor, Inténtelo más tarde.";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                    tituloVentanaEmergente,
                    descripcionVentanaEmergente
                );

                ventanaEmergente.Show();
            });
        }

        public static void CrearVentanaMensajeTimeOut()
        {
            string tituloVentanaEmergente = "Tiempo de espera excedido";
            string descriptionEmergentWindow = "La operación ha excedido el tiempo de espera. Por favor, inténtelo más tarde.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearErrorMensajeVentanaBaseDatos()
        {
            string tituloVentanaEmergente = "Error de la base de datos";
            string descriptionEmergentWindow = "Upss... Ocurrió un error en la base de datos. Porfavor, inténtelo más tarde";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearMensajeVentanaServidorError()
        {
            string tituloVentanaEmergente = "Error en el servidor";
            string descriptionEmergentWindow = "Upss... Ocurrió un error en el servidor. Por favor, inténtelo más tarde";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                    tituloVentanaEmergente,
                    descriptionEmergentWindow
                );

                ventanaEmergente.Show();
            });
        }

        public static void CrearMensajeVentanaInesperadoError()
        {
            string tituloVentanaEmergente = "Error inesperado";
            string descriptionEmergentWindow = "Upss... Ocurrió un error inesperado. Por favor, inténtelo más tarde";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                    tituloVentanaEmergente,
                    descriptionEmergentWindow
                );

                ventanaEmergente.Show();
            });
        }


        public static void CrearSalaEsperaNoEncontradaMensajeVentana()
        {
            string tituloVentanaEmergente = "Lobby no encontrado";
            string descriptionEmergentWindow = "El lobby al que estas intentando entrar no existe.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeReporteExitoso()
        {
            string tituloVentanaEmergente = "Reporte exitoso";
            string descriptionEmergentWindow = "El jugador ha sido reportado. Agradecemos tu apoyo.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeJugadorReportado()
        {
            string tituloVentanaEmergente = "Jugador ya reportado";
            string descriptionEmergentWindow = "Ya has reportado a este jugador.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeJugadorBanneado()
        {
            string tituloVentanaEmergente = "Has sido baneado";
            string descriptionEmergentWindow = "Has acumulado el máximo de reportes permitidos. Se restringirá tu cuenta por un tiempo establecido.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeAnfitrionDejoSalaEspera()
        {
            string tituloVentanaEmergente = "Lobby eliminado";
            string descriptionEmergentWindow = "El host salio del lobby. Regresaras al menú principal.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeSalaEsperaLlena()
        {
            string tituloVentanaEmergente = "Lobby lleno";
            string descriptionEmergentWindow = "El lobby al que estas intentando entrar esta lleno.";

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }


    }
}
