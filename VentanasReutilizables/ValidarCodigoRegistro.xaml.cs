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
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Vistas.Perfil;
using trofeoCazador.Utilidades;

namespace trofeoCazador.VentanasReutilizables
{
    public partial class ValidarCodigoRegistro : Window
    {
        private readonly JugadorDataContract jugador;
        private readonly string codigoEnviado;
        private readonly string correo;
        public ValidarCodigoRegistro(JugadorDataContract jugador, string correo, string codigoEnviado)
        {
            InitializeComponent();
            this.jugador = jugador;  
            this.codigoEnviado = codigoEnviado; 
            this.correo = correo;

        }
        private void BtnEnviar(object sender, RoutedEventArgs e)
        {
            string codigoIngresado = tbCode.Text.Trim();

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            if (proxy.ValidarCodigo(codigoIngresado, codigoEnviado))
            {
                if (jugador != null)
                {

                    proxy.AgregarJugador(jugador);
                    this.Close();
                    VentanasEmergentes.CrearVentanaEmergente(Properties.Resources.lbTituloExito, Properties.Resources.lbDescripcionCuentaCreada);
                }
                else
                {
                    NavigationWindow navigationWindow = new NavigationWindow
                    {
                        Content = new EditarContrasenia(correo)
                    };
                    navigationWindow.Show();
                    this.Close();
                }
                
            }
            else
            {
               lbCodigoError.Visibility = Visibility.Visible;
            }
        }

        private void CerrarVentana(object sender, MouseButtonEventArgs e)
        {
            this.Close(); 
        }
    }



}
