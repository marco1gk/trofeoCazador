using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using trofeoCazador.ServicioDelJuego;
using System.Windows.Media.Animation;
using System.Windows.Input;
using trofeoCazador.Recursos;
using System.ServiceModel;
using System.Threading.Tasks;
using trofeoCazador.Recursos.ElementosPartida;
using System.Windows.Media;

namespace trofeoCazador.Vistas.PartidaJuego
{
    public partial class XAMLTablero : Page, IServicioPartidaCallback
    {
        private ServicioPartidaClient cliente;
        private List<JugadorPartida> jugadores;
        private string idPartida;
        private JugadorDataContract jugadorActual = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        private Mazo mazo;
        private Dictionary<string, ObservableCollection<Carta>> cartasDeJugadores = new Dictionary<string, ObservableCollection<Carta>>();
        public ObservableCollection<Ficha> Fichas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEnMano { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Carta> CartasDescarte { get; set; } = new ObservableCollection<Carta>();
        private ObservableCollection<Carta> CartasEnMazo { get; set; } = new ObservableCollection<Carta>();
        private ObservableCollection<Carta> CartasEnEscondite { get; set; } = new ObservableCollection<Carta>();

        private string jugadorTurnoActual;
        private Dado dado;

        public XAMLTablero(List<JugadorPartida> jugadores, string idPartida)
        {
            InitializeComponent();
            SetupClient();
            this.jugadores = jugadores;
            mazo = new Mazo();
            mazo.InicializarMazo();
            CargarFichas();
            dado = new Dado(DadoImagen);
            dado.DadoLanzado += ManejarResultadoDado;
            this.idPartida = idPartida;
            MostrarJugadores();
            cliente.RegistrarJugador(jugadorActual.NombreUsuario);
            DadoImagen.IsEnabled = false;
            FichasItemsControl.ItemsSource = Fichas;
            FichasManoItemsControl.ItemsSource = FichasEnMano;
            ZonaDescarte.ItemsSource = CartasDescarte;
            CartasManoItemsControl.ItemsSource = cartasDeJugadores;
            ZonaMazoCartas.ItemsSource = CartasEnMazo;
            ZonaMazoCartas.IsEnabled = false;
            FichasManoItemsControl.IsEnabled = false;
            CartasEsconditeItemsControl.ItemsSource = CartasEnEscondite;
            CartasManoItemsControl.IsEnabled = false;
            //ZonaFichasJugador2.ItemsSource = FichasEnMano;
            //ZonaFichasJugador3.ItemsSource = FichasEnMano;
            //ZonaFichasJugador4.ItemsSource = FichasEnMano;

        }

        private void SetupClient()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            cliente = new ServicioPartidaClient(instanceContext);
        }

        public void MostrarJugadores()
        {
            var jugadoresEnPartida = jugadores
                .Where(j => j.NombreUsuario != jugadorActual.NombreUsuario)
                .ToList();

            var areasJugadores = new[]
            {
                new { Nombre = NombreJugador2, Imagen = Jugador2Imagen, Area = AreaJugador2 },
                new { Nombre = NombreJugador3, Imagen = Jugador3Imagen, Area = AreaJugador3 },
                new { Nombre = NombreJugador4, Imagen = Jugador4Imagen, Area = AreaJugador4 }
            };

            for (int i = 0; i < jugadoresEnPartida.Count && i < areasJugadores.Length; i++)
            {
                var jugador = jugadoresEnPartida[i];
                var area = areasJugadores[i];

                area.Nombre.Text = jugador.NombreUsuario;
                string rutaImagen = ObtenerRutaImagenPerfil(jugador.NumeroFotoPerfil);
                area.Imagen.Source = new BitmapImage(new Uri(rutaImagen, UriKind.Relative));
                area.Area.Visibility = Visibility.Visible;
            }
        }


        private void BtnClicIniciarJuego(object sender, RoutedEventArgs e)
        {
            try
            {
                cliente.CrearPartida(jugadores.ToArray(), idPartida);
                cliente.EmpezarTurno(idPartida);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la partida: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //CARTAS

        private void BarajarYRepartirCartas(List<JugadorPartida> jugadores)
        {
            BarajarMazo();
            RepartirCartasAJugadores(jugadores);
            ConfigurarCartasDeMano();
            AgregarCartasAPilaMazo(mazo);
        }

        private void BarajarMazo()
        {
            mazo.Barajar();
        }

        private void RepartirCartasAJugadores(List<JugadorPartida> jugadores)
        {
            int[] cartasPorJugador = { 3, 4, 5, 6 };

            for (int i = 0; i < jugadores.Count; i++)
            {
                var jugador = jugadores[i];
                int cantidadCartas = cartasPorJugador[i];

                cartasDeJugadores[jugador.NombreUsuario] = new ObservableCollection<Carta>();

                var cartasRepartidas = mazo.RepartirCartas(cantidadCartas);
                foreach (var carta in cartasRepartidas)
                {
                    AsignarCartaAJugador(carta);
                }
            }
        }

        private void ConfigurarCartasDeMano()
        {
            if (cartasDeJugadores.ContainsKey(jugadorActual.NombreUsuario))
            {
                CartasManoItemsControl.ItemsSource = cartasDeJugadores[jugadorActual.NombreUsuario];
            }
        }

        private void AgregarCartasAPilaMazo(Mazo mazo)
        {
            mazo.Barajar();

            foreach (var carta in mazo.Cartas)
            {
                double desplazamiento = CartasEnMazo.Count/4;
                carta.PosicionX = desplazamiento;
                carta.PosicionY = desplazamiento;
                CartasEnMazo.Add(carta);
            }            
        }

        private Thickness margenInicialCarta = new Thickness();

        private void CartaMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                if (margenInicialCarta == new Thickness())
                {
                    margenInicialCarta = border.Margin;
                }

                ThicknessAnimation animacionMargen = new ThicknessAnimation
                {
                    From = border.Margin,
                    To = new Thickness(margenInicialCarta.Left, margenInicialCarta.Top - 20, margenInicialCarta.Right, margenInicialCarta.Bottom),
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                border.BeginAnimation(Border.MarginProperty, animacionMargen);
            }
        }

        private void CartaMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                ThicknessAnimation animacionMargen = new ThicknessAnimation
                {
                    From = border.Margin,
                    To = margenInicialCarta,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                border.BeginAnimation(Border.MarginProperty, animacionMargen);
            }
        }

        private void CartaMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                border.BorderBrush = border.BorderBrush == Brushes.Blue ? Brushes.Transparent : Brushes.Blue;
                border.BorderThickness = new Thickness(border.BorderThickness == new Thickness(0) ? 2 : 0);
            }
        }

        //FICHAS

        private void CargarFichas()
        {
            for (int i = 1; i <= 6; i++)
            {
                Fichas.Add(new Ficha { IdFicha = i, RutaImagenFicha = $"/Recursos/ElementosPartida/ImagenesPartida/Fichas/Ficha{i}.png" });
            }
        }

        private void ResetearFichas()
        {
            foreach (var ficha in FichasEnMano.ToList())
            {
                Fichas.Add(ficha);
            }

            FichasEnMano.Clear();

        }

        /*private void ActualizarZonaFichas()
        {
            Metodos.MostrarMensaje($"Nombre jugador 2: {NombreJugador2.Text}");
            if (NombreJugador2.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador2.ItemsSource = FichasEnMano;
            }
            else if (NombreJugador3.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador3.ItemsSource = FichasEnMano;
            }
            else if (NombreJugador4.Text == jugadorTurnoActual)
            {
                ZonaFichasJugador4.ItemsSource = FichasEnMano;
            }
        }*/

        //DADO

        private void ImagenDado_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cliente.LanzarDado(idPartida, jugadorActual.NombreUsuario);
        }
        private string ObtenerRutaImagenPerfil(int numeroFotoPerfil)
        {
            switch (numeroFotoPerfil)
            {
                case 1:
                    return "/Recursos/FotosPerfil/abeja.jpg";
                case 2:
                    return "/Recursos/FotosPerfil/cazador.jpg";
                default:
                    return "/Recursos/FotosPerfil/cazador.jpg";
            }
        }

        private async void ManejarResultadoDado(int resultadoDado)
        {
            await Task.Delay(1000);
            Ficha fichaSeleccionada = SeleccionarFichaPorResultado(resultadoDado);

            if (jugadorTurnoActual == jugadorActual.NombreUsuario)
            {
                if (PuedeTomarFicha(fichaSeleccionada))
                {
                    await TomarFicha(fichaSeleccionada);
                }
                else
                {
                    await ManejarFichaDuplicada();
                }
            }
        }

        private Ficha SeleccionarFichaPorResultado(int resultadoDado)
        {
            return Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);
        }

        private bool PuedeTomarFicha(Ficha fichaSeleccionada)
        {
            return fichaSeleccionada != null && !FichasEnMano.Contains(fichaSeleccionada);
        }

        private async Task TomarFicha(Ficha fichaSeleccionada)
        {
            FichasEnMano.Add(fichaSeleccionada);
            Fichas.Remove(fichaSeleccionada);
            await Task.Delay(1000);

            MessageBoxResult decision = MostrarDialogoDecision();

            if (decision != MessageBoxResult.Yes)
            {
                DadoImagen.IsEnabled = false;
                await ResolverFichas();
                await FinalizarTurno();
            }
        }

        private MessageBoxResult MostrarDialogoDecision()
        {
            return MessageBox.Show(
                "Obtuviste una ficha. ¿Quieres continuar o parar?",
                "Decisión de turno",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
        }

        private async Task ManejarFichaDuplicada()
        {
            await Task.Delay(1000);
            bool tieneBlammo = VerificarCartaEnMano("Carta6");
            bool tieneNanners = VerificarCartaEnMano("Carta5");

            if (tieneBlammo || tieneNanners)
            {
                MostrarOpcionesDeCartas(tieneBlammo, tieneNanners);
            }
            else
            {
                await ManejarTurnoSinOpciones();
            }
        }

        private bool VerificarCartaEnMano(string tipoCarta)
        {
            return cartasDeJugadores[jugadorActual.NombreUsuario].Any(c => c.Tipo == tipoCarta);
        }

        private async Task ManejarTurnoSinOpciones()
        {
            Metodos.MostrarMensaje("Esta ficha ya la tenías, por lo que termina tu turno. Pero puedes tomar una carta del mazo.");
            ZonaMazoCartas.IsEnabled = true;

            bool cartaTomadaDelMazo = await EsperarTomaDeCarta();

            ZonaMazoCartas.IsEnabled = false;
            await FinalizarTurno();
        }

        private async Task<bool> EsperarTomaDeCarta()
        {
            bool cartaTomada = false;

            ZonaMazoCartas.MouseDown += (s, e) =>
            {
                cartaTomada = true;
            };

            while (!cartaTomada)
            {
                await Task.Delay(100);
            }

            return cartaTomada;
        }

        private async Task FinalizarTurno()
        {
            await Task.Run(() => cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario));
        }


        private Task ResolverFichas()
        {
            FichasManoItemsControl.IsEnabled = true;
            return Task.Run(async () =>
            {
                while (FichasEnMano.Count > 0)
                {
                    await Task.Delay(100);
                }
                //FichasManoItemsControl.IsEnabled = false;
            });
            
        }


        private void MostrarOpcionesDeCartas(bool tieneBlammo, bool tieneNanners)
        {
            var mensaje = "Obtuviste una ficha repetida, puedes usar una carta para salvarte:\n";
            if (tieneBlammo) mensaje += "- Blammo: Repetir tirada.\n";
            if (tieneNanners) mensaje += "- Nanners: Ignorar tirada.\n";

            var respuesta = MessageBox.Show(mensaje, "Opciones", MessageBoxButton.YesNoCancel);

            if (respuesta == MessageBoxResult.Yes && tieneBlammo)
            {
                UsarCarta("Carta6");
            }
            else if (respuesta == MessageBoxResult.No && tieneNanners)
            {
                UsarCarta("Carta5");
            }
            else
            {
                cliente.FinalizarTurno(idPartida, jugadorActual.NombreUsuario);
            }
        }

        private void UsarCarta(string tipoCarta)
        {
            var carta = cartasDeJugadores[jugadorActual.NombreUsuario].FirstOrDefault(c => c.Tipo == tipoCarta);
            if (carta != null)
            {
                cartasDeJugadores[jugadorActual.NombreUsuario].Remove(carta);
                AgregarCartaAPilaDescarte(carta);
                MessageBox.Show($"Usaste una carta {tipoCarta}.");
            }
        }

        private void AgregarCartaAPilaDescarte(Carta carta)
        {
            double desplazamiento = CartasDescarte.Count * 2;
            carta.PosicionX = desplazamiento;
            carta.PosicionY = desplazamiento;

            CartasDescarte.Add(carta);
        }

        private void SeleccionarCarta(object sender, MouseButtonEventArgs e)
        {
            var cartaSeleccionada = ObtenerCartaSeleccionada(sender);
            if (cartaSeleccionada != null)
            {
                MoverCartaAEscondite(cartaSeleccionada);
            }
            else
            {
                Metodos.MostrarMensaje("Debe seleccionar una carta");
            }
        }

        private Carta ObtenerCartaSeleccionada(object sender)
        {
            return (sender as Border)?.DataContext as Carta;
        }

        private void MoverCartaAEscondite(Carta carta)
        {
            cartasDeJugadores[jugadorActual.NombreUsuario].Remove(carta);
            CartasEnEscondite.Add(carta);
        }


        private void Mazo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EsMazoVacio())
            {
                Metodos.MostrarMensaje("No hay cartas en el mazo.");
            }
            else
            {
                var cartaSuperior = ObtenerCartaSuperiorDelMazo();
                AsignarCartaAJugador(cartaSuperior);
            }
        }

        private bool EsMazoVacio()
        {
            return CartasEnMazo == null || !CartasEnMazo.Any();
        }

        private Carta ObtenerCartaSuperiorDelMazo()
        {
            var cartaSuperior = CartasEnMazo.Last();
            CartasEnMazo.Remove(cartaSuperior);
            return cartaSuperior;
        }

        private void AsignarCartaAJugador(Carta carta)
        {
            if (cartasDeJugadores.ContainsKey(jugadorActual.NombreUsuario))
            {
                cartasDeJugadores[jugadorActual.NombreUsuario].Add(carta);
            }
            else
            {
                cartasDeJugadores[jugadorActual.NombreUsuario] = new ObservableCollection<Carta> { carta };
            }
        }


        private void Ficha_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fichaSeleccionada = (sender as Border)?.DataContext as Ficha;
            if (fichaSeleccionada != null)
            {
                EjecutarAccionFicha(fichaSeleccionada);
            }
        }

        private void EjecutarAccionFicha(Ficha fichaSeleccionada)
        {
            switch (fichaSeleccionada.IdFicha)
            {
                case 1:
                    AccionFichaTomarCartasMazo(fichaSeleccionada);
                    break;

                case 2:
                    AccionFichaGuardarCartasEscondite(fichaSeleccionada);
                    break;

                case 3:
                    AccionFichaRobarOGuardar(fichaSeleccionada);
                    break;

                case 4:
                    AccionFichaRobarCartaAJugador(fichaSeleccionada);
                    break;

                case 5:
                    AccionFichaRevelarCarta(fichaSeleccionada);
                    break;

                case 6:
                    AccionFichaCambiarFicha(fichaSeleccionada);
                    break;
                default:
                    Metodos.MostrarMensaje("Opcion incorrecta.");
                    break;
            }
        }

        private void AccionFichaCambiarFicha(Ficha fichaSeleccionada)
        {
            if (Fichas.Count > 0)
            {
                Metodos.MostrarMensaje("Selecciona una ficha disponible en la mesa para intercambiarla.");

                FichasItemsControl.IsEnabled = true;
                FichasItemsControl.MouseDown += async (s, e) =>
                {
                    var fichaEnMesa = (e.OriginalSource as FrameworkElement)?.DataContext as Ficha;

                    if (fichaEnMesa != null)
                    {
                        FichasEnMano.Add(fichaEnMesa);
                        Fichas.Remove(fichaEnMesa);
                        FichaResuelta(fichaSeleccionada);

                        Metodos.MostrarMensaje($"Intercambiaste la ficha {fichaSeleccionada.IdFicha} por la ficha {fichaEnMesa.IdFicha}.");
                        FichasItemsControl.IsEnabled = false;
                    }
                };

            }
            else
            {
                Metodos.MostrarMensaje("Esta ficha no tiene efecto ya que no queda alguna ficha por la cual intercambiarla.");
                FichaResuelta(fichaSeleccionada);
            }

            
        }


        private void AccionFichaRevelarCarta(Ficha fichaSeleccionada)
        {
            FichaResuelta(fichaSeleccionada);

        }
        private void AccionFichaRobarCartaAJugador(Ficha fichaSeleccionada)
        {
            FichaResuelta(fichaSeleccionada);
        }

        private void AccionFichaRobarOGuardar(Ficha fichaSeleccionada)
        {
            Metodos.MostrarMensaje("Puedes hacer 1 de las siguientes opciones:\n1) Robar una carta del mazo\n2) Guardar una carta en el escondite.");
            ZonaMazoCartas.IsEnabled = true;
            CartasManoItemsControl.IsEnabled = true;

            bool accionRealizada = false;

            ZonaMazoCartas.MouseDown += async (sender, args) =>
            {
                if (!accionRealizada)
                {
                    if (CartasEnMazo.Any())
                    {
                        Metodos.MostrarMensaje("Has robado una carta del mazo.");
                        accionRealizada = true;

                        FichaResuelta(fichaSeleccionada);

                        ZonaMazoCartas.IsEnabled = false;
                        CartasManoItemsControl.IsEnabled = false;
                    }
                    else
                    {
                        Metodos.MostrarMensaje("No hay cartas en el Mazo.");
                    }
                }
            };

            CartasManoItemsControl.MouseDown += async (sender, args) =>
            {
                if (!accionRealizada)
                {
                    if (cartasDeJugadores[jugadorActual.NombreUsuario].Any())
                    {
                        Metodos.MostrarMensaje("Has guardado una carta en el escondite.");
                        accionRealizada = true;

                        FichaResuelta(fichaSeleccionada);

                        ZonaMazoCartas.IsEnabled = false;
                        CartasManoItemsControl.IsEnabled = false;
                    }
                    else
                    {
                        Metodos.MostrarMensaje("No tienes cartas para guardar");
                    }
                }
            };
        }


        private void AccionFichaGuardarCartasEscondite(Ficha fichaSeleccionada)
        {
            if (cartasDeJugadores[jugadorActual.NombreUsuario].Count >= 2)
            {
                Metodos.MostrarMensaje("Puedes guardar 2 cartas en el escondite, selecciona las que deseas agregar al escondite.");
                CartasManoItemsControl.IsEnabled = true;
                int cartasGuardadasEscondite = 0;

                CartasManoItemsControl.MouseDown += async (s, e) =>
                {
                    cartasGuardadasEscondite++;
                    if (cartasGuardadasEscondite == 2)
                    {
                        CartasManoItemsControl.IsEnabled = false;
                        FichaResuelta(fichaSeleccionada);
                    }
                };
            }
            else
            {
                Metodos.MostrarMensaje("No hay cartas suficientes en la mano.");
            }
        }

        private void AccionFichaTomarCartasMazo(Ficha fichaSeleccionada)
        {
            if(CartasEnMazo.Count >= 2)
            {
                Metodos.MostrarMensaje("Puedes tomar 2 cartas del Mazo, da clic 2 veces en el.");
                ZonaMazoCartas.IsEnabled = true;
                int cartasTomadasMazo = 0;

                ZonaMazoCartas.MouseDown += async (s, e) =>
                {
                    cartasTomadasMazo++;
                    if (cartasTomadasMazo == 2)
                    {
                        ZonaMazoCartas.IsEnabled = false;
                        FichaResuelta(fichaSeleccionada);
                    }
                };

                
            }
            else
            {
                Metodos.MostrarMensaje("No hay cartas suficientes en el Mazo.");
            }
            
        }

        private void FichaResuelta(Ficha fichaSeleccionada)
        {
            Fichas.Add(fichaSeleccionada);
            FichasEnMano.Remove(fichaSeleccionada);
        }

        private void VerEscondite_Click(object sender, RoutedEventArgs e)
        {
            if (CartasEsconditeItemsControl.Visibility == Visibility.Collapsed)
            {
                CartasEsconditeItemsControl.Visibility = Visibility.Visible;

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
                CartasEsconditeItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
            else
            {
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
                fadeOut.Completed += (s, ev) => CartasEsconditeItemsControl.Visibility = Visibility.Collapsed;
                CartasEsconditeItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        //NOTIFICACIONES
        public void NotificarTurnoIniciado(string nombreUsuario)
        {
            ResetearFichas();
            jugadorTurnoActual = nombreUsuario;
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = true;
            }
            else
            {
                DadoImagen.IsEnabled = false;
            }
        }
        public void NotificarTurnoTerminado(string nombreUsuario)
        {
            if (nombreUsuario == jugadorActual.NombreUsuario)
            {
                DadoImagen.IsEnabled = false;
                ResetearFichas();
            }
        }

        public void NotificarResultadoAccion(string action, bool success)
        {
            //TODO
        }

        public void NotificarPartidaCreada(string idPartida)
        {
            this.idPartida = idPartida;
            BarajarYRepartirCartas(jugadores);
        }

        public void NotificarResultadoDado(string nombreUsuario, int resultadoDado)
        {
            dado.LanzarDado(resultadoDado);
        }

        
    }
}


