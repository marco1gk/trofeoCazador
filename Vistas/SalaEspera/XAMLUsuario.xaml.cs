using System;
using System.Windows;
using System.Windows.Controls;

namespace trofeoCazador.Vistas.SalaEspera
{
    public partial class JugadorControl : UserControl
    {
        public static readonly DependencyProperty EsAnfitrionProperty =
            DependencyProperty.Register(
                nameof(EsAnfitrion),
                typeof(bool),
                typeof(JugadorControl),
                new PropertyMetadata(false));

        public bool EsAnfitrion
        {
            get => (bool)GetValue(EsAnfitrionProperty);
            set => SetValue(EsAnfitrionProperty, value);
        }
        private static void OnEsAnfitrionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as XAMLSalaEspera;
        }


        public static readonly DependencyProperty NombreUsuarioProperty =
            DependencyProperty.Register("NombreUsuario", typeof(string), typeof(JugadorControl), new PropertyMetadata(string.Empty));

        public string NombreUsuario
        {
            get { return (string)GetValue(NombreUsuarioProperty); }
            set { SetValue(NombreUsuarioProperty, value); }
        }

        public event EventHandler<string> JugadorExpulsado;

        public JugadorControl()
        {
            InitializeComponent();
        }

        private void ExpulsarJugador_Click(object sender, RoutedEventArgs e)
        {
            JugadorExpulsado?.Invoke(this, NombreUsuario);
        }
    }
}
