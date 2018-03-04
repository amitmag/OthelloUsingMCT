using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player1 : Player
    {
        private char rival;
        public void getPlayers              // players ids
        (
            ref string player1_1,
            ref string player1_2
        )
        {
            player1_1 = "206348005";        // id1
            player1_2 = "203183470";        // id2
        }

        public Tuple<int, int> playYourTurn
       (
           Board board,
           TimeSpan timesup,
           char playerChar          // 1 or 2
       )
        {
            Tuple<int, int> toReturn = null;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //initial root
            rival = getRivalPlayer(playerChar);
            State root = new State(board, rival, null, 0, 0);

            sw.Stop();
            double timeToLevel = 0.8 * timesup.Ticks;  // get timer's time
            int i = 0;
            sw.Start();
            int count = 0;
            while (sw.ElapsedMilliseconds < timeToLevel)
            {
                //select the next state
                State nextState = select(root);

                //expend the state
                expend(nextState, playerChar);

                //choose node to explore
                nextState = chooseNodeToExplore(nextState);

                //simulte
                Tuple<int, int> winner = simulate(nextState, sw, timeToLevel);
                
                //update scores up the tree
                update(nextState, convertToChar(winner.Item1), winner.Item2);

            }

            State nextStep = root.getChildWithMaxScore();
            toReturn = new Tuple<int, int>(nextStep.row, nextStep.col);
            return toReturn;

        }

        // find the char that represents the rival player
        internal static char getRivalPlayer(char currentPlayer)
        {
            if (currentPlayer == '1')
                return '2';
            else
                return '1';
        }

        //select the best chils of the state
        private State select(State state)
        {
            State tempState = new State(state);

            while (tempState.children.Count != 0)
            {
                if (state.isCorner() || state.isCloseToCornerPositive() > 0)
                    return state;
                tempState = getBestNextState(tempState);

            }
            return tempState;

        }

        // expand the children of the current node
        private void expend(State state, char playerChar)
        {
            //check if the game ended
            if (state.board.isTheGameEnded())
                return;
            List<Tuple<int, int>> legalSteps = state.board.getLegalMoves(playerChar);
            int num = legalSteps.Count;

            for (int i = 0; i < num; i++)
            {
                Tuple<int, int> child = legalSteps[i];
                Board childBoard = new Board(state.board);
                childBoard.fillPlayerMove(playerChar, child.Item1, child.Item2);
                State childState = new State(childBoard, getRivalPlayer(state.player), state, child.Item1, child.Item2);
                state.children.Add(childState);


            }

        }

        /// choose random child to expend. If the state doesn't have children, return the state
        private State chooseNodeToExplore(State state)
        {
            int numberOfChildrens = state.children.Count;

            //chack if the state has children. If not, return the current state
            if (numberOfChildrens == 0)
                return state;

            //choose random child from state child
            int chosenChild = new Random().Next(0, numberOfChildrens);
            return state.children[chosenChild];

        }

        // play random steps until the game ends
        private Tuple<int, int> simulate(State state, Stopwatch currenttime, double totalTime)
        {
            State tempState = new State(state);

            //play random steps until the game ends
            for (int i = 0; i < 45000 / state.board._n && !tempState.board.isTheGameEnded() && currenttime.ElapsedMilliseconds < totalTime * 0.99; i++)
            {
                tempState.switchPlayer();

                //play random turn
                List<Tuple<int, int>> legalMoves = state.board.getLegalMoves(tempState.player);
                if (legalMoves.Count > 0)
                {
                    int randomMove = new Random().Next(0, legalMoves.Count);
                    Tuple<int, int> chosenMove = legalMoves[randomMove];
                    tempState.board.fillPlayerMove(tempState.player, chosenMove.Item1, chosenMove.Item2);
                }

            }
            Tuple<int, int> winner = findWinner(tempState.board);
            
            //return the winner char
            return findWinner(tempState.board);
        }

        // play random moves from the legals moves the player can do
        private void playRandomTurn(State state)
        {
            List<Tuple<int, int>> legalMoves = state.board.getLegalMoves(state.player);
            if (legalMoves.Count == 0)
                return;
            int randomMove = new Random().Next(0, legalMoves.Count);
            Tuple<int, int> chosenMove = legalMoves[randomMove];
            state.board.fillPlayerMove(state.player, chosenMove.Item1, chosenMove.Item2);

        }

        // return tuple of the winner player number and his score. If it's a tie, return 0.
        private Tuple<int, int> findWinner(Board board)
        {
            Tuple<int, int> scores = board.gameScore();
            if (scores.Item1 > scores.Item2)
                return new Tuple<int, int>(1, scores.Item1);
            else if (scores.Item1 < scores.Item2)
                return new Tuple<int, int>(2, scores.Item2);
            return new Tuple<int, int>(0, scores.Item1);
        }

        //update the score for the state
        private void update(State state, char winnerPlayer, int winnerScore)
        {
            State tempState = new State(state);
            while (state != null)
            {
                state.visitsCounter++;
                if (state.player == winnerPlayer)
                    state.score += winnerScore;
                else if (winnerPlayer == getRivalPlayer(state.player))
                    state.score -= winnerScore;
                else if (winnerPlayer == 0 && state.score == 0)
                    state.score += -5;

                state = state.parentState;

            }
        }

        //convert int to char
        private char convertToChar(int c)
        {
            if (c == 1)
                return '1';
            else if (c == 2)
                return '2';
            return '0';
        }

        //get the best next state according to huristic
        private State getBestNextState(State state)
        {
            List<State> children = state.children;
            Double maxScore = Int16.MinValue;
            State maxChild = null;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].visitsCounter == 0)
                    return children[i];

                double stateHuristic = children[i].calculateHuristic();
                if (stateHuristic > maxScore)
                {
                    maxScore = stateHuristic;
                    maxChild = children[i];
                }
            }


            return maxChild;
        }



        private Tuple<int, int> getBestScoreMove(List<Tuple<int, int>> moves, Board board)
        {
            int maxScore = 0;
            Tuple<int, int> toReturn = null;
            for (int i = 0; i < moves.Count; i++)
            {
                Tuple<int, int> currentMove = moves[i];
                if (board._boardCosts[currentMove.Item1, currentMove.Item2] > maxScore)
                {
                    maxScore = board._boardCosts[currentMove.Item1, currentMove.Item2];
                    toReturn = currentMove;
                }
            }
            return toReturn;

        }


    }

    internal class State
    {
        internal State parentState;
        internal List<State> children;
        internal int visitsCounter;
        internal int score;
        internal Board board;
        internal char player;
        internal int row;
        internal int col;
        internal char rivalPlayer;


        public State(int visitsCounter, int score, Board board, char player)
        {
            this.visitsCounter = visitsCounter;
            this.score = score;
            this.board = board;
            this.player = player;
            this.children = new List<State>();
            rivalPlayer = Player1.getRivalPlayer(player);
        }

        public State(Board board, char player, State parentState, int row, int col)
        {
            this.board = board;
            this.player = player;
            this.parentState = parentState;
            this.row = row;
            this.col = col;
            this.children = new List<State>();
            rivalPlayer = Player1.getRivalPlayer(player);


        }

        public State(State state)
        {
            this.parentState = state.parentState;
            this.children = state.children;
            this.visitsCounter = state.visitsCounter;
            this.score = state.score;
            this.board = state.board;
            this.player = state.player;
            this.player = state.rivalPlayer;
        }

        // switch players
        internal void switchPlayer()
        {
            char temp = player;
            player = Player1.getRivalPlayer(player);
            rivalPlayer = temp;

        }

        internal State getChildWithMaxScore()
        {
            if (children.Count == 0)
                return this;
            int maxScore = 0;

            int stateWithMaxScore = 0;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].score > maxScore)
                {
                    maxScore = children[i].score;
                    stateWithMaxScore = i;
                }

            }
            return children[stateWithMaxScore];

        }

        internal double calculateHuristic()
        {
            double avgScore = (double)(score / visitsCounter);
            return avgScore + totalMovesHuristic() + (stepValueHuristic() * cornersHuristic());
        }

        // check if the state is corner
        internal Boolean isCorner()
        {
            int boardSize = board._n;
            if (row == 0 && (col == 0 || col == boardSize - 1))
                return true;
            if (row == (boardSize - 1) && (col == 0 || col == boardSize - 1))
                return true;
            return false;

        }

        //calculte huristic according to total value of the step
        private double stepValueHuristic()
        {

            int playerTotalValue = 0, rivalTotalValue = 0;

            for (int i = 0; i < board._n; i++)
            {
                for (int j = 0; j < board._n; j++)
                {
                    char currentPlayer = board._boardGame[i, j];
                    if (currentPlayer == player)
                        playerTotalValue += board._boardCosts[i, j];
                    else if (currentPlayer == rivalPlayer)
                        rivalTotalValue += board._boardCosts[i, j];
                }
            }

            double valueDiffrence = playerTotalValue - rivalTotalValue;
            double totalValue = playerTotalValue + rivalTotalValue;
            return 100 * (valueDiffrence / totalValue);
        }

        //calaulte huristic according to the  steps that the players can do
        private double totalMovesHuristic()
        {
            int movesForPlayer = board.getLegalMoves(player).Count;
            int movesForRival = board.getLegalMoves(rivalPlayer).Count;
            int totalMoves = movesForPlayer + movesForRival;
            if (totalMoves > 0)
                return 100 * (movesForPlayer - movesForRival) / totalMoves;
            else
                return 0;
        }

        //calculate huristic according to the corners of each player
        private double cornersHuristic()
        {
            int playerCorners = findHowMuchCornersForPlayer(player);
            int rivalCorners = findHowMuchCornersForPlayer(rivalPlayer);

            int totalCorners = playerCorners + rivalCorners;

            if (totalCorners > 0)
                return 100 * (playerCorners - rivalCorners) / totalCorners;
            else
                return 0;
        }

        private int findHowMuchCornersForPlayer(char currentPlayer)
        {
            int totalCorners = 0;
            int boardSize = board._n;
            if (board._boardGame[0, 0] == currentPlayer)
                totalCorners++;
            if (board._boardGame[0, boardSize - 1] == currentPlayer)
                totalCorners++;
            if (board._boardGame[boardSize - 1, 0] == currentPlayer)
                totalCorners++;
            if (board._boardGame[boardSize - 1, boardSize - 1] == currentPlayer)
                totalCorners++;
            return totalCorners;


        }

        internal int isCloseToCornerPositive()
        {
            int boarSize = board._n;

            if (row == 0 && (col == 2 || col == boarSize - 3))
                return 2;
            else if (row == 2 && (col == 0 || col == boarSize - 1))
                return 2;
            else if (row == boarSize - 3 && (col == 0 || col == boarSize - 1))
                return 2;
            else if (row == boarSize - 1 && (col == 2 || col == boarSize - 3))
                return 2;
            return 1;
        }

        internal int isBorderOrCloseToCorner()
        {
            int boardSize = board._n;
            if ((row == 0 || row == boardSize - 1) && (col != 0 && col != 3 && col != boardSize - 3 && col != boardSize - 1))
                return -2;
            if ((col == 0 || col == boardSize - 1) && (row != 0 && row != 3 && row != boardSize - 3 && row != boardSize - 1))
                return -2;
            if ((row == 1 || row == boardSize - 2) && (col == 1 || col == boardSize - 2))
                return -2;
            return 0;
        }

        private double distanceFromMiddle()
        {
            int middle = board._n / 2;
            double rowDistance = Math.Pow(row - middle, 2);
            double colDistance = Math.Pow(col - middle, 2);
            return Math.Sqrt(rowDistance + colDistance);

        }

        public string toString()
        {
            return "(" + row + "," + col + ")";
        }









    }

}