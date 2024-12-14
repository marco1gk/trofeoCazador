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
using System.Windows.Shapes;
using trofeoCazador.Recursos.ElementosPartida;

namespace trofeoCazador.Vistas.PartidaJuego
{
    /// <summary>
    /// Lógica de interacción para DialogoSeleccionCarta.xaml
    /// </summary>
    public partial class DialogoSeleccionCarta : Window
    {
        public CartaCliente CartaSeleccionada { get; private set; }

        public DialogoSeleccionCarta(IEnumerable<CartaCliente> cartas)
        {
            InitializeComponent();
            DataContext = new DialogoSeleccionCartaViewModel(cartas, SeleccionarCarta);
        }

        private void SeleccionarCarta(CartaCliente carta)
        {
            CartaSeleccionada = carta;
            DialogResult = true; // Cierra el diálogo
        }
    }

}
