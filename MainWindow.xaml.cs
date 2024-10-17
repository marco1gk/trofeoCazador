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
using trofeoCazador.Servidor;

namespace trofeoCazador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
     
            //   Servidor.GestionUsuarioClient proxy = Servidor.GestionUsuarioClient();
            Servidor.GestionUsuarioClient s = new Servidor.GestionUsuarioClient();
            Jugador jugador = new Jugador();
            jugador.usuario = "dasda";
            s.agregarJugador(jugador);
           

        }
    }
}
