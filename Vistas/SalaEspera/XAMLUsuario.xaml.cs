using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace trofeoCazador.Vistas.SalaEspera
{
    public partial class JugadorControl : UserControl
    {
        public static readonly DependencyProperty NombreUsuarioProperty =
     DependencyProperty.Register("NombreUsuario", typeof(string), typeof(JugadorControl), new PropertyMetadata(string.Empty));

        public string NombreUsuario
        {
            get { return (string)GetValue(NombreUsuarioProperty); }
            set { SetValue(NombreUsuarioProperty, value); }
        }
        
        public JugadorControl()
        {
            InitializeComponent();
        }
    }


}

