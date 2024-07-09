using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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
    bool displayDiskPreview;

    public Player currentPlayer { get; private set; }
    public Player previousPlayer { get; private set; }

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

        if (gamemode == Gamemode.VsAI)
        {
            if (aiFirst)
            {
                currentPlayer = new MinMaxAI(1);
                previousPlayer = new Player(2, GameManager.PlayerOneName);
                canvasManager.DisplayTurnInfo("AI's turn");
            }
            else
            {
                currentPlayer = new Player(2, GameManager.PlayerOneName);
                previousPlayer = new MinMaxAI(1);
                canvasManager.DisplayTurnInfo(currentPlayer.name + "'s turn");
            }
        }
        else
        {
            currentPlayer = new Player(1, GameManager.PlayerOneName);
            currentPlayer = new Player(1, GameManager.PlayerTwoName);
        }
    }

    void Update()
    {
        if (!won)
        {
            float screenPercent = Input.mousePosition.x / Screen.width * 100;
            if (displayDiskPreview && screenPercent > 25 && screenPercent < 75)
            {
                dropPreview.transform.position = new Vector3(Input.mousePosition.x, dropPreview.transform.position.y, dropPreview.transform.position.z);
                dropPreview.color = currentPlayer.id == 1 ? new Color(1, 0, 0, 0.4f) : new Color(1, 1, 0, 0.4f);
            }
            else
            {
                dropPreview.color = Color.clear;
            }
        }
        else
        {
            dropPreview.color = Color.clear;
        }
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

    void QuickEndGame()
    {
        won = true;
        canvasManager.DisplayWinText("Game ended!");
        canvasManager.ShowResetButton(true);
    }

    public void AddDiskButton(int collum)
    {
        if (waiting || won || !board.IsValidLocation( collum)) { return; }

        waiting = true;
        displayDiskPreview = false;
        DropDisk(currentPlayer, collum);
    }

    private async Task DropDisk(Player player, int collum)
    {
        if (board.DropDisk(player, collum))
        {
            Instantiate(player.id == 1 ? RedDisk : BlueDisk, spawnLocations[collum].transform.position, spawnLocations[collum].transform.rotation);

            if (board.CheckForWin(player))
            {
                Win(player, previousPlayer);
                return;
            }

            currentPlayer = previousPlayer;
            previousPlayer = player;

            if (currentPlayer is MinMaxAI)
            {
                await AIMove();
            }
            else
            {
                StartCoroutine(TurnDelay());
            }
        }
    }

    private async Task AIMove()
    {
        canvasManager.DisplayTurnInfo("AI's Turn");

        Stopwatch stopWatch = new();
        stopWatch.Start();

        int aiMove = await Task.Run(() => (currentPlayer as MinMaxAI).MinMax(board, previousPlayer, 7, -Mathf.Infinity, Mathf.Infinity, true).Item1);
        print("Minmax output: " + aiMove);
        Debug.Log((currentPlayer as MinMaxAI).branches);
        (currentPlayer as MinMaxAI).branches = 0;

        stopWatch.Stop();
        Debug.Log($"AI spent {stopWatch.ElapsedMilliseconds} milliseconds thinking.");

        StartCoroutine(AITurn(aiMove));
    }

    IEnumerator AITurn(int aiCollum)
    {
        yield return new WaitForSeconds(1);
        DropDisk(currentPlayer, aiCollum);
        Print2DArray(board.array);
        StartCoroutine(TurnDelay());
    }

    IEnumerator TurnDelay()
    {
        canvasManager.DisplayTurnInfo(currentPlayer.name + "'s Turn");
        waiting = false;
        yield return new WaitForSeconds(1);
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
