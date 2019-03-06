using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Backgammon
{
    public class Dices
    {
        Random _rand = new Random();
        public int Cube1 { get; private set; }
        public int Cube2 { get; private set; }
        public bool IsCube1Used { get; set; }
        public bool IsCube2Used { get; set; }
        public bool AreCubesDouble { get; set; }
        public bool IsRolled { get; set; }
        int counter = 0;

        public Dices(int cube1 = 0, int cube2 = 0)
        {
            Cube1 = cube1;
            Cube2 = cube2;
        }
        public void Roll()
        {
            IsRolled = true;
            Cube1 = _rand.Next(1, 7);
            Cube2 = _rand.Next(1, 7);
            IsCube1Used = false;
            IsCube2Used = false;
            if (Cube1 == Cube2)
                AreCubesDouble = true;
            else
                AreCubesDouble = false;
        }

        public void UseCube(int number)
        {
            if (AreCubesDouble)
            {
                counter++;
                if (counter > 2)
                {
                    if (number == Cube1 && !IsCube1Used)
                        IsCube1Used = true;
                    else if (number == Cube2)
                        IsCube2Used = true;
                }
            }
            else
            {
                if (number == Cube1)
                    IsCube1Used = true;
                else if (number == Cube2)
                    IsCube2Used = true;
            }

        }

        public void ResetDices()
        {
            IsCube1Used = false;
            IsCube2Used = false;
            AreCubesDouble = false;
            IsRolled = false;
            counter = 0;
        }

    }
}
