using Battleships.Interfaces.UI;
using Common;
using Common.Packets.C2S.Lobby;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for LobbyView.xaml
    /// </summary>
    public partial class LobbyView : UserControl, IEmbeddedViewShowAnimationStart, IEmbeddedViewHideAnimationStart
    {
        public ObservableCollection<PlayerDisplay> AvailablePlayers;
        public ObservableCollection<PlayerDisplay> SentPlayerInvites;
        public ObservableCollection<PlayerDisplay> IncomingPlayerInvites;
        
        public int PlayersOnline = 0;

        public LobbyView()
        {
            AvailablePlayers = new ObservableCollection<PlayerDisplay>();
            SentPlayerInvites = new ObservableCollection<PlayerDisplay>();
            IncomingPlayerInvites = new ObservableCollection<PlayerDisplay>();
            InitializeComponent();
        }

        public void OnHideAnimationStart()
        {
            ProtoClient.OnInitialLobbyDataReceived -= ProtoClient_OnInitialLobbyDataReceived;
            ProtoClient.OnLobbyPlayerJoined -= ProtoClient_OnLobbyPlayerJoined;
            ProtoClient.OnLobbyPlayerLeft -= ProtoClient_OnLobbyPlayerLeft;
            ProtoClient.OnServerPlayerCountChanged -= ProtoClient_OnServerPlayerCountChanged;

            ProtoClient.OnLobbyPlayerInviteSent -= ProtoClient_OnLobbyPlayerInviteSent;
            ProtoClient.OnLobbySentPlayerInviteRevoked -= ProtoClient_OnLobbySentPlayerInviteRevoked;

            ProtoClient.OnLobbyPlayerInviteReceived -= ProtoClient_OnLobbyPlayerInviteReceived;
            ProtoClient.OnLobbyIncomingPlayerInviteRevoked -= ProtoClient_OnLobbyIncomingPlayerInviteRevoked;

            ProtoClient.OnInitNewGame -= ProtoClient_OnInitNewGame;
        }

        private void ProtoClient_OnInitNewGame(S2C_InitNewGame data)
        {
            this.Invoke((d) => 
            {
                this.GetMainWindow().TransitionInto(new GameView(d));
            }, data);
        }

        public void OnShowAnimationStart()
        {
            listBoxPlayersAvailable.ItemsSource = AvailablePlayers;
            listBoxSentPlayerInvites.ItemsSource = SentPlayerInvites;
            listBoxReceivedPlayerInvites.ItemsSource = IncomingPlayerInvites;
            labelLocalPlayerName.Content = ProtoClient.LocalPlayer?.Nickname;

            ProtoClient.OnInitialLobbyDataReceived += ProtoClient_OnInitialLobbyDataReceived;
            ProtoClient.OnLobbyPlayerJoined += ProtoClient_OnLobbyPlayerJoined;
            ProtoClient.OnLobbyPlayerLeft += ProtoClient_OnLobbyPlayerLeft;
            ProtoClient.OnServerPlayerCountChanged += ProtoClient_OnServerPlayerCountChanged;

            ProtoClient.OnLobbyPlayerInviteSent += ProtoClient_OnLobbyPlayerInviteSent;
            ProtoClient.OnLobbySentPlayerInviteRevoked += ProtoClient_OnLobbySentPlayerInviteRevoked;

            ProtoClient.OnLobbyPlayerInviteReceived += ProtoClient_OnLobbyPlayerInviteReceived;
            ProtoClient.OnLobbyIncomingPlayerInviteRevoked += ProtoClient_OnLobbyIncomingPlayerInviteRevoked;

            ProtoClient.OnInitNewGame += ProtoClient_OnInitNewGame;

            ProtoClient.JoinJobby();
        }

        private void ProtoClient_OnLobbyIncomingPlayerInviteRevoked(S2C_RevokedIncomingPlayerInvite data)
        {
            IncomingPlayerInvitesRemove(data.Source);
        }

        private void ProtoClient_OnLobbyPlayerInviteReceived(Common.Packets.S2C.S2C_IncomingPlayerInvite data)
        {
            IncomingPlayerInvitesAdd(data.Source);
        }

        private void ProtoClient_OnLobbySentPlayerInviteRevoked(S2C_RevokedSentPlayerInvite data)
        {
            SentPlayerInvitesRemove(data.Destination);
        }

        private void ProtoClient_OnLobbyPlayerInviteSent(S2C_SentPlayerInvite data)
        {
            SentPlayerInvitesAdd(data.Destination);
        }

        private void ProtoClient_OnServerPlayerCountChanged(S2C_ServerPlayersOnlineCount data)
        {
            UpdatePlayerOnlineCount(data.PlayersOnline);
        }
        private void ProtoClient_OnInitialLobbyDataReceived(Common.Packets.S2C.Lobby.S2C_InitialLobbyData data)
        {
            this.Invoke((i) =>
            {
                labelOnlinePlayers.Content = PlayersOnline = i.PlayersOnline;
                if (data.AvailablePlayers != null)
                {
                    lock (AvailablePlayers)
                    {
                        foreach (var p in data.AvailablePlayers)
                        {
                            AvailablePlayers.Add(p);
                        }
                    }
                }

            }, data);

        }
        private void ProtoClient_OnLobbyPlayerJoined(Common.Packets.S2C.Lobby.S2C_LobbyPlayerJoined data)
        {
            AvailablePlayersAdd(data.Player);
        }
        private void ProtoClient_OnLobbyPlayerLeft(S2C_LobbyPlayerLeft data)
        {
            AvailablePlayersRemove(data.Player.PlayerID);
        }

        public void AvailablePlayersAdd(PlayerDisplay player)
        {
            this.Invoke((p) =>
            {
                lock (AvailablePlayers)
                {
                    AvailablePlayers.Add(p);
                }
            }, player);
        }
        public void AvailablePlayersRemove(ShortGuid id)
        {
            this.Invoke((d) =>
            {
                AvailablePlayers.SafelyRemoveById(d);
                SentPlayerInvites.SafelyRemoveById(d);
                IncomingPlayerInvites.SafelyRemoveById(d);

            }, id);
        }

        public void SentPlayerInvitesAdd(PlayerDisplay player)
        {
            this.Invoke((p) =>
            {
                lock (SentPlayerInvites)
                {
                    SentPlayerInvites.Add(p);
                }
                AvailablePlayers.SafelyRemoveById(p.PlayerID);
            }, player);
        }
        public void SentPlayerInvitesRemove(PlayerDisplay player)
        {
            this.Invoke((p) =>
            {
                SentPlayerInvites.SafelyRemoveById(p.PlayerID);
                AvailablePlayersAdd(p);
            }, player);
        }

        public void IncomingPlayerInvitesAdd(PlayerDisplay player)
        {
            this.Invoke((p) =>
            {
                lock (IncomingPlayerInvites)
                {
                    IncomingPlayerInvites.Add(p);
                }
                AvailablePlayers.SafelyRemoveById(p.PlayerID);
            }, player);
        }
        public void IncomingPlayerInvitesRemove(PlayerDisplay player)
        {
            this.Invoke((p) =>
            {
                IncomingPlayerInvites.SafelyRemoveById(p.PlayerID);
                AvailablePlayersAdd(p);
            }, player);
        }

        public void UpdatePlayerOnlineCount(int online)
        {
            this.Invoke((d) =>
            {
                labelOnlinePlayers.Content = d;
            }, online);
        }


        private void buttonSendInvite_Click(object sender, RoutedEventArgs e)
        {
            var destination = listBoxPlayersAvailable.SelectedItem as PlayerDisplay;
            if (destination != null)
            {
                ProtoClient.SendPlayerInvite(destination.PlayerID);
            }
        }

        private void buttonRevokeInvite_Click(object sender, RoutedEventArgs e)
        {
            var destination = listBoxSentPlayerInvites.SelectedItem as PlayerDisplay;
            if (destination != null)
            {
                ProtoClient.RevokeSentPlayerInvite(destination.PlayerID);
            }
        }

        private void buttonAcceptInvite_Click(object sender, RoutedEventArgs e)
        {
            var source = listBoxReceivedPlayerInvites.SelectedItem as PlayerDisplay;
            if (source != null)
            {
                ProtoClient.AcceptIncomingPlayerInvite(source.PlayerID);
            }
        }
    }
}
