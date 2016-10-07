using Common.Packets.C2S.Auth;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections.TCP;
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
using System.Net;
using Common;
using Battleships.Interfaces.UI;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IEmbeddedViewHideAnimationStart
    {
        private bool _defaultServer = false;

        public LoginView()
        {
            if (Net.ENABLE_DISCOVERY)
            {
                ProtoClient.OnServerDiscovered += ProtoClient_OnServerDiscovered;
                try
                {
                    ProtoClient.DiscoverServer();
                }
                catch
                {
                    // ... screw this
                }
            }
            InitializeComponent();

        }

        private void ProtoClient_OnServerDiscovered(EndPoint e)
        {
            if (_defaultServer) return;
            _defaultServer = true;
            this.Invoke((Action)delegate { this.serverTextBox.Text = e.ToString(); });
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            ProtoClient.OnHandshakeReceived += ProtoClient_OnHandshakeReceived;
            connectButton.IsEnabled = false;
            Task.Factory.StartNew((o) => 
            {
                string[] array = (string[])o;
                string server = array[0];
                string nick = array[1];

                try
                {
                    ProtoClient.Connect(server);
                    ProtoClient.SendHandshake(ProtoClient.VERSION, nick);
                }
                catch
                {
                    this.Invoke(() => 
                    {
                        MessageBox.Show(this.GetMainWindow(), $"Could not establish connection with {this.serverTextBox.Text}");
                        connectButton.IsEnabled = true;
                    });
                    
                }
            }, new string[] { this.serverTextBox.Text, this.nickTextBox.Text });

        }

        private void ProtoClient_OnHandshakeReceived(PacketHeader packetHeader, NetworkCommsDotNet.Connections.Connection connection, Common.Packets.S2C.S2C_HandshakeResponse incomingObject)
        {
            ProtoClient.OnHandshakeReceived -= ProtoClient_OnHandshakeReceived;
            if(incomingObject.OK)
            {
                //this.Invoke((Action)delegate { MessageBox.Show(this.GetMainWindow(), "OK!!"); });
                this.Invoke((Action)delegate { this.GetMainWindow().TransitionInto(new LobbyView()); });
            }
            else
            {
                this.Invoke((Action)delegate 
                {
                    connectButton.IsEnabled = true; MessageBox.Show(this.GetMainWindow(), incomingObject.Message);
                });
            }
        }

        public void OnHideAnimationStart()
        {
            ProtoClient.OnHandshakeReceived -= ProtoClient_OnHandshakeReceived;
        }
    }
}
