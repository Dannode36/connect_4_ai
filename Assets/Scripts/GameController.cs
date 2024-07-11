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

        switch (gamemode)
        {
            case Gamemode.VsAI:
                if (aiFirst)
                {
                    currentPlayer = new MinMaxAI(1);
                    previousPlayer = new Player(2, GameManager.PlayerOneName);
                    waiting = true;
                    _ = AIMove();
                }
                else
                {
                    currentPlayer = new Player(1, GameManager.PlayerOneName);
                    previousPlayer = new MinMaxAI(2);
                }
                break;
            default:
                currentPlayer = new Player(1, GameManager.PlayerOneName);
                previousPlayer = new Player(2, GameManager.PlayerTwoName);
                break;
        }

        canvasManager.DisplayTurnInfo(currentPlayer.name + "'s turn");
    }

    void Update()
    {
        if (!won)
        {
            float screenPercent = Input.mousePosition.x / Screen.width * 100;
            if (!waiting && screenPercent > 25 && screenPercent < 75)
            {
                dropPreview.transform.position = new Vector3(Input.mousePosition.x, dropPreview.transform.position.y, dropPreview.transform.position.z);
                dropPreview.color = currentPlayer.id == 1 ? new Color(1, 0, 0, 0.4f) : new Color(1, 1, 0, 0.4f);
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
        _ = DropDisk(currentPlayer, collum);
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
            else if (board.CheckForFull())
            {
                Tie(previousPlayer, currentPlayer); //board should always fill up on the second player
                Debug.Log("First player is " + previousPlayer.name);
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
        Debug.Log("Minmax output: " + aiMove);
        Debug.Log((currentPlayer as MinMaxAI).branches);
        (currentPlayer as MinMaxAI).branches = 0;

        stopWatch.Stop();
        Debug.Log($"AI spent {stopWatch.ElapsedMilliseconds}ms thinking.");

        StartCoroutine(AITurn(aiMove));
    }

    IEnumerator AITurn(int aiCollum)
    {
        yield return new WaitForSeconds(0.8f);
        _ = DropDisk(currentPlayer, aiCollum);
        Print2DArray(board.array);
        //StartCoroutine(TurnDelay());
    }

    IEnumerator TurnDelay()
    {
        canvasManager.DisplayTurnInfo(currentPlayer.name + "'s Turn");
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
