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
using trofeoCazador.ServicioDelJuego;


namespace trofeoCazador.Vistas.Perfil
{
    public partial class XAMLPerfil : Page
    {
        public XAMLPerfil()
        {
            InitializeComponent();
            int idJugador = ObtenerIdJugadorActual();
        }

        private void CargarPerfil(int idJugador)
        {
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = proxy.ObtenerJugador(idJugador);
            if (jugador != null)
            {
                UsuarioLabel.Content = jugador.NombreUsuario;
                CorreoLabel.Content = jugador.Correo;
            }
        }

        private int ObtenerIdJugadorActual()
        {
            return 1;
        }

    }
}