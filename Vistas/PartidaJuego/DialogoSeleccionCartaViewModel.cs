using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using trofeoCazador.Recursos.ElementosPartida;

namespace trofeoCazador.Vistas.PartidaJuego
{
    public class DialogoSeleccionCartaViewModel
    {
        public ObservableCollection<CartaCliente> Cartas { get; }
        public ICommand SeleccionarCartaCommand { get; }

        private readonly Action<CartaCliente> seleccionarCartaCallback;

        public DialogoSeleccionCartaViewModel(IEnumerable<CartaCliente> cartas, Action<CartaCliente> seleccionarCartaCallback)
        {
            Cartas = new ObservableCollection<CartaCliente>(cartas);
            SeleccionarCartaCommand = new RelayCommand<CartaCliente>(seleccionarCartaCallback);
        }
    }
}
