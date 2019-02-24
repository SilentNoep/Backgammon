using Common;
using Common.Backgammon;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFClient.Infra;

namespace WPFClient.Services
{
    public class ChatService : IChatService
    {
        HubConnection hubConnection;
        IHubProxy userHubProxy;
        Dictionary<string, string> UserDetails;
        public User User { get; set; }
        public User SelectedUser { get; set; }
        public bool HasEvents { get; set; }
        public bool IsConnected { get; set; }
        public void GetUser(User user)            // Get The User
        {
            User = user;
        }
        public void GetSelectedUser(User user)            // Get The Selected User
        {
            SelectedUser = user;
        }


        #region Enter App / Chat Stuff
        public void ConnectToHub()
        {
            UserDetails = new Dictionary<string, string>();
            UserDetails.Add("UserName", User.UserName);
            hubConnection = new HubConnection("http://localhost:52527/",UserDetails);
            userHubProxy = hubConnection.CreateHubProxy("UserHub");
            IsConnected = true;
        }                               //connect to the hub

        public void ListenToGroupMessages(Action<string, string> SendMessageToAllAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastMessage", (string name, string message) => MessageNotificated(SendMessageToAllAction, name, message));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }

        public void SendMessageToAll(string msg)             // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            Task connectTask = Task.Run(() =>
            {
                if (hubConnection.State == ConnectionState.Connected)
                {
                    userHubProxy.Invoke("SendToAll", msg).Wait(500);
                }
            });
            connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();
        }

        public void ListenToClientMessages(Action<string, string> SendMessageToAllAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastMessageToClient", (string name, string message) => MessageNotificated(SendMessageToAllAction, name, message));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }

        public void SendMessageToClient(string msg, string username)             // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            Task connectTask = Task.Run(() =>
            {
                if (hubConnection.State == ConnectionState.Connected)
                {
                    userHubProxy.Invoke("SendTo", msg, username).Wait(500);
                }
            });
            connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();
        }

        public void ListenToStatusChangedEvents(Action<User> LogInNotification, Action<User> LogOffNotification, Action<User> InGameNotification, User user)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            //Server methods should be called on non UI thread
            Task connectTask = Task.Run(() =>
            {
                userHubProxy.On("LogInNotificated", (User userName) => LogInNotificated(LogInNotification, userName));
                hubConnection.Start().Wait();

            });
            connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();

            Task connectTask2 = Task.Run(() =>
            {
                userHubProxy.On("LogOffNotificated", (User userName) => LogOffNotificated(LogOffNotification, userName));
                hubConnection.Start().Wait();
            });
            connectTask2.ConfigureAwait(false);
            connectTask2.Wait();

            Task connectTask3 = Task.Run(() =>
            {
                userHubProxy.On("InGameNotificated", (User userName) => InGameNotificated(InGameNotification, userName));
                hubConnection.Start().Wait();
            });
            connectTask3.ConfigureAwait(false);
            connectTask3.Wait();

            HasEvents = true;

        }

        public void SignIn(User user)   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("SignIn", user).Wait(500);

            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void SignOut(User user)
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("SignOut", user).Wait(5000);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }            // SHOOT EVENT TO WHOEVER SIGNED UP TO IT

        public void InGame(User user)
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("InGame", user).Wait(5000);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }            // SHOOT EVENT TO WHOEVER SIGNED UP TO IT

        public void DisconnectFromServer()
        {
            //Server methods should be called on non UI thread
            Task connectTask = Task.Run(() =>
            {
                // for now
                hubConnection.Dispose();
                IsConnected = false;

            });

            connectTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();
        }                   // disconnect from the hub

        public void LogOffNotificated(Action<User> whatToExecute, User message)
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(message);
                   });

        }               //מעטפת 

        public void LogInNotificated(Action<User> whatToExecute, User message)                         //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(message);
                   });
        }

        public void InGameNotificated(Action<User> whatToExecute, User message)                         //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(message);
                   });
        }



        public void MessageNotificated(Action<string, string> whatToExecute, string name, string message)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(name, message);
                   });
        }





        public void ListenToGameInvitations(Action<User> InviteGameAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastInvitationGame", (User name) => InviteGameNotificated(InviteGameAction, name));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }

        public async Task<string> InviteClientForGame(User fromClient, User toClient)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            var sendGameReqestTask = await userHubProxy.Invoke<string>("InviteToGame", fromClient, toClient);
            return sendGameReqestTask;
        }

        public void InviteGameNotificated(Action<User> whatToExecute, User name)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(name);
                   });
        }

        public void ListenAnswerToGameInvitations(Action<User, bool> InviteGameAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastAnswerInvitationGame", (User name, bool answer) => AnswerInviteGameNotificated(InviteGameAction, name, answer));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }

        public async Task<string> AnswerInviteClientForGame(User fromClient, User toClient, bool answer)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread

            var answerGameRequestTask = await userHubProxy.Invoke<string>("AnswerInviteToGame", fromClient, toClient, answer);
            return answerGameRequestTask;
        }

        public void AnswerInviteGameNotificated(Action<User, bool> whatToExecute, User name, bool answer)                          //מעטפת
        {
            var letssee = hubConnection.LastError;
            //View Model methods should run on the main thread

            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                    
                       // Code to run on the GUI thread.
                       whatToExecute(name, answer);
                       
                   });
           

            
        }
        #endregion

        #region Game
        public void ListenToGetPlayer(Action<Player> GetPlayers) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask = Task.Run(() =>
                       {
                           userHubProxy.On("GetMyPlayer", (Player action) => GetPlayerNotificated(GetPlayers, action));
                           hubConnection.Start().Wait();
                       });
                       connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                       connectTask.Wait();
                   });
        }

        public void GetPlayer(User fromClient)                  // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("GetPlayers", fromClient).Wait(100);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void GetPlayerNotificated(Action<Player> whatToExecute, Player player)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(player);
                   });
        }


        public void ListenToDiceRoll(Action<Board,bool> RollDices) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask = Task.Run(() =>
                         {
                             userHubProxy.On("broadcastDicesToClientAndMe", (Board action, bool isMyTurn) => RollDicesNotificated(RollDices, action, isMyTurn));
                             hubConnection.Start().Wait();
                         });
                       connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                       connectTask.Wait();
                   });
        }

        public void RollDice(string toClient)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("GetDicesNumbers", toClient).Wait(100);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void RollDicesNotificated(Action<Board, bool> whatToExecute, Board board, bool isMyTurn)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(board, isMyTurn);
                   });
        }



        public void ListenToBoardUpdated(Action<Board> UpdatedBoard) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask = Task.Run(() =>
                       {
                           userHubProxy.On("broadcastChosenSpikeToClientAndMe", (Board action) => BoardUpdatedNotificated(UpdatedBoard, action));
                           hubConnection.Start().Wait();
                       });
                       connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                       connectTask.Wait();
                   });
        }

        public void GetOrRemovePick(string toClient,int spikeChosen,Player player)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("GetOrRemovePick", toClient, spikeChosen, player).Wait(100);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void BoardUpdatedNotificated(Action<Board> whatToExecute, Board board)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(board);
                   });
        }



        public void ListenToBoardUpdatedAndTurn(Action<Board, bool,int,int,bool,bool> UpdatedBoard) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask2 = Task.Run(() =>
                       {
                           userHubProxy.On("broadcastMovedChipToClientAndMe", (Board action, bool IsMyTurn, int NumberTurn,int TotalTurns, bool IsTurnCanceled, bool didPlayerMove) => BoardUpdatedAndChangeTurnNotificated(UpdatedBoard, action, IsMyTurn,NumberTurn,TotalTurns, IsTurnCanceled, didPlayerMove));
                           hubConnection.Start().Wait();
                       });
                       connectTask2.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                       connectTask2.Wait();
                   });
        }

        public void MoveChipToSpike(string toClient, int spikeChosen, Player player)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("MoveChipToSpike", toClient, spikeChosen, player).Wait(100);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void BoardUpdatedAndChangeTurnNotificated(Action<Board, bool,int,int,bool,bool> whatToExecute, Board board, bool isMyTurn, int NumberTurn, int TotalTurns, bool IsTurnCanceled, bool didPlayerMove)         //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(board, isMyTurn,NumberTurn,TotalTurns, IsTurnCanceled, didPlayerMove);
                   });
        }







        #endregion
    }
}
