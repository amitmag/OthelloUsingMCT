using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player2 : Player
    {
        private char MaxPlayer;


        public void getPlayers              // players ids
        (
            ref string player1_1,
            ref string player1_2
        )
        {
            player1_1 = "204208037";        // id1
            player1_2 = "201605870";        // id2
        }
        public Tuple<int, int> playYourTurn
        (
            Board board,
            TimeSpan timesup,
            char playerChar          // 1 or 2
        )
        {
            /**************our algorithem *********************/
            MaxPlayer = playerChar;
            //var watch = new Stopwatch();
            //watch.Start();
            Tuple<int, int> toReturn = null;
            int iMiddle = GetMidd(board);
            double MinDist = Int32.MaxValue;
            List<Tuple<int, int>> possibleMoves = board.getLegalMoves(playerChar);
            for (int iIndex = 0, iSize = possibleMoves.Count; iIndex < iSize; iIndex++)
            {
                Tuple<int, int> tuple = possibleMoves[iIndex];
                if (isCorner(tuple.Item1, tuple.Item2, board))
                {
                    toReturn = tuple;
                    break;
                }

                if (isEdge(tuple, board) || isNearCorner(tuple, board))
                {
                    continue;
                }
                if (null == toReturn)
                {
                    toReturn = tuple;
                    MinDist = GetDist(tuple, iMiddle);
                    continue;
                }
                double dTemp = GetDist(tuple, iMiddle);
                if (dTemp < MinDist)
                {
                    MinDist = dTemp;
                    toReturn = tuple;
                }
            }
            if (null == toReturn)
            {
                MinDist = Int32.MaxValue;
                for (int iIndex = 0, iSize = possibleMoves.Count; iIndex < iSize; iIndex++)
                {
                    Tuple<int, int> tuple = possibleMoves[iIndex];
                    if (isNearCorner(tuple, board))
                    {
                        continue;
                    }
                    if (null == toReturn)
                    {
                        toReturn = tuple;
                        MinDist = GetDist(tuple, iMiddle);
                        continue;
                    }
                    double dTemp = GetDist(tuple, iMiddle);
                    if (dTemp < MinDist)
                    {
                        MinDist = dTemp;
                        toReturn = tuple;
                    }
                }
            }
            if (null == toReturn)
            {
                for (int iIndex = 0, iSize = possibleMoves.Count; iIndex < iSize; iIndex++)
                {
                    Tuple<int, int> tuple = possibleMoves[iIndex];
                    if (0 == iIndex)
                    {
                        toReturn = tuple;
                        MinDist = GetDist(tuple, iMiddle);
                        continue;
                    }
                    double dTemp = GetDist(tuple, iMiddle);
                    if (dTemp < MinDist)
                    {
                        MinDist = dTemp;
                        toReturn = tuple;
                    }
                }
            }

            //Console.WriteLine("Time for move: " + watch.ElapsedMilliseconds);
            return toReturn;
        }

        /********************some private funcs that help *************************/
        private bool isNearCorner(Tuple<int, int> tuple, Board board)
        {
            int iSize = board._n;
            int iRow = tuple.Item1;
            int iCol = tuple.Item2;
            if ((iRow == 1 && iCol == 1) || (iRow == 1 && iCol == iSize - 4) || (iRow == iSize - 2 && iCol == 1) || (iRow == iSize - 2 && iCol == iSize - 2))
            {
                return true;
            }
            if ((iRow == 1 && iCol == 0) || (iRow == 1 && iCol == iSize - 1) || (iRow == iSize - 2 && iCol == 0) || (iRow == iSize - 2 && iCol == iSize - 1))
            {
                return true;
            }
            if ((iRow == 0 && iCol == 1) || (iRow == 0 && iCol == iSize - 2) || (iRow == iSize - 1 && iCol == iSize - 2))
            {
                return true;
            }
            return false;
        }

        private bool isEdge(Tuple<int, int> tuple, Board board)
        {
            int iSize = board._n;
            int iRow = tuple.Item1, iCol = tuple.Item2;
            if (iRow == 0 || iRow == iSize - 1 || iCol == 0 || iCol == iSize - 1)
            {
                return true;
            }
            return false;

        }

        private double GetDist(Tuple<int, int> tuple, int iMiddle)
        {
            double dRows = Math.Pow(tuple.Item1 - iMiddle, 2);
            double dCols = Math.Pow(tuple.Item2 - iMiddle, 2);
            return Math.Sqrt(dRows + dCols);
        }





        private bool isCorner(int iRow, int iCol, Board board)
        {
            int iSize = board._n;
            if ((iRow == 0 && iCol == 0) || (iRow == 0 && iCol == iSize - 1) || (iRow == iSize - 1 && iCol == 0) || (iRow == iSize - 1 && iCol == iSize - 1))
            {
                return true;
            }
            return false;
        }

        private int GetMidd(Board board)
        {
            return board._n / 2;
        }




    }


}
