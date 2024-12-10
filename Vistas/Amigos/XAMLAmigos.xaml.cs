using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using trofeoCazador.Utilidades;
using trofeoCazador.Vistas;

namespace trofeoCazador.Vistas.Amigos
{

    public partial class XAMLAmigos : Page
    {
        private readonly GestorUsuariosConectadosClient cliente;
        public XAMLAmigos()
        {
            InitializeComponent();
            InstanceContext contexto = new InstanceContext(this);
            cliente = new GestorUsuariosConectadosClient(contexto);
            MostrarComoUsuarioActivo();
            MostrarDatos();
            
        }

        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        public void DesregistrarUsuarioActual()
        {
            if (cliente != null)
            {
                cliente.DesregistrarUsuarioDeUsuariosEnLinea(SingletonSesion.Instancia.NombreUsuario);
                Console.WriteLine($"Usuario {SingletonSesion.Instancia.NombreUsuario} desregistrado.");
            }
        }
        public void MostrarComoUsuarioActivo()
        {
            cliente.RegistrarUsuarioAUsuariosConectados(SingletonSesion.Instancia.JugadorId, SingletonSesion.Instancia.NombreUsuario);
        }

        private void MostrarDatos()
        {

            try
            {
                CargarAmigosJugador();

            }
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }
    }
}