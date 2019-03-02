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
        static HubConnection hubConnection;
        static IHubProxy userHubProxy;
        Dictionary<string, string> UserDetails;

        public IHubProxy UserHubProxy { get => userHubProxy; }
        public HubConnection HubConnection { get => hubConnection; }

        #region Enter App / Chat Stuff
        public void ListenToGroupMessages(Action<string, string> SendMessageToAllAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            userHubProxy.On("broadcastMessage", (string userName, string message) =>
            {
                Application.Current.Dispatcher.Invoke(() => SendMessageToAllAction.Invoke(userName, message));
            });
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
        public void ListenToStatusChangedEvents(Action<UserDetails> LogInNotification, Action<UserDetails> LogOffNotification, Action<UserDetails> InGameNotification, Action<UserDetails>RegisterNotification, UserDetails user)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            //Server methods should be called on non UI thread
            Task connectTask = Task.Run(() =>
            {
                userHubProxy.On("LogInNotificated", (UserDetails userName) => Notify(LogInNotification, userName));
                hubConnection.Start().Wait();

            });
            connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();

            Task connectTask2 = Task.Run(() =>
            {
                userHubProxy.On("LogOffNotificated", (UserDetails userName) => Notify(LogOffNotification, userName));
                hubConnection.Start().Wait();
            });
            connectTask2.ConfigureAwait(false);
            connectTask2.Wait();

            Task connectTask3 = Task.Run(() =>
            {
                userHubProxy.On("InGameNotificated", (UserDetails userName) => Notify(InGameNotification, userName));
                hubConnection.Start().Wait();
            });
            connectTask3.ConfigureAwait(false);
            connectTask3.Wait();

            Task connectTask4 = Task.Run(() =>
            {
                userHubProxy.On("RegisterNotificated", (UserDetails userName) => Notify(RegisterNotification, userName));
                hubConnection.Start().Wait();
            });
            connectTask4.ConfigureAwait(false);
            connectTask4.Wait();


        }
        public void ListenToGameInvitations(Action<UserDetails> InviteGameAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastInvitationGame", (UserDetails name) => InviteGameNotificated(InviteGameAction, name));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }
        public void ListenAnswerToGameInvitations(Action<UserDetails, bool> InviteGameAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("broadcastAnswerInvitationGame", (UserDetails name, bool answer) => AnswerInviteGameNotificated(InviteGameAction, name, answer));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }
        public void ListenGetUserDetails (Action<UserDetails> InviteGameAction)   //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS)
        {
            Application.Current.Dispatcher.Invoke(
                 () =>
                 {
                     // Code to run on the GUI thread.
                     Task connectTask = Task.Run(() =>
                     {
                         userHubProxy.On("GetMyUserDetails", (UserDetails name) => Notify(InviteGameAction, name));
                         hubConnection.Start().Wait();
                     });
                     connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                     connectTask.Wait();
                 });
        }


        public async Task<IEnumerable<UserDetails>> GetAllUsers()
        {
            try
            {
                var getAllUsersTask = await userHubProxy.Invoke<IEnumerable<UserDetails>>("GetAllUsers");
                return getAllUsersTask.OrderByDescending(u => u.Status).ThenBy(u => u.UserName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> SignIn(CommonUser user)   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            try
            {
                await ConnectToServer(user);
                var registerTask = userHubProxy.Invoke<string>("SignIn", user);
                return registerTask.Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public string SignOut()
        {
            try
            {
                var logOffTask = userHubProxy.Invoke<string>("SignOut");
                hubConnection.Dispose();
                return logOffTask.Result;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }            // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        public void InGame()
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("InGame").Wait(5000);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }            // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        public async Task<string> Register(CommonUser user)
        {
            try
            {
                await ConnectToServer(user);
                var registerTask = userHubProxy.Invoke<string>("Register", user);
                return registerTask.Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public async Task<string> InviteClientForGame(UserDetails fromClient, UserDetails toClient)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            var sendGameReqestTask = await userHubProxy.Invoke<string>("InviteToGame", fromClient, toClient);
            return sendGameReqestTask;
        }
        public async Task<string> AnswerInviteClientForGame(UserDetails fromClient, UserDetails toClient, bool answer)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread

            var answerGameRequestTask = await userHubProxy.Invoke<string>("AnswerInviteToGame", fromClient, toClient, answer);
            return answerGameRequestTask;
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
        public void GetUserDetails()
        {
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("GetMyUserDetails").Wait(200);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
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
        public void InviteGameNotificated(Action<UserDetails> whatToExecute, UserDetails name)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(name);
                   });
        }
        public void AnswerInviteGameNotificated(Action<UserDetails, bool> whatToExecute, UserDetails name, bool answer)                          //מעטפת
        {
            Application.Current.Dispatcher.Invoke(() => {
                whatToExecute(name, answer);
            });
        }
        public void Notify(Action<UserDetails> whatToExecute, UserDetails name)                          //מעטפת
        {
            Application.Current.Dispatcher.Invoke(() => {
                whatToExecute(name);
            });
        }





        private async Task<bool> ConnectToServer(CommonUser user)
        {
            try
            {

                UserDetails = new Dictionary<string, string>();
                UserDetails.Add("UserName", user.UserName);
                hubConnection = new HubConnection("http://localhost:52527/", UserDetails);
                userHubProxy = hubConnection.CreateHubProxy("UserHub");
                await hubConnection.Start();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void DisconnectFromServer()
        {
            //Server methods should be called on non UI thread
            Task connectTask = Task.Run(() =>
            {
                // for now
                hubConnection.Dispose();
            });
            connectTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            connectTask.Wait();
        }                   // disconnect from the hub
       
        #endregion








    }
}
