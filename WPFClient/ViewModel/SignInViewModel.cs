using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFClient.Infra;
using WPFClient.Messages;

namespace WPFClient.ViewModel
{


    public class SignInViewModel : ViewModelBase, IPageViewModel
    {

        #region Members
        IChatService _chatService;
        IDialogService _messageService;
        IServerService _serverService;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService;
        private string _userName;
        private string _password;
        #endregion

        #region Properties

        public string Name { get; set; } = "Sign In";
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }
        public User User { get; set; }


        #endregion

        #region Commands
        public RelayCommand SignInUserCommand { get; set; }
        public RelayCommand GoToRegisterView { get; set; }
        #endregion

        public SignInViewModel(IChatService chatService, IDialogService messageService, IServerService serverService, GalaSoft.MvvmLight.Views.INavigationService navigationService)
        {
            _messageService = messageService;
            _serverService = serverService;
            _chatService = chatService;
            _navigationService = navigationService;
            InitCommands();
        }

        #region Methods
        public void InitCommands()
        {
            SignInUserCommand = new RelayCommand(() =>
            {
                if (UserName == "" || UserName == null || Password == "" || Password == null)
                {
                    _messageService.ShowError("Username & Password are required!", "Invalid Input");
                    return;
                }
                if (Password.Length < 4 || UserName.Length < 4)
                {
                    _messageService.ShowError("Username & Password Must Have Atleast 4 Characters", "Invalid Input");
                    return;
                }
                User = new User() { UserName = this.UserName, Password = this.Password };
                UserName = "";
                Password = "";
                if (_serverService.ConnectToServerSignIn("checkUserValidation", User))
                {
                    _messageService.ShowMessage("You Successfully Logged In!", "GREAT!");
                    _chatService.GetUser(User);
                    Messenger.Default.Send(new PageService()
                    {
                        currentPage = new LobbyViewModel(_chatService, _messageService, _serverService, _navigationService)
                    });
                }
                else
                {
                    _messageService.ShowError("UserName Or Password Does not Match!", "Please Try Again!");
                }
            });
            GoToRegisterView = new RelayCommand(() =>
            {
                Messenger.Default.Send(new PageService()
                {
                    currentPage = new RegisterViewModel(_chatService, _messageService, _serverService, _navigationService)
                });
            });
        }
        #endregion

    }
}









