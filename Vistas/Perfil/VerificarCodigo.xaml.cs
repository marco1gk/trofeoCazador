using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
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
using trofeoCazador.Vistas.Perfil;
using trofeoCazador.Recursos;

namespace trofeoCazador.Vistas.Perfil
{
    public partial class EditarCorreoCodigo : Page
    {
        public EditarCorreoCodigo()
        {
            InitializeComponent();
        }
        GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
        SingletonSesion sesion = SingletonSesion.Instancia;

        private void btnClicEnviarCodigo(object sender, RoutedEventArgs e)
        {

            string codigoIngresado = CodigoTextBox.Text.Trim();
            string codigoEnviado = sesion.CodigoVerificacion;
            int longitudMaximaCodigo = 6;

            if (!Metodos.ValidarEntradaVacia(codigoIngresado))
            {
                Metodos.MostrarMensaje("Por favor, ingrese el código.");
                return;
            }

            if(!Metodos.ValidarLongitudDeEntrada(codigoIngresado, longitudMaximaCodigo))
            {
                Metodos.MostrarMensaje("El código debe ser un número de 6 dígitos.");
                return;
            }

            if (proxy.ValidarCodigo(codigoIngresado, codigoEnviado))
            {
                if (proxy.EditarCorreo(sesion.JugadorId, sesion.NuevoCorreo))
                {
                    Metodos.MostrarMensaje("El correo ha sido actualizado con éxito.");
                    this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));
                }
                else
                    Metodos.MostrarMensaje("Hubo un problema al intentar actualizar el correo, intentelo de nuevo más tarde.");      
            }
            else
            {
                Metodos.MostrarMensaje("Código incorrecto, por favor intenta de nuevo.");
            }
        }

        private void btnClicSolicitarNuevoCodigo(object sender, RoutedEventArgs e)
        {
            proxy.EnviarCodigoConfirmacion(sesion.Correo);   
        }
    }
}
