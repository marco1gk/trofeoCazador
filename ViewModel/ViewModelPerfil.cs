using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trofeoCazador.Servidor;

namespace trofeoCazador.ViewModel
{
    public class PerfilViewModel : INotifyPropertyChanged
    {
        private readonly IGestionUsuario _gestionUsuario;

        public Jugadorr JugadorActual { get; set; }

        public PerfilViewModel()
        {
            _gestionUsuario = new gestionUsuarioServicio(); 
            CargarDatosJugador(1); 
        }

        private void CargarDatosJugador(int idJugador)
        {
            JugadorActual = _gestionUsuario.ObtenerJugador(idJugador); // Obtiene el jugador del servicio
            OnPropertyChanged(nameof(JugadorActual)); // Notifica a la vista que los datos han cambiado
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
