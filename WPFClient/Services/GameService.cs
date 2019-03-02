using Common;
using Common.Backgammon;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient.Services
{
    public class GameService
    {
        
        public string GameId { get; set; }
        HubConnection hubConnection;
        public IHubProxy userHubProxy { get; private set; }
        public Board Board { get; private set; }
        public string UserName { get; private set; }
        public string EnemyName { get; private set; }




        public GameService(string userName, IHubProxy hubProxy, HubConnection HubConnection)
        {
            UserName = userName;
            userHubProxy = hubProxy;
            hubConnection = HubConnection;
        }

        #region Game
        public void ListenToGetPlayer(Action<Player,UserDetails> GetPlayers) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask = Task.Run(() =>
                       {
                           userHubProxy.On("GetMyPlayer", (Player player, UserDetails user) => GetPlayerNotificated(GetPlayers, player, user));
                           hubConnection.Start().Wait();
                       });
                       connectTask.ConfigureAwait(false);  //Does not return to and deadlocks the UI thread after execution
                       connectTask.Wait();
                   });
        }

        public void GetPlayer()                  // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
        {
            //Server methods should be called on non UI thread
            Task registerTask = Task.Run(() =>
            {
                userHubProxy.Invoke("GetPlayers").Wait(100);
                return;
            });
            registerTask.ConfigureAwait(false);//Does not return to and deadlocks the UI thread after execution
            registerTask.Wait();
        }

        public void GetPlayerNotificated(Action<Player, UserDetails> whatToExecute, Player player, UserDetails user)                          //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(player, user);
                   });
        }


        public void ListenToDiceRoll(Action<Board, bool> RollDices) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
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

        public void GetOrRemovePick(string toClient, int spikeChosen, Player player)                   // SHOOT EVENT TO WHOEVER SIGNED UP TO IT
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



        public void ListenToBoardUpdatedAndTurn(Action<Board, bool, int, int, bool, bool> UpdatedBoard) //SIGN UP TO THIS EVENT (WHENEVER IT SHOOTS) 
        {
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       Task connectTask2 = Task.Run(() =>
                       {
                           userHubProxy.On("broadcastMovedChipToClientAndMe", (Board action, bool IsMyTurn, int NumberTurn, int TotalTurns, bool IsTurnCanceled, bool didPlayerMove) => BoardUpdatedAndChangeTurnNotificated(UpdatedBoard, action, IsMyTurn, NumberTurn, TotalTurns, IsTurnCanceled, didPlayerMove));
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

        public void BoardUpdatedAndChangeTurnNotificated(Action<Board, bool, int, int, bool, bool> whatToExecute, Board board, bool isMyTurn, int NumberTurn, int TotalTurns, bool IsTurnCanceled, bool didPlayerMove)         //מעטפת
        {
            //View Model methods should run on the main thread
            Application.Current.Dispatcher.Invoke(
                   () =>
                   {
                       // Code to run on the GUI thread.
                       whatToExecute(board, isMyTurn, NumberTurn, TotalTurns, IsTurnCanceled, didPlayerMove);
                   });
        }



        #endregion

    }
}
