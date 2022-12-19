using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MinMaxAI : Player
{
    public int branches;
    public MinMaxAI(int id, GameController game) : base (id, "AI")
    {

    }

    public Tuple<int, float> MinMax(Board board, int depth, float alpha, float beta, bool maximizing)
    {
        var validLocations = board.ValidCollums();
        if (depth == 0 || board.CheckForFull() || board.CheckForWin(board.ai) || board.CheckForWin(board.player1))
        {
            if (board.CheckForWin(board.ai))
            {
                return new Tuple<int, float>(0, 1000000000);
            }
            else if (board.CheckForWin(board.player1))
            {
                return new Tuple<int, float>(0, -1000000000);
            }
            if (depth == 0)
            {
                int score = board.ScoreBoard(board.ai);
                return new Tuple<int, float>(0, score);
            }
            return new Tuple<int, float>(0, 0);
        }

        if (maximizing)
        {
            float maxEval = -Mathf.Infinity;
            int bestColumn = 3;

            foreach (int col in validLocations)
            {
                Board boardCopy = new Board(board);
                boardCopy.DropDisk(board.ai, col);

                branches ++;
                float eval = MinMax(boardCopy, depth - 1, alpha, beta, false).Item2;
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestColumn = col;
                }

                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return new Tuple<int, float>(bestColumn, maxEval);
        }
        else
        {
            float minEval = Mathf.Infinity;
            int bestColumn = 3;

            foreach (int col in validLocations)
            {
                Board boardCopy = new Board(board);
                boardCopy.DropDisk(board.player1, col);

                branches ++;
                float eval = MinMax(boardCopy, depth - 1, alpha, beta, true).Item2;
                if (eval < minEval)
                {
                    minEval = eval;
                    bestColumn = col;
                }
                beta = Mathf.Min(beta, minEval);

                if (beta <= alpha)
                {
                    break;
                }
            }
            return new Tuple<int, float>(bestColumn, minEval);

        }
    }
}
