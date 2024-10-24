﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections;
using System.Runtime.Remoting.Proxies;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarUsuarioNombre : Page
    {
        private JugadorDataContract jugador;
        public EditarUsuarioNombre()
        {
            InitializeComponent();
            jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
            CargarUsuarioJugador();
        }

        private void CargarUsuarioJugador()
        {
            if (jugador != null)
            {
                NombreUsuarioActualLabel.Content = jugador.NombreUsuario;
            }
        }

        private void btnClicGuardar(object sender, RoutedEventArgs e)
        {
            string nuevoNombreUsuario = NuevoNombreUsuarioTextBox.Text.Trim();
            int longitudValidaNombreUsuario = 50;

            if (!Metodos.ValidarEntradaVacia(nuevoNombreUsuario))
            {
                Metodos.MostrarMensaje("Por favor, debe ingresar un nuevo nombre de usuario.");
                return;
            }

            if (!Metodos.ValidarLongitudDeEntrada(nuevoNombreUsuario, longitudValidaNombreUsuario))
            {
                Metodos.MostrarMensaje("El nombre de usuario no puede tener más de 50 caracteres.");
                return;
            }

            if (!Metodos.ValidarEntradaIgual(jugador.NombreUsuario, nuevoNombreUsuario))
            {
                Metodos.MostrarMensaje("El nuevo nombre de usuario es igual al actual.");
                return;
            }

            SingletonSesion sesion = SingletonSesion.Instancia;
            GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
            bool resultado = proxy.EditarNombreUsuario(sesion.JugadorId, nuevoNombreUsuario);

            try
            {

                if (resultado)
                {
                    Metodos.MostrarMensaje("Nombre de usuario actualizado con éxito.");
                    this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                }
                else
                {
                    Metodos.MostrarMensaje("Hubo un problema al actualizar el nombre de usuario.");
                }
            }
            catch (Exception ex)
            {
                Metodos.MostrarMensaje("Error al conectar con el servicio: " + ex.Message);
            }
        }
    }
}
