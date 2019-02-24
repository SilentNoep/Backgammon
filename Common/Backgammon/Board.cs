using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Backgammon
{
    public class Board
    {
        public Cell[] Cells { get; set; }
        public Cell EatenWhiteCell { get; set; }
        public Cell EatenRedCell { get; set; }
        public Cell WhitePile { get; set; }
        public Cell RedPile { get; set; }
        public Dices Dices { get; set; }

        public Board()
        {
            Cells = new Cell[24];
        }

        public void NewGame()
        {
            Cells[0] = (new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 2, ID = 0 });
            Cells[1] = new Cell() { ID = 1, ColorOfCell = Color.Empty };
            Cells[2] = new Cell() { ID = 2, ColorOfCell = Color.Empty };
            Cells[3] = new Cell() { ID = 3, ColorOfCell = Color.Empty };
            Cells[4] = new Cell() { ID = 4, ColorOfCell = Color.Empty };
            Cells[5] = (new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 5, ID = 5 });
            Cells[6] = new Cell() { ID = 6, ColorOfCell = Color.Empty };
            Cells[7] = (new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 3, ID = 7 });
            Cells[8] = new Cell() { ID = 8, ColorOfCell = Color.Empty };
            Cells[9] = new Cell() { ID = 9, ColorOfCell = Color.Empty };
            Cells[10] = new Cell() { ID = 10, ColorOfCell = Color.Empty };
            Cells[11] = (new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 5, ID = 11 });
            Cells[12] = (new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 5, ID = 12 });
            Cells[13] = new Cell() { ID = 13, ColorOfCell = Color.Empty };
            Cells[14] = new Cell() { ID = 14, ColorOfCell = Color.Empty };
            Cells[15] = new Cell() { ID = 15, ColorOfCell = Color.Empty };
            Cells[16] = (new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 3, ID = 16 });
            Cells[17] = new Cell() { ID = 17, ColorOfCell = Color.Empty };
            Cells[18] = (new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 5, ID = 18 });
            Cells[19] = new Cell() { ID = 19, ColorOfCell = Color.Empty };
            Cells[20] = new Cell() { ID = 20, ColorOfCell = Color.Empty };
            Cells[21] = new Cell() { ID = 21, ColorOfCell = Color.Empty };
            Cells[22] = new Cell() { ID = 22, ColorOfCell = Color.Empty };
            Cells[23] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 23 };

            EatenWhiteCell = new Cell() { ID = -1, ColorOfCell = Color.White };
            EatenRedCell = new Cell() { ID = 24, ColorOfCell = Color.Red };
            Dices = new Dices();
            WhitePile = new Cell() { ID = 50, ColorOfCell = Color.White };
            RedPile = new Cell() { ID = -50, ColorOfCell = Color.Red};


            //Cells[0] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 0 };
            //Cells[1] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 1 };
            //Cells[2] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 2 };
            //Cells[3] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 3 };
            //Cells[4] = new Cell() { ColorOfCell = Color.Red, NumOfSoldiers = 2, ID = 4 };
            //Cells[5] = new Cell() { ColorOfCell = Color.Empty, ID = 5 };
            //Cells[6] = new Cell() { ColorOfCell = Color.Empty, ID = 6 };
            //Cells[7] = new Cell() { ColorOfCell = Color.Empty, ID = 7 };
            //Cells[8] = new Cell() { ColorOfCell = Color.Empty, ID = 8 };
            //Cells[9] = new Cell() { ColorOfCell = Color.Empty, ID = 9 };
            //Cells[10] = new Cell() { ColorOfCell = Color.Empty, ID = 10 };
            //Cells[11] = new Cell() { ColorOfCell = Color.Empty, ID = 11 };
            //Cells[12] = new Cell() { ColorOfCell = Color.Empty, ID = 12 };
            //Cells[13] = new Cell() { ColorOfCell = Color.Empty, ID = 13 };
            //Cells[14] = new Cell() { ColorOfCell = Color.Empty, ID = 14 };
            //Cells[15] = new Cell() { ColorOfCell = Color.Empty, ID = 15 };
            //Cells[16] = new Cell() { ColorOfCell = Color.Empty, ID = 16 };
            //Cells[17] = new Cell() { ColorOfCell = Color.Empty, ID = 17 };
            //Cells[18] = new Cell() { ColorOfCell = Color.Empty, ID = 18 };
            //Cells[19] = new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 4, ID = 19 };
            //Cells[20] = new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 4, ID = 20 };
            //Cells[21] = new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 4, ID = 21 };
            //Cells[22] = new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 4, ID = 22 };
            //Cells[23] = new Cell() { ColorOfCell = Color.White, NumOfSoldiers = 4, ID = 23 };


        }
    }
}
