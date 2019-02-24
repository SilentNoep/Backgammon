using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Infra;
using WPFClient.Messages;

namespace WPFClient.ViewModel
{
    public class LobbyViewModel : ViewModelBase, IPageViewModel
    {
        #region Members
        private IChatService _chatService;
        private IDialogService _messageService;
        private IServerService _serverService;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService;
        private string _message;
        private ObservableCollection<string> _groupMessages;
        private ObservableCollection<string> _currentMessages;
        private ObservableCollection<User> usersOnline;
        private Dictionary<string, ObservableCollection<string>> _allChats;
        private User _myUser;
        private User _selectedUser;
        #endregion


        #region Properties / Commands
        public string Name { get; set; } = "Lobby";
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<string> GroupMessages
        {
            get { return _groupMessages; }
            set
            {
                _groupMessages = value;
                RaisePropertyChanged();
            }
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
        public ObservableCollection<User> AllUsers
        {
            get { return usersOnline; }
            set
            {
                usersOnline = value;
                RaisePropertyChanged();
            }
        }
        public Dictionary<string, ObservableCollection<string>> AllChats
        {
            get { return _allChats; }
            set
            {
                _allChats = value;
                RaisePropertyChanged();
            }
        }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value; RaisePropertyChanged();
                PlayGameCommand.RaiseCanExecuteChanged();
            }
        }
        public User MyUser
        {
            get { return _myUser; }
            set { _myUser = value; RaisePropertyChanged(); }
        }




        public RelayCommand PlayGameCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand PressGroupChatRoomCommand { get; set; }
        public RelayCommand<User> SelectUserCommand { get; set; }
        #endregion

        public LobbyViewModel(IChatService chatService, IDialogService messageService, IServerService serverService, GalaSoft.MvvmLight.Views.INavigationService navigationService)
        {
            GroupMessages = new ObservableCollection<string>();
            AllChats = new Dictionary<string, ObservableCollection<string>>();
            CurrentMessages = new ObservableCollection<string>();
            CurrentMessages = GroupMessages;
            _chatService = chatService;
            _messageService = messageService;
            _serverService = serverService;
            _navigationService = navigationService;
            InitCommands();
            MyUser = _chatService.User;
            var allusers = _serverService.GetAllUsers("");
            var userwithoutme = allusers.Where(p => p.UserName != MyUser.UserName);
            AllUsers = new ObservableCollection<User>(userwithoutme);
            AllChats.Add("Lobby", GroupMessages);
            Message = "";
            if (_chatService.IsConnected || _chatService.HasEvents)
            {
                _chatService.ListenToStatusChangedEvents(OnUserLoggedIn, OnUserLoggedOff,OnUserInGame, MyUser);
                _chatService.ListenToGroupMessages(SendMessageToAllAction);
                _chatService.ListenToClientMessages(SendMessageToClient);
                _chatService.ListenToGameInvitations(SendGameInvitation);
                _chatService.SignIn(MyUser);
                _chatService.ListenAnswerToGameInvitations(AnswerOfGameInvitation);
            }
            if (!_chatService.IsConnected)
            {
                _chatService.ConnectToHub();
            }
        }

        #region Methods
        private void InitCommands()
        {
            SendMessageCommand = new RelayCommand(() =>
            {
                if(Message != "")
                {
                    if (SelectedUser == null)
                        _chatService.SendMessageToAll(Message);
                    else
                        _chatService.SendMessageToClient(Message, SelectedUser.UserName);
                    Message = "";
                }
            });
            SelectUserCommand = new RelayCommand<User>((p) =>
            {
                if (p != null)
                {
                    if (p.Status == Status.Online)
                    {
                        SelectedUser = p;
                        _chatService.GetSelectedUser(SelectedUser);
                        if (AllChats.ContainsKey(p.UserName))
                            CurrentMessages = AllChats[p.UserName];
                        else
                        {
                            AllChats.Add(p.UserName, new ObservableCollection<string>());
                            CurrentMessages = AllChats[p.UserName];
                        }
                        Message = "";
                    }
                    else if(p.Status == Status.InGame)
                    {
                        _messageService.ShowError("User Is In Middle of a Game Please TRY AGAIN WHEN HE's Finished", "User In Game");
                        SelectedUser = null;
                    }
                    else
                    {
                        _messageService.ShowError("User Is OFFLINE TRY AGAIN WHEN HE IS ONLINE", "Offline User");
                        SelectedUser = null;
                    }
                       

                }
            });
            PressGroupChatRoomCommand = new RelayCommand(() =>
            {
                SelectedUser = null;
                _chatService.GetSelectedUser(null);
                CurrentMessages = GroupMessages;
            });

            PlayGameCommand = new RelayCommand(async () =>
            {
                _chatService.User.HasInvitedGame = true;
                string w00t = await _chatService.InviteClientForGame(MyUser, SelectedUser);
            }, () => SelectedUser == null ? false : true);
        }


        private void SendGameInvitation(User userNameInvite)
        {
            if (_messageService.ShowQuestion($"{userNameInvite.UserName} Invited you to a game. do you accept?", "Invitation to game"))
            {
                SelectedUser = userNameInvite;
                _chatService.AnswerInviteClientForGame(MyUser, userNameInvite, true);
            }
            else                                        // decline invitation
                _chatService.AnswerInviteClientForGame(MyUser, userNameInvite, false);
        }


        private void AnswerOfGameInvitation(User userNamethatInvited, bool Answer)
        {
            if (Answer)
            {
                _serverService.ConnectToServerInGame("", MyUser);
                _chatService.InGame(MyUser);
                _chatService.DisconnectFromServer();
                _navigationService.NavigateTo("", SelectedUser);
            }
            //else
            //    _messageService.ShowMessage($"{userNamethatInvited} has declined your invitation", "Declined");


        }

        private void SendMessageToClient(string name, string msg)
        {
            if (!AllChats.ContainsKey(name))
                AllChats.Add(name, new ObservableCollection<string>());
            AllChats[name].Add($"{DateTime.Now.ToString("HH:mm")} : {name} : {msg}");
        }

        private void SendMessageToAllAction(string name, string msg)
        {
            GroupMessages.Add($"{DateTime.Now.ToString("HH:mm")} : {name} : {msg}");
        }

        public void OnUserInGame(User userName)
        {
            var allusers = _serverService.GetAllUsers("");
            var userwithoutme = allusers.Where(p => p.UserName != MyUser.UserName);
            AllUsers = new ObservableCollection<User>(userwithoutme);
        }
        public void OnUserLoggedIn(User userName)
        {
            var allusers = _serverService.GetAllUsers("");
            var userwithoutme = allusers.Where(p => p.UserName != MyUser.UserName);
            AllUsers = new ObservableCollection<User>(userwithoutme);
        }
        public void OnUserLoggedOff(User userName)
        {
            var allusers = _serverService.GetAllUsers("");
            var userwithoutme = allusers.Where(p => p.UserName != MyUser.UserName);
            AllUsers = new ObservableCollection<User>(userwithoutme);
        }

        #endregion

    }
}
