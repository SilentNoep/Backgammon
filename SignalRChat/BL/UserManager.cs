using Common;
using SignalRChat.DAL;
using SignalRChat.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalRChat.BL
{
    public class UserManager
    {
        private readonly Repository _rep = new Repository();

        public IEnumerable<UserDetails> GetAllUsers()
        {
            List<UserDetails> allUsers = new List<UserDetails>();

            foreach (User user in _rep.GetAllUsers())
            {
                var userStatus = new UserDetails()
                {
                    UserName = user.UserName,
                    Wins = user.Wins,
                    Losses = user.Losses,
                    Status = user.Status
                };

                allUsers.Add(userStatus);
            }
            return allUsers;
        }

        public string Register(CommonUser user)
        {
            if (user == null) return "User is null";
            if (string.IsNullOrWhiteSpace(user.UserName))
                return "Can't choose an empty name";
            if (user.UserName.Length > 10)
                return "Can't choose a name with more than 10 letters";
            if (user.UserName.Contains(" "))
                return "Can't choose a name with spaces";
            if (_rep.GetAllUsers().Any((u) => u.UserName == user.UserName))
                return "User already exists";
            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Count() < 4)
                return "Password length must be 4 or more";
            _rep.AddUser(new User()
            {
                UserName = user.UserName,
                Password = user.Password,
                Status = Status.Online,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Wins = 0,
                Losses = 0,
                Birthdate = user.Birthdate,
                DateUserRegistered = DateTime.Now,
                LastOnline = DateTime.Now
            });
            return "";
        }

        public string LogIn(CommonUser user)
        {
            if (user == null) return "User is null";
            if (!_rep.GetAllUsers().Any((u) => u.UserName == user.UserName))
                return "User doesn't exists";
            if (string.IsNullOrWhiteSpace(user.Password) || !_rep.IsValid(user.UserName, user.Password))
                return "Incorrect password";
            var originalUser = _rep.GetUser(user.UserName);
            //if (originalUser.Status == Status.Online || originalUser.Status == Status.InGame)
            //    return "Username already in use";

            _rep.ChangeUserStatus(originalUser, Status.Online);
            return "";
        }

        public string LogOff(string username)
        {
            if (username == null || username == "") return "User is null";
            if (!_rep.GetAllUsers().Any((u) => u.UserName == username))
                return "User doesn't exists";


            var originalUser = _rep.GetUser(username);
            _rep.ChangeUserStatus(originalUser, Status.Offline);
            return "";
        }

        public string EnteredGame(string username)
        {
            if (username == null || username == "") return "User is null";
            if (!_rep.GetAllUsers().Any((u) => u.UserName == username))
                return "User doesn't exists";


            var originalUser = _rep.GetUser(username);
            _rep.ChangeUserStatus(originalUser, Status.InGame);
            return "";
        }

        public void UserInvited(string username, bool DidInvite)
        {
            if (username == null || username == "") return;
            if (!_rep.GetAllUsers().Any((u) => u.UserName == username))
                return;


            var originalUser = _rep.GetUser(username);
            _rep.HasInvited(originalUser, DidInvite);
        }

        public bool DidUserInvite(string username)
        {
            var originalUser = _rep.GetUser(username);
            return _rep.DidUserInvite(originalUser);
        }

    }




}