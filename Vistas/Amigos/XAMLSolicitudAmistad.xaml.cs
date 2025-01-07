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
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.Amigos
{
   
    public partial class XAMLSolicitudAmistad : UserControl
    {
        private const string BTN_ACEPTAR = "Aceptar";
        private const string BTN_RECHAZAR = "Rechazar";
        private readonly string nombreUsuario;

        public event EventHandler<ArgumentosDeEventoDeClicDeBotón> BotonUsado;

        public XAMLSolicitudAmistad(string nombreUsuario)
        {
            InitializeComponent();
            this.nombreUsuario = nombreUsuario;
            lbnombreUsuario.Content = nombreUsuario;
        }

        private void ImgAceptarSolicitudAmistad_Click(object sender, MouseButtonEventArgs e)
        {
            BotonUsado?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTN_ACEPTAR, nombreUsuario));
        }

        private void ImgRechazarSolicitudAmistad_Click(object sender, MouseButtonEventArgs e)
        {
            BotonUsado?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTN_RECHAZAR, nombreUsuario));
        }
    }

}
