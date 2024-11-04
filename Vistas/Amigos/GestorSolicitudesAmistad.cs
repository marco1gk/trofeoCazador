using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using trofeoCazador.ServicioDelJuego;
using trofeoCazador.Utilidades;

namespace trofeoCazador.Vistas.Amigos
{
    public partial class XAMLAmigos : Page, IGestorDeSolicitudesDeAmistadCallback
    {
        SingletonSesion sesion = SingletonSesion.Instancia;
        private void SuscribeUserToOnlineFriendsDictionary()
        {
            InstanceContext context = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient friendRequestManagerClient = new GestorDeSolicitudesDeAmistadClient(context);
           
            try
            {
                friendRequestManagerClient.AddToOnlineFriendshipDictionary(sesion.NombreUsuario);
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


        private void BtnSendRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendRequest();
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


        private void SendRequest()
        {
            lbFriendRequestUsernameError.Visibility = Visibility.Visible;

            string usernamePlayerRequested = tbxUsernameSendRequest.Text.Trim();
            int idPlayer = sesion.JugadorId;

            if (ValidateSendRequest(idPlayer, usernamePlayerRequested))
            {
                AddRequestFriendship(idPlayer, usernamePlayerRequested);
                SendFriendRequest(usernamePlayerRequested);

                Console.WriteLine("");

                tbxUsernameSendRequest.Text = string.Empty;
            }
            else
            {
             //   EmergentWindows.CreateEmergentWindow(Properties.Resources.lbFriendRequest,
               //     Properties.Resources.tbkFriendRequestErrorDescription);
            }
        }

        private void AddRequestFriendship(int idPlayer, string usernamePlayerRequested)
        {
            GestorAmistadClient friendshipManagerClient = new GestorAmistadClient();

            friendshipManagerClient.AddRequestFriendship(idPlayer, usernamePlayerRequested);
        }


        private void SendFriendRequest(string usernamePlayerRequested)
        {
            InstanceContext context = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient friendRequestManagerClient = new GestorDeSolicitudesDeAmistadClient(context);

            friendRequestManagerClient.SendFriendRequest(sesion.NombreUsuario, usernamePlayerRequested);
        }

        private bool ValidateSendRequest(int idPlayer, string usernamePlayerRequested)
        {
            bool isRequestValid = false;

            if (UtilidadesDeValidacion.EsNombreUsuarioValido(usernamePlayerRequested))
            {
                GestorAmistadClient friendshipManagerClient = new GestorAmistadClient();

                isRequestValid = friendshipManagerClient.ValidateFriendRequestSending(idPlayer, usernamePlayerRequested);
            }
            else
            {
                lbFriendRequestUsernameError.Visibility = Visibility.Visible;
            }

            return isRequestValid;
        }

        private void BtnFriends_Click(object sender, RoutedEventArgs e)
        {
            scrollViewerFriends.Visibility = Visibility.Visible;
            scrollViewerFriendsRequest.Visibility = Visibility.Visible;
            Console.WriteLine("amikos");

    
        }

        private void BtnFriendsRequest_Click(object sender, RoutedEventArgs e)
        {
            ShowFriendRequests();
        }

        private void ShowFriendRequests()
        {
            scrollViewerFriendsRequest.Visibility = Visibility.Visible;
            scrollViewerFriends.Visibility = Visibility.Visible;

            

            stackPanelFriendsRequest.Children.Clear();
            string[] usernamePlayers = GetCurrentFriendRequests();

            if (usernamePlayers != null)
            {
                AddUsersToFriendsRequestList(usernamePlayers);
            }
        }

        

        private string[] GetCurrentFriendRequests()
        {
            GestorAmistadClient friendshipManagerClient = new GestorAmistadClient();
            string[] usernamePlayers = null;

            try
            {
                usernamePlayers = friendshipManagerClient.GetUsernamePlayersRequesters(sesion.JugadorId);
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

            return usernamePlayers;
        }

        private void AddUsersToFriendsRequestList(string[] usernamePlayers)
        {
            if (usernamePlayers != null)
            {
                foreach (string username in usernamePlayers)
                {
                    AddUserToFriendRequestList(username);
                }
            }
        }

        private void AddUserToFriendRequestList(string username)
        {
            XAMLFriendRequestItemComponent friendRequestItem = CreateFriendRequestItemControl(username);
            stackPanelFriendsRequest.Children.Add(friendRequestItem);
        }

        private XAMLFriendRequestItemComponent CreateFriendRequestItemControl(string username)
        {
            string idItem = "lbRequest";
            string idUserItem = idItem + username;
            XAMLFriendRequestItemComponent friendRequestItem = new XAMLFriendRequestItemComponent(username);
            friendRequestItem.Name = idUserItem;
            friendRequestItem.ButtonClicked += FriendRequestItem_BtnClicked;

            return friendRequestItem;
        }

        private void FriendRequestItem_BtnClicked(object sender, ButtonClickEventArgs e)
        {
            string btnAccept = "Accept";
            string btnReject = "Reject";

            if (e.ButtonName.Equals(btnAccept))
            {
                AcceptFriendRequest(e.Username);
            }

            if (e.ButtonName.Equals(btnReject))
            {
                RejectFriendRequest(e.Username);
            }
        }

        private void AcceptFriendRequest(string usernameSender)
        {
            InstanceContext context = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient friendRequestManagerClient = new GestorDeSolicitudesDeAmistadClient(context);

            try
            {
                friendRequestManagerClient.AcceptFriendRequest(sesion.JugadorId, sesion.NombreUsuario, usernameSender);
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

        private void RejectFriendRequest(string username)
        {
            InstanceContext context = new InstanceContext(this);
            GestorDeSolicitudesDeAmistadClient friendRequestManagerClient = new GestorDeSolicitudesDeAmistadClient(context);

            try
            {
                friendRequestManagerClient.RejectFriendRequest(sesion.JugadorId, username);
                RemoveFriendRequestFromStackPanel(username);
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

        private void RemoveFriendRequestFromStackPanel(string username)
        {
            string idItem = "lbRequest";
            string idFriendRequestItem = idItem + username;

            XAMLFriendRequestItemComponent friendRequestItemToRemove = FindFriendRequeustItemControlById(idFriendRequestItem);

            if (friendRequestItemToRemove != null)
            {
                stackPanelFriendsRequest.Children.Remove(friendRequestItemToRemove);
            }
        }

        private XAMLFriendRequestItemComponent FindFriendRequeustItemControlById(string idFriendRequestItem)
        {
            foreach (XAMLFriendRequestItemComponent item in stackPanelFriendsRequest.Children)
            {
                if (item.Name == idFriendRequestItem)
                {
                    return item;
                }
            }

            return null;
        }

        public void NotifyNewFriendRequest(string username)
        {
            AddUserToFriendRequestList(username);
        }

        public void NotifyFriendRequestAccepted(string username)
        {
           // AddUserToFriendsList(username, ONLINE_STATUS_PLAYER_HEX_COLOR);
            RemoveFriendRequestFromStackPanel(username);
        }

        public void NotifyDeletedFriend(string username)
        {
            RemoveFriendFromFriendsList(username);
        }

        private void RemoveFriendFromFriendsList(string username)
        {
            string idItem = "lb";
            string idUserItem = idItem + username;

            XAMLActiveUserItemControl userOnlineItemToRemove = FindActiveUserItemControlById(idUserItem);

            if (userOnlineItemToRemove != null)
            {
                stackPanelFriends.Children.Remove(userOnlineItemToRemove);
            }
        }


    }
}
