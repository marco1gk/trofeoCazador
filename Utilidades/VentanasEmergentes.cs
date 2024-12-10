using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.VentanasReutilizables;
using System.Windows;
using trofeoCazador.Componentes.SalaEspera;

namespace trofeoCazador.Utilidades
{
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
            string tituloVentanaEmergente = Properties.Resources.lbConexionFallida;
            string descripcionVentanaEmergente = Properties.Resources.lbDetallesConexionFallida;

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
            string tituloVentanaEmergente = Properties.Resources.lbTituloExcepcionTimeOut;
            string descriptionEmergentWindow = Properties.Resources.lbDetallesExcepcionTimeOut;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearErrorMensajeVentanaBaseDatos()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloExcepcionBaseDeDatos;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionExcepcionBaseDeDatos;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearMensajeVentanaServidorError()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloExcepcionServidor;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionExcepcionServidor;

            Application.Current.Dispatcher.Invoke(() =>
            {
                XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                    tituloVentanaEmergente,
                    descriptionEmergentWindow
                );

                ventanaEmergente.Show();
            });
        }

        public static void CrearMensajeVentanaErrorInesperado()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloExcepcionInesperada;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionExcepcionInesperada;

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
            string tituloVentanaEmergente = Properties.Resources.lbTituloSalaEsperaNoEncotrada;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionSalaEsperaNoEncotrada;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeReporteExitoso()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloReporteExitoso;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionReporteExitoso;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeJugadorReportado()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloJugadorReportado;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionJugadorReportado;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeJugadorBanneado()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloJugadorCastigado;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionJugadorCastigado;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeAnfitrionDejoSalaEspera()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloAnfitrionSalioSalaEspera;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionAnfitrionDejoSalaEsperra;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }

        public static void CrearVentanaMensajeSalaEsperaLlena()
        {
            string tituloVentanaEmergente = Properties.Resources.lbTituloSalaEsperaLlena;
            string descriptionEmergentWindow = Properties.Resources.lbDescripcionSalaEsperaLlena;

            XAMLVentanaEmergente ventanaEmergente = new XAMLVentanaEmergente(
                tituloVentanaEmergente,
                descriptionEmergentWindow
            );

            ventanaEmergente.Show();
        }


    }
}
