using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum Status { Online, InGame, Offline }
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Status Status { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateUserRegistered { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastOnline { get; set; }
        public bool HasInvitedGame { get; set; }



    }
}
