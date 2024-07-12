using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using System;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject GameOptionsCanvas;
    public GameObject SettingsCanvas;
    public GameObject LeaderboardCanvas;

    public TMP_InputField FirstNameInput;
    public TMP_InputField SecondNameInput;

    Stack<GameObject> menuStack = new();

    private void Start()
    {
        menuStack = new();
        menuStack.Push(MenuCanvas);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl))
        {
            GameManager.gamemode = Gamemode.Secret;
            GameManager.PlayerOneName = "Tom";
            GameManager.PlayerTwoName = "Jerry";
            SceneManager.LoadSceneAsync(1);
        }
    }

    public void SetGamemode(bool vsAi)
    {
        GameManager.gamemode = vsAi ? Gamemode.VsAI : Gamemode.COOP;
    }

    public void SetAIStarts(bool state)
    {
        GameManager.aiFirst = state;
    }

    public void StartGame()
    {
        GameManager.PlayerOneName = FirstNameInput.text;
        GameManager.PlayerTwoName = SecondNameInput.text;
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void PreLoadGame()
    {
        SecondNameInput.interactable = !(GameManager.gamemode == Gamemode.VsAI);
        OpenMenu(GameOptionsCanvas);
    }

    public void EnterSettingsMenu()
    {
        OpenMenu(SettingsCanvas);
    }

    public void EnterLeaderboardMenu()
    {
        OpenMenu(LeaderboardCanvas);
    }

    public void BackMenu()
    {
        if(menuStack.Count < 2) //this should never happen
        {
            MenuCanvas.SetActive(true);
        }
        else
        {
            menuStack.Pop().SetActive(false);
            menuStack.Peek().SetActive(true);
        }
    }

    private void OpenMenu(GameObject canvas)
    {
        menuStack.Peek().SetActive(false);

        menuStack.Push(canvas);
        menuStack.Peek().SetActive(true);
    }
}
