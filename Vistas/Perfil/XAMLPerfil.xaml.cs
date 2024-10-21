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
            SingletonSesion sesion = SingletonSesion.Instancia;
            InitializeComponent();
            CargarPerfil(sesion.JugadorId);


        }

        private void CargarPerfil(int IdJugador)
        {
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = proxy.ObtenerJugador(IdJugador);
            if (jugador != null)
            {
                UsuarioLabel.Content = jugador.NombreUsuario;
                CorreoLabel.Content = jugador.Correo;
            }
            
        }


    }
}