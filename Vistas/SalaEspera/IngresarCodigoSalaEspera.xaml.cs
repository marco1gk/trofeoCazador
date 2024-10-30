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
using trofeoCazador.Vistas.SalaEspera;

namespace trofeoCazador.Vistas
{
    public partial class IngresarCodigoSalaEspera : Page
    {
        public IngresarCodigoSalaEspera()
        {
            InitializeComponent();
        }

        private void BtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string codigoLobby = txtCodigoLobby.Text.Trim();

            if (string.IsNullOrEmpty(codigoLobby))
            {
                MessageBox.Show("Por favor, ingresa un código de lobby válido.");
                return;
            }

            // Navegar a la página de sala de espera pasando el código de lobby
           // NavigationService.Navigate(new XAMLSalaEspera(codigoLobby));
        }
    }
}
