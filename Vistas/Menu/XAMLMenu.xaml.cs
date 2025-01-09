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
        private readonly XAMLAmigos paginaAmigos;
        public XAMLMenu(XAMLAmigos paginaAmigos)
        {
            InitializeComponent();
            this.paginaAmigos = paginaAmigos;
        }
        public XAMLMenu()
        {
            InitializeComponent();
        }

        private void BtnPerfil(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Perfil/XAMLPerfil.xaml", UriKind.Relative));

        }
       
        private void BtnSalaEspera_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;         }
            this.NavigationService.Navigate(new Uri("Vistas/SalaEspera/XAMLSalaEspera.xaml", UriKind.Relative));

        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            paginaAmigos?.DesregistrarUsuarioActual();
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

        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService == null)
            {
                return;
            }
            this.NavigationService.Navigate(new Uri("Vistas/Estadisticas/XAMLEstadisticas.xaml", UriKind.Relative));
        }
    }
}
