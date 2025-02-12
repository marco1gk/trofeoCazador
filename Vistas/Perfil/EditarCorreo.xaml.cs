﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using trofeoCazador.Recursos;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas.InicioSesion;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class XAMLEditarCorreo : Page
    {
        private JugadorDataContract jugador;
        public XAMLEditarCorreo()
        {
            InitializeComponent();

            try
            {
                jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
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
            CargarCorreoActual();
        }
        private void CargarCorreoActual()
        {
            if (jugador != null)
            {
                CorreoActualLabel.Content = jugador.Correo;
            }
        }
        private void BtnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoCorreo = NuevoCorreoTextBox.Text.Trim();
            int longitudMaximaCorreo = 100;

            try
            {
                if (!ValidarCorreo(nuevoCorreo, longitudMaximaCorreo))
                {
                    return;
                }

                if (jugador != null)
                {
                    GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
                    string codigoVerificacion = proxy.EnviarCodigoConfirmacion(jugador.Correo);

                    if (!string.IsNullOrEmpty(codigoVerificacion))
                    {
                        SingletonSesion.Instancia.NuevoCorreo = nuevoCorreo;
                        SingletonSesion.Instancia.CodigoVerificacion = codigoVerificacion;
                        NavigationService.Navigate(new Uri("Vistas/Perfil/VerificarCodigo.xaml", UriKind.Relative));
                    }
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

        private bool ValidarCorreo(string correo, int longitudMaxima)
        {
            if (Metodos.ValidarEntradaVacia(correo))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCorreoVacio);
                return false;
            }

            if (!Metodos.ValidarEntradaIgual(jugador.Correo, correo))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCorreoIgual);
                return false;
            }

            if (!Metodos.ValidarLongitudDeEntrada(correo, longitudMaxima))
            {
                VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, Properties.Resources.lbCorreoExceso);
                return false;
            }

            return true;
        }

    }
}
