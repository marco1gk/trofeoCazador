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
using System.Windows.Shapes;

namespace trofeoCazador.VentanasReutilizables
{
    public partial class XAMLVentanaEmergente : Window
    {
        private readonly Window ventanaPrincipal;


        public XAMLVentanaEmergente(string titulo, string descripcion)
        {
            InitializeComponent();

            ventanaPrincipal = Application.Current.MainWindow;
            lbTituloVentanaEmergente.Content = titulo;
            tbDescripcionVentanaEmergente.Text = descripcion;

            ConfigurarVentanaEmergente();
        }

        private void ConfigurarVentanaEmergente()
        {
            this.Owner = ventanaPrincipal;

            AjustarDimensionesVentana();
            EstablecerCentroVentana();
        }

        private void AjustarDimensionesVentana()
        {
            this.Width = ventanaPrincipal.Width;
            this.Height = ventanaPrincipal.Height;
        }

        private void EstablecerCentroVentana()
        {
            double centroX = ventanaPrincipal.Left + (ventanaPrincipal.Width - this.Width) / 2;
            double centroY = ventanaPrincipal.Top + (ventanaPrincipal.Height - this.Height) / 2;
            this.Left = centroX;
            this.Top = centroY;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
