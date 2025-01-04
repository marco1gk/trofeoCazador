using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Estadisticas
{
    public partial class XAMLEstadisticas : Page
    {
        public List<JugadorEstadisticas> Estadisticas { get; set; }

        public XAMLEstadisticas()
        {
            InitializeComponent();
            CargarEstadisticas();
        }

        private void CargarEstadisticas()
        {
            try
            {
                using (var servicio = new EstadisticasGlobalesClient())
                {
                    var estadisticasGlobales = servicio.ObtenerEstadisticasGlobales();
                    Estadisticas = new List<JugadorEstadisticas>();

                    foreach (var estadisticasJugador in estadisticasGlobales)
                    {
                        string nombreJugador = ObtenerNombreUsuarioPorIdJugador(estadisticasJugador.IdJugador);

                        var jugadorEstadisticas = new JugadorEstadisticas
                        {
                            NombreJugador = nombreJugador,
                            NumeroVictorias = estadisticasJugador.NumeroVictorias
                        };

                        Estadisticas.Add(jugadorEstadisticas);
                    }

                    this.DataContext = this;
                }
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
               // NavigationService.Navigate(new XAMLInicioSesion());
            }

            catch (FaultException)
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
             //   NavigationService.Navigate(new XAMLInicioSesion());
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

        private string ObtenerNombreUsuarioPorIdJugador(int idJugador)
        {
            GestionCuentaServicioClient servicio = new GestionCuentaServicioClient();
            return servicio.ObtenerNombreUsuarioPorIdJugador(idJugador);
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
                NavigationService.GoBack();   
        }
    }

    public class JugadorEstadisticas
    {
        public string NombreJugador { get; set; }
        public int NumeroVictorias { get; set; }
    }
}
