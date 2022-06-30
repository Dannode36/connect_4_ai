using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GameController : MonoBehaviour
{
    bool won = false;
    bool gameReady = false;
    bool playersTurn = true;
    bool displayPreview = true;

    public bool aiThinking = false;
    bool vsAI = false;

    Board board;

    public Player currentPlayer;
    Player previousPlayer;

    public GameObject baseCollider;
    public Transform[] spawnLocations;
    public GameObject BlueDisk;
    public GameObject RedDisk;

    public GameObject GameCanvas;
    CanvasManger canvasManager;

    public Image dropPreview;

    private void Start()
    {
        //Application.targetFrameRate = 60;

        if (GameManager.single)
        {
            LoadSingleplayer();
        }
        else
        {
            LoadCoop();
        }
        canvasManager = GameCanvas.GetComponent<CanvasManger>();

        GameManager.NewGame();

        print(GameManager.PlayerOneName);
    }

    Color clear = new Color(0, 0, 0, 0);
    void Update()
    {
        if (!won)
        {
            float screenPercent = Input.mousePosition.x / Screen.width * 100;
            if (displayPreview && screenPercent > 25 && screenPercent < 75)
            {
                dropPreview.transform.position = new Vector3(Input.mousePosition.x, dropPreview.transform.position.y, dropPreview.transform.position.z);
                dropPreview.color = currentPlayer.id == 1 ? new Color(1, 0, 0, 0.4f) : new Color(1, 1, 0, 0.4f);
            }
            else
            {
                dropPreview.color = clear;
            }
        }
        else
        {
            dropPreview.color = clear;
        }
    }

    void LoadSingleplayer()
    {
        vsAI = true;
        board = new Board(6, 7, new Player(1, GameManager.PlayerOneName), new MinMaxAI(2, this));
        currentPlayer = board.player1;
        previousPlayer = board.ai;
        gameReady = true;
    }

    void LoadCoop()
    {
        board = new Board(6, 7, new Player(1, GameManager.PlayerOneName), new Player(2, GameManager.PlayerTwoName));
        currentPlayer = board.player1;
        previousPlayer = board.player2;
        gameReady = true;
    }

    IEnumerator AITurn(int aiCollum)
    {
        playersTurn = false;
        yield return new WaitForSeconds(1);
        print("Minmax output: " + aiCollum);
        DropDiskAsync(board.ai , aiCollum);
        Print2DArray(board.array);
        StartCoroutine(TurnDelay());
    }

    IEnumerator TurnDelay()
    {
        yield return new WaitForSeconds(1);
        playersTurn = true;
        gameReady = true;
        displayPreview = true;
    }

    IEnumerator BoardReset()
    {
        yield return new WaitForSeconds(4);
        baseCollider.SetActive(false);
        yield return new WaitForSeconds(2);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void Win(Player winner, Player looser, bool vsAI)
    {
        canvasManager.DisplayWinTitle(winner.name, looser.name);
        won = true;
        GameManager.Win(winner, looser);

        StartCoroutine("BoardReset");
    }

    public void AddDisk(int collum)
    {
        if (!gameReady || won || !board.IsValidLocation( collum)) { return; }

        gameReady = false;
        DropDiskAsync(currentPlayer, collum);
    }

    private async Task DropDiskAsync(Player player, int collum)
    {
        displayPreview = false;

        if (board.DropDisk(player, collum))
        {
            Instantiate(player.id == 1 ? RedDisk : BlueDisk, spawnLocations[collum].transform.position, spawnLocations[collum].transform.rotation);

            if (board.CheckForWin(player))
            {
                Win(player, previousPlayer, vsAI);
                return;
            }

            previousPlayer = player;

            if (!vsAI)
            {
                currentPlayer = player == board.player1 ? board.player2 : board.player1;
            }
            else
            {
                currentPlayer = player == board.player1 ? board.ai : board.player1;
            }

            if (playersTurn && vsAI)
            {
                aiThinking = true;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                int aiMove = await Task.Run(() => board.ai.MinMax(board, 7, -Mathf.Infinity, Mathf.Infinity, true).Item1);

                stopWatch.Stop();
                Debug.Log($"AI spent {stopWatch.ElapsedMilliseconds} milliseconds thinking.");

                StartCoroutine(AITurn(aiMove));
                aiThinking = false;
            }
            else
            {
                StartCoroutine(TurnDelay());
            }
        }
    }

    void Print2DArray(int[,] array)
    {
        string printString = "";
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int collum = 0; collum < array.GetLength(1); collum++)
            {
                printString += $" {array[row, collum]},";
            }
            printString += Environment.NewLine;
        }
        print(printString);
    }
}

public class Board
{
    public readonly int[,] array;

    public readonly Player player1;
    public readonly Player player2;
    public readonly MinMaxAI ai;

    public Board(int rows, int collums, Player player1, Player player2)
    {
        array = new int[rows, collums];
        this.player1 = player1;
        this.player2 = player2;
    }
    public Board(int rows, int collums, Player player1, MinMaxAI ai)
    {
        array = new int[rows, collums];
        this.player1 = player1;
        this.ai = ai;
    }
    public Board(Board boardToCopy)
    {
        array = (int[,])boardToCopy.array.Clone();
        player1 = boardToCopy.player1;
        player1 = boardToCopy.player1;
        ai = boardToCopy.ai;
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
        List<int> validCollums = new List<int>();

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
        int player = currentPlayer.id;

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
        int printScore = 0;
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
                    if (array[i, j + x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i, j + x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i + x, j] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i + x, j] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i - x, j + x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j + x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i - x, j - x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j - x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
            }
        }
        return score;
    }
    public int ScoreBoard(Player currentPlayer, int[,] array)
    {
        int score = 0;
        int printScore = 0;
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
                    if (array[i, j + x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i, j + x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i + x, j] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i + x, j] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i - x, j + x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j + x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
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
                    if (array[i - x, j - x] == currentPlayer.id)
                    {
                        playerPiece++;
                    }
                    else if (array[i - x, j - x] == (currentPlayer.id == 1 ? 2 : 1))
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
                if (newScore > printScore)
                {
                    printScore = newScore;
                }
            }
        }
        return score;
    }

    public int EvaluateScore(int playerPieces, int opPieces, int empty)
    {
        int score = 0;
        if (playerPieces == 4)
        {
            score += 100;
        }
        else if (playerPieces == 3 && empty == 1)
        {
            score += 5;
        }
        else if (playerPieces == 2 && empty == 2)
        {
            score += 2;
        }
        else if (opPieces == 3 && empty == 1)
        {
            score -= 4;
        }
        return score;
    }

    public bool DropDisk(Player player, int collum)
    {
        for (int i = array.GetLength(0) - 1; i > -1; i--)
        {
            if (array[i, collum] == 0)
            {
                array[i, collum] = player.id;
                return true;
            }
        }
        return false;
    }

    public void Print2DArray()
    {
        string printString = "";
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int collum = 0; collum < array.GetLength(1); collum++)
            {
                printString += $" {array[row, collum]},";
            }
            printString += Environment.NewLine;
        }
        Debug.Log(printString);
    }
    public void Print2DArray(int score)
    {
        string printString = "";
        for (int row = 0; row < array.GetLength(0); row++)
        {
            for (int collum = 0; collum < array.GetLength(1); collum++)
            {
                printString += $" {array[row, collum]},";
            }
            printString += Environment.NewLine;
        }
        Debug.Log(printString + $" Score: {score}");
    }
}
