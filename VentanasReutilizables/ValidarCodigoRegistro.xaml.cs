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


namespace trofeoCazador.VentanasReutilizables
{
    public partial class ValidarCodigoRegistro : Window
    {
        private readonly JugadorDataContract _jugador;
        private readonly string _codigoEnviado;
        private readonly string _correo;
        public ValidarCodigoRegistro(JugadorDataContract jugador, string correo, string codigoEnviado)
        {
            InitializeComponent();
            _jugador = jugador;  
            _codigoEnviado = codigoEnviado; 
            _correo = correo;

        }

        private void BtnEnviar(object sender, RoutedEventArgs e)
        {
            string codigoIngresado = tbxCode.Text.Trim();

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            if (proxy.ValidarCodigo(codigoIngresado, _codigoEnviado))
            {
                if(_jugador != null)
                {
                  
                    proxy.AgregarJugador(_jugador);
                    MessageBox.Show("Cuenta creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); 
                }
                else
                {
                    NavigationWindow navigationWindow = new NavigationWindow
                    {
                        Content = new EditarContrasenia(_correo)
                    };
                    navigationWindow.Show();
                    this.Close();
                }
                
            }
            else
            {
                MessageBox.Show("El código ingresado es incorrecto. Inténtalo nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CerrarVentana(object sender, MouseButtonEventArgs e)
        {
            this.Close(); 
        }
    }



}
