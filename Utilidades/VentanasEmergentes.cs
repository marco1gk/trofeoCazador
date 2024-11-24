using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.VentanasReutilizables;
using System.Windows;

namespace trofeoCazador.Utilidades
{//todo
    //falta pasar los mensajes a resources
    //asi se hace Properties.Resources.lb...
    public static class VentanasEmergentes
    {
        public static void CreateEmergentWindow(string titleEmergentWindow, string descriptionEmergentWindow)
        {
            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateEmergentWindowNoModal(string titleEmergentWindow, string descriptionEmergentWindow)
        {
            XAMLVentanaEmergente emergentWindowNoModal = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindowNoModal.Show();
        }

        public static void CreateConnectionFailedMessageWindow()
        {
            string titleEmergentWindow = "Fallo la conexion";
            string descriptionEmergentWindow = "No se pudo conectar con el servidor. Por favor, Inténtelo más tarde.";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                    titleEmergentWindow,
                    descriptionEmergentWindow
                );

                emergentWindow.Show();
            });
        }

        public static void CreateTimeOutMessageWindow()
        {
            string titleEmergentWindow = "Tiempo de espera excedido";
            string descriptionEmergentWindow = "La operación ha excedido el tiempo de espera. Por favor, inténtelo más tarde.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateDataBaseErrorMessageWindow()
        {
            string titleEmergentWindow = "Error de la base de datos";
            string descriptionEmergentWindow = "Upss... Ocurrió un error en la base de datos. Porfavor, inténtelo más tarde";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateServerErrorMessageWindow()
        {
            string titleEmergentWindow = "Error en el servidor";
            string descriptionEmergentWindow = "Upss... Ocurrió un error en el servidor. Por favor, inténtelo más tarde";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                    titleEmergentWindow,
                    descriptionEmergentWindow
                );

                emergentWindow.Show();
            });
        }

        public static void CreateUnexpectedErrorMessageWindow()
        {
            string titleEmergentWindow = "Error inesperado";
            string descriptionEmergentWindow = "Upss... Ocurrió un error inesperado. Por favor, inténtelo más tarde";

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                    titleEmergentWindow,
                    descriptionEmergentWindow
                );

                emergentWindow.Show();
            });
        }


        public static void CreateLobbyNotFoundMessageWindow()
        {
            string titleEmergentWindow = "Lobby no encontrado";
            string descriptionEmergentWindow = "El lobby al que estas intentando entrar no existe.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateSuccesfulReportMessageWindow()
        {
            string titleEmergentWindow = "Reporte exitoso";
            string descriptionEmergentWindow = "El jugador ha sido reportado. Agradecemos tu apoyo.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateReportedPlayerMessageWindow()
        {
            string titleEmergentWindow = "Jugador ya reportado";
            string descriptionEmergentWindow = "Ya has reportado a este jugador.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateBannedPlayerMessageWindow()
        {
            string titleEmergentWindow = "Has sido baneado";
            string descriptionEmergentWindow = "Has acumulado el máximo de reportes permitidos. Se restringirá tu cuenta por un tiempo establecido.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateHostLeftLobbyMessageWindow()
        {
            string titleEmergentWindow = "Lobby eliminado";
            string descriptionEmergentWindow = "El host salio del lobby. Regresaras al menú principal.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }

        public static void CreateLobbyIsFullMessageWindow()
        {
            string titleEmergentWindow = "Lobby lleno";
            string descriptionEmergentWindow = "El lobby al que estas intentando entrar esta lleno.";

            XAMLVentanaEmergente emergentWindow = new XAMLVentanaEmergente(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.Show();
        }


    }
}
