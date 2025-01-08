using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.Menu;
using trofeoCazador.Vistas.SalaEspera;

namespace trofeoCazador.Vistas.Victoria
{
    public partial class XAMLVictoria : Page
    {
        private readonly KeyValuePair<JugadorDataContract, int>[] marcador;
        private SingletonSesion sesion = SingletonSesion.Instancia;
        private readonly int puntajeGanador;
        private int idGanador;
        private readonly string invitado="Invitado";

        public XAMLVictoria(string idPartida, KeyValuePair<JugadorDataContract, int>[] marcador, int puntajeGanador)
        {
            InitializeComponent();
            this.marcador = marcador;
            this.puntajeGanador = puntajeGanador;
            MostrarResultados();
            PosicionesJuego();
        }
        private void ActualizarVictorias(int idJugador)
        {
            if (idJugador > 0)
            {
                EstadisticasGlobalesClient marcadorGestor = new EstadisticasGlobalesClient();

                try
                {
                    marcadorGestor.ActualizarVictorias(idJugador);
                    Console.WriteLine("Las victorias se actualizaron correctamente.");
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
                }
                catch (FaultException)
                {
                    VentanasEmergentes.CrearMensajeVentanaServidorError();
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
                finally
                {
                    if (marcadorGestor.State == System.ServiceModel.CommunicationState.Faulted)
                    {
                        marcadorGestor.Abort();
                    }
                    else
                    {
                        marcadorGestor.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine("ID del jugador no válido.");
            }
        }



        private void MostrarResultados()
        {
            stackPanelMarcador.Children.Clear();
            bool ganador = false; 

            for (int i = 0; i < marcador.Length; i++)
            {
                var jugador = marcador[i];
                var grid = new Grid { Height = 72 };

                var textBlockPosicion = new TextBlock
                {
                    Text = $"{i + 1}°",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontFamily = new FontFamily("Titan One"),
                    FontSize = 42,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                var border = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    Background = Brushes.White,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(7),
                    Padding = new Thickness(5),
                    Width = 425,
                    Height = 65,
                    HorizontalAlignment = HorizontalAlignment.Right,
                };

                var gridBorde = new Grid();
                var nombreUsuario = new TextBlock
                {
                    Text = jugador.Key.NombreUsuario,
                    FontFamily = new FontFamily("Inter"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 24,
                    Margin = new Thickness(17, 0, 68, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var puntos = new TextBlock
                {
                    Text = $"{jugador.Value}",
                    FontFamily = new FontFamily("Inter"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 24,
                    Margin = new Thickness(235, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                gridBorde.Children.Add(nombreUsuario);
                gridBorde.Children.Add(puntos);
                border.Child = gridBorde;
                grid.Children.Add(textBlockPosicion);
                grid.Children.Add(border);
                stackPanelMarcador.Children.Add(grid);


                if (jugador.Value == puntajeGanador && !ganador)
                {
                    ganador = true; 
                    if (jugador.Key.NombreUsuario == sesion.NombreUsuario)
                    {
                        lbnombreUsuario.Content = sesion.NombreUsuario;
                        lbVictoria.Visibility = Visibility.Visible; 
                        lbDerrota.Visibility = Visibility.Collapsed; 
                    }
                    else if (jugador.Key.NombreUsuario == invitado)
                    {
                        lbnombreUsuario.Content = invitado;
                        lbVictoria.Visibility = Visibility.Visible; 
                        lbDerrota.Visibility = Visibility.Collapsed; 
                    }
                }
                else
                {
                    if (jugador.Key.NombreUsuario == sesion.NombreUsuario)
                    {
                        lbnombreUsuario.Content = sesion.NombreUsuario;
                        lbDerrota.Visibility = Visibility.Visible;
                        lbVictoria.Visibility = Visibility.Collapsed; 
                    }
                    else if (jugador.Key.NombreUsuario == invitado)
                    {
                        lbnombreUsuario.Content = invitado;
                        lbDerrota.Visibility = Visibility.Visible;
                        lbVictoria.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                       
                        lbnombreUsuario.Content = jugador.Key.NombreUsuario;
                        lbDerrota.Visibility = Visibility.Visible; 
                        lbVictoria.Visibility = Visibility.Collapsed; 
                    }
                }
            }
        }

        private void PosicionesJuego()
        {
            var marcadorOrdenado = marcador
                .OrderByDescending(kv => kv.Value)
                .ToList();

            bool ganadorEncontrado = false; 

            for (int i = 0; i < marcadorOrdenado.Count; i++)
            {
                var jugador = marcadorOrdenado[i].Key.NombreUsuario;
                var idJugador = marcadorOrdenado[i].Key.JugadorId;
                var puntaje = marcadorOrdenado[i].Value;

                if (puntaje == puntajeGanador && !ganadorEncontrado)
                {
                    idGanador = idJugador;
                    ActualizarVictorias(idGanador);
                    ganadorEncontrado = true;
                }
            }
        }
        

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new XAMLSalaEspera());
        }
    }
}
