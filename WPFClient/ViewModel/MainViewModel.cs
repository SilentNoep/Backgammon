using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WPFClient.Infra;
using WPFClient.Messages;

namespace WPFClient.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private IPageViewModel _currentPageViewModel;
        private IServerService _serverService;
        private IChatService _chatService;
        private IDialogService _messageService;
        private SignInViewModel SignInVM;
        #endregion

        #region Properties / Commands

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<User> AllUsers { get; set; }
        public User MyUser { get; set; }
        public RelayCommand<CancelEventArgs> WindowClosingCommand { get; set; }
        #endregion

        public MainViewModel(IChatService chatService, IDialogService messageService, IServerService serverService, GalaSoft.MvvmLight.Views.INavigationService navigationService)
        {
            _serverService = serverService;
            _chatService = chatService;
            _messageService = messageService;
            SignInVM = new SignInViewModel(chatService, messageService, serverService, navigationService);
            Messenger.Default.Register(this, new Action<PageService>(ChangeViewModel));
            WindowClosingCommand = new RelayCommand<CancelEventArgs>((args) =>
            {
                if (!_messageService.ShowQuestion("Are You Sure You Want To Exit?", "Bye!"))
                    args.Cancel = true;
                else
                {
                    if (_chatService.User != null)
                    {
                        _serverService.DisconnectFromServer("", _chatService.User);
                        _chatService.SignOut(_chatService.User);
                        _chatService.DisconnectFromServer();
                        _messageService.ShowMessage("Signed Out SuccessFully", "Bye!");
                    }
                }
              


            });
            // Set starting page
            CurrentPageViewModel = SignInVM;
        }

        #region Methods

        private void ChangeViewModel(PageService page)
        {
            CurrentPageViewModel = page.currentPage;
        }
        #endregion
    }
}