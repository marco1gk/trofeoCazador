using System;
using System.Windows;
using System.Windows.Controls;

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

        // Evento que notifica cuando se expulsa a un jugador
        public event EventHandler<string> JugadorExpulsado;

        public JugadorControl()
        {
            InitializeComponent();
        }

        private void ExpulsarJugador_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para notificar la expulsión
            JugadorExpulsado?.Invoke(this, NombreUsuario);
        }
    }
}
