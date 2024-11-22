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
    public partial class XAMLActiveUserItemControl : UserControl
    {
        private const string BTN_DELETE_FRIEND = "DeleteFriend";
        private readonly string _username;

        public event EventHandler<ArgumentosDeEventoDeClicDeBotón> ButtonClicked;

        // Propiedad IsConnected para cambiar el color según el estado.
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    Console.WriteLine("Estado actualizado: " + (_isConnected ? "Conectado" : "Desconectado"));

                    // Cambiar el color aquí
                    rectangleStatusPlayer.Fill = _isConnected ? Brushes.Green : Brushes.Red;

                    Console.WriteLine($"Nuevo color en GUI: {rectangleStatusPlayer.Fill}");
                }
            }
        }





        public XAMLActiveUserItemControl(string username)
        {
            InitializeComponent();
            _username = username;
            lbUsername.Content = _username;
          
            // Inicializa con el color de desconectado (rojo).
           // rectangleStatusPlayer.Fill = Brushes.Red;
        }

        private void ImgOptionPlayer_Click(object sender, MouseButtonEventArgs e)
        {
            if (gridOptionsPlayer.Visibility == Visibility.Visible)
            {
                gridOptionsPlayer.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridOptionsPlayer.Visibility = Visibility.Visible;
            }
        }

        private void BtnDeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, new ArgumentosDeEventoDeClicDeBotón(BTN_DELETE_FRIEND, _username));
        }
    }
}
