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

namespace trofeoCazador.Vistas.Menu
{
    /// <summary>

    /// </summary>
    public partial class XAMLMenu : Page
    {
        public XAMLMenu()
        {
            InitializeComponent();
        }
        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;//veo  veo
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));

        }

        private void BtnLobby(object sender, RoutedEventArgs e)
        {
            // Navegar a la página de registro de usuario
            if (this.NavigationService == null)
            {
                return;//veo  veo
            }
            this.NavigationService.Navigate(new Uri("Vistas/SalaEspera/XAMLSalaEspera.xaml", UriKind.Relative));

        }

        private void BtnSalir(object sender, RoutedEventArgs e)
        {
            SingletonSesion.Instancia.LimpiarSesion();
            NavigationService.GoBack();
        }

        private void BtnUnirseLobby(object sender, RoutedEventArgs e)
        {

        }
    }
}
