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
        IHubProxy UserHubProxy { get; }
        HubConnection HubConnection { get; }


        Task<IEnumerable<UserDetails>> GetAllUsers();

        void ListenToGroupMessages(Action<string, string> SendMessageToAllAction);    //SIGN UP TO EVENT
        void SendMessageToAll(string msg);                                               // FIRE EVENT


        void ListenToClientMessages(Action<string, string,string> SendMessageToAllAction);     //SIGN UP TO EVENT
        void SendMessageToClient(string msg, string toClient);                             // FIRE EVENT






        void ListenToStatusChangedEvents(Action<UserDetails> LogInNotification, Action<UserDetails> LogOffNotification, Action<UserDetails> InGameNotification, Action<UserDetails> RegisterNotification, UserDetails user);    //SIGN UP TO EVENT SignInEVENT && SignOutEVENT
        void Notify(Action<UserDetails> whatToExecute, UserDetails message);                 // MAKES SURE I WORK ON UI THREAD
        Task<string> SignIn(CommonUser user);                                                          // FIRE EVENT
        string SignOut();                                                        // FIRE EVENT
        void InGame();                                                         // FIRE EVENT
        Task<string> Register(CommonUser user);

        void ListenToGameInvitations(Action<UserDetails> InviteGameAction);                   //SIGN UP TO EVENT
        Task<string> InviteClientForGame(UserDetails fromClient, UserDetails toClient);                       // FIRE EVENT
        void InviteGameNotificated(Action<UserDetails> whatToExecute, UserDetails name);             // MAKES SURE I WORK ON UI THREAD

        void ListenAnswerToGameInvitations(Action<UserDetails, bool> InviteGameAction);    //SIGN UP TO EVENT
        Task<string> AnswerInviteClientForGame(UserDetails fromClient, UserDetails toClient, bool answer);                  // FIRE EVENT
        void AnswerInviteGameNotificated(Action<UserDetails, bool> whatToExecute, UserDetails name, bool answer);                          // MAKES SURE I WORK ON UI THREAD

        void DisconnectFromServer();                                             //DISCONNECT ME FROM SERVER


        void GetUserDetails();
        void ListenGetUserDetails(Action<UserDetails> InviteGameAction);



    }
}
