using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Backgammon
{
    public class Player
    {
        public string UserName { get; set; }
        public Color Color { get; set; }
        public int NumOfSoldiersOnBoard { get; set; }
        public bool IsBaseOnLeft { get; set; }
        public bool IsMyTurn { get; set; }
        public bool HasRolled { get; set; }
        public bool IsInFinalStage { get; set; }

    }
}
