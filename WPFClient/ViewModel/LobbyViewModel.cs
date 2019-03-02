using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFClient.Infra;
using WPFClient.Models;

namespace WPFClient.ViewModel
{
    public class LobbyViewModel : ViewModelBase
    {
        #region Members
        private IChatService _chatService;
        private Infra.IDialogService _messageService;
        private INavigationService _navigationService;
        private string _message;
        private bool DidISendMessage;
        private ObservableCollection<ChatMessage> _groupMessages;
        private ObservableCollection<ChatMessage> _currentMessages;
        private ObservableCollection<UserDetails> usersOnline;
        private Dictionary<string, ObservableCollection<ChatMessage>> _allChats;
        private Dictionary<string, SolidColorBrush> _usersColors;
        private UserDetails _myUser;
        private UserDetails _selectedUser;
        Random rnd;
        #endregion


        #region Properties / Commands
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<ChatMessage> GroupMessages
        {
            get { return _groupMessages; }
            set
            {
                _groupMessages = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<ChatMessage> CurrentMessages
        {
            get { return _currentMessages; }
            set
            {
                _currentMessages = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<UserDetails> AllUsers
        {
            get { return usersOnline; }
            set
            {
                usersOnline = value;
                RaisePropertyChanged();
            }
        }
        public Dictionary<string, ObservableCollection<ChatMessage>> AllChats
        {
            get { return _allChats; }
            set
            {
                _allChats = value;
                RaisePropertyChanged();
            }
        }
        public UserDetails SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value; RaisePropertyChanged();
                PlayGameCommand.RaiseCanExecuteChanged();
            }
        }
        public UserDetails MyUser
        {
            get { return _myUser; }
            set { _myUser = value; RaisePropertyChanged(); }
        }


        public RelayCommand GetMyUser { get; set; }
        public RelayCommand GetUsersList { get; set; }
        public RelayCommand PlayGameCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand PressGroupChatRoomCommand { get; set; }
        public RelayCommand<UserDetails> SelectUserCommand { get; set; }
        #endregion

        public LobbyViewModel(IChatService chatService, Infra.IDialogService messageService, INavigationService navigationService)
        {
            rnd = new Random();
            GroupMessages = new ObservableCollection<ChatMessage>();
            AllChats = new Dictionary<string, ObservableCollection<ChatMessage>>();
            AllChats.Add("Lobby", GroupMessages);
            CurrentMessages = new ObservableCollection<ChatMessage>();
            CurrentMessages = GroupMessages;
            _chatService = chatService;
            _messageService = messageService;
            _navigationService = navigationService;
            InitCommands();
            Message = "";
            RegisterToEvents();
        }

        #region Methods
        private void InitCommands()
        {
            GetMyUser = new RelayCommand(() =>
            {
                _chatService.GetUserDetails();
            });
            GetUsersList = new RelayCommand(async () =>
            {
                AllUsers = new ObservableCollection<UserDetails>(await _chatService.GetAllUsers());
                _usersColors = new Dictionary<string, SolidColorBrush>();
                foreach (var item in AllUsers)
                    _usersColors.Add(item.UserName, GetRandomColor());
                AllUsers.Remove(AllUsers.First((u) => u.UserName == MyUser.UserName));
            });
            SendMessageCommand = new RelayCommand(() =>
            {
                if (Message != "")
                {
                    if (SelectedUser == null)
                        _chatService.SendMessageToAll(Message);
                    else
                        _chatService.SendMessageToClient(Message, SelectedUser.UserName);
                    Message = "";
                    DidISendMessage = true;
                }
            });
            SelectUserCommand = new RelayCommand<UserDetails>((p) =>
            {
                if (p != null)
                {
                    if (p.Status == Status.Online)
                    {
                        SelectedUser = p;
                        //_chatService.GetSelectedUser(SelectedUser);
                        if (AllChats.ContainsKey(p.UserName))
                            CurrentMessages = AllChats[p.UserName];
                        else
                        {
                            AllChats.Add(p.UserName, new ObservableCollection<ChatMessage>());
                            CurrentMessages = AllChats[p.UserName];
                        }
                        Message = "";
                    }
                    else if (p.Status == Status.InGame)
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
                CurrentMessages = GroupMessages;
            });

            PlayGameCommand = new RelayCommand(async () =>
            {
                MyUser.HasInvitedGame = true;
                string w00t = await _chatService.InviteClientForGame(MyUser, SelectedUser);
            }, () => SelectedUser == null ? false : true);
        }

        private void RegisterToEvents()
        {
            _chatService.ListenToStatusChangedEvents(OnUserLoggedIn, OnUserLoggedOff, OnUserInGame, OnUserRegistered, MyUser);
            _chatService.ListenToGroupMessages(SendMessageToAllAction);
            _chatService.ListenToClientMessages(SendMessageToClient);
            _chatService.ListenToGameInvitations(SendGameInvitation);
            _chatService.ListenAnswerToGameInvitations(AnswerOfGameInvitation);
            _chatService.ListenGetUserDetails(GetUser);
        }
        private void GetUser(UserDetails MyUserFromServer)
        {
            MyUser = MyUserFromServer;
        }


        private void SendGameInvitation(UserDetails userNameInvite)
        {
            if (_messageService.ShowQuestion($"{userNameInvite.UserName} Invited you to a game. do you accept?", "Invitation to game"))
            {
                SelectedUser = userNameInvite;
                if (!AllChats.ContainsKey(SelectedUser.UserName))
                    AllChats.Add(SelectedUser.UserName, new ObservableCollection<ChatMessage>());
                CurrentMessages = AllChats[SelectedUser.UserName];
                _chatService.AnswerInviteClientForGame(MyUser, userNameInvite, true);
            }
            else                                        // decline invitation
                _chatService.AnswerInviteClientForGame(MyUser, userNameInvite, false);
        }


        private void AnswerOfGameInvitation(UserDetails userNamethatInvited, bool Answer)
        {
            if (Answer)
            {
                //_chatService.InGame();
                _chatService.DisconnectFromServer();
                _navigationService.NavigateTo("", SelectedUser);
            }
            //else
            //    _messageService.ShowMessage($"{userNamethatInvited} has declined your invitation", "Declined");


        }

        private void SendMessageToClient(string nameSentTo, string msg, string sender)
        {
            if (!AllChats.ContainsKey(nameSentTo))
                AllChats.Add(nameSentTo, new ObservableCollection<ChatMessage>());
            AllChats[nameSentTo].Add(new ChatMessage() { Text = msg, IsYou = DidISendMessage, Color = _usersColors[sender], Time = DateTime.Now.ToShortTimeString(), Sender = sender});
            DidISendMessage = false;
        }

        private void SendMessageToAllAction(string name, string msg)
        {
            GroupMessages.Add(new ChatMessage() { Text = msg, IsYou = DidISendMessage, Color = _usersColors[name], Time = DateTime.Now.ToShortTimeString(), Sender = name});
            DidISendMessage = false;
        }





        public void OnUserInGame(UserDetails userName)
        {
            var user = AllUsers.First((u) => u.UserName == userName.UserName);
            user.Status = userName.Status;
            AllUsers = new ObservableCollection<UserDetails>(AllUsers.OrderByDescending(u => u.Status).ThenBy(u => u.UserName));
        }

        public void OnUserRegistered(UserDetails userName)
        {
            _usersColors.Add(userName.UserName, GetRandomColor());
            AllUsers.Add(userName);
            AllUsers = new ObservableCollection<UserDetails>(AllUsers.OrderByDescending(u => u.Status).ThenBy(u => u.UserName));
        }

        public void OnUserLoggedIn(UserDetails userName)
        {
            var user = AllUsers.First((u) => u.UserName == userName.UserName);
            user.Status = userName.Status;
            AllUsers = new ObservableCollection<UserDetails>(AllUsers.OrderByDescending(u => u.Status).ThenBy(u => u.UserName));
        }

        public void OnUserLoggedOff(UserDetails userName)
        {
            var user = AllUsers.First((u) => u.UserName == userName.UserName);
            user.Status = userName.Status;
            if (userName.Wins != 0)
                user.Wins = userName.Wins;
            AllUsers = new ObservableCollection<UserDetails>(AllUsers.OrderByDescending(u => u.Status).ThenBy(u => u.UserName));
        }

        private SolidColorBrush GetRandomColor()
        {
            var color = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 180), (byte)rnd.Next(1, 180), (byte)rnd.Next(1, 180)));
            return color;
        }

        #endregion

    }
}
