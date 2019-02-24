using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Backgammon
{
    public enum Color { Red, White, Empty }
    public class Cell
    {
        public int ID { get; set; }
        public int NumOfSoldiers { get; set; }
        public Color ColorOfCell { get; set; }
        public bool IsPicked { get; set; }
    }
}