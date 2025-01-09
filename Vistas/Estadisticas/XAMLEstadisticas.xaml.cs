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

        private static string ObtenerNombreUsuarioPorIdJugador(int idJugador)
        {
            string regreso = string.Empty;
            try
            {
                GestionCuentaServicioClient servicio = new GestionCuentaServicioClient();
                regreso = servicio.ObtenerNombreUsuarioPorIdJugador(idJugador);
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
            return regreso;
        }

        private void BtnAtras_Click(object sender, RoutedEventArgs e)
        {
                NavigationService.GoBack();   
        }
    }

  
}
