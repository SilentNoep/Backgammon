using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Backgammon
{
    public class BackgammonManager
    {
        #region Properties
        public Board bgBoard { get; set; }
        public Player FirstPlayer { get; set; }
        public Player SecondPlayer { get; set; }
        public Player CurrentTurnPlayer { get; set; }
        public int FirstIndex { get; set; }
        public int Moves { get; set; }
        public int Moved { get; set; }
        public bool IsTurnCanceled { get; set; }
        public bool DidPlayerMove { get; set; }
        #endregion

        public BackgammonManager()
        {
            Moves = 2;
            FirstIndex = -10;
            FirstPlayer = new Player();
            SecondPlayer = new Player();
            CurrentTurnPlayer = new Player();
            bgBoard = new Board();
            InitBoard();
        }

        #region Methods

        private void CleanSelectedSpike()
        {
            foreach (var item in bgBoard.Cells)
            {
                item.IsPicked = false;
            }
            bgBoard.EatenWhiteCell.IsPicked = false;
            bgBoard.EatenRedCell.IsPicked = false;
        }

        public Board RollDices()
        {
            bgBoard.Dices.ResetDices();
            IsTurnCanceled = false;
            DidPlayerMove = false;
            CountSoldiers();
            CleanSelectedSpike();
            bgBoard.Dices.Roll();
            if (bgBoard.Dices.AreCubesDouble)
                Moves = 4;
            else
                Moves = 2;


            if (IsThereEatenSoldiers())
            {
                if (!CanEatenSolderCanMoveAtAll())
                    SwitchTurn(CurrentTurnPlayer);
            }
            else
            {
                if(!CheckIfAllSoldierAreInBase())
                {
                    if (!CanAnySoldierMoveAtAll())
                        SwitchTurn(CurrentTurnPlayer);
                }
             
            }


            return bgBoard;
        }

        public void InitBoard()
        {
            bgBoard.NewGame();
        }

        public Player InitPlayer(User user)
        {
            if (user.HasInvitedGame)
            {
                FirstPlayer.User = user;
                FirstPlayer.Color = Color.White;
                FirstPlayer.IsMyTurn = true;
                FirstPlayer.IsBaseOnLeft = false;
                CurrentTurnPlayer = FirstPlayer;
                return FirstPlayer;

            }
            else
            {
                SecondPlayer.User = user;
                SecondPlayer.Color = Color.Red;
                SecondPlayer.IsMyTurn = false;
                SecondPlayer.IsBaseOnLeft = true;
                return SecondPlayer;
            }
        }

        private void SwitchTurn(Player playerToSwitchFrom)
        {
            if (playerToSwitchFrom.User.UserName == FirstPlayer.User.UserName)
                CurrentTurnPlayer = SecondPlayer;
            else
                CurrentTurnPlayer = FirstPlayer;

            Moved = 0;
            Moves = 2;

        }

        public bool IsMyTurn(string username)
        {
            if (CurrentTurnPlayer.User.UserName == username)
                return true;
            else
                return false;
        }

        public Board GetOrRemovePick(int chosenChipInSpikeIndex, Player player)
        {

            CleanSelectedSpike();
            if (CurrentTurnPlayer.User.UserName == player.User.UserName)
            {
                if (player.Color == bgBoard.EatenRedCell.ColorOfCell && bgBoard.EatenRedCell.NumOfSoldiers > 0 && chosenChipInSpikeIndex != bgBoard.EatenRedCell.ID)
                {

                }
                else if (player.Color == bgBoard.EatenWhiteCell.ColorOfCell && bgBoard.EatenWhiteCell.NumOfSoldiers > 0 && chosenChipInSpikeIndex != bgBoard.EatenWhiteCell.ID)
                {

                }
                else
                {
                    if (FirstIndex != chosenChipInSpikeIndex)
                    {
                        if (bgBoard.EatenRedCell.ID == chosenChipInSpikeIndex)
                        {
                            if (player.Color == bgBoard.EatenRedCell.ColorOfCell && bgBoard.EatenRedCell.NumOfSoldiers > 0)
                            {
                                FirstIndex = chosenChipInSpikeIndex;
                                bgBoard.EatenRedCell.IsPicked = true;
                            }
                        }
                        else if (bgBoard.EatenWhiteCell.ID == chosenChipInSpikeIndex && bgBoard.EatenWhiteCell.NumOfSoldiers > 0)
                        {
                            if (player.Color == bgBoard.EatenWhiteCell.ColorOfCell)
                            {
                                FirstIndex = chosenChipInSpikeIndex;
                                bgBoard.EatenWhiteCell.IsPicked = true;
                            }
                        }
                        else if (bgBoard.WhitePile.ID == chosenChipInSpikeIndex)
                        {

                        }
                        else if (bgBoard.RedPile.ID == chosenChipInSpikeIndex)
                        {

                        }
                        else if (player.Color == bgBoard.Cells[chosenChipInSpikeIndex].ColorOfCell)
                        {
                            FirstIndex = chosenChipInSpikeIndex;
                            bgBoard.Cells[chosenChipInSpikeIndex].IsPicked = true;
                        }
                    }
                    else
                    {
                        if (bgBoard.EatenRedCell.ID == chosenChipInSpikeIndex && bgBoard.EatenRedCell.NumOfSoldiers > 0)
                        {
                            if (player.Color == bgBoard.EatenRedCell.ColorOfCell)
                            {
                                FirstIndex = -10;
                                bgBoard.EatenRedCell.IsPicked = false;
                            }
                        }
                        else if (bgBoard.EatenWhiteCell.ID == chosenChipInSpikeIndex && bgBoard.EatenWhiteCell.NumOfSoldiers > 0)
                        {
                            if (player.Color == bgBoard.EatenWhiteCell.ColorOfCell)
                            {
                                FirstIndex = -10;
                                bgBoard.EatenWhiteCell.IsPicked = false;
                            }
                        }
                        else if (player.Color == bgBoard.Cells[chosenChipInSpikeIndex].ColorOfCell)
                        {
                            FirstIndex = -10;
                            bgBoard.Cells[chosenChipInSpikeIndex].IsPicked = false;
                        }
                    }
                }
            }
            return bgBoard;
        }

        public Board MoveChip(int chosenSpikeToPutChipIndex, Player player)
        {
            bool isThereEnemySoldierInCell = false;
            if (chosenSpikeToPutChipIndex != bgBoard.EatenRedCell.ID && chosenSpikeToPutChipIndex != bgBoard.EatenWhiteCell.ID &&
                chosenSpikeToPutChipIndex != bgBoard.RedPile.ID && chosenSpikeToPutChipIndex != bgBoard.WhitePile.ID)
                isThereEnemySoldierInCell = (bgBoard.Cells[chosenSpikeToPutChipIndex].NumOfSoldiers == 1 && bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell != Color.Empty
                       && bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell != player.Color);

            if (CurrentTurnPlayer.Color == player.Color)
            {
                if (chosenSpikeToPutChipIndex != bgBoard.RedPile.ID && chosenSpikeToPutChipIndex != bgBoard.WhitePile.ID)
                {
                    if (player.Color == bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell || bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell == Color.Empty || isThereEnemySoldierInCell)
                    {
                        if (!player.IsBaseOnLeft)
                        {
                            if (FirstIndex < chosenSpikeToPutChipIndex)
                            {
                                int numberOfCubePicked = chosenSpikeToPutChipIndex - FirstIndex;
                                if (!isThereEnemySoldierInCell)
                                {
                                    if (CanMoveToChosenSpike(numberOfCubePicked))
                                    {
                                        MoveToSpike(chosenSpikeToPutChipIndex, player, numberOfCubePicked);
                                        DidPlayerMove = true;
                                    }
                                        
                                    else
                                    {
                                        bgBoard.Cells[FirstIndex].IsPicked = false;
                                        FirstIndex = -10;
                                        DidPlayerMove = false;
                                    }
                                }
                                else
                                {
                                    if (CanMoveToChosenSpike(numberOfCubePicked))
                                    {
                                        EatSoldier(chosenSpikeToPutChipIndex, player);
                                        MoveToSpike(chosenSpikeToPutChipIndex, player, numberOfCubePicked);
                                        DidPlayerMove = true;
                                    }
                                    else
                                    {
                                        bgBoard.Cells[FirstIndex].IsPicked = false;
                                        FirstIndex = -10;
                                        DidPlayerMove = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (FirstIndex > chosenSpikeToPutChipIndex)
                            {
                                int numberOfCube = FirstIndex - chosenSpikeToPutChipIndex;
                                if (!isThereEnemySoldierInCell)
                                {
                                    if (CanMoveToChosenSpike(numberOfCube))
                                    {
                                        MoveToSpike(chosenSpikeToPutChipIndex, player, numberOfCube);
                                        DidPlayerMove = true;
                                    }
                                    else
                                    {
                                        bgBoard.Cells[FirstIndex].IsPicked = false;
                                        FirstIndex = -10;
                                        DidPlayerMove = false;
                                    }
                                }
                                else
                                {
                                    if (CanMoveToChosenSpike(numberOfCube))
                                    {
                                        EatSoldier(chosenSpikeToPutChipIndex, player);
                                        MoveToSpike(chosenSpikeToPutChipIndex, player, numberOfCube);
                                        DidPlayerMove = true;
                                    }
                                    else
                                    {
                                        bgBoard.Cells[FirstIndex].IsPicked = false;
                                        FirstIndex = -10;
                                        DidPlayerMove = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (CheckIfAllSoldierAreInBase())
                    {
                        int number = 0;

                        if (CurrentTurnPlayer.Color == Color.White)
                        {
                            switch (FirstIndex)
                            {
                                case 18:
                                    number = 12;
                                    break;
                                case 19:
                                    number = 14;
                                    break;
                                case 20:
                                    number = 16;
                                    break;
                                case 21:
                                    number = 18;
                                    break;
                                case 22:
                                    number = 20;
                                    break;
                                case 23:
                                    number = 22;
                                    break;
                            }
                        }
                        else if (CurrentTurnPlayer.Color == Color.Red)
                            number = -1;

                        if (CanMoveToPile(FirstIndex - number))
                        {
                            MoveToPile(FirstIndex - number);
                            DidPlayerMove = true;
                        }
                        else
                        {
                            bgBoard.Cells[FirstIndex].IsPicked = false;
                            FirstIndex = -10;
                            DidPlayerMove = false;
                        }
                    }
                }

            }
            return bgBoard;
        }

        private void MoveToPile(int dice)
        {
            CleanSelectedSpike();
            bgBoard.Cells[FirstIndex].NumOfSoldiers--;
            if (bgBoard.Cells[FirstIndex].NumOfSoldiers == 0)
                bgBoard.Cells[FirstIndex].ColorOfCell = Color.Empty;

            if (CurrentTurnPlayer.Color == Color.Red)
                bgBoard.RedPile.NumOfSoldiers++;
            else
                bgBoard.WhitePile.NumOfSoldiers++;

            CurrentTurnPlayer.NumOfSoldiersOnBoard--;


            Moved++;
            bgBoard.Dices.UseCube(dice);
            FirstIndex = -10;
            CountSoldiers();
            if (CheckifWonGame())
            {

            }
            else
            {
                if (Moved == Moves)
                {
                    SwitchTurn(CurrentTurnPlayer);
                    return;
                }
            }



        }

        private bool CanMoveToChosenSpike(int numberOfCubePicked)
        {
            if (!bgBoard.Dices.IsCube1Used && numberOfCubePicked == bgBoard.Dices.Cube1)
                return true;
            else if (!bgBoard.Dices.IsCube2Used && numberOfCubePicked == bgBoard.Dices.Cube2)
                return true;
            else
                return false;
        }

        private bool CanMoveToPile(int numberOfCubePicked)
        {
            if (!bgBoard.Dices.IsCube1Used && numberOfCubePicked == bgBoard.Dices.Cube1)
                return true;
            else if (!bgBoard.Dices.IsCube2Used && numberOfCubePicked == bgBoard.Dices.Cube2)
                return true;


            else if (numberOfCubePicked != bgBoard.Dices.Cube1 && numberOfCubePicked != bgBoard.Dices.Cube2 ||
                bgBoard.Dices.IsCube1Used && numberOfCubePicked != bgBoard.Dices.Cube2 ||
                bgBoard.Dices.IsCube2Used && numberOfCubePicked != bgBoard.Dices.Cube1)
            {
                if (CurrentTurnPlayer.Color == Color.White)
                {
                    if (bgBoard.Dices.Cube1 > numberOfCubePicked)
                    {
                        for (int i = 18; i != FirstIndex; i++)
                        {
                            if (bgBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                    else if (bgBoard.Dices.Cube2 > numberOfCubePicked)
                    {
                        for (int i = 18; i != FirstIndex; i++)
                        {
                            if (bgBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                }
                else if (CurrentTurnPlayer.Color == Color.Red)
                {
                    if (bgBoard.Dices.Cube1 > numberOfCubePicked)
                    {
                        for (int i = 5; i != FirstIndex; i--)
                        {
                            if (bgBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                    else if (bgBoard.Dices.Cube2 > numberOfCubePicked)
                    {
                        for (int i = 5; i != FirstIndex; i--)
                        {
                            if (bgBoard.Cells[i].NumOfSoldiers > 0)
                                return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private void MoveToSpike(int chosenSpikeToPutChipIndex, Player player, int numberOfCubePicked)
        {
            CleanSelectedSpike();
            if (FirstIndex == bgBoard.EatenRedCell.ID)
                bgBoard.EatenRedCell.NumOfSoldiers--;
            else if (FirstIndex == bgBoard.EatenWhiteCell.ID)
                bgBoard.EatenWhiteCell.NumOfSoldiers--;
            else
            {
                bgBoard.Cells[FirstIndex].NumOfSoldiers--;
                if (bgBoard.Cells[FirstIndex].NumOfSoldiers == 0)
                    bgBoard.Cells[FirstIndex].ColorOfCell = Color.Empty;
            }

            bgBoard.Cells[chosenSpikeToPutChipIndex].NumOfSoldiers++;
            bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell = player.Color;
            FirstIndex = -10;
            Moved++;
            bgBoard.Dices.UseCube(numberOfCubePicked);
            if (Moved == Moves)
            {
                SwitchTurn(player);
                return;
            }

            if (IsThereEatenSoldiers())
            {
                if (!CanEatenSolderCanMoveAtAll())
                {
                    SwitchTurn(CurrentTurnPlayer);
                    IsTurnCanceled = true;
                }

            }
            else
            {
                if (!CanAnySoldierMoveAtAll())
                {
                    SwitchTurn(CurrentTurnPlayer);
                    IsTurnCanceled = true;
                }

            }
        }

        private void EatSoldier(int chosenSpikeToPutChipIndex, Player player)
        {
            bgBoard.Cells[chosenSpikeToPutChipIndex].NumOfSoldiers = 0;
            bgBoard.Cells[chosenSpikeToPutChipIndex].ColorOfCell = Color.Empty;
            if (player.Color == Color.Red)
                bgBoard.EatenWhiteCell.NumOfSoldiers++;
            else if (player.Color == Color.White)
                bgBoard.EatenRedCell.NumOfSoldiers++;
        }

        private bool CanEatenSolderCanMoveAtAll()
        {
            if (CurrentTurnPlayer.Color == Color.Red)
            {
                if (bgBoard.EatenRedCell.NumOfSoldiers > 0)
                {
                    if ((bgBoard.Cells[23].ColorOfCell == Color.White && bgBoard.Cells[23].NumOfSoldiers > 1) &&
                  (bgBoard.Cells[22].ColorOfCell == Color.White && bgBoard.Cells[22].NumOfSoldiers > 1) &&
                  (bgBoard.Cells[21].ColorOfCell == Color.White && bgBoard.Cells[21].NumOfSoldiers > 1) &&
                  (bgBoard.Cells[20].ColorOfCell == Color.White && bgBoard.Cells[20].NumOfSoldiers > 1) &&
                  (bgBoard.Cells[19].ColorOfCell == Color.White && bgBoard.Cells[19].NumOfSoldiers > 1) &&
                  (bgBoard.Cells[18].ColorOfCell == Color.White && bgBoard.Cells[18].NumOfSoldiers > 1))
                        return false;
                    else if (!bgBoard.Dices.IsCube1Used && bgBoard.Dices.IsCube2Used)
                    {
                        int SpikePossible = bgBoard.EatenRedCell.ID - bgBoard.Dices.Cube1;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1)
                            return false;
                    }
                    else if (!bgBoard.Dices.IsCube2Used && bgBoard.Dices.IsCube1Used)
                    {
                        int SpikePossible = bgBoard.EatenRedCell.ID - bgBoard.Dices.Cube2;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1)
                            return false;
                    }
                    else if (!bgBoard.Dices.IsCube2Used && !bgBoard.Dices.IsCube1Used)
                    {
                        int SpikePossible = bgBoard.EatenRedCell.ID - bgBoard.Dices.Cube1;
                        int SpikePossible2 = bgBoard.EatenRedCell.ID - bgBoard.Dices.Cube2;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1 &&
                            bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible2].NumOfSoldiers > 1)
                            return false;
                    }
                }
            }
            else if (CurrentTurnPlayer.Color == Color.White)
            {
                if (bgBoard.EatenWhiteCell.NumOfSoldiers > 0)
                {
                    if ((bgBoard.Cells[0].ColorOfCell == Color.Red && bgBoard.Cells[0].NumOfSoldiers > 1) &&
                   (bgBoard.Cells[1].ColorOfCell == Color.Red && bgBoard.Cells[1].NumOfSoldiers > 1) &&
                   (bgBoard.Cells[2].ColorOfCell == Color.Red && bgBoard.Cells[2].NumOfSoldiers > 1) &&
                   (bgBoard.Cells[3].ColorOfCell == Color.Red && bgBoard.Cells[3].NumOfSoldiers > 1) &&
                   (bgBoard.Cells[4].ColorOfCell == Color.Red && bgBoard.Cells[4].NumOfSoldiers > 1) &&
                   (bgBoard.Cells[5].ColorOfCell == Color.Red && bgBoard.Cells[5].NumOfSoldiers > 1))
                        return false;
                    else if (!bgBoard.Dices.IsCube1Used && bgBoard.Dices.IsCube2Used)
                    {
                        int SpikePossible = bgBoard.Dices.Cube1 + bgBoard.EatenWhiteCell.ID;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1)
                            return false;
                    }
                    else if (!bgBoard.Dices.IsCube2Used && bgBoard.Dices.IsCube1Used)
                    {
                        int SpikePossible = bgBoard.Dices.Cube2 + bgBoard.EatenWhiteCell.ID;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1)
                            return false;
                    }
                    else if (!bgBoard.Dices.IsCube2Used && !bgBoard.Dices.IsCube1Used)
                    {
                        int SpikePossible = bgBoard.Dices.Cube1 + bgBoard.EatenWhiteCell.ID;
                        int SpikePossible2 = bgBoard.Dices.Cube2 + bgBoard.EatenWhiteCell.ID;
                        if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers > 1 &&
                            bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible2].NumOfSoldiers > 1)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool CanAnySoldierMoveAtAll()
        {
            foreach (Cell cell in bgBoard.Cells)
            {
                if (CurrentTurnPlayer.Color == Color.Red)
                {
                    if (cell.ColorOfCell == Color.Red)
                    {

                        int SpikePossible = cell.ID - bgBoard.Dices.Cube1;
                        int SpikePossible2 = cell.ID - bgBoard.Dices.Cube2;
                        if (!bgBoard.Dices.IsCube1Used && bgBoard.Dices.IsCube2Used)
                        {
                            if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0)
                                continue;
                            else if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||
                                (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                return true;
                        }
                        else if (!bgBoard.Dices.IsCube2Used && bgBoard.Dices.IsCube1Used)
                        {
                            if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0)
                                continue;
                            else if (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Empty ||
                                (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible2].NumOfSoldiers == 1))
                                return true;
                        }
                        else if (!bgBoard.Dices.IsCube2Used && !bgBoard.Dices.IsCube1Used)
                        {
                            if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0)  // if 1 cant
                            {
                                if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0) // if 2 cant
                                    continue;
                                else if (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Empty ||  // if 2 can and 1 cant
                           (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible2].NumOfSoldiers == 1))
                                    return true;
                                continue;   // if both cant
                            }
                            else   // if 1 can
                            {
                                if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||
                             (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                    return true;
                            }
                            if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0)  // if 2 cant
                            {
                                if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0) // if 1 cant
                                    continue;
                                else if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||         // if 1 can and 2 cant
                             (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                    return true;
                                continue;   // if both cant
                            }
                            else // if 2 can
                            {
                                if (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red || bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Empty ||  // if 2 can
                           (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White && bgBoard.Cells[SpikePossible2].NumOfSoldiers == 1))
                                    return true;
                            }
                        }
                    }
                }
                else if (CurrentTurnPlayer.Color == Color.White)
                {
                    if (cell.ColorOfCell == Color.White)
                    {
                        int SpikePossible = bgBoard.Dices.Cube1 + cell.ID;
                        int SpikePossible2 = bgBoard.Dices.Cube2 + cell.ID;


                        if (!bgBoard.Dices.IsCube1Used && bgBoard.Dices.IsCube2Used)
                        {
                            if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0)
                                continue;

                            else if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||
                                (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                return true;
                        }
                        else if (!bgBoard.Dices.IsCube2Used && bgBoard.Dices.IsCube1Used)
                        {
                            if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0)
                                continue;
                            else if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||
                                (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                return true;
                        }
                        else if (!bgBoard.Dices.IsCube2Used && !bgBoard.Dices.IsCube1Used)
                        {
                            if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0)  // if 1 cant
                            {
                                if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0) // if 2 cant
                                    continue;
                                else if (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Empty ||  // if 2 can and 1 cant
                           (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible2].NumOfSoldiers == 1))
                                    return true;
                                continue;   // if both cant
                            }
                            else   // if 1 can
                            {
                                if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||
                             (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                    return true;
                            }
                            if (SpikePossible2 > bgBoard.Cells.Length - 1 || SpikePossible2 < 0)  // if 2 cant
                            {
                                if (SpikePossible > bgBoard.Cells.Length - 1 || SpikePossible < 0) // if 1 cant
                                    continue;
                                else if (bgBoard.Cells[SpikePossible].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible].ColorOfCell == Color.Empty ||         // if 1 can and 2 cant
                             (bgBoard.Cells[SpikePossible].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible].NumOfSoldiers == 1))
                                    return true;
                                continue;   // if both cant
                            }
                            else // if 2 can
                            {
                                if (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.White || bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Empty ||  // if 2 can
                           (bgBoard.Cells[SpikePossible2].ColorOfCell == Color.Red && bgBoard.Cells[SpikePossible2].NumOfSoldiers == 1))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckIfAllSoldierAreInBase()
        {
            int numOfSoldiers = CurrentTurnPlayer.NumOfSoldiersOnBoard;
            int counter = 0;
            if (CurrentTurnPlayer.Color == Color.Red)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (bgBoard.Cells[i].ColorOfCell == Color.Red)
                        counter += bgBoard.Cells[i].NumOfSoldiers;
                }
            }
            else if (CurrentTurnPlayer.Color == Color.White)
            {

                for (int i = 18; i < bgBoard.Cells.Length; i++)
                {
                    if (bgBoard.Cells[i].ColorOfCell == Color.White)
                        counter += bgBoard.Cells[i].NumOfSoldiers;
                }
            }
            if (counter == CurrentTurnPlayer.NumOfSoldiersOnBoard)
                return true;
            return false;
        }

        private bool IsThereEatenSoldiers()
        {
            if (CurrentTurnPlayer.Color == Color.Red)
            {
                if (bgBoard.EatenRedCell.NumOfSoldiers > 0)
                    return true;
            }
            else
            {
                if (bgBoard.EatenWhiteCell.NumOfSoldiers > 0)
                    return true;
            }
            return false;
        }

        private bool CheckifWonGame()
        {
            if (CurrentTurnPlayer.NumOfSoldiersOnBoard == 0)
                return true;
            return false;
        }

        private void CountSoldiers()
        {
            int counter = 0;
            if(CurrentTurnPlayer.Color == Color.Red)
            {
                foreach (Cell cell in bgBoard.Cells)
                {
                    if(cell.ColorOfCell == Color.Red)
                    {
                        counter += cell.NumOfSoldiers;
                    }
                }

              
            }
            else if (CurrentTurnPlayer.Color == Color.White)
            {
                foreach (Cell cell in bgBoard.Cells)
                {
                    if (cell.ColorOfCell == Color.White)
                    {
                        counter += cell.NumOfSoldiers;
                    }
                }
            }

            CurrentTurnPlayer.NumOfSoldiersOnBoard = counter;
        }
        #endregion






    }
}
