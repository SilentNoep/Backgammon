using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WPFClient.Infra;
using WPFClient.Messages;

namespace WPFClient.ViewModel
{
    public class RegisterViewModel : ViewModelBase, IPageViewModel
    {
        #region Fields
        IChatService _chatService;
        IServerService _serverService;
        IDialogService _messageService;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService;
        private string _firstName;
        private string _lastName;
        private string _userName;
        private string _password;
        private string _confirmPassword;
        private DateTime _birthdate;

        #endregion

        #region Propeties
        public string Name { get; set; } = "Register";
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

        public User User { get; set; }
        #endregion

        #region Commands
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand GoToSignInView { get; set; }
        #endregion

        public RegisterViewModel(IChatService chatService, IDialogService messageService, IServerService serverService, GalaSoft.MvvmLight.Views.INavigationService navigationService)
        {
            FirstName = "";
            LastName = "";
            UserName = "";
            Password = "";
            ConfirmPassword = "";
            _birthdate = DateTime.Now;
            _serverService = serverService;
            _chatService = chatService;
            _messageService = messageService;
            _navigationService = navigationService;
            InitCommands();
        }

        #region Methods
        private void InitCommands()
        {
            RegisterCommand = new RelayCommand(() =>
            {
                if (UserName == "" || Password == "" || FirstName == "" || LastName == "" || ConfirmPassword == "" || Birthdate == null)
                {
                    _messageService.ShowError("ALL Fields are required!!","Invalid Input!");
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
                User = new User()
                {
                    UserName = this.UserName,
                    Password = this.Password,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Birthdate = this.Birthdate
                };

                if (_serverService.ConnectToServerRegister("register", User))
                {
                    _chatService.GetUser(User);
                    this.UserName = "";
                    this.Password = "";
                    this.FirstName = "";
                    this.LastName = "";
                    _messageService.ShowMessage("You Successfully Registered!!", "GREAT!!");
                    Messenger.Default.Send(new PageService()
                    {
                        currentPage = new LobbyViewModel(_chatService, _messageService, _serverService, _navigationService)
                    });
                }
                else
                    _messageService.ShowError("Can't Connect To Server.", "Please Try Again");

            });

            GoToSignInView = new RelayCommand(() =>
            {
                Messenger.Default.Send(new PageService()
                {
                    currentPage = new SignInViewModel(_chatService, _messageService, _serverService, _navigationService)
                });
            });
        }
        #endregion
    }
}

