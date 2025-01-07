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
        readonly JugadorDataContract jugador = new JugadorDataContract();
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

        private bool EditarContraseña()
        {
            GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
            string contraseniaNueva = ContrasenaNuevaTextBox.Password.Trim();

            if (correo != null)
            {
                return EditarContraseñaConCorreo(proxy, contraseniaNueva);
            }
            else
            {
                string contraseniaIngresada = ContraseniaActualTextBox.Password.Trim();
                return EditarContraseñaSinCorreo(proxy, contraseniaIngresada, contraseniaNueva);
            }
        }

        private bool EditarContraseñaConCorreo(GestionCuentaServicioClient proxy, string contraseniaNueva)
        {
            if (EsEntradaValida(contraseniaNueva))
            {
                if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    return ProcesarEdicionConCorreo(proxy, contraseniaNueva);
                }
                else
                {
                    MostrarError(Properties.Resources.lbContraseñaSegura);
                    return false;
                }
            }
            return false;
        }

        private bool EditarContraseñaSinCorreo(GestionCuentaServicioClient proxy, string contraseniaIngresada, string contraseniaNueva)
        {
            if (EsEntradaValida(contraseniaIngresada, contraseniaNueva))
            {
                if (!proxy.VerificarContrasena(contraseniaIngresada, jugador.Correo))
                {
                    MostrarError(Properties.Resources.lbContraseñaNoCoincide);
                    return false;
                }

                if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    return ProcesarEdicionSinCorreo(proxy, contraseniaNueva);
                }
                else
                {
                    MostrarError(Properties.Resources.lbContraseñaSegura);
                    return false;
                }
            }
            return false;
        }

        private bool EsEntradaValida(string contraseniaNueva)
        {
            if (Metodos.ValidarEntradaVacia(contraseniaNueva))
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
                return false;
            }
            return true;
        }

        private bool EsEntradaValida(string contraseniaIngresada, string contraseniaNueva)
        {
            if (Metodos.ValidarEntradaVacia(contraseniaIngresada) || Metodos.ValidarEntradaVacia(contraseniaNueva))
            {
                MostrarError(Properties.Resources.lbLlenarCamposObligatorios);
                return false;
            }
            return true;
        }

        private void MostrarError(string mensaje)
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
