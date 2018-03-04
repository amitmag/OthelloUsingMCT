using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class GameController
    {
        int _numberOfGames;
        int _boardSize;
        int _gameLevel;
        bool _printResults;
        Player _player1;
        Player _player2;
        public GameController
        (
            int numberOfGames,
            int boardSize,
            int gameLevel,
            bool printResults,
            Player player1,
            Player player2
        )
        {
            _numberOfGames = numberOfGames;
            _boardSize = boardSize;
            _gameLevel = gameLevel;
            _printResults = printResults;
            _player1 = player1;
            _player2 = player2;
        }

        public Tuple<int, int> Run()
        {
            Tuple<int, int> currentScore;
            int player1score = 0;
            int player2score = 0;

            for (int game = 0; game < _numberOfGames; game++)
            {
                Board board1 = new Board(_boardSize);
                Board board2 = new Board(board1);
                board2.switchBoard();
                currentScore = RunOneGame(board1, '1');
                if (currentScore.Item1 > currentScore.Item2)
                {
                    player1score += 1;
                }
                else
                    player2score += 1;

                currentScore = RunOneGame(board2, '2');
                if (currentScore.Item1 > currentScore.Item2)
                {
                    player1score += 1;
                }
                else
                    player2score += 1;// player2 starts

                /*
                currentScore = RunOneGame(board1, '1');                                             // player1 starts
                player1score += currentScore.Item1;
                player2score += currentScore.Item2;

                currentScore = RunOneGame(board2, '2');                                             // player2 starts
                player1score += currentScore.Item1;
                player2score += currentScore.Item2;
                */
            }
            return new Tuple<int, int>(player1score, player2score);
        }



        private Tuple<int, int> RunOneGame
        (
            Board board,
            char currentPlayerChar
        )
        {
            while (!board.isTheGameEnded())
            {
                if (_printResults)
                {
                    board.printTheBoardGame();
                    board.printTheBoardCosts();
                    printGameResults(board.gameScore());
                    Console.WriteLine("\nPlayer " + currentPlayerChar + " turn .. ");
                    Console.ReadLine();
                }

                if (board.getLegalMoves(currentPlayerChar).Count == 0)
                {
                    if (_printResults)
                    {
                        Console.WriteLine("Player " + currentPlayerChar + " got no legal moves .. ");
                        Console.ReadLine();
                        Console.Clear();

                    }
                    currentPlayerChar = Board.otherPlayer(currentPlayerChar);
                    continue;
                }

                Stopwatch timer = Stopwatch.StartNew();                                                                     // new timer
                int turnTime = levelToTime();                                                                            // turn time
                Player currentPlayer = getPlayer(currentPlayerChar);                                                             // get current player
                Tuple<int, int> selectedAction = currentPlayer.playYourTurn(new Board(board), new TimeSpan(turnTime), currentPlayerChar);  // run player turn
                timer.Stop();                                                                                                               // stop the timer
                if (_printResults)
                {
                    Console.WriteLine("Selected action: [ " + selectedAction.Item1 + " , " + selectedAction.Item2 + " ]");
                    Console.ReadLine();
                    Console.Clear();

                }
                TimeSpan timespan = timer.Elapsed;                                                                            // get timer's time

                
                if
                (
                    timespan.TotalMilliseconds > turnTime ||                                                                                // check timeout
                    selectedAction == null ||
                    !board.isLegalMove(currentPlayerChar, selectedAction.Item1, selectedAction.Item2)                                       // check move legality
                )
                    return board.gameScoreStopBeforeFinishing(Board.otherPlayer(currentPlayerChar));
                else                                                                                                                        // no timeout
                {
                    board.fillPlayerMove(currentPlayerChar, selectedAction.Item1, selectedAction.Item2);
                }
                currentPlayerChar = Board.otherPlayer(currentPlayerChar);
            }
            return board.gameScore();
        }

        private void printGameResults(Tuple<int, int> scores)
        {
            Console.WriteLine("***   Score   ***");
            Console.WriteLine("Player 1: " + scores.Item1);
            Console.WriteLine("Player 2: " + scores.Item2);
        }

        private Player getPlayer
        (
            char player
        )
        {
            Type playerType;
            if (player == '1')
                playerType = _player1.GetType();
            else
                playerType = _player2.GetType();
            Player newPlayer = (Player)Activator.CreateInstance(playerType);
            return newPlayer;
        }

        private int levelToTime()
        {
            if (_gameLevel == 1)
                return 4000;
            else if (_gameLevel == 2)
                return 150;
            else if (_gameLevel == 3)
                return 100;
            else if (_gameLevel == 4)
                return 80;
            else
                return 50;
        }
    }
}
