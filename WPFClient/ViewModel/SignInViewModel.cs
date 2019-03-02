using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFClient.Infra;


namespace WPFClient.ViewModel
{


    public class SignInViewModel : ViewModelBase
    {

        #region Members
        IChatService _chatService;
        Infra.IDialogService _messageService;
        private IFrameNavigationService _navigationService;
        private string _userName;
        private string _password;
        #endregion

        #region Properties

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



        #endregion

        #region Commands
        public RelayCommand SignInUserCommand { get; set; }
        public RelayCommand GoToRegisterView { get; set; }
        #endregion

        public SignInViewModel(IChatService chatService, Infra.IDialogService messageService, IFrameNavigationService navigationService)
        {
            _messageService = messageService;
            _chatService = chatService;
            _navigationService = navigationService;
            InitCommands();
        }

        #region Methods
        public void InitCommands()
        {
            SignInUserCommand = new RelayCommand(async() =>
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
                CommonUser newUser = new CommonUser() { UserName = this.UserName, Password = this.Password };
                UserName = "";
                Password = "";

                var ErrorMessage = await _chatService.SignIn(newUser);
                if (ErrorMessage != "")
                    _messageService.ShowError(ErrorMessage, "Sign In");
                else
                {
                    //_messageService.ShowMessage("You Successfully Logged In!", "GREAT!");
                    _navigationService.NavigateTo("LobbyWindow");
                }

            });


            GoToRegisterView = new RelayCommand(() =>
            {
                UserName = "";
                Password = "";
                _navigationService.NavigateTo("RegisterWindow");
            });
        }
        #endregion

    }
}









