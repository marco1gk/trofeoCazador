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
using System.Windows.Media;s
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using trofeoCazador.Vistas;

namespace trofeoCazador.Vistas.Amigos
{
    /// <summary>
    /// Lógica de interacción para XAMLAmigos.xaml
    /// </summary>
    public partial class XAMLAmigos : Page
    {
        public XAMLAmigos()
        {
            InitializeComponent();
            MostrarDatos();
        }


        private void CambiarEstadoEnUI(string nombreUsuario, bool conectado)
        {
            Dispatcher.Invoke(() =>
            {
                var amigoControl = stackPanelFriends.Children
                    .OfType<XAMLActiveUserItemControl>()
                    .FirstOrDefault(control => control.lbUsername.Content.ToString() == nombreUsuario);

                if (amigoControl != null)
                {
                    // Actualiza el estado de conexión, lo que cambiará el color.
                    amigoControl.IsConnected = conectado;
                }
                else
                {
                    // Crea un nuevo control para el usuario si no existe en la lista.
                    var nuevoAmigoControl = new XAMLActiveUserItemControl(nombreUsuario)
                    {
                        IsConnected = conectado
                    };
                    stackPanelFriends.Children.Add(nuevoAmigoControl);
                }
            });
        }


        public void BtnCloseFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("este");
        }


        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }



        private void MostrarDatos()
        {
            bool exito = false;

            try
            {
                
              MostrarComoUsuarioActivo();
                CargarAmigosJugador();


                exito = true;
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