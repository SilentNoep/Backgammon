using Common;
using SignalRChat.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SignalRChat.DAL
{
    public class Repository
    {


        public IEnumerable<User> GetAllUsers()
        {
            using (var ctx = new BackgammonContext())
            {
                return ctx.Users.ToList();
            }
        }

        public void AddUser(User user)
        {
            if (user == null) throw new NullReferenceException();

            user.Password = SHA256Hash(user.Password);
            using (var ctx = new BackgammonContext())
            {
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
        }

        public void UpdateDetails(User user)
        {
            if (user == null) throw new NullReferenceException();

            using (var ctx = new BackgammonContext())
            {
                var originalEntity = ctx.Users.Find(user.Id);
                ctx.Entry(originalEntity).CurrentValues.SetValues(user);
                ctx.SaveChanges();
            }
        }


        public User GetUser(string userName)
        {
            using (var ctx = new BackgammonContext())
            {
                return ctx.Users.Where(p => p.UserName == userName).FirstOrDefault();
            }
        }

        public bool IsValid(string userName, string passWord)
        {
            bool IsValid = false;
            using (var ctx = new BackgammonContext())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    if (SHA256Hash(passWord) == user.Password)
                    {
                        //if (user.Status == Status.Online)
                        //{
                        IsValid = true;
                        user.Status = Status.Online;
                        ctx.SaveChanges();
                        //}
                    }
                }
            }
            return IsValid;
        }



        public void ChangeUserStatus(User user, Status status)
        {
            using (var ctx = new BackgammonContext())
            {
                var userStatus = ctx.Users.FirstOrDefault(u => u.UserName == user.UserName);
                userStatus.Status = status;
                ctx.SaveChanges();
            }
        }

        public void AddLossToUser(User user)
        {
            using (var ctx = new BackgammonContext())
            {
                var User = ctx.Users.FirstOrDefault(u => u.UserName == user.UserName);
                User.Losses++;
                ctx.SaveChanges();
            }
        }

        public void HasInvited(User user, bool DidInvite)
        {
            using (var ctx = new BackgammonContext())
            {
                var User = ctx.Users.FirstOrDefault(u => u.UserName == user.UserName);
                User.HasInvitedGame = DidInvite;
                ctx.SaveChanges();
            }
        }

        public bool DidUserInvite(User user)
        {
            using (var ctx = new BackgammonContext())
            {
                var User = ctx.Users.FirstOrDefault(u => u.UserName == user.UserName);
                return User.HasInvitedGame;
            }
        }








        private static string SHA256Hash(string Data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
    }
}