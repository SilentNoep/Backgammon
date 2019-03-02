using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WPFClient.Infra;


namespace WPFClient.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        #region Fields
        IChatService _chatService;
        Infra.IDialogService _messageService;
        private IFrameNavigationService _navigationService;
        private string _firstName;
        private string _lastName;
        private string _userName;
        private string _password;
        private string _confirmPassword;
        private DateTime _birthdate;

        #endregion

        #region Propeties
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged();

            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged();

            }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();

            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged();

            }
        }
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                RaisePropertyChanged();

            }
        }
        public DateTime Birthdate
        {
            get { return _birthdate; }
            set
            {
                _birthdate = value;
                RaisePropertyChanged();

            }
        }


        #endregion

        #region Commands
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand GoToSignInView { get; set; }
        #endregion

        public RegisterViewModel(IChatService chatService, Infra.IDialogService messageService, IFrameNavigationService navigationService)
        {
            FirstName = "";
            LastName = "";
            UserName = "";
            Password = "";
            ConfirmPassword = "";
            _birthdate = DateTime.Now;
            _chatService = chatService;
            _messageService = messageService;
            _navigationService = navigationService;
            InitCommands();
        }

        #region Methods
        private void InitCommands()
        {
            RegisterCommand = new RelayCommand(async () =>
            {
                if (UserName == "" || Password == "" || FirstName == "" || LastName == "" || ConfirmPassword == "" || Birthdate == null)
                {
                    _messageService.ShowError("ALL Fields are required!!", "Invalid Input!");
                    return;
                }
                if (Password.Length < 4 || UserName.Length < 4)
                {
                    _messageService.ShowError("Username & Password Must Have Atleast 4 Characters", "Invalid Input!");
                    return;
                }
                if (FirstName.Length < 3 || LastName.Length < 3)
                {
                    _messageService.ShowError("FirstName & LastName Must Have Atleast 3 Characters", "Invalid Input!");
                    return;
                }
                if (Password != ConfirmPassword)
                {
                    _messageService.ShowError("Make Sure to CONFIRM the RIGHT Password!", "Invalid Input!");
                    return;
                }
                CommonUser newUser = new CommonUser()
                {
                    UserName = this.UserName,
                    Password = this.Password,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Birthdate = this.Birthdate
                };
                var errorMessage = await _chatService.Register(newUser);
                if (errorMessage != "")
                    _messageService.ShowError(errorMessage, "Register");
                else
                {
                    this.UserName = "";
                    this.Password = "";
                    this.FirstName = "";
                    this.LastName = "";
                    _navigationService.NavigateTo("LobbyWindow");
                }
            });

            GoToSignInView = new RelayCommand(() =>
            {
                this.UserName = "";
                this.Password = "";
                this.FirstName = "";
                this.LastName = "";
                _navigationService.NavigateTo("SignInWindow");
            });
        }
        #endregion
    }
}

