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
        public XAMLAmigos()
        {
            InitializeComponent();
            MostrarDatos();
            
        }

        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }



        private void MostrarDatos()
        {

            try
            {
                
             // MostrarComoUsuarioActivo();
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