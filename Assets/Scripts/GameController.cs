using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static UnityEngine.Rendering.DebugManager;

public class GameController : MonoBehaviour
{
    [InspectorButton("QuickEndGame")]
    public bool EndGame;

    private Gamemode gamemode;
    static public bool aiFirst;
    static public bool swapStarting;
    private Board board;

    bool won;
    bool waiting;

    public Player CurrentPlayer { get; private set; }
    public Player PreviousPlayer { get; private set; }

    public GameObject baseCollider;
    public Transform[] spawnLocations;
    public GameObject BlueDisk;
    public GameObject RedDisk;

    public GameObject GameCanvas;
    public CanvasManger canvasManager;

    public Image dropPreview;

    private void Start()
    {
        gamemode = GameManager.gamemode;
        aiFirst = GameManager.aiFirst;
        swapStarting = GameManager.swapStarting;

        board = new Board(6, 7);

        switch (gamemode)
        {
            case Gamemode.VsAI:
                if (aiFirst)
                {
                    CurrentPlayer = new MinMaxAI(1);
                    PreviousPlayer = new Player(2, GameManager.PlayerOneName);
                    waiting = true;
                    StartCoroutine(AITurn());
                }
                else
                {
                    CurrentPlayer = new Player(1, GameManager.PlayerOneName);
                    PreviousPlayer = new MinMaxAI(2);
                }
                break;
            case Gamemode.Secret:
                CurrentPlayer = new MinMaxAI(1);
                PreviousPlayer = new MinMaxAI(2);
                waiting = true;
                StartCoroutine(AITurn());
                break;
            default:
                CurrentPlayer = new Player(1, GameManager.PlayerOneName);
                PreviousPlayer = new Player(2, GameManager.PlayerTwoName);
                break;
        }

        canvasManager.DisplayTurnInfo(CurrentPlayer.name + "'s turn");
    }

    void Update()
    {
        if (!won)
        {
            float screenPercent = Input.mousePosition.x / Screen.width * 100;
            if (!waiting && screenPercent > 25 && screenPercent < 75)
            {
                dropPreview.transform.position = new Vector3(Input.mousePosition.x, dropPreview.transform.position.y, dropPreview.transform.position.z);
                dropPreview.color = CurrentPlayer.turnNumber == 1 ? new Color(1, 0, 0, 0.4f) : new Color(1, 1, 0, 0.4f);
                return;
            }
        }
        dropPreview.color = Color.clear;
    }

    public void ResetBoard()
    {
        if (swapStarting)
        {
            aiFirst = !aiFirst;
        }
        StartCoroutine(ResetCoroutine());
        canvasManager.ShowResetButton(false);
    }

    IEnumerator ResetCoroutine()
    {
        baseCollider.SetActive(false);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Win(Player winner, Player looser)
    {
        won = true;
        GameManager.Win(winner, looser);
        canvasManager.DisplayWinTitle(winner.name, looser.name);
        canvasManager.ShowResetButton(true);
    }

    void Tie(Player playerOne, Player playerTwo)
    {
        won = true;
        GameManager.Tie(playerOne, playerTwo);
        canvasManager.DisplayWinText("Tie");
        canvasManager.ShowResetButton(true);
    }

    public void AddDiskButton(int collum)
    {
        if (waiting || won || !board.IsValidLocation( collum)) { return; }

        waiting = true;
        DropDisk(CurrentPlayer, collum);
    }

    private void DropDisk(Player player, int collum)
    {
        if (board.DropDisk(player, collum))
        {
            Instantiate(player.turnNumber == 1 ? RedDisk : BlueDisk, spawnLocations[collum].transform.position, spawnLocations[collum].transform.rotation);

            if (board.CheckForWin(player))
            {
                Win(player, PreviousPlayer);
                return;
            }
            else if (board.CheckForFull())
            {
                Tie(PreviousPlayer, CurrentPlayer); //board should always fill up on the second player
                return;
            }

            CurrentPlayer = PreviousPlayer;
            PreviousPlayer = player;

            if (CurrentPlayer is MinMaxAI)
            {
                StartCoroutine(AITurn());
            }
            else
            {
                StartCoroutine(TurnDelay());
            }
        }
    }

    IEnumerator AITurn()
    {
        Stopwatch stopWatch = new();
        stopWatch.Start();
        var minMax = Task.Run(() => (CurrentPlayer as MinMaxAI).MinMax(board, PreviousPlayer, 7, -Mathf.Infinity, Mathf.Infinity, true).Item1);
        while (!minMax.IsCompleted)
        {
            yield return null; //waiting...
        }
        stopWatch.Stop();
        Debug.Log($"AI spent {stopWatch.ElapsedMilliseconds}ms thinking.");
        Debug.Log("Minmax output: " + minMax.Result);

        yield return new WaitForSeconds(0.8f); //Small pause to prevent disk overlap (funky physics)

        DropDisk(CurrentPlayer, minMax.Result);
        Print2DArray(board.array);
    }

    IEnumerator TurnDelay()
    {
        canvasManager.DisplayTurnInfo(CurrentPlayer.name + "'s Turn");
        yield return new WaitForSeconds(0.8f);
        waiting = false;
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
        Debug.Log(printString);
    }
}
