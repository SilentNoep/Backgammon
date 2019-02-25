using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Infra
{
    public interface IServerService
    {

        IEnumerable<User> GetAllUsers(string Uri);

        string ConnectToServerSignIn(string Uri,User user);
        bool ConnectToServerRegister(string Uri, User user);
        void DisconnectFromServer(string Uri, User user);
        bool ConnectToServerInGame(string Uri, User user);
        bool AddLossToUser(string Uri, User user);
    }
}
