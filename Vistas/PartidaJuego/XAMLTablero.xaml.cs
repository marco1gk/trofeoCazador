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
using System.Threading;

namespace trofeoCazador.Vistas.PartidaJuego
{
    public partial class XAMLTablero : Page, IServicioPartidaCallback
    {
        private ServicioPartidaClient cliente;
        private List<JugadorPartida> jugadores;
        private string idPartida;
        private JugadorDataContract jugadorActual = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        private ObservableCollection<CartaCliente> CartasEnMano { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<Ficha> Fichas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEstaticas { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<Ficha> FichasEnMano { get; set; } = new ObservableCollection<Ficha>();
        private ObservableCollection<CartaCliente> CartasDescarte { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<CartaCliente> CartasEnMazo { get; set; } = new ObservableCollection<CartaCliente>();
        private ObservableCollection<CartaCliente> CartasEnEscondite { get; set; } = new ObservableCollection<CartaCliente>();

        private Dado dado;
        private enum ModoSeleccionCarta
        {
            MoverAlEscondite,
            DefenderRobo
        }

        private ModoSeleccionCarta modoSeleccionActual;

        public XAMLTablero(List<JugadorPartida> jugadores, string idPartida)
        {
            if (!Thread.CurrentThread.IsBackground && Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new InvalidOperationException("El constructor XAMLTablero debe ejecutarse en un subproceso STA.");
            }

            Console.WriteLine("Entra al método que causa error");
            InitializeComponent();
            SetupClient();

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.jugadores = jugadores;
                CargarFichasEstaticas();
                dado = new Dado(DadoImagen);
                this.idPartida = idPartida;
                MostrarJugadores();
                cliente.RegistrarJugador(jugadorActual.NombreUsuario);

                FichasItemsControl.ItemsSource = Fichas;
                FichasManoItemsControl.ItemsSource = FichasEnMano;
                ZonaDescarte.ItemsSource = CartasDescarte;
                CartasManoItemsControl.ItemsSource = CartasEnMano;
                ZonaMazoCartas.ItemsSource = CartasEnMazo;
                CartasEsconditeItemsControl.ItemsSource = CartasEnEscondite;
            });
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
                cliente.RepartirCartas(idPartida);
                cliente.EmpezarTurno(idPartida);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la partida: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void CargarFichasEstaticas()
        {
            for (int i = 1; i <= 6; i++)
            {
                FichasEstaticas.Add(new Ficha { IdFicha = i, RutaImagenFicha = $"/Recursos/ElementosPartida/ImagenesPartida/Fichas/Ficha{i}.png" });
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
            
            if (PuedeTomarFicha(fichaSeleccionada))
            {
                await TomarFicha(fichaSeleccionada);
            }
            else
            {
                await ManejarFichaDuplicada();
            }
        }

        private Ficha SeleccionarFichaPorResultado(int resultadoDado)
        {
            return Fichas.FirstOrDefault(f => f.IdFicha == resultadoDado);
        }

        private bool PuedeTomarFicha(Ficha fichaSeleccionada)
        {
            return fichaSeleccionada != null;
        }

        private async Task TomarFicha(Ficha fichaSeleccionada)
        {
            cliente.TomarFichaMesa(idPartida, fichaSeleccionada.IdFicha);
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
                "Obtuviste una ficha. ¿Quieres continuar tirando?",
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
            return CartasEnMano.Any(c => c.Tipo == tipoCarta);
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
            var carta = CartasEnMano.FirstOrDefault(c => c.Tipo == tipoCarta);
            if (carta != null)
            {
                cliente.UtilizarCarta(idPartida, carta.IdCarta, jugadorActual.NombreUsuario);
                MessageBox.Show($"Usaste una carta {tipoCarta}.");
            }
        }

        private void AgregarCartaAPilaDescarte(CartaCliente carta)
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
                switch (modoSeleccionActual)
                {
                    case ModoSeleccionCarta.MoverAlEscondite:
                        MoverCartaAEscondite(cartaSeleccionada);
                        break;
                    case ModoSeleccionCarta.DefenderRobo:
                        UsarCartaBloqueoRobo(cartaSeleccionada);
                        break;
                    default:
                        Metodos.MostrarMensaje("Acción no válida.");
                        break;
                }
            }
            else
            {
                Metodos.MostrarMensaje("Debe seleccionar una carta");
            }
        }

        private CartaCliente ObtenerCartaSeleccionada(object sender)
        {
            return (sender as Border)?.DataContext as CartaCliente;
        }

        private void MoverCartaAEscondite(CartaCliente carta)
        {
            cliente.AgregarCartaAEscondite(jugadorActual.NombreUsuario, carta.IdCarta, idPartida);
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
                cliente.TomarCartaDeMazo(idPartida, jugadorActual.NombreUsuario, cartaSuperior.IdCarta);
            }
        }

        private bool EsMazoVacio()
        {
            return CartasEnMazo == null || !CartasEnMazo.Any();
        }

        private CartaCliente ObtenerCartaSuperiorDelMazo()
        {
            var cartaSuperior = CartasEnMazo.Last();
            return cartaSuperior;
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
                    var fichaEnMesaSeleccionada = (e.OriginalSource as FrameworkElement)?.DataContext as Ficha;

                    if (fichaEnMesaSeleccionada != null)
                    {
                        cliente.TomarFichaMesa(idPartida, fichaEnMesaSeleccionada.IdFicha);
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);

                        Metodos.MostrarMensaje($"Intercambiaste la ficha {fichaSeleccionada.IdFicha} por la ficha {fichaEnMesaSeleccionada.IdFicha}.");
                        FichasItemsControl.IsEnabled = false;
                    }
                };

            }
            else
            {
                Metodos.MostrarMensaje("Esta ficha no tiene efecto ya que no queda alguna ficha por la cual intercambiarla.");
                cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
            }

            
        }

        private void HabilitarClickEnAreasJugadores(bool habilitar)
        {
            AreaJugador2.IsEnabled = habilitar;
            AreaJugador3.IsEnabled = habilitar;
            AreaJugador4.IsEnabled = habilitar;
        }

        private void RobarCartaDeJugador(object sender, MouseButtonEventArgs e)
        {
            var areaSeleccionada = sender as StackPanel;
            if (areaSeleccionada != null)
            {
                var jugadorObjetivo = ObtenerJugadorDesdeArea(areaSeleccionada);
                if (jugadorObjetivo != null)
                {
                    // Deshabilitar clicks para evitar múltiples selecciones
                    HabilitarClickEnAreasJugadores(false);
                    cliente.RobarCartaAJugador(jugadorObjetivo.NombreUsuario, idPartida);
                }
            }
        }

        private JugadorPartida ObtenerJugadorDesdeArea(StackPanel area)
        {
            if (area == AreaJugador2) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador2.Text);
            if (area == AreaJugador3) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador3.Text);
            if (area == AreaJugador4) return jugadores.FirstOrDefault(j => j.NombreUsuario == NombreJugador4.Text);
            return null;
        }

        private void AccionFichaRevelarCarta(Ficha fichaSeleccionada)
        {
            cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);

        }
        private void AccionFichaRobarCartaAJugador(Ficha fichaSeleccionada)
        {
            // Desuscribirse para evitar duplicados
            AreaJugador2.MouseDown -= RobarCartaDeJugador;
            AreaJugador3.MouseDown -= RobarCartaDeJugador;
            AreaJugador4.MouseDown -= RobarCartaDeJugador;

            // Mostrar mensaje y habilitar áreas
            Metodos.MostrarMensaje("Haz clic en el jugador al que deseas robar una carta.");
            HabilitarClickEnAreasJugadores(true);

            // Asignar eventos una vez
            AreaJugador2.MouseDown += RobarCartaDeJugador;
            AreaJugador3.MouseDown += RobarCartaDeJugador;
            AreaJugador4.MouseDown += RobarCartaDeJugador;
        }

        private void AccionFichaRobarOGuardar(Ficha fichaSeleccionada)
        {
            Metodos.MostrarMensaje("Puedes hacer 1 de las siguientes opciones:\n1) Robar una carta del mazo\n2) Guardar una carta en el escondite.");
            ZonaMazoCartas.IsEnabled = true;
            CartasManoItemsControl.IsEnabled = true;
            modoSeleccionActual = ModoSeleccionCarta.MoverAlEscondite;

            bool accionRealizada = false;

            ZonaMazoCartas.MouseDown += async (sender, args) =>
            {
                if (!accionRealizada)
                {
                    if (CartasEnMazo.Any())
                    {
                        Metodos.MostrarMensaje("Has robado una carta del mazo.");
                        accionRealizada = true;

                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);

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
                    if (CartasEnMano.Any())
                    {
                        Metodos.MostrarMensaje("Has guardado una carta en el escondite.");
                        accionRealizada = true;

                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);

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
            if (CartasEnMano.Count >= 2)
            {
                Metodos.MostrarMensaje("Puedes guardar 2 cartas en el escondite, selecciona las que deseas agregar al escondite.");
                CartasManoItemsControl.IsEnabled = true;
                modoSeleccionActual = ModoSeleccionCarta.MoverAlEscondite;
                int cartasGuardadasEscondite = 0;

                CartasManoItemsControl.MouseDown += async (s, e) =>
                {
                    cartasGuardadasEscondite++;
                    if (cartasGuardadasEscondite == 2)
                    {
                        CartasManoItemsControl.IsEnabled = false;
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
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
                        cliente.DevolverFichaAMesa(fichaSeleccionada.IdFicha, idPartida);
                    }
                };

                
            }
            else
            {
                Metodos.MostrarMensaje("No hay cartas suficientes en el Mazo.");
            }
            
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

        /*public Mazo CrearMazoCartas(List<int> idCartas)
        {
            Mazo mazoActual = new Mazo();
            foreach (var idCarta in idCartas)
            {
                var carta = mazo.Cartas.FirstOrDefault(c => c.IdCarta == idCarta);
                mazoActual.Cartas.Add(carta);
            }
            return mazoActual;
        }*/

        //NOTIFICACIONES
        public void NotificarTurnoIniciado(string nombreUsuario)
        {
            dado.DadoLanzado -= ManejarResultadoDado;
            dado.DadoLanzado += ManejarResultadoDado;
            DadoImagen.IsEnabled = true;
        }
        public void NotificarTurnoTerminado(string nombreUsuario)
        {
            foreach (var ficha in FichasEnMano)
            {
                cliente.DevolverFichaAMesa(ficha.IdFicha, idPartida);
            }
            DadoImagen.IsEnabled = false;
            dado.DadoLanzado -= ManejarResultadoDado;
            
        }

        public void NotificarResultadoAccion(string action, bool success)
        {
            //TODO
        }

        public void NotificarPartidaCreada(string idPartida)
        {
            this.idPartida = idPartida;
            DadoImagen.IsEnabled = false;
            ZonaMazoCartas.IsEnabled = false;
            FichasManoItemsControl.IsEnabled = false;
            CartasManoItemsControl.IsEnabled = false;
            CargarFichas();
        }

        public void NotificarResultadoDado(string nombreUsuario, int resultadoDado)
        {
            dado.LanzarDado(resultadoDado);
        }
        public void NotificarCartasEnMano(Carta[] cartasRepartidas)
        {
            foreach(var carta in cartasRepartidas)
            {
                CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = carta.RutaImagen, Asignada = true};
                CartasEnMano.Add(cartaCliente);
            }
            foreach(var carta in CartasEnMano)
            {
                Console.WriteLine($"IDCarta: {carta.IdCarta} RutaImagen: {carta.RutaImagen}");
            }
        }

        public void NotificarCartasEnMazo(Carta[] cartasEnMazo)
        {
            foreach(var carta in cartasEnMazo)
            {
                CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = carta.RutaImagen, Asignada = false};
                CartasEnMazo.Add(cartaCliente);
            }
            foreach (var carta in CartasEnMazo)
            {
                Console.WriteLine($"Carta en mazo: {carta.IdCarta}");
            }
        }

        public void NotificarCartaAgregadaAMano(Carta carta)
        {
            CartaCliente cartaCliente = new CartaCliente { Tipo = carta.Tipo, IdCarta = carta.IdCarta, RutaImagen = carta.RutaImagen, Asignada = true };
            CartasEnMano.Add(cartaCliente);
        }

        public void NotificarCartaTomadaMazo(int idCarta)
        {
            var cartaTomada = CartasEnMazo.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasEnMazo.Remove(cartaTomada);
        }

        public void NotificarFichaTomadaMesa(string jugadorTurnoActual, int idFicha)
        {
            //Metodos.MostrarMensaje($"Jugador turno actual: {jugadorTurnoActual}\nJugador Actual: {jugadorActual.NombreUsuario}");
            var fichaSeleccionada = Fichas.FirstOrDefault(f => f.IdFicha == idFicha);
            Fichas.Remove(fichaSeleccionada);

            if(jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                FichasEnMano.Add(fichaSeleccionada);
            }
        }

        public void NotificarCartaAgregadaADescarte(Carta cartaUtilizada)
        {
            CartaCliente carta = new CartaCliente { IdCarta = cartaUtilizada.IdCarta, RutaImagen = cartaUtilizada.RutaImagen, Tipo = cartaUtilizada.Tipo, Asignada = false};
            CartasDescarte.Add(carta);
        }

        public void NotificarCartaUtilizada(int idCartaUtilizada)
        {
            var cartaUtilizada = CartasEnMano.FirstOrDefault(c => c.IdCarta == idCartaUtilizada);
            CartasEnMano.Remove(cartaUtilizada);
        }

        public void NotificarFichaDevuelta(int idFicha, string nombreJugadorTurnoActual)
        {
            if(jugadorActual.NombreUsuario == nombreJugadorTurnoActual)
            {
                var ficha = FichasEnMano.FirstOrDefault(f => f.IdFicha == idFicha);
                FichasEnMano.Remove(ficha);
                Fichas.Add(ficha);
            }
            else
            {
                var fichaDevuelta = FichasEstaticas.FirstOrDefault(f => f.IdFicha == idFicha);
                Fichas.Add(fichaDevuelta);
            }
        }

        public void NotificarCartaAgregadaAEscondite(int idCarta)
        {
            var carta = CartasEnMano.FirstOrDefault(c => c.IdCarta == idCarta);
            CartasEnMano.Remove(carta);
            CartasEnEscondite.Add(carta);
        }

        public void NotificarIntentoRoboCarta(string jugadorObjetivoRobo)
        {
            if (jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                MessageBoxResult decision = DecisionDefenderseRobo();

                if (decision == MessageBoxResult.Yes)
                {
                    CartasManoItemsControl.IsEnabled = true;
                    modoSeleccionActual = ModoSeleccionCarta.DefenderRobo;
                }
                else
                {
                    cliente.RobarCarta(jugadorObjetivoRobo, idPartida);
                }
            }
        }

        public void NotificarCartaRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual)
        {
            if(jugadorActual.NombreUsuario == jugadorObjetivoRobo)
            {
                var cartaRobada = CartasEnMano.FirstOrDefault(c => c.IdCarta == carta.IdCarta);
                CartasEnMano.Remove(cartaRobada);
            }

            if(jugadorActual.NombreUsuario == jugadorTurnoActual)
            {
                CartaCliente cartaRobada = new CartaCliente { IdCarta = carta.IdCarta, Tipo = carta.Tipo, RutaImagen = carta.RutaImagen, Asignada = true};
                CartasEnMano.Add(cartaRobada);
                //cliente.DevolverFichaAMesa(4, idPartida);
            }
            
        }

        private void UsarCartaBloqueoRobo(CartaCliente cartaSeleccionada)
        {
            switch (cartaSeleccionada.Tipo)
            {
                case "Carta7":
                    UsarCarta7(cartaSeleccionada.IdCarta);
                    break;
                case "Carta8":
                    UsarCarta8(cartaSeleccionada);
                    break;
                default:
                    Metodos.MostrarMensaje("La carta seleccionada no sirve para bloquear un robo.");
                    break;

            }
        }

        private void UsarCarta3(int idCarta)
        {




        }

        private void UsarCarta7(int idCarta)
        {
            if (CartasEnMazo.Count >= 2)
            {
                cliente.UtilizarCarta(idPartida, idCarta, jugadorActual.NombreUsuario);
                Metodos.MostrarMensaje("Puedes tomar 2 cartas del Mazo, da clic 2 veces en el.");
                ZonaMazoCartas.IsEnabled = true;
                int cartasTomadasMazo = 0;

                ZonaMazoCartas.MouseDown += async (s, e) =>
                {
                    cartasTomadasMazo++;
                    if (cartasTomadasMazo == 2)
                    {
                        ZonaMazoCartas.IsEnabled = false;
                        //cliente.DevolverFichaAMesa(4, idPartida);
                    }
                };
                
            }
            else
            {
                Metodos.MostrarMensaje("No hay cartas suficientes en el Mazo.");
            }
        }

        private void UsarCarta8(CartaCliente carta)
        {

        }
        private MessageBoxResult DecisionDefenderseRobo()
        {
            return MessageBox.Show(
                "Has sido elegido como objeto de robo. ¿Quieres defenderte?",
                "Decision de defensa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
        }
    }
}


