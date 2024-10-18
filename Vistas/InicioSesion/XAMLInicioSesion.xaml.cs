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

namespace trofeoCazador.Vistas.InicioSesion
{
    /// <summary>
    /// Interaction logic for XAMLInicioSesion.xaml
    /// </summary>
    public partial class XAMLInicioSesion : Page
    {
        public XAMLInicioSesion()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRegistrarCuenta(object sender, RoutedEventArgs e)
        {
            // Navegar a la página de registro de usuario
            if (this.NavigationService == null)
            {
                return;//veo  veo
            }
            this.NavigationService.Navigate(new Uri("Vistas/RegistroUsuario/XAMLRegistroUsuario.xaml", UriKind.Relative));

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}
