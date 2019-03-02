using Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WPFClient.Infra;

namespace WPFClient.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private IChatService _chatService;
        private Infra.IDialogService _messageService;
        private IFrameNavigationService _navigationService;
        #endregion

        #region Properties / Commands
        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.NavigateTo("SignInWindow");
                    }));
            }
        }
        public RelayCommand<CancelEventArgs> WindowClosingCommand { get; set; }
        #endregion

        public MainViewModel(IChatService chatService, Infra.IDialogService messageService, IFrameNavigationService navigationService)
        {
            _chatService = chatService;
            _messageService = messageService;
            _navigationService = navigationService;
            WindowClosingCommand = new RelayCommand<CancelEventArgs>((args) =>
            {
                if (!_messageService.ShowQuestion("Are You Sure You Want To Exit?", "Bye!"))
                    args.Cancel = true;
                else
                {
                    //if (_chatService.User != null)
                    //{
                    _chatService.SignOut();
                    //    _messageService.ShowMessage("Signed Out SuccessFully", "Bye!");
                    //}
                }
            });
        }
    }
}