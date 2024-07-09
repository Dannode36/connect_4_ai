using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MinMaxAI : Player
{
    public int branches;
    public MinMaxAI(int id) : base (id, "AI")
    {

    }

    public Tuple<int, float> MinMax(Board board, Player opposition, int depth, float alpha, float beta, bool maximizing)
    {
        var validLocations = board.ValidCollums();
        if (depth == 0 || board.CheckForFull() || board.CheckForWin(this) || board.CheckForWin(opposition))
        {
            if (board.CheckForWin(this))
            {
                return new Tuple<int, float>(0, 1000000000);
            }
            else if (board.CheckForWin(opposition))
            {
                return new Tuple<int, float>(0, -1000000000);
            }
            if (depth == 0)
            {
                int score = board.ScoreBoard(this);
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
                Board boardCopy = new(board);
                boardCopy.DropDisk(this, col);

                branches ++;
                float eval = MinMax(boardCopy, opposition, depth - 1, alpha, beta, false).Item2;
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
                Board boardCopy = new(board);
                boardCopy.DropDisk(opposition, col);

                branches ++;
                float eval = MinMax(boardCopy, opposition, depth - 1, alpha, beta, true).Item2;
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
