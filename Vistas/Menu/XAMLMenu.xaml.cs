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
using trofeoCazador.Vistas.Amigos;

namespace trofeoCazador.Vistas.Menu
{
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
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));

        }
       
        private void BtnSalaEspera(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;         }
            this.NavigationService.Navigate(new Uri("Vistas/SalaEspera/XAMLSalaEspera.xaml", UriKind.Relative));

        }

        private void BtnSalir(object sender, RoutedEventArgs e)
        {
            SingletonSesion.Instancia.LimpiarSesion();
            NavigationService.GoBack();
        }

        private void BtnUnirseSalaEspera(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAmigos(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Amigos/XAMLAmigos.xaml", UriKind.Relative));

        }
    }
}
