using Common;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Views;

namespace WPFClient.Services
{
    class NavigationService : INavigationService
    {
        public string CurrentPageKey => throw new NotImplementedException();
        //GameWindow GameWindow;
        public void GoBack()
        {
            //GameWindow.Close();
            //GameWindow = null;
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            //GameWindow =
            new GameWindow(parameter as User).ShowDialog();



        }

    }
}
