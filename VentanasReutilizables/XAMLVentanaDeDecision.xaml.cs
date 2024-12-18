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

namespace trofeoCazador.VentanasReutilizables
{
    public partial class XAMLVentanaDeDecision : Window
    {
        private readonly Window ventanaPrincipal;
        private TaskCompletionSource<bool> Resultado;
        public XAMLVentanaDeDecision()
        {
            InitializeComponent();
        }

        public XAMLVentanaDeDecision(string titulo, string descripcion)
        {
            InitializeComponent();

            ventanaPrincipal = Application.Current.MainWindow;
            lbTituloVentanaDeDecision.Content = titulo;
            tbDescripcionVentanaDeDecision.Text = descripcion;

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

        private void BtnSi_Click(object sender, RoutedEventArgs e)
        {
            Resultado.SetResult(true);
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Resultado.SetResult(false);
            this.Close();
        }

        public Task<bool> MostrarDecision()
        {
            Resultado = new TaskCompletionSource<bool>();
            this.Show();
            return Resultado.Task;
        }
    }
}
