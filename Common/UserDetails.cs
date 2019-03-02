using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum Status { Online, InGame, Offline }

    public class UserDetails
    {
        public string UserName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public Status Status { get; set; }
        public bool HasInvitedGame { get; set; }
    }
}
