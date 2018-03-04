using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Board
    {
        public int _n;
        public char[,] _boardGame;
        public int[,] _boardCosts;
        public int _squaresLeft;

        public Board(int n)
        {
            _n = n;
            _squaresLeft = n * n - 4;
            createNewBoard();
        }
        public Board(Board toCopy)
        : this(toCopy._n, toCopy._boardGame, toCopy._boardCosts, toCopy._squaresLeft)
        {
        }

        public Board(int n, char[,] boardGame, int[,] boardCosts, int squaresLeft)
        {
            _n = n;

            _boardGame = new char[_n, _n];
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                {
                    _boardGame[i, j] = boardGame[i, j];
                }

            _boardCosts = new int[_n, _n];
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                {
                    _boardCosts[i, j] = boardCosts[i, j];
                }

            _squaresLeft = squaresLeft;
        }

        private void createNewBoard()
        {
            Random rand = new Random();
            _boardGame = new char[_n, _n];
            _boardCosts = new int[_n, _n];
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                {
                    _boardGame[i, j] = '0';
                }
            _boardGame[(_n / 2) - 1, (_n / 2) - 1] = '2';
            _boardGame[(_n / 2), (_n / 2)] = '2';
            _boardGame[(_n / 2) - 1, (_n / 2)] = '1';
            _boardGame[(_n / 2), (_n / 2) - 1] = '1';
            List<int> pointsList = new List<int>();
            for (int i = 0; i < _n * _n; i++)
                pointsList.Add(i);
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                {
                    int pointsIndex = rand.Next(pointsList.Count);
                    _boardCosts[i, j] = pointsList[pointsIndex];
                    pointsList.RemoveAt(pointsIndex);
                }
        }

        public bool isTheGameEnded()
        {
            if (_squaresLeft == 0)
                return true;
            if (getLegalMoves('1').Count == 0 &&
                getLegalMoves('2').Count == 0)
                return true;
            return false;
        }

        public List<Tuple<int, int>> getLegalMoves(char player)
        {
            List<Tuple<int, int>> legalMoves = new List<Tuple<int, int>>();
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                {
                    if (isLegalMove(player, i, j))
                        legalMoves.Add(new Tuple<int, int>(i, j));
                }
            return legalMoves;
        }

        public bool fillPlayerMove(char player, int row, int col)
        {
            if (!isLegalMove(player, row, col))
                return false;

            _boardGame[row, col] = player;

            if (isLegalDown(player, row, col))
                fillDown(player, row, col);

            if (isLegalUp(player, row, col))
                fillUp(player, row, col);

            if (isLegalRight(player, row, col))
                fillRight(player, row, col);

            if (isLegalLeft(player, row, col))
                fillLeft(player, row, col);

            if (isLegalDiagonalDownLeft(player, row, col))
                fillDiagonalDownLeft(player, row, col);

            if (isLegalDiagonalDownRight(player, row, col))
                fillDiagonalDownRight(player, row, col);

            if (isLegalDiagonalUpLeft(player, row, col))
                fillDiagonalUpLeft(player, row, col);

            if (isLegalDiagonalUpRight(player, row, col))
                fillDiagonalUpRight(player, row, col);

            _squaresLeft--;
            return true;
        }

        public bool isLegalMove(
            char player,
            int row,
            int col
        )
        {
            if (row > _n - 1 ||
                col > _n - 1 ||
                row < 0 ||
                col < 0 ||
                _boardGame[row, col] != '0')
                return false;
            if (isLegalDown(player, row, col) ||
                isLegalUp(player, row, col) ||
                isLegalRight(player, row, col) ||
                isLegalLeft(player, row, col) ||
                isLegalDiagonalDownLeft(player, row, col) ||
                isLegalDiagonalDownRight(player, row, col) ||
                isLegalDiagonalUpLeft(player, row, col) ||
                isLegalDiagonalUpRight(player, row, col))
                return true;
            return false;
        }

        private bool isLegalDown
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1; i < _n; i++)
            {
                if (_boardGame[i, col] == '0')
                    return false;
                if (_boardGame[i, col] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, col] == player)
                    return true;
                if (_boardGame[i, col] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalUp
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1; i >= 0; i--)
            {
                if (_boardGame[i, col] == '0')
                    return false;
                if (_boardGame[i, col] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, col] == player)
                    return true;
                if (_boardGame[i, col] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int j = col + 1; j < _n; j++)
            {
                if (_boardGame[row, j] == '0')
                    return false;
                if (_boardGame[row, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[row, j] == player)
                    return true;
                if (_boardGame[row, j] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int j = col - 1; j >= 0; j--)
            {
                if (_boardGame[row, j] == '0')
                    return false;
                if (_boardGame[row, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[row, j] == player)
                    return true;
                if (_boardGame[row, j] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalDiagonalUpRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1, j = col + 1; i >= 0 && j < _n; i--, j++)
            {
                if (_boardGame[i, j] == '0')
                    return false;
                if (_boardGame[i, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, j] == player)
                    return true;
                if (_boardGame[i, j] == player)
                    return false;
            }
            return false;
        }


        private bool isLegalDiagonalUpLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (_boardGame[i, j] == '0')
                    return false;
                if (_boardGame[i, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, j] == player)
                    return true;
                if (_boardGame[i, j] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalDiagonalDownRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1, j = col + 1; i < _n && j < _n; i++, j++)
            {
                if (_boardGame[i, j] == '0')
                    return false;
                if (_boardGame[i, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, j] == player)
                    return true;
                if (_boardGame[i, j] == player)
                    return false;
            }
            return false;
        }

        private bool isLegalDiagonalDownLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1, j = col - 1; i < _n && j >= 0; i++, j--)
            {
                if (_boardGame[i, j] == '0')
                    return false;
                if (_boardGame[i, j] == otherPlayer(player))
                    otherPlayerFlag = true;
                if (otherPlayerFlag && _boardGame[i, j] == player)
                    return true;
                if (_boardGame[i, j] == player)
                    return false;
            }
            return false;
        }

        private bool fillDown
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1; i < _n; i++)
            {
                if (_boardGame[i, col] == otherPlayer(player))
                    _boardGame[i, col] = player;
                if (otherPlayerFlag && _boardGame[i, col] == player)
                    return true;
            }
            return false;
        }

        private bool fillUp
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1; i >= 0; i--)
            {
                if (_boardGame[i, col] == otherPlayer(player))
                    _boardGame[i, col] = player;
                if (otherPlayerFlag && _boardGame[i, col] == player)
                    return true;
            }
            return false;
        }

        private bool fillRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int j = col + 1; j < _n; j++)
            {
                if (_boardGame[row, j] == otherPlayer(player))
                    _boardGame[row, j] = player;
                if (otherPlayerFlag && _boardGame[row, j] == player)
                    return true;
            }
            return false;
        }

        private bool fillLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int j = col - 1; j >= 0; j--)
            {
                if (_boardGame[row, j] == otherPlayer(player))
                    _boardGame[row, j] = player;
                if (otherPlayerFlag && _boardGame[row, j] == player)
                    return true;
            }
            return false;
        }

        private bool fillDiagonalUpRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1, j = col + 1; i >= 0 && j < _n; i--, j++)
            {
                if (_boardGame[i, j] == otherPlayer(player))
                    _boardGame[i, j] = player;
                if (otherPlayerFlag && _boardGame[row, i] == player)
                    return true;
            }
            return false;
        }


        private bool fillDiagonalUpLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (_boardGame[i, j] == otherPlayer(player))
                    _boardGame[i, j] = player;
                if (otherPlayerFlag && _boardGame[row, i] == player)
                    return true;
            }
            return false;
        }

        private bool fillDiagonalDownRight
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1, j = col + 1; i < _n && j < _n; i++, j++)
            {
                if (_boardGame[i, j] == otherPlayer(player))
                    _boardGame[i, j] = player;
                if (otherPlayerFlag && _boardGame[row, i] == player)
                    return true;
            }
            return false;
        }

        private bool fillDiagonalDownLeft
        (
            char player,
            int row,
            int col
        )
        {
            bool otherPlayerFlag = false;
            for (int i = row + 1, j = col - 1; i < _n && j >= 0; i++, j--)
            {
                if (_boardGame[i, j] == otherPlayer(player))
                    _boardGame[i, j] = player;
                if (otherPlayerFlag && _boardGame[row, i] == player)
                    return true;
            }
            return false;
        }

        public static char otherPlayer(char player)
        {
            if (player == '1')
                return '2';
            if (player == '2')
                return '1';
            return '0';
        }


        public void printTheBoardGame()
        {
            Console.WriteLine("Board Game:");
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (j == 0)
                        Console.Write("| ");
                    Console.Write(_boardGame[i, j]);
                    Console.Write(" | ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        public void printTheBoardCosts()
        {
            Console.WriteLine("Board Costs:");
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (j == 0)
                        Console.Write("| ");
                    Console.Write(_boardCosts[i, j]);
                    Console.Write("\t | ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        public Tuple<int, int> gameScore()
        {
            int player1score = 0;
            int player2score = 0;
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                    if (_boardGame[i, j] == '1')
                        player1score += _boardCosts[i, j];
                    else if (_boardGame[i, j] == '2')
                        player2score += _boardCosts[i, j];
            return new Tuple<int, int>(player1score, player2score);
        }

        public Tuple<int, int> gameScoreStopBeforeFinishing(char winningPlayer)
        {
            int player1score = 0;
            int player2score = 0;
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                    if (winningPlayer == '1')
                        player1score += _boardCosts[i, j];
                    else if (winningPlayer == '2')
                        player2score += _boardCosts[i, j];
            return new Tuple<int, int>(player1score, player2score);
        }

        public void switchBoard()
        {
            for (int i = 0; i < _n; i++)
                for (int j = 0; j < _n; j++)
                    if (_boardGame[i, j] == '1')
                        _boardGame[i, j] = '2';
                    else if (_boardGame[i, j] == '2')
                        _boardGame[i, j] = '1';
        }

    }
}
