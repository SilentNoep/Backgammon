using Common;
using Common.Backgammon;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Infra;

namespace WPFClient.ViewModel
{
    public class GameViewModel : ViewModelBase, IPageViewModel
    {
        #region Members
        private IChatService _chatService;
        private IDialogService _messageService;
        private IServerService _serverService;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService;
        private ObservableCollection<string> _currentMessages;
        private bool _isMyTurn;
        private User _myUser;
        private User _selectedUser;
        private Cell _selectedCell;
        private Player _myPlayer;
        private string _message;
        private string _messageToSend;
        private Board _myBoard;
        private BackgammonManager bg;
        #endregion

        #region Properties
        public string Name { get; set; } = "Game";
        public Board MyBoard
        {
            get { return _myBoard; }
            set { _myBoard = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<string> CurrentMessages
        {
            get { return _currentMessages; }
            set
            {
                _currentMessages = value;
                RaisePropertyChanged();
            }
        }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged();
            }
        }
        public User MyUser
        {
            get { return _myUser; }
            set { _myUser = value; RaisePropertyChanged(); }
        }
        public Cell SelectedCell
        {
            get { return _selectedCell; }
            set
            {
                _selectedCell = value;
                RaisePropertyChanged();
            }

        }
        public Player MyPlayer
        {
            get { return _myPlayer; }
            set { _myPlayer = value; RaisePropertyChanged(); }
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }
        public string MessageToSend
        {
            get { return _messageToSend; }
            set { _messageToSend = value; RaisePropertyChanged(); }
        }
        public bool IsMyTurn
        {
            get { return _isMyTurn; }
            set
            {
                _isMyTurn = value;
                RaisePropertyChanged();
                RollDicesCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand<CancelEventArgs> WindowClosingCommand { get; set; }
        public RelayCommand RollDicesCommand { get; set; }
        public RelayCommand<Cell> SelectSpikeCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        #endregion

        public GameViewModel(IChatService chatService, IDialogService messageService, IServerService serverService, GalaSoft.MvvmLight.Views.INavigationService navigationService)
        {
            bg = new BackgammonManager();
            MyPlayer = new Player();
            CurrentMessages = new ObservableCollection<string>();
            _chatService = chatService;
            _messageService = messageService;
            _serverService = serverService;
            _navigationService = navigationService;
            MyUser = _chatService.User;
            _chatService.ListenToDiceRoll(GetDices);
            _chatService.ListenToGetPlayer(GetPlayer);
            _chatService.GetPlayer(MyUser);
            _chatService.ListenToBoardUpdated(UpdateBoard);
            _chatService.ListenToBoardUpdatedAndTurn(UpdateBoardAndTurn);
            _chatService.ListenToClientMessages(SendMessageToClient);
            MyBoard = new Board();
            MyBoard = bg.bgBoard;

            WindowClosingCommand = new RelayCommand<CancelEventArgs>((args) =>
            {
                if (!_messageService.ShowQuestion("Are You Sure You Want To Exit? It Will Be Considered A LOSS!!", "Bye!"))
                    args.Cancel = true;
                else
                {
                    if (_chatService.User != null)
                    {
                        _serverService.ConnectToServerSignIn("", MyUser);
                        _chatService.SignIn(MyUser);
                        _serverService.AddLossToUser("", MyUser);
                    }
                }
            });


            SendMessageCommand = new RelayCommand(() =>
            {
                if (Message != "")
                {
                    _chatService.SendMessageToClient(MessageToSend, SelectedUser.UserName);
                    MessageToSend = "";
                }
            });

            RollDicesCommand = new RelayCommand(() =>
            {
                if (MyPlayer.IsMyTurn)
                {
                    if (!MyPlayer.HasRolled)
                    {
                        MyPlayer.HasRolled = true;
                        _chatService.RollDice(SelectedUser.UserName);
                    }
                    else
                        Message = $"{MyUser.UserName} You Have Rolled Already! Please Pick Your Move !!";
                }
                else
                    Message = $"Its {SelectedUser.UserName}'s Turn !!";

            }, () => IsMyTurn);

            SelectSpikeCommand = new RelayCommand<Cell>((p) =>
            {
                if (MyPlayer.IsMyTurn)
                {
                    if (MyPlayer.HasRolled)
                    {
                        if (SelectedCell != null)
                        {
                            if (SelectedCell.ID == p.ID)
                            {
                                _chatService.GetOrRemovePick(SelectedUser.UserName, SelectedCell.ID, MyPlayer);
                                SelectedCell = null;
                            }
                            else
                            {

                                int number = 0;
                                if (!MyPlayer.IsBaseOnLeft)
                                {
                                    if (SelectedCell.ID < p.ID)
                                    {
                                        if (CanMoveToChosenSpike(p.ID - SelectedCell.ID))
                                        {
                                            if (p.ColorOfCell == MyPlayer.Color || p.ColorOfCell == Color.Empty || p.NumOfSoldiers == 1)
                                            {

                                                _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                SelectedCell = null;
                                            }
                                            else
                                                Message = $"{MyUser.UserName} You Can't Move To A Cell With RED's !!";
                                        }
                                        else
                                        {
                                            if (p.ID == MyBoard.WhitePile.ID)
                                            {
                                                switch (SelectedCell.ID)
                                                {
                                                    case 18:
                                                        number = 12;
                                                        break;
                                                    case 19:
                                                        number = 14;
                                                        break;
                                                    case 20:
                                                        number = 16;
                                                        break;
                                                    case 21:
                                                        number = 18;
                                                        break;
                                                    case 22:
                                                        number = 20;
                                                        break;
                                                    case 23:
                                                        number = 22;
                                                        break;
                                                }

                                                if (CanMoveToPile(SelectedCell.ID - number))
                                                    _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                else
                                                {
                                                    _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                    Message = $"{MyUser.UserName} You Cant GO THERE!!";
                                                }
                                                SelectedCell = null;
                                            }
                                            else
                                                Message = $"{MyUser.UserName} You MUST Move Accordingly to the Dices!!";
                                        }

                                    }
                                    else
                                        Message = $"{MyUser.UserName} You Cant GO BACKWARDS!!";
                                }
                                else
                                {
                                    if (SelectedCell.ID > p.ID)
                                    {
                                        if (CanMoveToChosenSpike(SelectedCell.ID - p.ID))
                                        {
                                            if (p.ColorOfCell == MyPlayer.Color || p.ColorOfCell == Color.Empty || p.NumOfSoldiers == 1)
                                            {
                                                _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                SelectedCell = null;
                                            }
                                            else
                                                Message = $"{MyUser.UserName} You Cant GO THERE With That Soldier!!";
                                        }
                                        else
                                        {
                                            if (p.ID == MyBoard.RedPile.ID)
                                            {
                                                number = -1;
                                                if (CanMoveToPile(SelectedCell.ID - number))
                                                    _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                else
                                                {
                                                    _chatService.MoveChipToSpike(SelectedUser.UserName, p.ID, MyPlayer);
                                                    Message = $"{MyUser.UserName} You Cant GO THERE With That Soldier!!";
                                                }

                                                SelectedCell = null;
                                            }
                                            else
                                                Message = $"{MyUser.UserName} You MUST Move Accordingly to the Dices!!";
                                        }
                                    }
                                    else
                                        Message = $"{MyUser.UserName} You Cant GO BACKWARDS!!";
                                }
                            }
                        }
                        else
                        {
                            if (p.ColorOfCell == MyPlayer.Color)
                            {
                                if (MyPlayer.Color == MyBoard.EatenRedCell.ColorOfCell && MyBoard.EatenRedCell.NumOfSoldiers > 0 && p.ID != MyBoard.EatenRedCell.ID)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick That Cell When You Got A EATEN SOLDIER!!";
                                }
                                else if (MyPlayer.Color == MyBoard.EatenWhiteCell.ColorOfCell && MyBoard.EatenWhiteCell.NumOfSoldiers > 0 && p.ID != MyBoard.EatenWhiteCell.ID)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick That Cell When You Got A EATEN SOLDIER!!";
                                }
                                else if (MyPlayer.Color == MyBoard.EatenWhiteCell.ColorOfCell && p.ID == MyBoard.EatenWhiteCell.ID && MyBoard.EatenWhiteCell.NumOfSoldiers == 0)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick That Cell When You Dont Have Any Eaten Soldier/s!!";
                                }
                                else if (MyPlayer.Color == MyBoard.EatenRedCell.ColorOfCell && p.ID == MyBoard.EatenRedCell.ID && MyBoard.EatenRedCell.NumOfSoldiers == 0)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick That Cell When You Dont Have Any Eaten Soldier/s!!";
                                }
                                else if (MyPlayer.Color == MyBoard.WhitePile.ColorOfCell && p.ID == MyBoard.EatenRedCell.ID && SelectedCell == null)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick This Cell, You Can Only Take Out Soldiers If All Your Soldiers Are In Home";
                                }
                                else if (MyPlayer.Color == MyBoard.RedPile.ColorOfCell && p.ID == MyBoard.RedPile.ID && SelectedCell == null)
                                {
                                    Message = $"{MyUser.UserName} You Cant Pick This Cell, You Can Only Take Out Soldiers If All Your Soldiers Are In Home";
                                }
                                else
                                {
                                    SelectedCell = p;
                                    _chatService.GetOrRemovePick(SelectedUser.UserName, SelectedCell.ID, MyPlayer);
                                }

                            }
                        }
                    }
                    else
                        Message = $"{MyUser.UserName} You Must Roll the Dices First !!";
                }
                else
                {
                    if (p.ColorOfCell == MyPlayer.Color)
                        Message = $"Its {SelectedUser.UserName}'s Turn !!";
                }
            });
        }

        #region Methods
        private bool CanMoveToChosenSpike(int numberOfCubePicked)
        {
            if (!MyBoard.Dices.IsCube1Used && numberOfCubePicked == MyBoard.Dices.Cube1)
                return true;
            else if (!MyBoard.Dices.IsCube2Used && numberOfCubePicked == MyBoard.Dices.Cube2)
                return true;
            else
                return false;
        }
        private bool CanMoveToPile(int numberOfCubePicked)
        {
            if (!MyBoard.Dices.IsCube1Used && numberOfCubePicked == MyBoard.Dices.Cube1)
                return true;
            else if (!MyBoard.Dices.IsCube2Used && numberOfCubePicked == MyBoard.Dices.Cube2)
                return true;


            else if (numberOfCubePicked != MyBoard.Dices.Cube1 && numberOfCubePicked != MyBoard.Dices.Cube2 ||
                MyBoard.Dices.IsCube1Used && numberOfCubePicked != MyBoard.Dices.Cube2 ||
                MyBoard.Dices.IsCube2Used && numberOfCubePicked != MyBoard.Dices.Cube1)
            {
                if (MyPlayer.Color == Color.White)
                {
                    if (MyBoard.Dices.Cube1 > numberOfCubePicked)
                    {
                        for (int i = 18; i != SelectedCell.ID; i++)
                        {
                            if (MyBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                    else if (MyBoard.Dices.Cube2 > numberOfCubePicked)
                    {
                        for (int i = 18; i != SelectedCell.ID; i++)
                        {
                            if (MyBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                }
                else if (MyPlayer.Color == Color.Red)
                {
                    if (MyBoard.Dices.Cube1 > numberOfCubePicked)
                    {
                        for (int i = 5; i != SelectedCell.ID; i--)
                        {
                            if (MyBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                    else if (MyBoard.Dices.Cube2 > numberOfCubePicked)
                    {
                        for (int i = 5; i != SelectedCell.ID; i--)
                        {
                            if (MyBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private void SendMessageToClient(string name, string msg)
        {
            CurrentMessages.Add($"{DateTime.Now.ToString("HH:mm")} : {name} : {msg}");
        }

        private void GetDices(Board board, bool isMyTurn)
        {
            MyBoard = board;


            if (MyPlayer.IsMyTurn && !isMyTurn || (!MyPlayer.IsMyTurn && isMyTurn))
            {
                if (MyPlayer.IsMyTurn && !isMyTurn)
                {
                    Message = $"{MyUser.UserName} Rolled And Got {MyBoard.Dices.Cube1} & {MyBoard.Dices.Cube2} - But Doesnt Have Any Option To Move A Soldier! \n {SelectedUser.UserName}'s TURN!!";
                    MyPlayer.HasRolled = false;
                }
                if (!MyPlayer.IsMyTurn && isMyTurn)
                    Message = $"{SelectedUser.UserName} Rolled And Got {MyBoard.Dices.Cube1} & {MyBoard.Dices.Cube2} - But Doesnt Have Any Option To Move A Soldier! \n {MyUser.UserName}'s TURN!!";
                MyPlayer.IsMyTurn = isMyTurn;
                IsMyTurn = isMyTurn;
            }
            else
            {
                if (MyPlayer.IsMyTurn)
                    Message = $"{MyUser.UserName} Rolled And Got {MyBoard.Dices.Cube1} & {MyBoard.Dices.Cube2} !!";
                else
                    Message = $"{SelectedUser.UserName} Rolled And Got {MyBoard.Dices.Cube1} & {MyBoard.Dices.Cube2} !!";
            }
        }

        private void GetPlayer(Player player)
        {
            MyPlayer = player;
            IsMyTurn = player.IsMyTurn;
            if (MyPlayer.IsMyTurn)
            {
                Message = $"{MyUser.UserName} Is The White's! \n{SelectedUser.UserName} Is The Red's! \n";
                Message += $"Its {MyUser.UserName}'s Turn to Roll!!";
            }
            else
            {
                Message = $"{SelectedUser.UserName} Is The White's! \n{MyUser.UserName} Is The Red's! \n";
                Message += $"Its {SelectedUser.UserName}'s Turn to Roll!!";
            }
        }

        private void UpdateBoard(Board board)
        {
            MyBoard = board;
            Message = "";
        }

        private void UpdateBoardAndTurn(Board board, bool isMyTurn, int numberTurn, int TotalTurns, bool isTurnCanceled, bool didPlayerMove)
        {

            MyBoard = board;
            if (isTurnCanceled)
            {
                if (MyPlayer.IsMyTurn && !isMyTurn)
                {
                    Message = $"{MyUser.UserName} Used What He Could, But Cant Use His Turn Anymore According To Dice! \n {SelectedUser.UserName}'s TURN!!";
                    MyPlayer.HasRolled = false;
                }
                if (!MyPlayer.IsMyTurn && isMyTurn)
                    Message = $"{SelectedUser.UserName}  Used What He Could, But Cant Use His Turn Anymore According To Dice! \n {MyUser.UserName}'s TURN!!";
                MyPlayer.IsMyTurn = isMyTurn;
                IsMyTurn = isMyTurn;
            }
            else
            {
                if (didPlayerMove)
                {
                    MyPlayer.IsMyTurn = isMyTurn;
                    IsMyTurn = isMyTurn;


                    if (numberTurn == 0)
                    {
                        if (MyPlayer.IsMyTurn)
                        {
                            Message = $"{SelectedUser.UserName} Has Picked!! \n";
                            Message += $"Its {MyUser.UserName}'s Turn to Roll the Dices!!";
                        }
                        else
                        {
                            MyPlayer.HasRolled = false;
                            Message = $"{MyUser.UserName} Has Picked!! \n";
                            Message += $"Its {SelectedUser.UserName}'s Turn to Roll Dices!!";
                        }
                    }
                    else
                    {
                        if (MyPlayer.IsMyTurn)
                        {
                            Message = $"{MyUser.UserName} Has Picked his {numberTurn} out of {TotalTurns} turn!! \n";
                        }
                        else
                        {
                            Message = $"{SelectedUser.UserName} Has Picked his {numberTurn} out of {TotalTurns} turn!! \n";
                        }
                    }
                }
            }
        }

        private void GameWon(string winnerUserName)
        {
            _messageService.ShowInfo($"{winnerUserName} Has Won !!", "Game Over");
            _navigationService.GoBack();
        }
        #endregion

    }
}
