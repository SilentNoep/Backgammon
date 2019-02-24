using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Infra;

namespace WPFClient.Messages
{
    public class PageService
    {
        public IPageViewModel currentPage { get; set; }
        public User currentUser { get; set; }
    }
}
