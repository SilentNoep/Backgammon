using Common;
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
        public User AddUser(string firstName, string lastName, string userName, string password, DateTime? birthdate)
        {
            User newUser;
            using (var ctx = new BackgammonContext())
            {
                var userCheck =(User) ctx.Users.FirstOrDefault(p => p.UserName == userName);
                if (userCheck != null)
                    return null;
                newUser = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = userName,
                    Password = SHA256Hash(password),
                    Wins = 0,
                    Losses = 0,
                    Birthdate = birthdate,
                    DateUserRegistered = DateTime.Now,
                    LastOnline = DateTime.Now,
                    Status = Status.Online
                };
                ctx.Users.Add(newUser);
                ctx.SaveChanges();
            }
            return newUser;
        }
        public IEnumerable<User> GetAllUsers()
        {
            using (var ctx = new BackgammonContext())
            {
                return ctx.Users.ToList();
            }
        }
        public User GetUser(string userName)
        {
            using (var ctx = new BackgammonContext())
            {
                return ctx.Users.Where(p => p.UserName == userName).FirstOrDefault();
            }
        }

        public bool IsValid(string userName,string passWord)
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