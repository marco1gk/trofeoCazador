using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLAmigos : Page, IGestorUsuariosConectadosCallback
    {
      //  
        private const string ONLINE_STATUS_PLAYER_HEX_COLOR = "#61FF00";
        private const string OFFLINE_STATUS_PLAYER_HEX_COLOR = "#FF5A5E59";



       
        private void LoadPlayerFriends()
        {
            GestorAmistadClient friendshipManagerClient = new GestorAmistadClient();
            SingletonSesion sesion = SingletonSesion.Instancia;
            string[] usernamePlayerFriends = friendshipManagerClient.GetListUsernameFriends(sesion.JugadorId);
            AddUsersToFriendsList(usernamePlayerFriends);
        }

        private void ShowAsActiveUser()
        {
            InstanceContext context = new InstanceContext(this);
            GestorUsuariosConectadosClient client = new GestorUsuariosConectadosClient(context);

            client.RegisterUserToOnlineUsers(sesion.JugadorId, sesion.NombreUsuario);
        }

        public void NotifyUserLoggedIn(string username)
        {
            bool isOnline = true;
            ChangeStatusPlayer(username, isOnline);
        }

        public void NotifyUserLoggedOut(string username)
        {
            bool isOnline = false;
            ChangeStatusPlayer(username, isOnline);
        }

        public void NotifyOnlineFriends(string[] onlineUsernames)
        {
            bool isOnline = true;

            ChangeStatusFriends(onlineUsernames, isOnline);
            SuscribeUserToOnlineFriendsDictionary();
        }

        private void ChangeStatusFriends(string[] onlineUsernames, bool isOnline)
        {
            foreach (string onlineUsername in onlineUsernames)
            {
                ChangeStatusPlayer(onlineUsername, isOnline);
            }
        }

        private void ChangeStatusPlayer(string username, bool isOnline)
        {
            string idLabel = "lb";
            string idUserItem = idLabel + username;
            XAMLActiveUserItemControl userOnlineItem = FindActiveUserItemControlById(idUserItem);

            if (userOnlineItem != null)
            {
                SolidColorBrush statusPlayerColor;

                if (isOnline)
                {
                    statusPlayerColor = Utilities.CreateColorFromHexadecimal(ONLINE_STATUS_PLAYER_HEX_COLOR);
                }
                else
                {
                    statusPlayerColor = Utilities.CreateColorFromHexadecimal(OFFLINE_STATUS_PLAYER_HEX_COLOR);
                }

                userOnlineItem.rectangleStatusPlayer.Fill = statusPlayerColor;
            }
        }

        private XAMLActiveUserItemControl FindActiveUserItemControlById(string idUserItem)
        {
            foreach (XAMLActiveUserItemControl item in stackPanelFriends.Children)
            {
                if (item.Name == idUserItem)
                {
                    return item;
                }
            }

            return null;
        }

        private void AddUsersToFriendsList(string[] onlineUsernames)
        {
            foreach (var username in onlineUsernames)
            {
                AddUserToFriendsList(username, OFFLINE_STATUS_PLAYER_HEX_COLOR);
            }
        }

        private void AddUserToFriendsList(string username, string connectionStatusPlayer)
        {
            XAMLActiveUserItemControl userOnlineItem = CreateActiveUserItemControl(username, connectionStatusPlayer);
            stackPanelFriends.Children.Add(userOnlineItem);
        }

        private XAMLActiveUserItemControl CreateActiveUserItemControl(string username, string haxadecimalColor)
        {
            string idItem = "lb";
            string idUserItem = idItem + username;
            XAMLActiveUserItemControl userOnlineItem = new XAMLActiveUserItemControl(username);
            userOnlineItem.Name = idUserItem;
            userOnlineItem.ButtonClicked += UserOnlineItem_BtnDeleteFriendClicked;
         

            return userOnlineItem;
        }

        private void UserOnlineItem_BtnDeleteFriendClicked(object sender, ButtonClickEventArgs e)
        {
            string btnDeleteFriend = "DeleteFriend";

            if (e.ButtonName.Equals(btnDeleteFriend))
            {
                DeleteFriend(e.Username);
            }
        }

        private void DeleteFriend(string usernameFriendToDelete)
        {
            InstanceContext context = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient friendRequestManagerClient = new GestorDeSolicitudesDeAmistadClient(context);

            try
            {
                
                friendRequestManagerClient.DeleteFriend(sesion.JugadorId, sesion.NombreUsuario, usernameFriendToDelete);
            }
            catch (EndpointNotFoundException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
            
            catch (FaultException ex)
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
    public static class Utilities
    {
        public static SolidColorBrush CreateColorFromHexadecimal(string hexadecimalColor)
        {
            SolidColorBrush solidColorBrush = null;

            if (hexadecimalColor != null)
            {
                try
                {
                    solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return solidColorBrush;
        }

        public static Image CreateImageByPath(string imagePath)
        {
            string absolutePath = BuildAbsolutePath(imagePath);

            Image styleImage = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(absolutePath));
            styleImage.Source = bitmapImage;

            return styleImage;
        }

        public static string BuildAbsolutePath(string relativePath)
        {
            string absolutePath = "";

            if (relativePath != null)
            {
                absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            return absolutePath;
        }
    }

}
