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

namespace trofeoCazador.Vistas.Amigos
{
   
    public partial class XAMLFriendRequestItemComponent : UserControl
    {
        private const string BTNACEPTAR = "Accept";
        private const string BTNRECHAZAR = "Reject";
        private readonly string _nombreUsuario;

        public event EventHandler<ArgumentosDeEventoDeClicDeBotón> ButtonClicked;

        public XAMLFriendRequestItemComponent(string nombreUsuario)
        {
            InitializeComponent();

            _nombreUsuario = nombreUsuario;
            lbUsername.Content = nombreUsuario;
        }

        private void ImgAceptarSolicitudAmistad_Click(object sender, MouseButtonEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTNACEPTAR, _nombreUsuario));
        }

        private void ImgRechazarSolicitudAmistad_Click(object sender, MouseButtonEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTNRECHAZAR, _nombreUsuario));
        }


    }

    public class ArgumentosDeEventoDeClicDeBotón : EventArgs
    {
        public string NombreBoton { get; private set; }
        public string NombreUsuario { get; private set; }

        public ArgumentosDeEventoDeClicDeBotón(string nombreBoton, string nombreUsuario)
        {
            NombreBoton = nombreBoton;
            NombreUsuario = nombreUsuario;
        }
    }
}
