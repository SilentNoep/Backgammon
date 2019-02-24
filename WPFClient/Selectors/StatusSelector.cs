using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFClient.Selectors
{
    public class StatusSelector : DataTemplateSelector
    {
        public DataTemplate UserOnlineTemplate { get; set; }
        public DataTemplate UserInGameTemplate { get; set; }
        public DataTemplate UserOfflineTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var student = item as User;
            if (student.Status == Status.Online)
                return UserOnlineTemplate;
            else if (student.Status == Status.InGame)
                return UserInGameTemplate;
            else
                return UserOfflineTemplate;
        }
    }
}
