using Common;
using Common.Backgammon;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Infra
{
    public interface IChatService
    {
        bool HasEvents { get; }
        bool IsConnected { get; }
        User User { get; }
        User SelectedUser { get; }

        void GetUser(User user);
        void GetSelectedUser(User user);


        void ConnectToHub();



        void ListenToGroupMessages(Action<string, string> SendMessageToAllAction);    //SIGN UP TO EVENT
        void SendMessageToAll(string msg);                                               // FIRE EVENT


        void ListenToClientMessages(Action<string, string> SendMessageToAllAction);     //SIGN UP TO EVENT
        void SendMessageToClient(string msg, string toClient);                             // FIRE EVENT






        void ListenToStatusChangedEvents(Action<User> LogInNotification, Action<User> LogOffNotification, Action<User> InGameNotification, User user);    //SIGN UP TO EVENT SignInEVENT && SignOutEVENT
        void LogInNotificated(Action<User> whatToExecute, User message);                 // MAKES SURE I WORK ON UI THREAD
        void SignIn(User user);                                                          // FIRE EVENT
        void LogOffNotificated(Action<User> whatToExecute, User message);                // MAKES SURE I WORK ON UI THREAD
        void SignOut(User user);                                                        // FIRE EVENT
        void InGameNotificated(Action<User> whatToExecute, User message);               // MAKES SURE I WORK ON UI THREAD
        void InGame(User user);                                                         // FIRE EVENT




        void ListenToGameInvitations(Action<User> InviteGameAction);                   //SIGN UP TO EVENT
        Task<string> InviteClientForGame(User fromClient, User toClient);                       // FIRE EVENT
        void InviteGameNotificated(Action<User> whatToExecute, User name);             // MAKES SURE I WORK ON UI THREAD




        void ListenAnswerToGameInvitations(Action<User, bool> InviteGameAction);    //SIGN UP TO EVENT
        Task<string> AnswerInviteClientForGame(User fromClient, User toClient, bool answer);                  // FIRE EVENT
        void AnswerInviteGameNotificated(Action<User, bool> whatToExecute, User name,bool answer);                          // MAKES SURE I WORK ON UI THREAD







        void DisconnectFromServer();                                             //DISCONNECT ME FROM SERVER





        void ListenToDiceRoll(Action<Board, bool> RollDices);    //SIGN UP TO EVENT ROLL DICES
        void RollDice(string toClient);                                // FIRE EVENT
        void RollDicesNotificated(Action<Board, bool> whatToExecute, Board board, bool isMyTurn);              // MAKES SURE I WORK ON UI THREAD


        void ListenToGetPlayer(Action<Player> GetPlayers);             ////SIGN UP TO EVENT GET PLAYER
        void GetPlayer(User fromClient);                                                               // FIRE EVENT
        void GetPlayerNotificated(Action<Player> whatToExecute, Player players);            // MAKES SURE I WORK ON UI THREAD



        void ListenToBoardUpdated(Action<Board> UpdatedBoard); 
        void GetOrRemovePick(string toClient, int spikeChosen, Player player);
        void BoardUpdatedNotificated(Action<Board> whatToExecute, Board cells);


        void ListenToBoardUpdatedAndTurn(Action<Board, bool, int, int, bool,bool> UpdatedBoard); 
        void MoveChipToSpike(string toClient, int spikeChosen, Player player);
        void BoardUpdatedAndChangeTurnNotificated(Action<Board, bool, int, int,bool,bool> whatToExecute, Board cells, bool isMyTurn, int NumberTurn, int TotalTurns, bool IsTurnCanceled, bool didPlayerMove);

    }
}
