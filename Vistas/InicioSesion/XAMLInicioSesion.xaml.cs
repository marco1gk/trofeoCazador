﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;
using trofeoCazador.VentanasReutilizables;
using trofeoCazador.Vistas.SalaEspera;
using trofeoCazador.Vistas.Amigos;
using System.ServiceModel;
using trofeoCazador.Vistas.Menu;

namespace trofeoCazador.Vistas.InicioSesion
{
    public partial class XAMLInicioSesion : Page
    {
        private const string FUENTE_SECUNDARIA = "Inter";
        private void BtnUnirseComoInvitado_Click(object sender, RoutedEventArgs e)
        {
            string codigoSala = tbInvitado.Text.Trim();
            if (string.IsNullOrEmpty(codigoSala))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbCodigo, Properties.Resources.lbCodigoError);
                return;
            }

            var salaEspera = new XAMLSalaEspera();
            NavigationService?.Navigate(salaEspera);
            salaEspera.UnirseComoInvitado(codigoSala);
        }

        private void IdiomaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbIdioma.SelectedItem is ComboBoxItem item)
            {
                string codigoCultura = item.Tag.ToString();
                App.CambiarIdioma(codigoCultura);
                this.NavigationService.Refresh();
            }
        }

        private static void BtnCorreo_Click(object sender, RoutedEventArgs e)
        {
            VentanaCorreoJugador ventanaCorreo = new VentanaCorreoJugador();
            ventanaCorreo.Show();
        }

        private void BtnRegistrarCuenta(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return; 
            }
            this.NavigationService.Navigate(new Uri("Vistas/RegistroUsuario/XAMLRegistroUsuario.xaml", UriKind.Relative));
        }

        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OcultarMensajesDeError();

                if (!ValidarCampos())
                {
                    MostrarMensajeCredencialesIncorrectas();
                    return;
                }

                JugadorDataContract jugador = AutenticarUsuario(tbUsuario.Text, tpContraseña.Password);

                if (jugador == null)
                {
                    ManejarCredencialesInvalidas();
                    return;
                }

                if (EsUsuarioEnLinea(jugador.NombreUsuario))
                {
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbUsuarioLinea,Properties.Resources.lbUsuarioYaEstaEnLinea);
                    return;
                }

                IniciarSesion(jugador);
            }
            catch (EndpointNotFoundException )
            {
                VentanasEmergentes.CrearConexionFallidaMensajeVentana();
            }
            catch (TimeoutException )
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
            catch (CommunicationException )
            {
                VentanasEmergentes.CrearMensajeVentanaServidorError();
            }
            catch (Exception)
            {
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }
        }

        private void OcultarMensajesDeError()
        {
            lbCredencialesIncorrectas.Visibility = Visibility.Hidden;
        }

        private void MostrarMensajeCredencialesIncorrectas()
        {
            lbCredencialesIncorrectas.Visibility = Visibility.Visible;
        }

        private static bool EsUsuarioEnLinea(string nombreUsuario)
        {
            bool estaEnLinea = false; 
            try
            {
                GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
                estaEnLinea = proxy.ValidarUsuarioEnLinea(nombreUsuario);
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
            return estaEnLinea;
        }


        private static JugadorDataContract AutenticarUsuario(string usuario, string contraseña)
        {
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            try
            {
                return proxy.ValidarInicioSesion(usuario, contraseña);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                VentanasEmergentes.CrearMensajeVentanaErrorInesperado();
            }

            return null;
        }

        private void IniciarSesion(JugadorDataContract jugador)
        {
            ConfigurarSesion(jugador);
            NavegarAMenuPrincipal();
        }

        private static void ConfigurarSesion(JugadorDataContract jugador)
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            sesion.JugadorId = jugador.JugadorId;
            sesion.NombreUsuario = jugador.NombreUsuario;
            sesion.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
            sesion.Correo = jugador.Correo;
            sesion.EstaActivo = true;

            XAMLAmigos paginaAmigos = new XAMLAmigos();
            paginaAmigos.MostrarComoUsuarioActivo();
        }
        private void NavegarAMenuPrincipal()
        {
            XAMLAmigos amigosPage = new XAMLAmigos();
            this.NavigationService.Navigate(new XAMLMenu(amigosPage));
        }

        private void ManejarCredencialesInvalidas()
        {
            MostrarMensajeCredencialesIncorrectas();
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbCredencialesIncorrectas, Properties.Resources.lbDescripcionCredencialesIncorrectas);
        }


        private bool ValidarCampos()
        {
            bool esValido = true;

            if (!UtilidadesDeValidacion.EsCorreoValido(tbUsuario.Text) || tbUsuario.Text.Equals(tbUsuario.Tag))
            {
                esValido = false;
            }

            if (!UtilidadesDeValidacion.EsContrasenaValida(tpContraseña.Password) || tpContraseña.Password.Equals(tpContraseña.Tag))
            {
                esValido = false;
            }

            return esValido;
        }
        private void TbxUsuarioObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tbUsuario.Text == (string)tbUsuario.Tag)
            {
                tbUsuario.Text = string.Empty;
                tbUsuario.Foreground = Brushes.Black;
                tbUsuario.FontFamily = new FontFamily(FUENTE_SECUNDARIA);
                tbUsuario.FontWeight = FontWeights.Bold;
            }
        }
        private void TbUsuarioPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsuario.Text))
            {
                tbUsuario.Text = (string)tbUsuario.Tag;
            }
        }

        private void PbContraseñaObtenerFoco(object sender, RoutedEventArgs e)
        {
            if (tpContraseña.Password == (string)tpContraseña.Tag)
            {
                tpContraseña.Password = string.Empty;
                tpContraseña.Foreground = Brushes.Black;
            }
        }

        private void PbContraseñaPerderFoco(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tpContraseña.Password))
            {
                tpContraseña.Password = (string)tpContraseña.Tag;
            }
        }

        private void BtnRecuperarContraseña_Click(object sender, MouseButtonEventArgs e)
        {
            BtnRecuperarContraseña.IsEnabled = false;
            VentanaCorreoJugador ventanaCorreo = new VentanaCorreoJugador();

            ventanaCorreo.Closed += (s, args) =>
            {
                BtnRecuperarContraseña.IsEnabled = true;
            };

            ventanaCorreo.Show();
        }

    }
}
