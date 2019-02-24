using Common;
using Common.Backgammon;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hubs.SignalRChat
{
    public class UserHub : Hub
    {

        static BackgammonManager bgManager = new BackgammonManager();


        static Dictionary<string, string> userConnections = new Dictionary<string, string>();  // value is usernames
        static Dictionary<string, string> userNames = new Dictionary<string, string>(); // value is connectionID


        private List<string> connIDList;


        #region Chat/Lobby
        public void SendToAll(string message)
        {
            Clients.All.broadcastMessage(userConnections[Context.ConnectionId], message);
        }
        public void NotifyMessage(string message, Action<string> userNotificationMethod)
        {
            Task.Run(() =>
            {
                userNotificationMethod(message);
            });
        }
        public void ReallySendToAll(string message)
        {
            NotifyMessage(message, SendToAll);
        }
        public void SendTo(string message, string toClient)
        {
            string fromClient = userConnections[Context.ConnectionId];
            if (userNames.ContainsKey(toClient))
            {
                Clients.Client(userNames[toClient]).broadcastMessageToClient(fromClient, message);
                Clients.Caller.broadcastMessageToClient(toClient, message);
            }
               
        }


        public void InGame(User User)
        {
            Notify(User, UserInGame);
        }
        public void SignIn(User User)
        {
            Notify(User, UserConnected);
        }
        public void SignOut(User User)
        {
            Notify(User, UserDisonnected);
        }
        public void UserInGame(User user)
        {
            this.Clients.Others.InGameNotificated(user);
        }
        public void UserConnected(User user)
        {
            this.Clients.Others.LogInNotificated(user);
        }
        public void UserDisonnected(User user)
        {
            this.Clients.Others.LogOffNotificated(user);
        }
        public void Notify(User user, Action<User> userNotificationMethod)
        {
            Task.Run(() =>
            {
                userNotificationMethod(user);
            });
        }
        #endregion

        #region Game
        public void InviteToGame(User fromClient, User toClient)
        {
            if (userNames.ContainsKey(toClient.UserName))
                Clients.Client(userNames[toClient.UserName]).broadcastInvitationGame(fromClient);
        }

        public void AnswerInviteToGame(User fromClient, User toClient, bool answer)
        {
            if (userNames.ContainsKey(toClient.UserName))
            {
                Clients.Client(userNames[toClient.UserName]).broadcastAnswerInvitationGame(fromClient, answer);
                Clients.Client(userNames[fromClient.UserName]).broadcastAnswerInvitationGame(fromClient, answer);
            }
        }

        public void GetDicesNumbers(string toClient)
        {
            Board board = bgManager.RollDices();
            string connIDFromClient = Context.ConnectionId;
            string connIDtoClient = userNames[toClient];
            var myUserName = Context.QueryString["UserName"].ToString();
            bool IsItMyTurn = bgManager.IsMyTurn(myUserName);
            bool IsItClientsTurn = bgManager.IsMyTurn(toClient);
            if (IsItMyTurn)
            {
                Clients.Client(connIDFromClient).broadcastDicesToClientAndMe(board, true);
                Clients.Client(connIDtoClient).broadcastDicesToClientAndMe(board, false);
            }
            if (IsItClientsTurn)
            {
                Clients.Client(connIDtoClient).broadcastDicesToClientAndMe(board, true);
                Clients.Client(connIDFromClient).broadcastDicesToClientAndMe(board, false);
            }
        }

        public void GetOrRemovePick(string toClient, int spikeChosen, Player player)
        {
            Board board = bgManager.GetOrRemovePick(spikeChosen, player);
            string connIDFromClient = Context.ConnectionId;
            string connIDtoClient = userNames[toClient];
            connIDList = new List<string>() { connIDFromClient, connIDtoClient };
            if (userNames.ContainsKey(toClient))
                Clients.Clients(connIDList).broadcastChosenSpikeToClientAndMe(board);
        }

        public void MoveChipToSpike(string toClient, int spikeChosen, Player player)
        {
            Board board = bgManager.MoveChip(spikeChosen, player);

            string connIDFromClient = Context.ConnectionId;
            string connIDtoClient = userNames[toClient];

            var myUserName = Context.QueryString["UserName"].ToString();
            bool IsItMyTurn = bgManager.IsMyTurn(myUserName);
            bool IsItClientsTurn = bgManager.IsMyTurn(toClient);
            bool IsTurnCanceled = bgManager.IsTurnCanceled;
            bool DidPlayerMove = bgManager.DidPlayerMove;
            int numberTurn = bgManager.Moved;
            int TotalMoves = bgManager.Moves;

            if (IsItMyTurn)
            {
                Clients.Client(connIDFromClient).broadcastMovedChipToClientAndMe(board, true,numberTurn,TotalMoves, IsTurnCanceled, DidPlayerMove);
                Clients.Client(connIDtoClient).broadcastMovedChipToClientAndMe(board, false, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
            }
            if (IsItClientsTurn)
            {
                Clients.Client(connIDtoClient).broadcastMovedChipToClientAndMe(board, true, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
                Clients.Client(connIDFromClient).broadcastMovedChipToClientAndMe(board, false, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
            }

        }

        public void GetPlayers(User fromClient)
        {
            Player player = bgManager.InitPlayer(fromClient);
            Clients.Caller.GetMyPlayer(player);
        }
        #endregion

        #region HubStatus
        public override Task OnConnected()
        {
            var userName = Context.QueryString["UserName"].ToString();
            userNames.Add(userName, Context.ConnectionId);
            userConnections.Add(Context.ConnectionId, userName);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userName = Context.QueryString["UserName"].ToString();
            userNames.Remove(userName);
            userConnections.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion

    }
}