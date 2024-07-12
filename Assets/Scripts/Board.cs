using System.Collections.Generic;

public class Board
{
    public readonly int[,] array;

    public Board(int rows, int collums)
    {
        array = new int[rows, collums];
    }
    public Board(Board boardToCopy)
    {
        array = (int[,])boardToCopy.array.Clone();
    }

    public bool IsValidLocation(int collum)
    {
        for (int i = array.GetLength(0) - 1; i > -1; i--)
        {
            if (array[i, collum] == 0)
            {
                return true;
            }
        }
        return false;
    }

    public List<int> ValidCollums()
    {
        List<int> validCollums = new();

        for (int i = 0; i < array.GetLength(1); i++)
        {
            if (IsValidLocation(i))
            {
                validCollums.Add(i);
            }
        }
        return validCollums;
    }

    public int NextFreeRow(int collum)
    {
        for (int i = array.GetLength(0) - 1; i > -1; i--)
        {
            if (array[i, collum] == 0)
            {
                return i;
            }
        }
        return 0;
    }

    public bool CheckForWin(Player currentPlayer)
    {
        int player = currentPlayer.turnNumber;

        // horizontalCheck 
        for (int j = 0; j < array.GetLength(1) - 3; j++)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array[i, j] == player && array[i, j + 1] == player && array[i, j + 2] == player && array[i, j + 3] == player)
                {
                    return true;
                }
            }
        }

        // verticalCheck
        for (int j = 0; j < array.GetLength(1); j++)
        {
            for (int i = 0; i < array.GetLength(0) - 3; i++)
            {
                if (array[i, j] == player && array[i + 1, j] == player && array[i + 2, j] == player && array[i + 3, j] == player)
                {
                    return true;
                }
            }
        }

        // ascendingDiagonalCheck 
        for (int j = 0; j < array.GetLength(1) - 3; j++)
        {
            for (int i = 3; i < array.GetLength(0); i++)
            {
                if (array[i, j] == player && array[i - 1, j + 1] == player && array[i - 2, j + 2] == player && array[i - 3, j + 3] == player)
                {
                    return true;
                }
            }
        }

        // descendingDiagonalCheck
        for (int j = 3; j < array.GetLength(1); j++)
        {
            for (int i = 3; i < array.GetLength(0); i++)
            {
                if (array[i, j] == player && array[i - 1, j - 1] == player && array[i - 2, j - 2] == player && array[i - 3, j - 3] == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckForFull()
    {
        foreach (int i in array)
        {
            if (i == 0)
            {
                return false;
            }
        }
        return true;
    }

    public int ScoreBoard(Player currentPlayer)
    {
        int score = 0;

        //Horizontal
        for (int j = 0; j < array.GetLength(1) - 3; j++)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int playerPiece = 0;
                int opPiece = 0;
                int empty = 0;

                for (int x = 0; x < 4; x++)
                {
                    if (array[i, j + x] == currentPlayer.turnNumber)
                    {
                        playerPiece++;
                    }
                    else if (array[i, j + x] == (currentPlayer.turnNumber == 1 ? 2 : 1))
                    {
                        opPiece++;
                    }
                    else
                    {
                        empty++;
                    }
                }

                int newScore = EvaluateScore(playerPiece, opPiece, empty);
                score += newScore;
            }
        }

        //Vertical
        for (int j = 0; j < array.GetLength(1); j++)
        {
            for (int i = 0; i < array.GetLength(0) - 3; i++)
            {
                int playerPiece = 0;
                int opPiece = 0;
                int empty = 0;

                for (int x = 0; x < 4; x++)
                {
                    if (array[i + x, j] == currentPlayer.turnNumber)
                    {
                        playerPiece++;
                    }
                    else if (array[i + x, j] == (currentPlayer.turnNumber == 1 ? 2 : 1))
                    {
                        opPiece++;
                    }
                    else
                    {
                        empty++;
                    }
                }

                int newScore = EvaluateScore(playerPiece, opPiece, empty);
                score += newScore;
            }
        }

        //Ascending Diagonal 
        for (int j = 0; j < array.GetLength(1) - 3; j++)
        {
            for (int i = 3; i < array.GetLength(0); i++)
            {
                int playerPiece = 0;
                int opPiece = 0;
                int empty = 0;

                for (int x = 0; x < 4; x++)
                {
                    if (array[i - x, j + x] == currentPlayer.turnNumber)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j + x] == (currentPlayer.turnNumber == 1 ? 2 : 1))
                    {
                        opPiece++;
                    }
                    else
                    {
                        empty++;
                    }
                }

                int newScore = EvaluateScore(playerPiece, opPiece, empty);
                score += newScore;
            }
        }

        //Descending Diagonal
        for (int j = 3; j < array.GetLength(1); j++)
        {
            for (int i = 3; i < array.GetLength(0); i++)
            {
                int playerPiece = 0;
                int opPiece = 0;
                int empty = 0;

                for (int x = 0; x < 4; x++)
                {
                    if (array[i - x, j - x] == currentPlayer.turnNumber)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j - x] == (currentPlayer.turnNumber == 1 ? 2 : 1))
                    {
                        opPiece++;
                    }
                    else
                    {
                        empty++;
                    }
                }

                int newScore = EvaluateScore(playerPiece, opPiece, empty);
                score += newScore;
            }
        }

        return score;
    }

    public int EvaluateScore(int playerPieces, int opPieces, int empty)
    {
        int score = 0;
        if (playerPieces == 4)
        {
            score += 1000;
        }
        else if (playerPieces == 3 && empty == 1)
        {
            score += 10;
        }
        else if (playerPieces == 2 && empty == 2)
        {
            score += 2;
        }
        else if (opPieces == 3 && empty == 1)
        {
            score -= 40;
        }
        return score;
    }

    public bool DropDisk(Player player, int collum)
    {
        for (int i = array.GetLength(0) - 1; i > -1; i--)
        {
            if (array[i, collum] == 0)
            {
                array[i, collum] = player.turnNumber;
                return true;
            }
        }
        return false;
    }
}
