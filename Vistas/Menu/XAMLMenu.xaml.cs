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
        private readonly XAMLAmigos amigosPage;
        public XAMLMenu(XAMLAmigos amigosPage)
        {
            InitializeComponent();
            this.amigosPage = amigosPage;
        }

        private void BtnPerfil(object sender, RoutedEventArgs e)
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
            amigosPage?.DesregistrarUsuarioActual();
            SingletonSesion.Instancia.LimpiarSesion();
            NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
        }
        private void BtnAmigos(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Amigos/XAMLAmigos.xaml", UriKind.Relative));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
