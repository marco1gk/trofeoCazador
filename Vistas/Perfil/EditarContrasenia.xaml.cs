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
        public EditarContrasenia()
        {
            InitializeComponent();
            jugador = Metodos.ObtenerDatosJugador(Metodos.ObtenerIdJugador());
        }

        private bool EditarContraseña()
        {
            GestionCuentaServicioClient proxy = Metodos.EstablecerConexionServidor();
            string contraseniaActual = jugador.ContraseniaHash;
            string contraseniaIngresada = ContraseniaActualTextBox.Password.Trim();
            string contraseniaNueva = ContrasenaNuevaTextBox.Password.Trim();

            if(Metodos.ValidarEntradaVacia(contraseniaIngresada) == false || Metodos.ValidarEntradaVacia(contraseniaNueva)== false)
            {
                Metodos.MostrarMensaje("Por favor, debe llenar todos los campos.");
                return false;
            }

            if(Metodos.ValidarEntradaIgual(contraseniaActual, contraseniaIngresada))
            {
                Metodos.MostrarMensaje("La contraseña ingresada no coincide con la actual.");
                return false;
            }

            if(Metodos.ValidarEntradaIgual(contraseniaActual, contraseniaNueva) == false)
            {
                Metodos.MostrarMensaje("La contraseña nueva no debe ser igual que la actual.");
                return false;
            }

            if (UtilidadesDeValidacion.EsContrasenaValida(contraseniaNueva))
            {
                if(proxy.EditarContraseña(jugador.Correo, contraseniaNueva))
                {
                    Metodos.MostrarMensaje("La contraseña ha sido actualizada con éxito.");
                    this.NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
                    return true;
                }
                else
                {
                    Metodos.MostrarMensaje("Hubo un error al intentar actualizar la contraseña, intentelo de nuevo más tarde.");
                    return false;
                }

            }
            else
            {
                Metodos.MostrarMensaje("La contraseña ingresada no cumple con los requisitos de contraseña segura.");
                return false;
            }

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditarContraseña();
        }
    }
}
