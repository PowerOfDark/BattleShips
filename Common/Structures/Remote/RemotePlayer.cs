using Common.Packets.S2C;
using Common.Packets.S2C.Game;
using Common.Packets.S2C.Lobby;
using Common.Structures.Common;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Common.Structures.Remote
{
    public class RemotePlayer : PlayerBase
    {
        public Connection Connection { get; set; }

        public ShortGuid ID { get { return Connection.ConnectionInfo.NetworkIdentifier; } }

        public Dictionary<ShortGuid, RemotePlayer> IncomingInvites { get; protected set; }
        /// <summary>
        /// List of players that this player has sent invites to
        /// </summary>
        public Dictionary<ShortGuid, RemotePlayer> SentInvites { get; protected set; }

        public Game Game { get; protected set; }

        public bool IsInGame { get { return Game != null; } }

        public RemotePlayer()
        {
            this.IncomingInvites = new Dictionary<ShortGuid, RemotePlayer>();
            this.SentInvites = new Dictionary<ShortGuid, RemotePlayer>();
            //this.IncomingInvites.CollectionChanged += IncomingInvites_CollectionChanged;
            //this.SentInvites.CollectionChanged += SentInvites_CollectionChanged;
        }

        public PlayerDisplay GetDisplay()
        {
            return new Common.PlayerDisplay(this.ID, this.Nickname);
        }

        public bool HasIncomingInviteFrom(RemotePlayer source)
        {
            lock(IncomingInvites)
            {
                return IncomingInvites.ContainsKey(source.ID);
            }
        }

        public bool HasSentInviteTo(RemotePlayer destination)
        {
            lock(SentInvites)
            {
                return SentInvites.ContainsKey(destination.ID);
            }
        }

        //private void IncomingInvites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //inbox
        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var from in e.NewItems)
        //        {
        //            this.Connection.Send(new S2C_IncomingPlayerInvite(from as RemotePlayer));
        //        }

        //    }
        //    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (var from in e.OldItems)
        //        {
        //            this.Connection.Send(new S2C_RevokedIncomingPlayerInvite((from as RemotePlayer).Connection.ConnectionInfo.NetworkIdentifier));
        //        }
        //    }
        //}

        //private void SentInvites_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //outbox
        //    if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var target in e.NewItems)
        //        {
        //            (target as RemotePlayer).IncomingInvites.Add(this);
        //            //S2C_SentPlayerInvite
        //        }

        //    }
        //    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach(var target in e.OldItems)
        //        {
        //            (target as RemotePlayer).IncomingInvites.Remove(this);
        //            //S2C_RevokeSentPlayerInvite
        //        }
        //    }
        //}

        /// <summary>
        /// Send a game invite.
        /// </summary>
        /// <param name="player2">Invitation target</param>
        public void SendInvite(RemotePlayer destination)
        {
            bool added = false;
            lock (this.SentInvites)
            {
                if (!this.SentInvites.ContainsKey(destination.ID))
                {
                    this.SentInvites.Add(destination.ID, destination);
                    added = true;
                }
            }
            if (added)
            {
                destination.ReceiveIncomingInvite(this);
                Connection.Send(new S2C_SentPlayerInvite(destination.GetDisplay()));
            }
        }

        public void RevokeSentInvite(RemotePlayer destination)
        {
            lock (this.SentInvites)
            {
                if(this.SentInvites.Remove(destination.ID))
                {
                    Connection.Send(new S2C_RevokedSentPlayerInvite(destination.GetDisplay()));
                    destination.RevokeIncomingInvite(this);
                }
            }

        }

        protected void ReceiveIncomingInvite(RemotePlayer source)
        {
            bool added = false;
            lock (this.IncomingInvites)
            {
                if (!this.IncomingInvites.ContainsKey(source.ID))
                {
                    this.IncomingInvites.Add(source.ID, source);
                    added = true;
                }
            }
            if (added)
            {
                this.Connection.Send(new S2C_IncomingPlayerInvite(source.GetDisplay()));
            }
        }
        public void RevokeIncomingInvite(RemotePlayer source)
        {
            lock (this.IncomingInvites)
            {
                if (this.IncomingInvites.Remove(source.ID))
                {
                    this.Connection.Send(new S2C_RevokedIncomingPlayerInvite(source.GetDisplay()));
                    source.RevokeSentInvite(this);
                }
            }
        }

        public void ClearAllInvites()
        {
            lock(this.IncomingInvites)
            {
                foreach(var incoming in IncomingInvites.ToArray())
                {
                    this.RevokeIncomingInvite(incoming.Value);
                }
            }

            lock(this.SentInvites)
            {
                foreach(var sent in SentInvites.ToArray())
                {
                    this.RevokeSentInvite(sent.Value);
                }
            }
        }

        public void JoinGame(Game game)
        {
            this.PlayerState = PlayerState.IN_GAME;
            this.Game = game;
            var player = game.GetPlayer(this);
            Connection.Send(new S2C_InitNewGame(game.RuleSet, player.Opponent.RemotePlayer.GetDisplay(), player.PlayerSide));
        }

        public void EndGame(BoardOwner winner, GameResult result)
        {
            this.Game = null;
            this.PlayerState = PlayerState.POST_GAME;
            Connection.Send(new S2C_GameEnded(winner, result));
        }
    }
}
