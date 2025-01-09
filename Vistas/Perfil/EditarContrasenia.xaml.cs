using System;
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
using trofeoCazador.Recursos;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarContrasenia : Page
    {
        readonly JugadorDataContract jugador;
        private readonly string correo;
        public EditarContrasenia(string correo)
        {
            InitializeComponent();
            jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
            this.correo = correo;
            if(correo != null)
            {
                LbContraseniaActual.Visibility = Visibility.Collapsed;
                ContraseniaActualTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                LbContraseniaActual.Visibility = Visibility.Visible;
                ContraseniaActualTextBox.Visibility = Visibility.Visible;
            }
        }

        private void EditarContraseña()
        {
            GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
            string contraseniaNueva = ContrasenaNuevaTextBox.Password.Trim();

            if (correo != null)
            {
                EditarContraseñaConCorreo(proxy, contraseniaNueva);
            }
            else
            {
                string contraseniaIngresada = ContraseniaActualTextBox.Password.Trim();
                EditarContraseñaSinCorreo(proxy, contraseniaIngresada, contraseniaNueva);
            }
        }


        private void EditarContraseñaConCorreo(GestionCuentaServicioClient proxy, string contraseniaNueva)
        {
            if (EsEntradaValida(contraseniaNueva))
            {
                if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    ProcesarEdicionConCorreo(proxy, contraseniaNueva);
                }
                else
                {
                    MostrarError(Properties.Resources.lbContraseñaSegura);
                }
            }
            else
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
            }
        }


        private void EditarContraseñaSinCorreo(GestionCuentaServicioClient proxy, string contraseniaIngresada, string contraseniaNueva)
        {
            if (EsEntradaValida(contraseniaIngresada, contraseniaNueva))
            {
                if (!proxy.VerificarContrasena(contraseniaIngresada, jugador.Correo))
                {
                    MostrarError(Properties.Resources.lbContraseñaNoCoincide);
                }
                else if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    ProcesarEdicionSinCorreo(proxy, contraseniaNueva);
                }
                else
                {
                    MostrarError(Properties.Resources.lbContraseñaSegura);
                }
            }
            else
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
            }
        }


        private static bool EsEntradaValida(string contraseniaNueva)
        {
            if (Metodos.ValidarEntradaVacia(contraseniaNueva))
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
                return false;
            }
            return true;
        }

        private static bool EsEntradaValida(string contraseniaIngresada, string contraseniaNueva)
        {
            if (Metodos.ValidarEntradaVacia(contraseniaIngresada) || Metodos.ValidarEntradaVacia(contraseniaNueva))
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
                return false;
            }
            return true;
        }

        private static void MostrarError(string mensaje)
        {
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, mensaje);
        }

        private bool ProcesarEdicionConCorreo(GestionCuentaServicioClient proxy, string contraseniaNueva)
        {
            if (proxy.EditarContraseña(correo, contraseniaNueva))
            {
                MostrarExito(Properties.Resources.lbContraseñaRestablecida);
                return true;
            }
            else
            {
                MostrarError(Properties.Resources.lbErrorActualizarContraseña);
                return false;
            }
        }

        private bool ProcesarEdicionSinCorreo(GestionCuentaServicioClient proxy, string contraseniaNueva)
        {
            if (proxy.EditarContraseña(jugador.Correo, contraseniaNueva))
            {
                MostrarExito(Properties.Resources.lbContraseñaActualizada);
                return true;
            }
            else
            {
                MostrarError(Properties.Resources.lbProblemaActualizandoContraseña);
                return false;
            }
        }

        private void MostrarExito(string mensaje)
        {
            VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloGenerico, mensaje);
            NavigationWindow navigationWindow = (NavigationWindow)Window.GetWindow(this);
            navigationWindow?.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditarContraseña();
        }
    }
}
