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

namespace trofeoCazador.VentanasReutilizables
{
    public partial class VentanaCorreoJugador : Window
    {
        public VentanaCorreoJugador()
        {
            InitializeComponent();
        }
        private void BtnClicIngresarCorreo(object sender, RoutedEventArgs e)
        {
            string correo = CorreoTextBox.Text.Trim();


            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            string codigoRecuperacion = proxy.EnviarCodigoConfirmacion(correo);
            if (!string.IsNullOrEmpty(codigoRecuperacion))
            {
                ValidarCodigoRegistro validarCodigo = new ValidarCodigoRegistro(null, correo, codigoRecuperacion);
                validarCodigo.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("No se encontró ninguna cuenta asociada a este correo.");
            }
        }
    }
}
