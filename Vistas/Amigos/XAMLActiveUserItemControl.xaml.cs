using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLActiveUserItemControl : UserControl
    {
        private const string BTN_BORRAR_AMIGO = "DeleteFriend";
        private readonly string nombreUsuario;

        public event EventHandler<ArgumentosDeEventoDeClicDeBotón> BotonUsado;

        public XAMLActiveUserItemControl(string nombreUsuario)
        {
            InitializeComponent();

            this.nombreUsuario = nombreUsuario;
            lbnombreUsuario.Content = this.nombreUsuario;
        }

        private void ImgOpcionesJugador_Click(object sender, MouseButtonEventArgs e)
        {
            if (gridOpcionesJugador.Visibility == Visibility.Visible)
            {
                gridOpcionesJugador.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridOpcionesJugador.Visibility = Visibility.Visible;
            }
        }

        private void BtnBorrarAmigo_Click(object sender, RoutedEventArgs e)
        {
            BotonUsado?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTN_BORRAR_AMIGO, nombreUsuario));
        }
    }
}
