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
        JugadorDataContract jugador = new JugadorDataContract();
        private string correo;
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
                if (Metodos.ValidarEntradaVacia(contraseniaNueva) == false)
                {
                    Metodos.MostrarMensaje("Por favor, debe llenar todos los campos.");
                    return false;
                }

                if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    if (proxy.EditarContraseña(correo, contraseniaNueva))
                    {
                        Metodos.MostrarMensaje("Su contraseña ha sido reestablecida.");
                        NavigationWindow navigationWindow = (NavigationWindow)Window.GetWindow(this);
                        navigationWindow?.Close();
                        return true;
                    }
                    else
                    {
                        Metodos.MostrarMensaje("Hubo un error al intentar reestablecer la contraseña, inténtelo de nuevo más tarde.");
                        return false;
                    }
                }
                else
                {
                    Metodos.MostrarMensaje("La contraseña ingresada no cumple con los requisitos de contraseña segura.");
                    return false;
                }
            }
            else
            {
                // Validar la contraseña actual y la nueva
                string contraseniaIngresada = ContraseniaActualTextBox.Password.Trim();

                if (Metodos.ValidarEntradaVacia(contraseniaIngresada) == false || Metodos.ValidarEntradaVacia(contraseniaNueva) == false)
                {
                    Metodos.MostrarMensaje("Por favor, debe llenar todos los campos.");
                    return false;
                }

                if (!proxy.VerificarContrasena(contraseniaIngresada, jugador.Correo))
                {
                    Metodos.MostrarMensaje("La contraseña ingresada no coincide con la actual.");
                    return false;
                }

                if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
                {
                    if (proxy.EditarContraseña(jugador.Correo, contraseniaNueva))
                    {
                        Metodos.MostrarMensaje("La contraseña ha sido actualizada con éxito.");
                        NavigationWindow navigationWindow = (NavigationWindow)Window.GetWindow(this);
                        navigationWindow?.Close();
                        return true;
                    }
                    else
                    {
                        Metodos.MostrarMensaje("Hubo un error al intentar actualizar la contraseña, inténtelo de nuevo más tarde.");
                        return false;
                    }
                }
                else
                {
                    Metodos.MostrarMensaje("La contraseña ingresada no cumple con los requisitos de contraseña segura.");
                    return false;
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditarContraseña();
        }
    }
}
