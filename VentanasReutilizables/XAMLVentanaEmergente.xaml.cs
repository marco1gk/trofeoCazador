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
        private readonly Window _ventanaPrincipal;

        public XAMLVentanaEmergente()
        {
            InitializeComponent();
        }

        public XAMLVentanaEmergente(string titulo, string descripcion)
        {
            InitializeComponent();

            _ventanaPrincipal = Application.Current.MainWindow;
            lbTitleEmergentWindow.Content = titulo;
            tbkDescriptionEmergentWindow.Text = descripcion;

            ConfigurarVentanaEmergente();

        }

        private void ConfigurarVentanaEmergente()
        {
            this.Owner = _ventanaPrincipal;

            AjustarDimensionesVentana();
            EstablecerCentroVentana();
        }

        private void AjustarDimensionesVentana()
        {
            this.Width = _ventanaPrincipal.Width;
            this.Height = _ventanaPrincipal.Height;
        }

        private void EstablecerCentroVentana()
        {
            double centerX = _ventanaPrincipal.Left + (_ventanaPrincipal.Width - this.Width) / 2;
            double centerY = _ventanaPrincipal.Top + (_ventanaPrincipal.Height - this.Height) / 2;
            this.Left = centerX;
            this.Top = centerY;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
