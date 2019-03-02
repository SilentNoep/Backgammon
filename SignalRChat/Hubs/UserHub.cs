using Common;
using Common.Backgammon;
using Microsoft.AspNet.SignalR;
using SignalRChat.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hubs.SignalRChat
{
    public class UserHub : Hub
    {
        static UserManager userManager = new UserManager();
        static GameManager bgManager = new GameManager();


        static Dictionary<string, string> userConnections = new Dictionary<string, string>();  // value is usernames
        static Dictionary<string, string> userNames = new Dictionary<string, string>(); // value is connectionID


        private List<string> connIDList;


        #region Chat/Lobby
        public IEnumerable<UserDetails> GetAllUsers()
        {
            return userManager.GetAllUsers();
        }
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
        public void InGame()
        {
            var userName = userConnections[Context.ConnectionId];
            userManager.EnteredGame(userName);
            Notify(GetUserDetails(userName, Status.InGame), UserInGame);
        }
        public string SignIn(CommonUser User)
        {
            var message = userManager.LogIn(User);
            
            Notify(GetUserDetails(User.UserName, Status.Online), UserConnected);
            userManager.UserInvited(User.UserName, false);
            return message;
        }
        public void SignOut()
        {
            var userName = userConnections[Context.ConnectionId];
            userManager.LogOff(userName);
            Notify(GetUserDetails(userName, Status.Offline), UserDisonnected);
        }
        public string Register(CommonUser User)
        {
            var message = userManager.Register(User);
            Notify(GetUserDetails(User.UserName, Status.Online), UserRegistered);
            return message;
        }

        public void GetMyUserDetails()
        {
            var userName = userConnections[Context.ConnectionId];
            Notify(GetUserDetails(userName, Status.Online), GetUser);
        }

        public void GetUser(UserDetails user)
        {
            Clients.Caller.GetMyUserDetails(user);
        }
        public void UserInGame(UserDetails user)
        {
            this.Clients.Others.InGameNotificated(user);
        }
        public void UserConnected(UserDetails user)
        {
            this.Clients.Others.LogInNotificated(user);
        }
        public void UserDisonnected(UserDetails user)
        {
            this.Clients.Others.LogOffNotificated(user);
        }
        public void UserRegistered(UserDetails user)
        {
            this.Clients.Others.RegisterNotificated(user);
        }

        public void Notify(UserDetails user, Action<UserDetails> userNotificationMethod)
        {
            Task.Run(() =>
            {
                userNotificationMethod(user);
            });
        }
        #endregion


        private UserDetails GetUserDetails(string userName, Status status, bool HasInvited = false)
        {
            return new UserDetails() { UserName = userName, Status = status, HasInvitedGame = HasInvited };
        }


        #region Game
        public void InviteToGame(UserDetails fromClient, UserDetails toClient)
        {
            userManager.UserInvited(fromClient.UserName,true);
            if (userNames.ContainsKey(toClient.UserName))
                Clients.Client(userNames[toClient.UserName]).broadcastInvitationGame(fromClient);
        }

        public void AnswerInviteToGame(UserDetails fromClient, UserDetails toClient, bool answer)
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
                Clients.Client(connIDFromClient).broadcastMovedChipToClientAndMe(board, true, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
                Clients.Client(connIDtoClient).broadcastMovedChipToClientAndMe(board, false, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
            }
            if (IsItClientsTurn)
            {
                Clients.Client(connIDtoClient).broadcastMovedChipToClientAndMe(board, true, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
                Clients.Client(connIDFromClient).broadcastMovedChipToClientAndMe(board, false, numberTurn, TotalMoves, IsTurnCanceled, DidPlayerMove);
            }

        }

        public void GetPlayers()
        {
            var userName = userConnections[Context.ConnectionId];
            bool DidInvite = userManager.DidUserInvite(userName);
            var userDetails = GetUserDetails(userName, Status.InGame, DidInvite);
            Player player = bgManager.InitPlayer(userDetails);
            Clients.Caller.GetMyPlayer(player,userDetails);
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