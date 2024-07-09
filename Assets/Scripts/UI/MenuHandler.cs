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
    public TMP_InputField FirstNameInput;
    public TMP_InputField SecondNameInput;

    Stack<GameObject> menuStack = new();

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
        SceneManager.LoadSceneAsync(1);
        GameManager.PlayerOneName = FirstNameInput.text;
        GameManager.PlayerTwoName = SecondNameInput.text;
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void PreLoadGame(GameObject activeCanvas)
    {
        SecondNameInput.interactable = !(GameManager.gamemode == Gamemode.VsAI);
        menuStack.Push(activeCanvas);
        activeCanvas.SetActive(false);
        GameOptionsCanvas.SetActive(true);
    }

    public void EnterSettingsMenu(GameObject activeCanvas)
    {
        menuStack.Push(activeCanvas);
        activeCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void GoBackMenu(GameObject activeCanvas)
    {
        activeCanvas.SetActive(false);
        if(menuStack.Count == 0)
        {
            MenuCanvas.SetActive(true);
        }
        else
        {
            menuStack.Pop().SetActive(true);
        }
    }
}
